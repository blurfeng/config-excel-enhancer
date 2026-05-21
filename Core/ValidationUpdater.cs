using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ClosedXML.Excel;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    public class ValidationUpdater
    {
        private const string EnumSheetName = "__enum_data"; // 双下划线开头能防止 Luban 等工具误识别为普通数据表

        /// <summary>
        /// 更新指定文件列表中的所有 xlsx。
        /// </summary>
        public static List<UpdateResult> UpdateFiles(
            IReadOnlyList<string> filePaths,
            List<EnumInfo> enums,
            bool hideEnumDataSheet,
            Action<UpdateResult> onFileProcessed,
            bool forceRewrite = false,
            Dictionary<string, Dictionary<string, string>>? beanFieldEnumMap = null)
        {
            var enumMap = enums.ToDictionary(e => e.Name, StringComparer.Ordinal);
            return ProcessFiles(filePaths, enumMap, hideEnumDataSheet, onFileProcessed, forceRewrite, beanFieldEnumMap);
        }

        /// <summary>
        /// 更新目录下所有 xlsx，每处理完一个文件回调 onFileProcessed，最终返回全部结果。
        /// </summary>
        public static List<UpdateResult> UpdateDirectory(
            string excelDirectory,
            List<EnumInfo> enums,
            bool hideEnumDataSheet,
            Action<UpdateResult> onFileProcessed,
            bool forceRewrite = false,
            Dictionary<string, Dictionary<string, string>>? beanFieldEnumMap = null)
        {
            var enumMap = enums.ToDictionary(e => e.Name, StringComparer.Ordinal);
            var files = Directory.EnumerateFiles(excelDirectory, "*.xlsx", SearchOption.AllDirectories);
            return ProcessFiles(files, enumMap, hideEnumDataSheet, onFileProcessed, forceRewrite, beanFieldEnumMap);
        }

        /// <summary>
        /// 遍历文件序列，逐个调用 UpdateWorkbook，统一处理 try/catch 和结果收集。
        /// 临时文件（~$ 开头）自动跳过。
        /// </summary>
        private static List<UpdateResult> ProcessFiles(
            IEnumerable<string> files,
            Dictionary<string, EnumInfo> enumMap,
            bool hideEnumDataSheet,
            Action<UpdateResult> onFileProcessed,
            bool forceRewrite,
            Dictionary<string, Dictionary<string, string>>? beanFieldEnumMap = null)
        {
            var results = new List<UpdateResult>();

            foreach (var file in files.Where(f => !Path.GetFileName(f).StartsWith("~$")))
            {
                var result = new UpdateResult { FilePath = file };

                try
                {
                    UpdateWorkbook(file, enumMap, hideEnumDataSheet, result, forceRewrite, beanFieldEnumMap);
                }
                catch (IOException ioEx)
                {
                    result.WasSkipped = true;
                    result.SkipReason = ioEx.Message;
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = ex.Message;
                }

                results.Add(result);
                onFileProcessed(result);
            }

            return results;
        }

        private static void UpdateWorkbook(
            string filePath,
            Dictionary<string, EnumInfo> enumMap,
            bool hideEnumDataSheet,
            UpdateResult result,
            bool forceRewrite = false,
            Dictionary<string, Dictionary<string, string>>? beanFieldEnumMap = null)
        {
            using var wb = new XLWorkbook(filePath);

            // 预扫描：若该文件没有任何枚举列，跳过，不创建/更新 __enum_data，不写盘
            if (!WorkbookHasEnumUsage(wb, enumMap, beanFieldEnumMap))
                return;

            var enumColumns = WriteEnumSheet(wb, enumMap, hideEnumDataSheet, result);

            // forceRewrite 时无论 Schema 是否变化都走完整重写路径
            if (forceRewrite)
                result.HasSchemaChange = true;

            if (result.HasSchemaChange)
            {
                // Schema 有变化：重建 DefinedNames 并重写所有 sheet 的验证规则
                UpdateDefinedNames(wb, enumMap, enumColumns);
                foreach (var ws in wb.Worksheets.ToList())
                {
                    if (ws.Name == EnumSheetName) continue;
                    ApplyValidationToSheet(ws, enumMap, result, rewriteValidation: true, beanFieldEnumMap);
                }
            }
            else
            {
                // Schema 无变化：仅补填空白单元格的默认值，不触碰验证规则
                foreach (var ws in wb.Worksheets.ToList())
                {
                    if (ws.Name == EnumSheetName) continue;
                    ApplyValidationToSheet(ws, enumMap, result, rewriteValidation: false, beanFieldEnumMap);
                }
            }

            // 只有确实发生了数据变更时才写盘，避免 git 误报和 xlsx 结构被意外改动
            if (result.HasSchemaChange || result.HasDataChange || result.HasVisibilityChange)
            {
                // ClosedXML 保存时会重写 VML 注释文件，导致注释尺寸被重置为默认值。
                // 在保存前对所有注释设置自动尺寸，保存后注释会根据内容自适应大小。
                foreach (var ws in wb.Worksheets)
                {
                    foreach (var cell in ws.CellsUsed(c => c.HasComment))
                        cell.GetComment().Style.Size.SetAutomaticSize();
                }

                wb.Save();
                result.WasSaved = true;
                // ClosedXML 不评估公式，保存后公式单元格的缓存值为空。
                // 设置 fullCalcOnLoad="1" 告知 Excel 打开时做完整重算，避免公式列显示为空。
                EnsureFullCalcOnLoad(filePath);
            }
        }

        /// <summary>
        /// Schema 无变化时不重建 sheet，仅读取现有列位置并返回。
        /// Schema 有变化时删除旧 sheet 并重新写入。
        /// </summary>
        private static Dictionary<string, int> WriteEnumSheet(
            XLWorkbook wb,
            Dictionary<string, EnumInfo> enumMap,
            bool hideEnumDataSheet,
            UpdateResult result)
        {
            result.HasSchemaChange = DetectSchemaChange(wb, enumMap);

            if (!result.HasSchemaChange)
            {
                // 直接读取现有 __enum_data 的列位置，检查并修正可见性
                if (wb.TryGetWorksheet(EnumSheetName, out var existingWs))
                {
                    var expectedVisibility = hideEnumDataSheet
                        ? XLWorksheetVisibility.VeryHidden
                        : XLWorksheetVisibility.Visible;
                    if (existingWs.Visibility != expectedVisibility)
                    {
                        existingWs.Visibility = expectedVisibility;
                        result.HasVisibilityChange = true;
                    }
                    return ReadEnumColumnPositions(existingWs);
                }
                // 理论上不会走到这里（DetectSchemaChange 在无 sheet 时返回 true）
                result.HasSchemaChange = true;
            }

            if (wb.TryGetWorksheet(EnumSheetName, out var toDelete))
                toDelete.Delete();

            var ws = wb.AddWorksheet(EnumSheetName);
            ws.Visibility = hideEnumDataSheet ? XLWorksheetVisibility.VeryHidden : XLWorksheetVisibility.Visible;

            var enumColumns = new Dictionary<string, int>(StringComparer.Ordinal);
            int col = 1;

            foreach (var (enumName, enumInfo) in enumMap)
            {
                ws.Cell(1, col).Value = enumName;
                for (int i = 0; i < enumInfo.Options.Count; i++)
                    ws.Cell(2 + i, col).Value = enumInfo.Options[i].Name;

                enumColumns[enumName] = col;
                col++;
            }

            return enumColumns;
        }

        private static Dictionary<string, int> ReadEnumColumnPositions(IXLWorksheet ws)
        {
            var cols = new Dictionary<string, int>(StringComparer.Ordinal);
            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
            for (int col = 1; col <= lastCol; col++)
            {
                var name = ws.Cell(1, col).GetString().Trim();
                if (!string.IsNullOrEmpty(name))
                    cols[name] = col;
            }
            return cols;
        }

        /// <summary>
        /// 读取 workbook 中现有的 _enum_data 内容，与新 enumMap 比较。
        /// 若 enum 数量、名称或选项列表有任何差异则返回 true。
        /// </summary>
        private static bool DetectSchemaChange(XLWorkbook wb, Dictionary<string, EnumInfo> enumMap)
        {
            if (!wb.TryGetWorksheet(EnumSheetName, out var ws))
                return true;

            var oldData = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            for (int col = 1; col <= lastCol; col++)
            {
                var name = ws.Cell(1, col).GetString().Trim();
                if (string.IsNullOrEmpty(name)) continue;

                var options = new List<string>();
                int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
                for (int row = 2; row <= lastRow; row++)
                {
                    var v = ws.Cell(row, col).GetString();
                    if (!string.IsNullOrEmpty(v)) options.Add(v);
                }
                oldData[name] = options;
            }

            if (oldData.Count != enumMap.Count) return true;

            foreach (var (name, info) in enumMap)
            {
                if (!oldData.TryGetValue(name, out var oldOptions)) return true;
                var newOptions = info.Options.Select(o => o.Name).ToList();
                if (!oldOptions.SequenceEqual(newOptions)) return true;
            }

            return false;
        }

        private static void UpdateDefinedNames(
            XLWorkbook wb,
            Dictionary<string, EnumInfo> enumMap,
            Dictionary<string, int> enumColumns)
        {
            foreach (var enumName in enumMap.Keys)
            {
                if (wb.DefinedNames.Contains(enumName))
                    wb.DefinedNames.Delete(enumName);
            }
            var staleNames = wb.DefinedNames.ValidNamedRanges()
                .Concat(wb.DefinedNames.InvalidNamedRanges())
                .Select(dn => dn.Name)
                .Where(name => !enumMap.ContainsKey(name))
                .ToList();
            foreach (var name in staleNames)
                wb.DefinedNames.Delete(name);

            if (wb.TryGetWorksheet(EnumSheetName, out var enumWs))
            {
                foreach (var (enumName, enumInfo) in enumMap)
                {
                    if (!enumColumns.TryGetValue(enumName, out var col)) continue;
                    var range = enumWs.Range(2, col, 1 + enumInfo.Options.Count, col);
                    wb.DefinedNames.Add(enumName, range);
                }
            }
        }

        /// <param name="rewriteValidation">
        /// true = 清除旧验证规则并重建（Schema 变化时）；
        /// false = 仅补填空白默认值，不触碰验证规则（Schema 无变化时）。
        /// </param>
        private static void ApplyValidationToSheet(
            IXLWorksheet ws,
            Dictionary<string, EnumInfo> enumMap,
            UpdateResult result,
            bool rewriteValidation,
            Dictionary<string, Dictionary<string, string>>? beanFieldEnumMap = null)
        {
            int typeRow = FindTypeRow(ws);
            if (typeRow < 0) return;

            int dataStartRow = FindDataStartRow(ws, typeRow + 1);
            if (dataStartRow < 0) return;

            int lastDataRow = FindLastDataRow(ws, dataStartRow);
            if (lastDataRow < dataStartRow) return;

            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
            if (lastCol < 2) return;

            // 预构建已使用单元格的坐标集，避免通过 ws.Cell() 隐式创建空单元格
            var cellsUsed = ws.CellsUsed().ToList();
            var usedCoords = new HashSet<(int row, int col)>(
                cellsUsed.Select(c => (c.Address.RowNumber, c.Address.ColumnNumber)));
            var rowsWithData = new HashSet<int>(
                cellsUsed.Select(c => c.Address.RowNumber));

            for (int col = 2; col <= lastCol; col++)
            {
                var typeName = ws.Cell(typeRow, col).GetString().Trim();
                var colRange = ws.Range(dataStartRow, col, lastDataRow, col);

                enumMap.TryGetValue(typeName, out var enumInfo);

                // Fallback：处理 bean 数据结构中含枚举字段的列
                // - Case 1：##type 行直接写了 bean 类型名（如 TargetSearchStats），在其他表头行找子字段名
                // - Case 2：##type 行为空（bean 展开后的后续子列），向左找覆盖该列的 bean 类型，再找子字段名
                if (enumInfo == null && beanFieldEnumMap != null)
                {
                    string? coveringBeanType = null;

                    if (!string.IsNullOrEmpty(typeName) && beanFieldEnumMap.ContainsKey(typeName))
                    {
                        coveringBeanType = typeName;
                    }
                    else if (string.IsNullOrEmpty(typeName))
                    {
                        // 向左扫描 ##type 行，找到第一个非空值：若是 bean 类型则记录，否则停止
                        for (int leftCol = col - 1; leftCol >= 2; leftCol--)
                        {
                            var leftType = ws.Cell(typeRow, leftCol).GetString().Trim();
                            if (string.IsNullOrEmpty(leftType)) continue;
                            if (beanFieldEnumMap.ContainsKey(leftType))
                                coveringBeanType = leftType;
                            break;
                        }
                    }

                    if (coveringBeanType != null)
                    {
                        var fieldEnumMap = beanFieldEnumMap[coveringBeanType];
                        var fieldName = FindFieldNameInOtherHeaderRows(ws, typeRow, dataStartRow, col, fieldEnumMap);
                        if (fieldName != null && fieldEnumMap.TryGetValue(fieldName, out var enumTypeName))
                            enumMap.TryGetValue(enumTypeName, out enumInfo);
                    }
                }

                if (enumInfo == null)
                {
                    if (!string.IsNullOrEmpty(typeName) && rewriteValidation)
                        ClearValidationForRange(ws, colRange);
                    continue;
                }

                result.EnumColumnsFound++;

                if (rewriteValidation)
                    ClearValidationForRange(ws, colRange);

                // 补填默认值：仅针对有数据的行、且该 enum 列单元格确实为空的情况
                if (!string.IsNullOrEmpty(enumInfo.DefaultOptionName))
                {
                    for (int row = dataStartRow; row <= lastDataRow; row++)
                    {
                        if (!rowsWithData.Contains(row)) continue;
                        if (usedCoords.Contains((row, col))) continue; // 已有值，跳过

                        ws.Cell(row, col).Value = enumInfo.DefaultOptionName;
                        result.DefaultsFilled++;
                        usedCoords.Add((row, col)); // 更新集合，防止同列多次判断
                    }
                }

                if (rewriteValidation)
                {
                    var dv = colRange.CreateDataValidation();
                    dv.List(enumInfo.Name, true);
                }
            }
        }

        private static void ClearValidationForRange(IXLWorksheet ws, IXLRange range)
        {
            var overlapping = ws.DataValidations.GetAllInRange(range.RangeAddress).ToList();
            foreach (var dv in overlapping)
                dv.RemoveRange(range);
        }

        private static int FindTypeRow(IXLWorksheet ws)
        {
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            for (int r = 1; r <= lastRow; r++)
            {
                if (ws.Cell(r, 1).GetString().Trim() == "##type")
                    return r;
            }
            return -1;
        }

        private static int FindDataStartRow(IXLWorksheet ws, int searchFrom)
        {
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            for (int r = searchFrom; r <= lastRow; r++)
            {
                var a = ws.Cell(r, 1).GetString();
                if (!a.StartsWith("##"))
                    return r;
            }
            return -1;
        }

        private static int FindLastDataRow(IXLWorksheet ws, int dataStartRow)
        {
            // 使用 CellsUsed 避免通过 ws.Cell() 创建空单元格
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            for (int r = lastRow; r >= dataStartRow; r--)
            {
                if (ws.Row(r).CellsUsed().Any())
                    return r;
            }
            return dataStartRow - 1;
        }

        public static bool RefreshFormulasViaExcel(IReadOnlyList<string> filePaths)
            => FunctionLibrary.RefreshFormulasViaExcel(filePaths);

        /// <summary>
        /// 在已保存的 xlsx 文件中设置 fullCalcOnLoad="1"。
        /// ClosedXML 保存时不评估公式，会清空公式单元格的缓存值；
        /// 此标志告知 Excel 打开文件时做完整重算，否则公式列显示为空。
        /// </summary>
        private static void EnsureFullCalcOnLoad(string filePath)
        {
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            using var zip = ZipFile.Open(filePath, ZipArchiveMode.Update);
            var entry = zip.GetEntry("xl/workbook.xml");
            if (entry == null) return;

            XDocument doc;
            using (var stream = entry.Open())
                doc = XDocument.Load(stream);

            var calcPr = doc.Root?.Element(ns + "calcPr");
            if (calcPr == null) return;

            if (calcPr.Attribute("fullCalcOnLoad")?.Value == "1") return;

            calcPr.SetAttributeValue("fullCalcOnLoad", "1");

            entry.Delete();
            var newEntry = zip.CreateEntry("xl/workbook.xml");
            using var outStream = newEntry.Open();
            doc.Save(outStream);
        }

        /// <summary>
        /// 快速预扫描：workbook 中是否存在至少一列类型名匹配 enumMap 的枚举列。
        /// 无枚举列的文件不需要 __enum_data，也不应被写盘。
        /// </summary>
        private static bool WorkbookHasEnumUsage(
            XLWorkbook wb,
            Dictionary<string, EnumInfo> enumMap,
            Dictionary<string, Dictionary<string, string>>? beanFieldEnumMap = null)
        {
            foreach (var ws in wb.Worksheets)
            {
                if (ws.Name == EnumSheetName) continue;
                int typeRow = FindTypeRow(ws);
                if (typeRow < 0) continue;

                int dataStartRow = beanFieldEnumMap != null ? FindDataStartRow(ws, typeRow + 1) : -1;
                int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

                for (int col = 2; col <= lastCol; col++)
                {
                    var typeName = ws.Cell(typeRow, col).GetString().Trim();
                    if (enumMap.ContainsKey(typeName))
                        return true;

                    // Fallback：检查 bean 字段枚举映射
                    if (beanFieldEnumMap != null)
                    {
                        string? coveringBeanType = null;

                        if (!string.IsNullOrEmpty(typeName) && beanFieldEnumMap.ContainsKey(typeName))
                        {
                            coveringBeanType = typeName;
                        }
                        else if (string.IsNullOrEmpty(typeName))
                        {
                            for (int leftCol = col - 1; leftCol >= 2; leftCol--)
                            {
                                var leftType = ws.Cell(typeRow, leftCol).GetString().Trim();
                                if (string.IsNullOrEmpty(leftType)) continue;
                                if (beanFieldEnumMap.ContainsKey(leftType))
                                    coveringBeanType = leftType;
                                break;
                            }
                        }

                        if (coveringBeanType != null)
                        {
                            var fieldEnumMap = beanFieldEnumMap[coveringBeanType];
                            var fieldName = FindFieldNameInOtherHeaderRows(ws, typeRow, dataStartRow, col, fieldEnumMap);
                            if (fieldName != null) return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 在除 ##type 行之外的所有表头行中，查找 col 列的非空值，并判断是否为 fieldEnumMap 中的已知字段名。
        /// 用于在 bean 展开列中，从其他 ##var 行定位实际的子字段名称。
        /// dataStartRow 小于等于 0 时扫描全部行。
        /// </summary>
        private static string? FindFieldNameInOtherHeaderRows(
            IXLWorksheet ws,
            int typeRow,
            int dataStartRow,
            int col,
            Dictionary<string, string> fieldEnumMap)
        {
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            int endRow = dataStartRow > 0 ? dataStartRow - 1 : lastRow;

            for (int r = 1; r <= endRow; r++)
            {
                if (r == typeRow) continue;
                var value = ws.Cell(r, col).GetString().Trim();
                if (!string.IsNullOrEmpty(value) && fieldEnumMap.ContainsKey(value))
                    return value;
            }
            return null;
        }
    }
}
