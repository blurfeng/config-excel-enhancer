using System.IO.Compression;
using System.Xml.Linq;
using ClosedXML.Excel;

namespace ConfigExcelEnhancer.Core
{
    /// <summary>
    /// Excel 中单列的元信息（用于生成表头和更新匹配）。
    /// </summary>
    public record ExcelColumnMeta(
        /// <summary>列键，用于与 XML 字段匹配。普通字段=字段名，结构体 Bean 子字段="parentField.subField"。</summary>
        string Key,
        /// <summary>写入 ##var 行（顶层字段名，结构体 Bean 仅首列非空）。</summary>
        string VarName,
        /// <summary>写入 ##type 行（类型名，结构体 Bean 仅首列非空）。</summary>
        string TypeName,
        /// <summary>写入子字段 ##var 行（普通字段为空，结构体 Bean 子字段填子字段名）。</summary>
        string SubVarName,
        /// <summary>写入 ##group 行的值。</summary>
        string Group,
        /// <summary>写入 ## 注释行的值。</summary>
        string Comment,
        /// <summary>所属父字段名（用于结构体 Bean 的列分组删除）。</summary>
        string GroupKey
    );

    /// <summary>
    /// 单个数据类的导出任务。
    /// </summary>
    public record ExcelExportTask(
        BeanInfo LeafBean,
        List<BeanField> AllFields,
        string TargetExcelPath
    );

    /// <summary>
    /// ExcelExporter 全局选项。
    /// </summary>
    public record ExcelExportOptions(
        string? DesignTemplatePath,
        IReadOnlyList<ExcelExportTask> Tasks,
        Dictionary<string, BeanInfo> BeanMap,
        HashSet<string> UsedAsField
    );

    /// <summary>
    /// 从 Luban Bean 定义创建或更新 Excel 文件，维护 Luban 兼容的表头结构。
    /// </summary>
    public static class ExcelExporter
    {
        private const string DefaultGroup = "c,s";
        private const string TypeColumnLabel = "$type";
        private const string ClassNameLabel = "数据类名";

        /// <summary>表头标记符号（Luban 标记为 ##var/##type/##group/##）。</summary>
        private const string HeaderSymbol = "##";

        // 需要跨子列合并的是「父字段名行」与「类型行」(WriteHeaders 布局中固定为第 1、2 行)。
        // 用行号而非 ##var/##type 关键字匹配：子字段行(##var)同样以 ## 开头，若按关键字会被误合并。

        // ── 公共入口 ──────────────────────────────────────────────────────

        public static void ExportAll(
            ExcelExportOptions options,
            Action<string, LogLevel> log,
            CancellationToken ct)
        {
            int total = options.Tasks.Count;
            for (int i = 0; i < total; i++)
            {
                ct.ThrowIfCancellationRequested();
                var task = options.Tasks[i];
                string fileName = Path.GetFileName(task.TargetExcelPath);
                try
                {
                    bool isNew = !File.Exists(task.TargetExcelPath);
                    if (isNew)
                        CreateExcel(task, options.DesignTemplatePath, options.BeanMap, options.UsedAsField, log);
                    else
                        UpdateExcel(task, options.BeanMap, options.UsedAsField, log);

                    log($"{(isNew ? "新建" : "更新")}完成：{fileName}", LogLevel.Ok);
                }
                catch (IOException)
                {
                    log($"文件被占用，跳过：{fileName}", LogLevel.Warn);
                }
                catch (Exception ex)
                {
                    log($"处理失败 {fileName}：{ex.Message}", LogLevel.Error);
                }
            }
        }

        // ── 新建 ──────────────────────────────────────────────────────────

