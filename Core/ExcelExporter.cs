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

            var columns = ExpandFields(task.AllFields, beanMap, usedAsField);

            // templateMs 必须在 wb.SaveAs 完成后才能 Dispose：ClosedXML 的 SaveAs
            // 会惰性复制模板里未修改的 parts（打印设置等），若流提前关闭则报
            // "Cannot access a closed Stream"。
            MemoryStream? templateMs = null;
            IXLWorkbook wb;
            if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
            {
                log($"  从模板复制：{Path.GetFileName(templatePath)}", LogLevel.Info);
                // 剥除模板中的 Table 结构：模板的 Table Header Row 可能与 Luban
                // 的 ##group 行重叠，WriteHeaders 把 "c,s" 写入后 ClosedXML 会把
                // 所有 Table 列名同步为 "c,s"，导致 SaveAs 时抛出重复 key 异常。
                var templateBytes = StripTablesFromXlsx(File.ReadAllBytes(templatePath));
                templateMs = new MemoryStream(templateBytes);
                wb = new XLWorkbook(templateMs);
                var firstWs = wb.Worksheets.First();
                firstWs.AutoFilter.Clear();
                // 清空第一张 Sheet 的内容（保留样式）
                firstWs.RangeUsed()?.Clear(XLClearOptions.Contents);
            }
            else
            {
                wb = new XLWorkbook();
                wb.AddWorksheet(task.LeafBean.Name);
            }

            var ws = wb.Worksheets.First();
            ws.Name = task.LeafBean.Name;

            WriteHeaders(ws, task.LeafBean.Name, columns);

            wb.SaveAs(task.TargetExcelPath);
            wb.Dispose();
            templateMs?.Dispose(); // SaveAs 完成后再关闭
        }

        // ── 更新（保留数据）────────────────────────────────────────────────

        private static void UpdateExcel(
            ExcelExportTask task,
            Dictionary<string, BeanInfo> beanMap,
            HashSet<string> usedAsField,
            Action<string, LogLevel> log)
        {
            var targetColumns = ExpandFields(task.AllFields, beanMap, usedAsField);

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
            var ws = wb.Worksheets.First();

            // 检测现有表头
            var headerInfo = DetectHeaders(ws);
            if (headerInfo.DataStartRow < 0)
            {
                // 找不到有效表头结构，视为新建
                log($"  未检测到有效表头，重建表头结构", LogLevel.Warn);
                ws.RangeUsed()?.Clear(XLClearOptions.Contents);
                WriteHeaders(ws, task.LeafBean.Name, targetColumns);
                if (loadedFromStream) wb.SaveAs(task.TargetExcelPath); else wb.Save();
                wb.Dispose();
                strippedMs?.Dispose();
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

            // 收集需要删除的列（从右到左，防止索引偏移）
            var colsToDelete = existingGroups
                .Where(g => toDelete.Contains(g.Key))
                .SelectMany(g => Enumerable.Range(g.StartCol, g.ColCount))
                .OrderByDescending(c => c)
                .ToList();

            foreach (var col in colsToDelete)
                ws.Column(col).Delete();

            // 处理表头行数变化（是否需要子字段行）
            bool needsSubRow = targetColumns.Any(c => !string.IsNullOrEmpty(c.SubVarName));
            bool hadSubRow   = headerInfo.SubVarRow > 0;

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

            // 在末尾追加新增字段的列
            if (toAdd.Count > 0)
            {
                int nextCol = (ws.LastColumnUsed()?.ColumnNumber() ?? 1) + 1;
                var newCols = targetColumns.Where(c => toAdd.Contains(c.GroupKey)).ToList();
                AppendColumns(ws, headerInfo, newCols, nextCol, task.LeafBean.Name);
            }

            if (loadedFromStream)
                wb.SaveAs(task.TargetExcelPath); // 从 MemoryStream 加载时须用 SaveAs
            else
                wb.Save();
            wb.Dispose();
            strippedMs?.Dispose(); // SaveAs 完成后再关闭
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

            // B 列（$type）
            ws.Cell(varRow,     2).Value = TypeColumnLabel;
            ws.Cell(typeRow,    2).Value = className;
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
