using ClosedXML.Excel;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    public record TableDesignOptions(
        string SourceExcelPath,
        IReadOnlyList<string> TargetFiles,
        bool IgnoreUnderscoreFiles,
        int SheetScope,            // 0=全部，1=仅第一个
        bool IgnoreUnderscoreSheets,
        string HeaderSymbol,
        bool AutoColumnWidth,
        bool MergeHeaderCells,
        string MergeHeaderKeywords,
        string XmlDirectory = "",       // 指定时在表设计后自动强制更新 Enum 验证
        bool HideEnumDataSheet = true,  // 强制更新 Enum 验证时是否隐藏 __enum_data
        bool BoolValidation = true      // 是否为 bool 列添加 TRUE/FALSE 数据验证
    );

    public static class TableDesignApplier
    {
        public static void Apply(
            TableDesignOptions options,
            IProgress<string> progress,
            Action<string, LogLevel> log,
            CancellationToken token)
        {
            if (!File.Exists(options.SourceExcelPath))
            {
                log($"模板文件不存在：{options.SourceExcelPath}", LogLevel.Error);
                return;
            }

            byte[] sourceBytes = File.ReadAllBytes(options.SourceExcelPath);
            int total = options.TargetFiles.Count;
            var savedFiles = new List<string>();

            for (int i = 0; i < total; i++)
            {
                token.ThrowIfCancellationRequested();
                string targetPath = options.TargetFiles[i];
                string fileName = Path.GetFileName(targetPath);
                progress.Report(fileName);

                if (options.IgnoreUnderscoreFiles && fileName.StartsWith("__"))
                {
                    log($"跳过（__开头）：{fileName}", LogLevel.Skip);
                    continue;
                }

                if (!File.Exists(targetPath))
                {
                    log($"文件不存在，跳过：{fileName}", LogLevel.Warn);
                    continue;
                }

                try
                {
                    ProcessTargetFile(sourceBytes, targetPath, options, log, token);
                    savedFiles.Add(targetPath);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (IOException)
                {
                    log($"文件被占用，跳过：{fileName}", LogLevel.Warn);
                }
                catch (Exception ex)
                {
                    log($"处理失败 {fileName}：{LogLibrary.FormatException(ex)}", LogLevel.Error);
                }
            }

            progress.Report(string.Empty);

            // Enum 验证——应用表设计后强制重写所有枚举列验证规则
            if (!string.IsNullOrWhiteSpace(options.XmlDirectory) && savedFiles.Count > 0)
            {
                log("扫描 XML Enum 定义...", LogLevel.Info);
                var enums = EnumScanner.ScanDirectory(options.XmlDirectory);
                if (enums.Count > 0)
                {
                    log($"找到 {enums.Count} 个 Enum，正在强制更新验证规则...", LogLevel.Info);
                    var enumNameSet = enums.Select(e => e.Name).ToHashSet(StringComparer.Ordinal);
                    var enumsForValidation = enums.ToList();
                    if (options.BoolValidation)
                    {
                        enumNameSet.Add("bool");
                        enumsForValidation.Add(new EnumInfo
                        {
                            Name = "bool",
                            Options =
                            [
                                new EnumOption { Name = "FALSE", Value = "0" },
                                new EnumOption { Name = "TRUE",  Value = "1" }
                            ]
                        });
                    }
                    var beanFieldEnumMap = EnumScanner.ScanBeanEnumFields(options.XmlDirectory, enumNameSet);
                    ValidationUpdater.UpdateFiles(
                        savedFiles,
                        enumsForValidation,
                        options.HideEnumDataSheet,
                        result =>
                        {
                            if (result.HasError)
                                log($"  [Enum] 错误 {result.FileName}：{result.ErrorMessage}", LogLevel.Error);
                            else if (result.WasSaved)
                                log($"  [Enum] 已更新：{result.FileName}", LogLevel.Ok);
                        },
                        forceRewrite: true,
                        beanFieldEnumMap: beanFieldEnumMap);
                    log("Enum 验证规则更新完成。", LogLevel.Ok);
                }
                else
                {
                    log("未找到任何 Enum 定义，跳过验证规则更新。", LogLevel.Warn);
                }
            }

            // Excel COM 刷新——重新计算公式缓存，使外部工具读取到正确的值
            if (savedFiles.Count > 0)
            {
                log($"正在通过 Excel 刷新 {savedFiles.Count} 个文件的公式缓存值...", LogLevel.Info);
                bool excelAvailable = FunctionLibrary.RefreshFormulasViaSTA(savedFiles);
                if (excelAvailable)
                    log("公式缓存值刷新完成。", LogLevel.Ok);
                else
                    log("本机未安装 Excel，已跳过公式缓存刷新。如需刷新，请安装 Microsoft Excel 后重试。", LogLevel.Warn);
            }
        }

        private static void ProcessTargetFile(
            byte[] sourceBytes,
            string targetPath,
            TableDesignOptions options,
            Action<string, LogLevel> log,
            CancellationToken token)
        {
            string fileName = Path.GetFileName(targetPath);

            using var workingWb = new XLWorkbook(new MemoryStream(sourceBytes));
            using var targetWb = new XLWorkbook(targetPath);

            // A. 从工作簿中删除 __ 开头的 Sheet（模板内部用，不输出）
            foreach (var sheet in workingWb.Worksheets.ToList())
                if (sheet.Name.StartsWith("__"))
                    sheet.Delete();

            var srcSheets = workingWb.Worksheets.OrderBy(s => s.Position).ToList();
            if (srcSheets.Count == 0)
            {
                log($"模板表中无有效Sheet，跳过：{fileName}", LogLevel.Warn);
                return;
            }

            // 根据范围/选项筛选需要应用设计的目标 Sheet
            var sheetsToProcess = targetWb.Worksheets.OrderBy(s => s.Position).ToList();
            if (options.SheetScope == 1)
                sheetsToProcess = sheetsToProcess.Take(1).ToList();
            if (options.IgnoreUnderscoreSheets)
                sheetsToProcess = sheetsToProcess.Where(s => !s.Name.StartsWith("__")).ToList();

            // B. 临时重命名所有源 Sheet，避免与目标 Sheet 名称冲突
            for (int i = 0; i < srcSheets.Count; i++)
                srcSheets[i].Name = $"__td_tmp_{i}__";

            // C. 按位置匹配——对每对 Sheet 应用设计（无需检查名称）
            int pairCount = Math.Min(srcSheets.Count, sheetsToProcess.Count);
            for (int i = 0; i < pairCount; i++)
            {
                token.ThrowIfCancellationRequested();
                var t2 = srcSheets[i];
                var t3 = sheetsToProcess[i];
                ApplyDesign(t2, t3, options);
                t2.Name = t3.Name;
                log($"{fileName} [{t3.Name}]", LogLevel.Ok);
            }

            // D. 删除没有对应目标 Sheet 的多余源 Sheet
            for (int i = pairCount; i < srcSheets.Count; i++)
                srcSheets[i].Delete();

            // E. 复制未被设计覆盖的目标 Sheet（__ 开头或超出范围的 Sheet）
            foreach (var t3 in targetWb.Worksheets.OrderBy(s => s.Position))
            {
                if (!workingWb.TryGetWorksheet(t3.Name, out _))
                {
                    t3.CopyTo(workingWb, t3.Name);
                    log($"  → 保留 [{t3.Name}]", LogLevel.Skip);
                }
            }

            // F. 还原 Sheet 顺序以匹配目标文件
            var orderedTarget = targetWb.Worksheets.OrderBy(s => s.Position).ToList();
            for (int idx = 0; idx < orderedTarget.Count; idx++)
            {
                if (workingWb.TryGetWorksheet(orderedTarget[idx].Name, out var ws))
                    ws.Position = idx + 1;
            }

            workingWb.SaveAs(targetPath);
            log($"已保存：{fileName}", LogLevel.Section);
        }

        private static void ApplyDesign(
            IXLWorksheet t2,
            IXLWorksheet t3,
            TableDesignOptions options)
        {
            // 1. 在清空内容前统计 T2 表头行数（文本内容仍存在）
            int t2HeaderCount = FunctionLibrary.CountHeaderRows(t2, options.HeaderSymbol);

            // 2. 取消所有已合并区域（确保 CopyDataOnly 能正确写入）
            foreach (var mr in t2.MergedRanges.ToList())
                mr.Unmerge();

            // 3. 清空 T2 数据，保留样式
            int t2LastRow = t2.LastRowUsed()?.RowNumber() ?? 0;
            int t2LastCol = t2.LastColumnUsed()?.ColumnNumber() ?? 0;
            if (t2LastRow > 0)
                t2.Range(1, 1, t2LastRow, t2LastCol).Clear(XLClearOptions.Contents);

            // 4. 调整表头行数以匹配 T3
            // 单元格级样式（填充、边框、对齐、字体）在 Clear 后仍保留。
            // AdjustHeaderRows 会从原第一行复制样式到新增的行。
            int t3HeaderCount = FunctionLibrary.CountHeaderRows(t3, options.HeaderSymbol);
            if (t3HeaderCount != t2HeaderCount)
                FunctionLibrary.AdjustHeaderRows(t2, t2HeaderCount, t3HeaderCount, t2LastCol);

            // 在复制数据前记录 T2 行范围，以便识别“多余”行
            int t2RowExtent = t2.LastRowUsed()?.RowNumber() ?? t3HeaderCount;

            // 5. 将 T3 数据复制到 T2（仅值/公式，不含样式）
            CopyDataOnly(t3, t2);

            // 6. 重置冻结窗格以覆盖表头行
            t2.SheetView.FreezeRows(t3HeaderCount);

            // 确定复制后的数据范围
            int dataLastRow = t2.LastRowUsed()?.RowNumber() ?? 1;
            int dataLastCol = t2.LastColumnUsed()?.ColumnNumber() ?? 1;

            // 将表头样式扩展到 T3 中有而 T2 模板中没有的列。
            // 使用内部单元格（而非边缘单元格）作为样式来源。
            if (dataLastCol > t2LastCol && t2LastCol > 0)
                FunctionLibrary.ExtendHeaderRowStyles(t2, t3HeaderCount, t2LastCol, dataLastCol);

            // 将第一数据行的对齐方式传播到所有后续数据行。
            // 涵盖模板范围内（可能缺少单元格级对齐）
            // 和超出范围（完全没有模板样式）的行。
            int firstDataRow = t3HeaderCount + 1;
            if (firstDataRow < dataLastRow)
            {
                var templateRow = t2.Row(firstDataRow);
                for (int r = firstDataRow + 1; r <= dataLastRow; r++)
                {
                    CopyRowAlignment(templateRow, t2.Row(r));
                    CopyRowCellAlignment(t2, firstDataRow, r, dataLastCol);
                }
            }

            // 7. 根据关键字行或指定行号合并表头单元格
            // 支持逗号和分号作为分隔符，如 "##type,1;2"
            if (options.MergeHeaderCells && !string.IsNullOrWhiteSpace(options.MergeHeaderKeywords))
            {
                var keywords = options.MergeHeaderKeywords
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (keywords.Length > 0)
                    FunctionLibrary.MergeHeaderEmptyCells(t2, keywords, dataLastRow, dataLastCol, options.HeaderSymbol);
            }

            // 8. 调整智能表格尺寸以覆盖新的数据范围
            ResizeTables(t2, dataLastRow, dataLastCol);

            // 9. 自动调整列宽（最后执行，确保内容已全部确定）
            if (options.AutoColumnWidth)
                FunctionLibrary.AutoFitColumns(t2, dataLastCol);
        }

        private static void CopyDataOnly(IXLWorksheet source, IXLWorksheet dest)
        {
            int lastRow = source.LastRowUsed()?.RowNumber() ?? 0;
            int lastCol = source.LastColumnUsed()?.ColumnNumber() ?? 0;
            if (lastRow == 0)
                return;

            foreach (var cell in source.Range(1, 1, lastRow, lastCol).CellsUsed())
            {
                var destCell = dest.Cell(cell.Address.RowNumber, cell.Address.ColumnNumber);
                if (cell.HasFormula)
                    destCell.FormulaA1 = cell.FormulaA1;
                else
                    destCell.Value = cell.Value;

                if (cell.HasComment)
                {
                    var src = cell.GetComment();
                    if (destCell.HasComment)
                        destCell.GetComment().Delete();
                    var dst = destCell.CreateComment();
                    dst.SetAuthor(src.Author);
                    dst.Visible = src.Visible;
                    dst.CopyFrom(src);
                    dst.Style.Size.SetAutomaticSize();
                    dst.Position.Column       = src.Position.Column;
                    dst.Position.ColumnOffset = src.Position.ColumnOffset;
                    dst.Position.Row          = src.Position.Row;
                    dst.Position.RowOffset    = src.Position.RowOffset;
                }
            }
        }

        // 复制行级对齐方式（用于超出模板范围的额外数据行）。
        private static void CopyRowAlignment(IXLRow source, IXLRow dest)
        {
            var src = source.Style.Alignment;
            dest.Style.Alignment.Horizontal = src.Horizontal;
            dest.Style.Alignment.Vertical   = src.Vertical;
            dest.Style.Alignment.WrapText    = src.WrapText;
        }

        // 按列复制 srcRow 到 destRow 的单元格级对齐方式
        // （用于将第一数据行的对齐传播到模板范围外的行）。
        private static void CopyRowCellAlignment(IXLWorksheet ws, int srcRow, int destRow, int lastCol)
        {
            for (int c = 1; c <= lastCol; c++)
            {
                var s = ws.Cell(srcRow, c).Style.Alignment;
                var d = ws.Cell(destRow, c).Style.Alignment;
                d.Horizontal = s.Horizontal;
                d.Vertical   = s.Vertical;
                d.WrapText   = s.WrapText;
            }
        }

        private static void ResizeTables(IXLWorksheet t2, int dataLastRow, int dataLastCol)
        {
            foreach (var table in t2.Tables.ToList())
            {
                int r1 = table.RangeAddress.FirstAddress.RowNumber;
                int c1 = table.RangeAddress.FirstAddress.ColumnNumber;
                int newLastRow = Math.Max(dataLastRow, r1 + 1);
                int newLastCol = Math.Max(dataLastCol, table.RangeAddress.LastAddress.ColumnNumber);
                table.Resize(t2.Range(r1, c1, newLastRow, newLastCol));
            }
        }
    }
}