        private static void CreateExcel(
            ExcelExportTask task,
            string? templatePath,
            Dictionary<string, BeanInfo> beanMap,
            HashSet<string> usedAsField,
            Action<string, LogLevel> log)
        {
            var dir = Path.GetDirectoryName(task.TargetExcelPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            // 注释行即智能表表头行，要求列名唯一非空：空注释写占位“列N”，重复加 _n 后缀。
            var columns = ApplyTableHeaderNames(ExpandFields(task.AllFields, beanMap, usedAsField));

            // 目标表头行数：含嵌套结构子字段行(##var)时为 5，否则 4；目标末列 = $type 列 + 字段列。
            bool hasSubRow = columns.Any(c => !string.IsNullOrEmpty(c.SubVarName));
            int targetHeaderRows = hasSubRow ? 5 : 4;
            int dataLastCol = 2 + columns.Count;

            // templateMs 必须在 wb.SaveAs 完成后才能 Dispose：ClosedXML 的 SaveAs
            // 会惰性复制模板里未修改的 parts（打印设置等），若流提前关闭则报
            // "Cannot access a closed Stream"。
            MemoryStream? templateMs = null;
            IXLWorkbook wb;
            bool fromTemplate = false;
            int templateHeaderRows = 0;
            int templateLastCol = 0;
            if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
            {
                log($"  从模板复制：{Path.GetFileName(templatePath)}", LogLevel.Info);
                // 从字节流加载模板，保留其「智能表」(Excel Table) 外观与计算列公式。
                templateMs = new MemoryStream(File.ReadAllBytes(templatePath));
                wb = new XLWorkbook(templateMs);
                fromTemplate = true;
                var firstWs = wb.Worksheets.First();
                // 统计模板表头行数与末列（在改动前，文本仍存在）。
                templateHeaderRows = FunctionLibrary.CountHeaderRows(firstWs, HeaderSymbol);
                templateLastCol = firstWs.LastColumnUsed()?.ColumnNumber() ?? 0;
            }
            else
            {
                wb = new XLWorkbook();
                wb.AddWorksheet(task.LeafBean.Name);
            }

            // templateMs 须在 wb 之后释放（SaveAs 惰性复制模板 parts，提前关闭流会报
            // "Cannot access a closed Stream"）。using var 按声明逆序释放，故先声明
            // templateMs 作用域、后声明 wb 作用域；同时保证异常路径下两者都被释放。
            using var templateScope = templateMs;
            using var wbScope = wb;

            var ws = wb.Worksheets.First();
            ws.Name = task.LeafBean.Name;

            // 1. 调整模板表头行数为 targetHeaderRows（会同步下移/上移数据行与智能表）。
            if (fromTemplate && templateHeaderRows != targetHeaderRows)
                FunctionLibrary.AdjustHeaderRows(ws, templateHeaderRows, targetHeaderRows, templateLastCol);

            // 2. 写表头。直接逐格覆盖（不预先清空表头行）：注释已唯一非空，活动智能表
            //    重命名字段时不会冲突；避免「批量清空表头格触发 ClosedXML 自动改名并产生重复列名」。
            WriteHeaders(ws, task.LeafBean.Name, columns);

            if (fromTemplate)
            {
                // 3. 把表头样式扩展到模板宽度之外的新列。
                if (dataLastCol > templateLastCol && templateLastCol > 0)
                    FunctionLibrary.ExtendHeaderRowStyles(ws, targetHeaderRows, templateLastCol, dataLastCol);

                // 4. 智能表调整为覆盖「B[注释行] : [末列][数据行]」。此时新范围内表头格均已填唯一值。
                DeduplicateCommentRow(ws, targetHeaderRows, dataLastCol);
                ResizeOrCreateTable(ws, targetHeaderRows, targetHeaderRows + 1, dataLastCol);

                // 5. 模板比目标更宽时，清理表外多余列的残留模板内容（已移出智能表，清空不会触发改名）。
                if (templateLastCol > dataLastCol)
                    ws.Range(1, dataLastCol + 1, targetHeaderRows + 1, templateLastCol)
                      .Clear(XLClearOptions.Contents);
            }

            // 6. 合并嵌套结构父列：父字段名行(1)与类型行(2)的连续空单元格。
            FunctionLibrary.MergeHeaderEmptyCells(ws, ["1", "2"], targetHeaderRows, dataLastCol, HeaderSymbol);

            // 7. 保留一行「假数据」承载计算列公式（智能表已覆盖该行）。
            if (fromTemplate)
                FinalizeDataRow(ws, targetHeaderRows, dataLastCol, columns.Count);

            // 8. 按实际表头行数冻结窗格。
            ws.SheetView.FreezeRows(targetHeaderRows);

            // 9. 自动列宽。
            FunctionLibrary.AutoFitColumns(ws, dataLastCol);

            wb.SaveAs(task.TargetExcelPath);
            // wb / templateMs 由方法末尾的 using var 作用域统一释放。
        }

        /// <summary>
        /// 数据行收尾：清空唯一一行数据行的字段值（保留样式），在 $type 列(B)写入计算列公式。
        /// 智能表范围已在调用前覆盖该数据行，此处不再处理表。
        /// </summary>
        private static void FinalizeDataRow(IXLWorksheet ws, int headerRows, int dataLastCol, int fieldCount)
        {
            int dataRow = headerRows + 1; // 唯一一行假数据

            // 清空数据行的旧示例值（保留样式），稍后只回填 B 列公式。
            ws.Range(dataRow, 1, dataRow, dataLastCol).Clear(XLClearOptions.Contents);

            // B 列计算列公式：当首个字段列(C)非空时填入类名(B2=##type 行类名)，否则留空。
            if (fieldCount > 0)
                ws.Cell(dataRow, 2).FormulaA1 = $"IF(C{dataRow}<>\"\",$B$2,\"\")";
        }

        /// <summary>
        /// 将智能表(Excel Table)调整为覆盖「B[注释行] : [末列][数据行]」；工作表无表时新建一个。
        /// </summary>
        private static void ResizeOrCreateTable(IXLWorksheet ws, int commentRow, int dataLastRow, int dataLastCol)
        {
            var range = ws.Range(commentRow, 2, dataLastRow, dataLastCol);
            var table = ws.Tables.FirstOrDefault();
            if (table != null)
                table.Resize(range);
            else
                range.CreateTable();
        }

        /// <summary>
        /// 为每列生成智能表表头用的「注释」：注释为空时按顺序写占位“列1”“列2”…
        /// （与 Excel/ClosedXML 自动列名逻辑一致）；重复项追加 _n 后缀。Excel 智能表要求
        /// 表头列名唯一非空，否则写入/保存时抛「重复 key」异常。$type 列(B)固定用「数据类名」，
        /// 这里预置到去重集合避免字段注释与其冲突。
        /// </summary>
        private static List<ExcelColumnMeta> ApplyTableHeaderNames(List<ExcelColumnMeta> columns)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal) { ClassNameLabel };
            int placeholder = 0;
            var result = new List<ExcelColumnMeta>(columns.Count);
            foreach (var c in columns)
            {
                string name = (c.Comment ?? string.Empty).Trim();
                if (name.Length == 0)
                    name = $"列{++placeholder}";

                string unique = name;
                int n = 2;
                while (!seen.Add(unique))
                    unique = $"{name}_{n++}";

                result.Add(unique == c.Comment ? c : c with { Comment = unique });
            }
            return result;
        }

        // ── 更新（保留数据）────────────────────────────────────────────────

        private static void UpdateExcel(
            ExcelExportTask task,
            Dictionary<string, BeanInfo> beanMap,
            HashSet<string> usedAsField,
            Action<string, LogLevel> log)
        {
            // 注释行即智能表表头行，要求列名唯一非空：空注释写占位“列N”，重复加 _n 后缀。
            var targetColumns = ApplyTableHeaderNames(ExpandFields(task.AllFields, beanMap, usedAsField));

            // 加载现有文件；若因 Table 列名重复无法加载，则自动剥除 Table 后重试。
            // strippedMs 与 templateMs 同理：SaveAs 完成前不能关闭。
            MemoryStream? strippedMs = null;
            XLWorkbook wb;
            bool loadedFromStream = false;
            try
            {
                wb = new XLWorkbook(task.TargetExcelPath);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("same key"))
            {
                log("  检测到文件含重复列名 Table，自动清理后继续...", LogLevel.Warn);
                var stripped = StripTablesFromXlsx(File.ReadAllBytes(task.TargetExcelPath));
                strippedMs = new MemoryStream(stripped);
                wb = new XLWorkbook(strippedMs);
                loadedFromStream = true;
            }

            // strippedMs 须在 wb 之后释放（SaveAs 完成前不能关闭流）。using var 按声明逆序释放，
            // 故先声明 strippedMs 作用域、后声明 wb 作用域，确保 wb 先于 strippedMs 释放；
            // 同时保证任意异常路径下两者都被释放（修复原先异常时的资源泄漏）。
            using var strippedScope = strippedMs;
            using var wbScope = wb;

            var ws = wb.Worksheets.First();

            // 检测现有表头
            var headerInfo = DetectHeaders(ws);
            if (headerInfo.DataStartRow < 0)
            {
                // 找不到有效表头结构，视为新建
                log($"  未检测到有效表头，重建表头结构", LogLevel.Warn);
                ws.RangeUsed()?.Clear(XLClearOptions.Contents);
                WriteHeaders(ws, task.LeafBean.Name, targetColumns);
                int rebuildHeaderRows = targetColumns.Any(c => !string.IsNullOrEmpty(c.SubVarName)) ? 5 : 4;
                int rebuildLastCol = 2 + targetColumns.Count;
                FunctionLibrary.MergeHeaderEmptyCells(ws, ["1", "2"], rebuildHeaderRows, rebuildLastCol, HeaderSymbol);
                FinalizeDataRow(ws, rebuildHeaderRows, rebuildLastCol, targetColumns.Count);
                DeduplicateCommentRow(ws, rebuildHeaderRows, rebuildLastCol);
                ResizeOrCreateTable(ws, rebuildHeaderRows, rebuildHeaderRows + 1, rebuildLastCol);
                ws.SheetView.FreezeRows(rebuildHeaderRows);
                FunctionLibrary.AutoFitColumns(ws, rebuildLastCol);
                if (loadedFromStream) wb.SaveAs(task.TargetExcelPath); else wb.Save();
                return;
            }

            // 解析现有列布局（从 B 列起，跳过 A 列行标记）
            var existingGroups = BuildExistingColumnGroups(ws, headerInfo);

            // 对比并计划变更
            var targetGroupKeys = targetColumns.Select(c => c.GroupKey).Distinct().ToList();
            var existingGroupKeys = existingGroups.Select(g => g.Key).ToList();

            var toDelete = existingGroupKeys.Except(targetGroupKeys).ToHashSet();
            var toAdd = targetGroupKeys.Except(existingGroupKeys).ToList();
            var toKeep = existingGroupKeys.Intersect(targetGroupKeys).ToHashSet();

            if (toDelete.Count > 0)
                log($"  删除字段：{string.Join(", ", toDelete)}", LogLevel.Warn);
            if (toAdd.Count > 0)
                log($"  新增字段：{string.Join(", ", toAdd)}", LogLevel.Info);
            if (toKeep.Count > 0)
                log($"  保留字段：{toKeep.Count} 个", LogLevel.Info);

            // 是否需要子字段行（结构体 Bean 展开）。orderMatches 判定与重排重建均要用。
            bool needsSubRow = targetColumns.Any(c => !string.IsNullOrEmpty(c.SubVarName));
            bool hadSubRow   = headerInfo.SubVarRow > 0;

            // 物理列顺序是否已与 XML 完全一致（含结构体子列、子字段行结构）。
            // 一致 → 走下方轻量更新（仅刷新表头/样式，最小化 git diff）；
            // 不一致（增/删/顺序/子列结构变化）→ 列重排重建，使列顺序与 XML 逐列对齐。
            var existingKeys = BuildExistingColumnKeys(ws, headerInfo);
            var targetKeys   = targetColumns.Select(c => c.Key).ToList();
            bool orderMatches = existingKeys.SequenceEqual(targetKeys, StringComparer.Ordinal)
                                && needsSubRow == hadSubRow;

            if (!orderMatches)
            {
                RebuildColumnsInOrder(ws, headerInfo, targetColumns, task.LeafBean.Name);
                if (loadedFromStream) wb.SaveAs(task.TargetExcelPath); else wb.Save();
                return;
            }

            // 收集需要删除的列（从右到左，防止索引偏移）
            var colsToDelete = existingGroups
                .Where(g => toDelete.Contains(g.Key))
                .SelectMany(g => Enumerable.Range(g.StartCol, g.ColCount))
                .OrderByDescending(c => c)
                .ToList();

            foreach (var col in colsToDelete)
                ws.Column(col).Delete();

            // 处理表头行数变化（needsSubRow/hadSubRow 已在上方计算）。
            // 注：能走到这里说明 orderMatches==true，故 needsSubRow==hadSubRow，
            // 下面两个分支实际不会进入；保留以兼容潜在的其他调用路径。
            if (needsSubRow && !hadSubRow)
            {
                // 需要插入一个子字段行。
                // 不使用 InsertRowsAbove：ClosedXML 在行移动时会以 ##group 行里
                // 大量重复的 "c,s" 字符串作为字典 key，触发重复 key 异常（issue #1114）。
                // 改用 Range.CopyTo 手动向下平移，再清空腾出的行。
                int insertAt = headerInfo.TypeRow + 1;
                int lastRow  = ws.LastRowUsed()?.RowNumber() ?? insertAt;
                int lastCol  = ws.LastColumnUsed()?.ColumnNumber() ?? 1;
                if (lastRow >= insertAt)
                    ws.Range(insertAt, 1, lastRow, lastCol)
                      .CopyTo(ws.Cell(insertAt + 1, 1));
                ws.Range(insertAt, 1, insertAt, lastCol).Clear(XLClearOptions.All);
                ws.Cell(insertAt, 1).Value = "##var";
                headerInfo = DetectHeaders(ws);
            }
            else if (!needsSubRow && hadSubRow)
            {
                // 同理：用 Range.CopyTo 向上平移替代 Row.Delete。
                int removeAt = headerInfo.SubVarRow;
                int lastRow  = ws.LastRowUsed()?.RowNumber() ?? removeAt;
                int lastCol  = ws.LastColumnUsed()?.ColumnNumber() ?? 1;
                if (lastRow > removeAt)
                    ws.Range(removeAt + 1, 1, lastRow, lastCol)
                      .CopyTo(ws.Cell(removeAt, 1));
                ws.Range(lastRow, 1, lastRow, lastCol).Clear(XLClearOptions.All);
                headerInfo = DetectHeaders(ws);
            }

            // 重写所有表头行（已保留的列）
            RewriteHeadersForKeptColumns(ws, headerInfo, existingGroups.Where(g => toKeep.Contains(g.Key)).ToList(),
                targetColumns, task.LeafBean.Name);

            // 在末尾追加新增字段的列（记录追加前末列，作为新列的样式参考列）
            int prevLastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 1;
            if (toAdd.Count > 0)
            {
                int nextCol = prevLastCol + 1;
                var newCols = targetColumns.Where(c => toAdd.Contains(c.GroupKey)).ToList();
                AppendColumns(ws, headerInfo, newCols, nextCol, task.LeafBean.Name);
            }

            // ── 样式收尾：以已有表头列为参考，给新列套样式，重新合并/冻结/列宽 ──
            headerInfo = DetectHeaders(ws);
            int headerRows = headerInfo.CommentRow > 0 ? headerInfo.CommentRow : targetColumns.Any(c => !string.IsNullOrEmpty(c.SubVarName)) ? 5 : 4;
            int finalLastCol = ws.LastColumnUsed()?.ColumnNumber() ?? prevLastCol;

            // 1. 给新增列套表头样式（参考追加前的已有末列）
            if (finalLastCol > prevLastCol && prevLastCol > 0)
                FunctionLibrary.ExtendHeaderRowStyles(ws, headerRows, prevLastCol, finalLastCol);

            // 2. 先取消表头区域已有合并区，避免旧合并残留，再按当前结构重新合并 struct 父列
            //    （仅合并父字段名行与类型行，避免子字段行被误合并）
            foreach (var mr in ws.MergedRanges.ToList())
                if (mr.RangeAddress.FirstAddress.RowNumber <= headerRows)
                    mr.Unmerge();
            FunctionLibrary.MergeHeaderEmptyCells(ws,
                [headerInfo.VarRow.ToString(), headerInfo.TypeRow.ToString()],
                headerRows, finalLastCol, HeaderSymbol);

            // 3. 按实际表头行数冻结窗格
            ws.SheetView.FreezeRows(headerRows);

            // 4. 自动列宽
            FunctionLibrary.AutoFitColumns(ws, finalLastCol);

            // 5. 调整智能表(Excel Table)：覆盖「## 注释行 ~ 末行数据」。表头列名已由
            //    ApplyTableHeaderNames 保证唯一非空，写入时活动智能表不会抛重复 key 异常。
            int updLastDataRow = Math.Max(ws.LastRowUsed()?.RowNumber() ?? headerRows, headerRows + 1);
            DeduplicateCommentRow(ws, headerInfo.CommentRow > 0 ? headerInfo.CommentRow : headerRows, finalLastCol);
            ResizeOrCreateTable(ws, headerRows, updLastDataRow, finalLastCol);

            if (loadedFromStream)
                wb.SaveAs(task.TargetExcelPath); // 从 MemoryStream 加载时须用 SaveAs
            else
                wb.Save();
            // wb / strippedMs 由方法末尾的 using var 作用域统一释放。
        }

        // ── 表头写入 ──────────────────────────────────────────────────────

        private static void WriteHeaders(IXLWorksheet ws, string className, List<ExcelColumnMeta> columns)
        {
            bool hasSubRow = columns.Any(c => !string.IsNullOrEmpty(c.SubVarName));

            // 行号分配
            int varRow     = 1;
            int typeRow    = 2;
            int subVarRow  = hasSubRow ? 3 : 0;
            int groupRow   = hasSubRow ? 4 : 3;
            int commentRow = hasSubRow ? 5 : 4;

            // A 列行标记
            ws.Cell(varRow,     1).Value = "##var";
            ws.Cell(typeRow,    1).Value = "##type";
            if (hasSubRow) ws.Cell(subVarRow, 1).Value = "##var";
            ws.Cell(groupRow,   1).Value = "##group";
            ws.Cell(commentRow, 1).Value = "##";

            // B 列（$type）。子字段行与组行的 B 列应为空——显式清空，避免残留模板数据。
            ws.Cell(varRow,     2).Value = TypeColumnLabel;
            ws.Cell(typeRow,    2).Value = className;
            if (hasSubRow) ws.Cell(subVarRow, 2).Value = string.Empty;
            ws.Cell(groupRow,   2).Value = string.Empty;
            ws.Cell(commentRow, 2).Value = ClassNameLabel;

            // 字段列（从 C 列起）
            int col = 3;
            foreach (var meta in columns)
            {
                ws.Cell(varRow,  col).Value = meta.VarName;
                ws.Cell(typeRow, col).Value = meta.TypeName;
                if (hasSubRow)
                    ws.Cell(subVarRow, col).Value = meta.SubVarName;
                ws.Cell(groupRow,   col).Value = meta.Group;
                ws.Cell(commentRow, col).Value = meta.Comment;
                col++;
            }
        }

        private static void RewriteHeadersForKeptColumns(
            IXLWorksheet ws,
            HeaderInfo info,
            List<ColumnGroup> keptGroups,
            List<ExcelColumnMeta> targetColumns,
            string className)
        {
            // 更新 B 列 $type / className
            ws.Cell(info.VarRow,     2).Value = TypeColumnLabel;
            ws.Cell(info.TypeRow,    2).Value = className;
            ws.Cell(info.CommentRow, 2).Value = ClassNameLabel;
            if (info.SubVarRow > 0)
                ws.Cell(info.SubVarRow, 2).Value = string.Empty;
            if (info.GroupRow > 0)
                ws.Cell(info.GroupRow, 2).Value = string.Empty;

            // 按现有列位置更新每个 kept 字段的表头
            foreach (var grp in keptGroups)
            {
                // 在 target 中找到对应字段（按 GroupKey 匹配）
                var matchingCols = targetColumns.Where(c => c.GroupKey == grp.Key).ToList();

                int currentCol = grp.StartCol;
                for (int i = 0; i < matchingCols.Count && currentCol <= grp.StartCol + grp.ColCount - 1; i++, currentCol++)
                {
                    var meta = matchingCols[i];
                    ws.Cell(info.VarRow,  currentCol).Value = meta.VarName;
                    ws.Cell(info.TypeRow, currentCol).Value = meta.TypeName;
                    if (info.SubVarRow > 0)
                        ws.Cell(info.SubVarRow, currentCol).Value = meta.SubVarName;
                    if (info.GroupRow > 0)
                        ws.Cell(info.GroupRow, currentCol).Value = meta.Group;
                    ws.Cell(info.CommentRow, currentCol).Value = meta.Comment;
                }
            }
        }

        private static void AppendColumns(
            IXLWorksheet ws,
            HeaderInfo info,
            List<ExcelColumnMeta> newCols,
            int startCol,
            string className)
        {
            int col = startCol;
            foreach (var meta in newCols)
            {
                ws.Cell(info.VarRow,  col).Value = meta.VarName;
                ws.Cell(info.TypeRow, col).Value = meta.TypeName;
                if (info.SubVarRow > 0)
                    ws.Cell(info.SubVarRow, col).Value = meta.SubVarName;
                if (info.GroupRow > 0)
                    ws.Cell(info.GroupRow, col).Value = meta.Group;
                ws.Cell(info.CommentRow, col).Value = meta.Comment;
                col++;
            }
        }

        // ── 表头检测 ──────────────────────────────────────────────────────

        private record HeaderInfo(
            int VarRow,
            int TypeRow,
            int SubVarRow,   // 0 = 不存在
            int GroupRow,
            int CommentRow,
            int DataStartRow // -1 = 未找到
        );

        private static HeaderInfo DetectHeaders(IXLWorksheet ws)
        {
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;

            int varRow1    = 0;
            int typeRow    = 0;
            int varRow2    = 0;  // 第二个 ##var 行（子字段行）
            int groupRow   = 0;
            int commentRow = 0;

            for (int r = 1; r <= lastRow; r++)
            {
                var marker = ws.Cell(r, 1).GetString().Trim();
                if (!marker.StartsWith("##"))
                    break; // 表头结束

                if (marker == "##var")
                {
                    if (varRow1 == 0) varRow1 = r;
                    else if (varRow2 == 0) varRow2 = r;
                }
                else if (marker == "##type")   typeRow    = r;
                else if (marker == "##group")  groupRow   = r;
                else if (marker == "##")       commentRow = r;
            }

            // DataStartRow = 第一个 A 列为空（且不是 ##）的行
            int dataStart = -1;
            for (int r = 1; r <= lastRow; r++)
            {
                var a = ws.Cell(r, 1).GetString().Trim();
                if (!a.StartsWith("##"))
                {
                    dataStart = r;
                    break;
                }
            }

            return new HeaderInfo(varRow1, typeRow, varRow2, groupRow, commentRow, dataStart);
        }

        // ── 现有列布局解析 ────────────────────────────────────────────────

        private record ColumnGroup(string Key, int StartCol, int ColCount);

        private static List<ColumnGroup> BuildExistingColumnGroups(IXLWorksheet ws, HeaderInfo info)
        {
            if (info.VarRow <= 0) return [];

            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
            var groups = new List<ColumnGroup>();

            string currentGroupKey = string.Empty;
            int groupStart = 0;

            for (int col = 3; col <= lastCol; col++) // 从 C 列起（B 列是 $type，跳过）
            {
                var varName = ws.Cell(info.VarRow, col).GetString().Trim();
                bool isNewGroup = !string.IsNullOrEmpty(varName);

                if (isNewGroup)
                {
                    // 完成上一组
                    if (!string.IsNullOrEmpty(currentGroupKey))
                        groups.Add(new ColumnGroup(currentGroupKey, groupStart, col - groupStart));

                    currentGroupKey = varName;
                    groupStart = col;
                }
            }

            // 完成最后一组
            if (!string.IsNullOrEmpty(currentGroupKey))
                groups.Add(new ColumnGroup(currentGroupKey, groupStart, lastCol - groupStart + 1));

            return groups;
        }

        /// <summary>
        /// 逐物理数据列（C..末列）求其「列身份键」，与 <see cref="ExpandFields"/> 产出的
        /// <see cref="ExcelColumnMeta.Key"/> 一一对应：##var 向右顺延得顶层字段名 topVar
        /// （结构体子列仅首列有 ##var），子字段行非空时取 subVar，
        /// Key = subVar 非空 ? "topVar.subVar" : "topVar"。用于「顺序是否一致」判定与重排匹配。
        /// </summary>
        private static List<string> BuildExistingColumnKeys(IXLWorksheet ws, HeaderInfo info)
        {
            var keys = new List<string>();
            if (info.VarRow <= 0) return keys;

            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
            string topVar = string.Empty;
            for (int col = 3; col <= lastCol; col++)
            {
                var v = ws.Cell(info.VarRow, col).GetString().Trim();
                if (!string.IsNullOrEmpty(v)) topVar = v;
                string sub = info.SubVarRow > 0 ? ws.Cell(info.SubVarRow, col).GetString().Trim() : string.Empty;
                keys.Add(sub.Length > 0 ? $"{topVar}.{sub}" : topVar);
            }
            return keys;
        }

        // ── 注释行物理去重（智能表要求表头列名唯一非空）──────────────────────

        /// <summary>
        /// 对物理 ## 注释行（列 2..<paramref name="lastCol"/>）去重：以「数据类名」(B 列) 为种子，
        /// 重复或为空的单元格写回 _2/_3… 后缀。避免 ClosedXML 创建/调整智能表时因表头列名
        /// 重复抛「An item with the same key has already been added」。
        /// 与 <see cref="ApplyTableHeaderNames"/> 不同：本方法作用于已写入的物理单元格，
        /// 可清除历史残留/重复的注释；且匹配数据列用 ##var 身份，故此处去重不影响列匹配。
        /// </summary>
        private static void DeduplicateCommentRow(IXLWorksheet ws, int commentRow, int lastCol)
        {
            if (commentRow <= 0 || lastCol < 2) return;

            // 种子：B 列（$type 列）注释固定为「数据类名」，保留不动，仅占位以便字段列与之撞名时退让。
            var seen = new HashSet<string>(StringComparer.Ordinal)
            {
                ClassNameLabel,
                ws.Cell(commentRow, 2).GetString().Trim(),
            };
            int placeholder = 0;
            for (int col = 3; col <= lastCol; col++)
            {
                var cell = ws.Cell(commentRow, col);
                string name = cell.GetString().Trim();
                if (name.Length == 0)
                    name = $"列{++placeholder}";

                string unique = name;
                int n = 2;
                while (!seen.Add(unique))
                    unique = $"{name}_{n++}";

                if (unique != cell.GetString())
                    cell.Value = unique;
            }
        }

        /// <summary>为 [firstDataRow..lastDataRow] 每个数据行的 B 列写入计算列公式。</summary>
        private static void SetComputedColumnFormula(IXLWorksheet ws, int firstDataRow, int lastDataRow)
        {
            for (int r = firstDataRow; r <= lastDataRow; r++)
                ws.Cell(r, 2).FormulaA1 = $"IF(C{r}<>\"\",$B$2,\"\")";
        }

        // ── 列重排重建（顺序与 XML 一致 + 保留数据/样式）────────────────────

        private enum HeaderRole { Var, Type, SubVar, Group, Comment }

        private record CapturedCell(XLCellValue Value, string? Formula, FunctionLibrary.StyleSnapshot Style);

        private record CapturedColumn(
            double Width,
            Dictionary<HeaderRole, FunctionLibrary.StyleSnapshot> HeaderStyle,
            CapturedCell?[] Data);

        /// <summary>
        /// 列重排重建：快照旧表（每列数据+样式+列宽，按身份键），清空工作表后按 XML 顺序重发。
        /// 保留列还原数据/样式/列宽；新增列数据留空、样式借相邻保留列；删除列丢弃。
        /// 调用前提：已确认列顺序与 XML 不一致（见 UpdateExcel 的 orderMatches 判定）。
        /// </summary>
        private static void RebuildColumnsInOrder(
            IXLWorksheet ws,
            HeaderInfo info,
            List<ExcelColumnMeta> targetColumns,
            string className)
        {
            int existingHeaderRows = info.CommentRow > 0 ? info.CommentRow : info.GroupRow;
            int lastCol  = ws.LastColumnUsed()?.ColumnNumber() ?? 2;
            int dataStart = info.DataStartRow > 0 ? info.DataStartRow : existingHeaderRows + 1;
            int lastRow  = ws.LastRowUsed()?.RowNumber() ?? existingHeaderRows;
            int dataCount = Math.Max(0, lastRow - dataStart + 1);

            // 旧表各行角色 → 行号
            var existingRole = new Dictionary<HeaderRole, int>
            {
                [HeaderRole.Var]     = info.VarRow,
                [HeaderRole.Type]    = info.TypeRow,
                [HeaderRole.Group]   = info.GroupRow,
                [HeaderRole.Comment] = info.CommentRow,
            };
            if (info.SubVarRow > 0) existingRole[HeaderRole.SubVar] = info.SubVarRow;

            // ── 快照（清空前，只读）──
            var aHeader = CaptureHeaderStyles(ws, 1, existingRole);
            var aData   = CaptureColumnData(ws, 1, dataStart, dataCount);
            var bHeader = CaptureHeaderStyles(ws, 2, existingRole);
            var bData   = CaptureColumnData(ws, 2, dataStart, dataCount);

            var existingKeys = BuildExistingColumnKeys(ws, info);
            var captured = new Dictionary<string, CapturedColumn>(StringComparer.Ordinal);
            for (int i = 0; i < existingKeys.Count; i++)
            {
                int col = 3 + i;
                string key = existingKeys[i];
                if (string.IsNullOrEmpty(key) || captured.ContainsKey(key)) continue; // 重复键以首列为准
                captured[key] = new CapturedColumn(
                    ws.Column(col).Width,
                    CaptureHeaderStyles(ws, col, existingRole),
                    CaptureColumnData(ws, col, dataStart, dataCount));
            }

            var headerRowHeight = new Dictionary<HeaderRole, double>();
            foreach (var kv in existingRole) headerRowHeight[kv.Key] = ws.Row(kv.Value).Height;
            var dataRowHeight = new double[dataCount];
            for (int k = 0; k < dataCount; k++) dataRowHeight[k] = ws.Row(dataStart + k).Height;

            // ── 目标行布局（依目标是否含结构体子字段行）──
            bool needsSubRow = targetColumns.Any(c => !string.IsNullOrEmpty(c.SubVarName));
            int tVar = 1, tType = 2;
            int tSub     = needsSubRow ? 3 : 0;
            int tGroup   = needsSubRow ? 4 : 3;
            int tComment = needsSubRow ? 5 : 4;
            int targetHeaderRows = tComment;

            var targetRole = new Dictionary<HeaderRole, int>
            {
                [HeaderRole.Var]     = tVar,
                [HeaderRole.Type]    = tType,
                [HeaderRole.Group]   = tGroup,
                [HeaderRole.Comment] = tComment,
            };
            if (tSub > 0) targetRole[HeaderRole.SubVar] = tSub;

            int finalLastCol = 2 + targetColumns.Count;
            int outputDataRows = Math.Max(dataCount, 1); // 至少保留一行承载计算列
            int lastDataRow = targetHeaderRows + outputDataRows;
            int maxCol = Math.Max(lastCol, finalLastCol);
            int maxRow = Math.Max(lastRow, lastDataRow);

            // ── 保留智能表：用「临时唯一表头」桥接，避免清空/调整时触发 ClosedXML 列名改名 ──
            // 不移除智能表（移除后重建会丢失其 Theme/条纹等样式）。改为：先把表头改成临时唯一值，
            // 把表 Resize 到最终几何（表头行随之就位，Theme 与所有表级属性原样保留），
            // 再清空表外区域并用真实值覆盖临时表头。
            foreach (var mr in ws.MergedRanges.ToList())
                if (mr.RangeAddress.FirstAddress.RowNumber <= Math.Max(existingHeaderRows, targetHeaderRows))
                    mr.Unmerge();

            var table = ws.Tables.FirstOrDefault();
            if (table != null)
            {
                // 旧表头改临时唯一值（保持当前表有效）；目标表头行也填临时唯一值（供 Resize）。
                for (int c = 2; c <= lastCol; c++)
                    ws.Cell(info.CommentRow, c).Value = $"__tmp_h_{c}";
                for (int c = 2; c <= finalLastCol; c++)
                    ws.Cell(targetHeaderRows, c).Value = $"__tmp_t_{c}";
                table.Resize(ws.Range(targetHeaderRows, 2, lastDataRow, finalLastCol));
            }

            // 移除旧的数据验证（枚举/布尔下拉）：它们按列位置绑定，列重排后全部失位；
            // 尤其落在「被删除列」上的验证，清空单元格后会残留空 sqref，导致 Excel「需要修复」。
            // 统一清除，由导出后的 Enum 验证步骤按新列位置重新应用。
            ws.DataValidations.Delete(_ => true);

            // ── 清空智能表之外的区域；当前表头行(targetHeaderRows, 2..finalLastCol) 保留待真实值覆盖 ──
            if (targetHeaderRows > 1)
                ws.Range(1, 1, targetHeaderRows - 1, maxCol).Clear(XLClearOptions.All);
            ws.Cell(targetHeaderRows, 1).Clear(XLClearOptions.All);
            if (maxCol > finalLastCol)
                ws.Range(targetHeaderRows, finalLastCol + 1, targetHeaderRows, maxCol).Clear(XLClearOptions.All);
            if (maxRow > targetHeaderRows)
                ws.Range(targetHeaderRows + 1, 1, maxRow, maxCol).Clear(XLClearOptions.All);

            // A 列：行标记 + 还原表头/数据单元格样式。
            // A 列在智能表之外，清空时被一并清掉，数据行的填充色等需手动还原。
            EmitColumnHeader(ws, 1, targetRole, AHeaderValue(), aHeader, null);
            EmitColumnData(ws, 1, targetHeaderRows, aData, styleOnly: true);

            // ── B 列（$type）：表头值固定、样式还原；数据样式还原（公式稍后统一回填）──
            EmitColumnHeader(ws, 2, targetRole, BHeaderValue(className), bHeader, null);
            EmitColumnData(ws, 2, targetHeaderRows, bData, styleOnly: true);

            // ── 字段列：按目标顺序逐列重发 ──
            for (int i = 0; i < targetColumns.Count; i++)
            {
                int outCol = 3 + i;
                var meta = targetColumns[i];

                if (captured.TryGetValue(meta.Key, out var cap))
                {
                    EmitColumnHeader(ws, outCol, targetRole, MetaHeaderValue(meta), cap.HeaderStyle, null);
                    EmitColumnData(ws, outCol, targetHeaderRows, cap.Data, styleOnly: false);
                    ws.Column(outCol).Width = cap.Width;
                }
                else
                {
                    // 新增列：样式/列宽借最近的保留列；数据留空但套用其行样式以保持表格外观一致
                    var refCap = FindReferenceColumn(targetColumns, captured, i);
                    EmitColumnHeader(ws, outCol, targetRole, MetaHeaderValue(meta), refCap?.HeaderStyle, null);
                    if (refCap != null)
                    {
                        for (int k = 0; k < refCap.Data.Length; k++)
                            if (refCap.Data[k] is { } rc)
                                FunctionLibrary.ApplyStyle(ws.Cell(targetHeaderRows + 1 + k, outCol), rc.Style);
                        if (refCap.Width > 0) ws.Column(outCol).Width = refCap.Width;
                    }
                }
            }

            // ── 收尾 ──
            FunctionLibrary.MergeHeaderEmptyCells(ws,
                [tVar.ToString(), tType.ToString()], targetHeaderRows, finalLastCol, HeaderSymbol);

            // 还原行高（表头按角色、数据按偏移）
            foreach (var kv in targetRole)
                if (headerRowHeight.TryGetValue(kv.Key, out var h) && h > 0)
                    ws.Row(kv.Value).Height = h;
            for (int k = 0; k < dataCount; k++)
                if (dataRowHeight[k] > 0) ws.Row(targetHeaderRows + 1 + k).Height = dataRowHeight[k];

            SetComputedColumnFormula(ws, targetHeaderRows + 1, lastDataRow);
            ws.SheetView.FreezeRows(targetHeaderRows);

            DeduplicateCommentRow(ws, tComment, finalLastCol);

            // 智能表已在上方 Resize 到最终几何（保留原 Theme）；仅当原本无表（流路径剥离后）才新建。
            if (table == null)
                ResizeOrCreateTable(ws, tComment, lastDataRow, finalLastCol);
        }

        /// <summary>取目标顺序中离 index 最近、且为「保留列」的捕获列，作为新增列的样式参考。</summary>
        private static CapturedColumn? FindReferenceColumn(
            List<ExcelColumnMeta> targetColumns,
            Dictionary<string, CapturedColumn> captured,
            int index)
        {
            for (int d = 1; d < targetColumns.Count; d++)
            {
                int l = index - d, r = index + d;
                if (l >= 0 && captured.TryGetValue(targetColumns[l].Key, out var cl)) return cl;
                if (r < targetColumns.Count && captured.TryGetValue(targetColumns[r].Key, out var cr)) return cr;
            }
            return null;
        }

        private static Func<HeaderRole, string> MetaHeaderValue(ExcelColumnMeta m) => role => role switch
        {
            HeaderRole.Var     => m.VarName,
            HeaderRole.Type    => m.TypeName,
            HeaderRole.SubVar  => m.SubVarName,
            HeaderRole.Group   => m.Group,
            HeaderRole.Comment => m.Comment,
            _ => string.Empty,
        };

        private static Func<HeaderRole, string> AHeaderValue() => role => role switch
        {
            HeaderRole.Var     => "##var",
            HeaderRole.Type    => "##type",
            HeaderRole.SubVar  => "##var",
            HeaderRole.Group   => "##group",
            HeaderRole.Comment => "##",
            _ => string.Empty,
        };

        private static Func<HeaderRole, string> BHeaderValue(string className) => role => role switch
        {
            HeaderRole.Var     => TypeColumnLabel,
            HeaderRole.Type    => className,
            HeaderRole.Comment => ClassNameLabel,
            _ => string.Empty,
        };

        private static Dictionary<HeaderRole, FunctionLibrary.StyleSnapshot> CaptureHeaderStyles(
            IXLWorksheet ws, int col, Dictionary<HeaderRole, int> roles)
        {
            var d = new Dictionary<HeaderRole, FunctionLibrary.StyleSnapshot>();
            foreach (var kv in roles)
                d[kv.Key] = FunctionLibrary.CaptureStyle(ws.Cell(kv.Value, col).Style);
            return d;
        }

        private static CapturedCell?[] CaptureColumnData(IXLWorksheet ws, int col, int dataStart, int dataCount)
        {
            var arr = new CapturedCell?[dataCount];
            for (int k = 0; k < dataCount; k++)
            {
                var cell = ws.Cell(dataStart + k, col);
                var style = FunctionLibrary.CaptureStyle(cell.Style);
                arr[k] = cell.HasFormula
                    ? new CapturedCell(default, cell.FormulaA1, style)
                    : new CapturedCell(cell.Value, null, style);
            }
            return arr;
        }

        private static void EmitColumnHeader(
            IXLWorksheet ws, int outCol,
            Dictionary<HeaderRole, int> targetRole,
            Func<HeaderRole, string> valueFor,
            Dictionary<HeaderRole, FunctionLibrary.StyleSnapshot>? capturedHeader,
            FunctionLibrary.StyleSnapshot? fallback)
        {
            foreach (var kv in targetRole)
            {
                var cell = ws.Cell(kv.Value, outCol);
                cell.Value = valueFor(kv.Key);
                if (capturedHeader != null && capturedHeader.TryGetValue(kv.Key, out var st))
                    FunctionLibrary.ApplyStyle(cell, st);
                else if (capturedHeader != null && capturedHeader.TryGetValue(HeaderRole.Var, out var vst))
                    FunctionLibrary.ApplyStyle(cell, vst); // 目标新增角色行（如新出现的子字段行）借父行样式
                else if (fallback.HasValue)
                    FunctionLibrary.ApplyStyle(cell, fallback.Value);
            }
        }

        private static void EmitColumnData(
            IXLWorksheet ws, int outCol, int targetHeaderRows,
            CapturedCell?[] data, bool styleOnly)
        {
            for (int k = 0; k < data.Length; k++)
            {
                if (data[k] is not { } cc) continue;
                var cell = ws.Cell(targetHeaderRows + 1 + k, outCol);
                FunctionLibrary.ApplyStyle(cell, cc.Style);
                if (styleOnly) continue;
                if (cc.Formula != null) cell.FormulaA1 = cc.Formula;
                else cell.Value = cc.Value;
            }
        }

        // ── 字段展开 ──────────────────────────────────────────────────────

        /// <summary>
        /// 将 BeanField 列表展开为 ExcelColumnMeta 列表。
        /// 结构体 Bean 展开为多列（每个子字段一列）。
        /// </summary>
        public static List<ExcelColumnMeta> ExpandFields(
            List<BeanField> fields,
            Dictionary<string, BeanInfo> beanMap,
            HashSet<string> usedAsField)
        {
            var result = new List<ExcelColumnMeta>();

            foreach (var field in fields)
            {
                var simpleName = BeanParser.StripNamespace(field.Type);

                if (beanMap.TryGetValue(simpleName, out var structBean)
                    && (structBean.Sep != null || usedAsField.Contains(simpleName))
                    && structBean.Fields.Count > 0)
                {
                    // 结构体 Bean 展开：首列写父字段名和 Bean 类型名，子列只写子字段名
                    bool first = true;
                    foreach (var sub in structBean.Fields)
                    {
                        result.Add(new ExcelColumnMeta(
                            Key:        $"{field.Name}.{sub.Name}",
                            VarName:    first ? field.Name : string.Empty,
                            TypeName:   first ? simpleName : string.Empty,
                            SubVarName: sub.Name,
                            Group:      EffectiveGroup(sub.Group),
                            Comment:    sub.Comment,
                            GroupKey:   field.Name
                        ));
                        first = false;
                    }
                }
                else
                {
                    // 普通字段：单列
                    result.Add(new ExcelColumnMeta(
                        Key:        field.Name,
                        VarName:    field.Name,
                        TypeName:   field.Type,
                        SubVarName: string.Empty,
                        Group:      EffectiveGroup(field.Group),
                        Comment:    field.Comment,
                        GroupKey:   field.Name
                    ));
                }
            }

            return result;
        }

        private static string EffectiveGroup(string group)
            => string.IsNullOrEmpty(group) ? DefaultGroup : group;

        // ── xlsx Table 清理（ZIP 级别）────────────────────────────────────

        /// <summary>
        /// 从 xlsx 字节流中移除所有 Excel Table 定义及相关 XML 引用。
        /// 根因：模板的 Table Header Row 可能与 Luban ##group 行（行号 4）重叠。
        /// WriteHeaders 把 "c,s" 写入后，ClosedXML 会把所有 Table 列名同步为 "c,s"，
        /// SaveAs 时构建列名字典触发重复 key 异常。在 ClosedXML 层面无法直接删除
        /// Table，因此在 ZIP 层面手动剥除。
        /// </summary>
        private static byte[] StripTablesFromXlsx(byte[] xlsxBytes)
        {
            const string tableRelType =
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table";
            XNamespace nsCT  = "http://schemas.openxmlformats.org/package/2006/content-types";
            XNamespace nsRel = "http://schemas.openxmlformats.org/package/2006/relationships";
            XNamespace nsWs  = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            using var srcMs = new MemoryStream(xlsxBytes);
            using var dstMs = new MemoryStream();

            using (var srcZip = new ZipArchive(srcMs, ZipArchiveMode.Read))
            using (var dstZip = new ZipArchive(dstMs, ZipArchiveMode.Create, leaveOpen: true))
            {
                var tableEntries = srcZip.Entries
                    .Select(e => e.FullName)
                    .Where(n => n.StartsWith("xl/tables/", StringComparison.OrdinalIgnoreCase))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var srcEntry in srcZip.Entries)
                {
                    if (tableEntries.Contains(srcEntry.FullName))
                        continue; // 丢弃 xl/tables/*.xml

                    var fn       = srcEntry.FullName;
                    var dstEntry = dstZip.CreateEntry(fn, CompressionLevel.Optimal);

                    bool isCT        = fn.Equals("[Content_Types].xml",
                                           StringComparison.OrdinalIgnoreCase);
                    bool isSheetRels = fn.StartsWith("xl/worksheets/_rels/",
                                           StringComparison.OrdinalIgnoreCase)
                                       && fn.EndsWith(".rels",
                                           StringComparison.OrdinalIgnoreCase);
                    bool isSheetXml  = fn.StartsWith("xl/worksheets/sheet",
                                           StringComparison.OrdinalIgnoreCase)
                                       && fn.EndsWith(".xml",
                                           StringComparison.OrdinalIgnoreCase);

                    if (isCT || isSheetRels || isSheetXml)
                    {
                        XDocument doc;
                        using (var s = srcEntry.Open()) doc = XDocument.Load(s);

                        if (isCT)
                            doc.Root!.Elements(nsCT + "Override")
                                     .Where(e => ((string?)e.Attribute("ContentType") ?? "")
                                                  .Contains(".table"))
                                     .Remove();
                        else if (isSheetRels)
                            doc.Root!.Elements(nsRel + "Relationship")
                                     .Where(e => ((string?)e.Attribute("Type") ?? "")
                                                  == tableRelType)
                                     .Remove();
                        else
                            doc.Root!.Element(nsWs + "tableParts")?.Remove();

                        using var dst = dstEntry.Open();
                        doc.Save(dst);
                    }
                    else
                    {
                        using var src = srcEntry.Open();
                        using var dst = dstEntry.Open();
                        src.CopyTo(dst);
                    }
                }
            }

            dstMs.Position = 0;
            return dstMs.ToArray();
        }
    }
}
