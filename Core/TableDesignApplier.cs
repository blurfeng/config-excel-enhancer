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
        bool HideEnumDataSheet = true   // 强制更新 Enum 验证时是否隐藏 __enum_data
    );

    public static class TableDesignApplier
    {
        public static void Apply(
            TableDesignOptions options,
            IProgress<(int current, int total, string fileName)> progress,
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
                progress.Report((i, total, fileName));

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
                    log($"处理失败 {fileName}：{ex.Message}", LogLevel.Error);
                }
            }

            progress.Report((total, total, string.Empty));

            // Enum 验证——应用表设计后强制重写所有枚举列验证规则
            if (!string.IsNullOrWhiteSpace(options.XmlDirectory) && savedFiles.Count > 0)
            {
                log("扫描 XML Enum 定义...", LogLevel.Info);
                var enums = EnumScanner.ScanDirectory(options.XmlDirectory);
                if (enums.Count > 0)
                {
                    log($"找到 {enums.Count} 个 Enum，正在强制更新验证规则...", LogLevel.Info);
                    ValidationUpdater.UpdateFiles(
                        savedFiles,
                        enums,
                        options.HideEnumDataSheet,
                        result =>
                        {
                            if (result.HasError)
                                log($"  [Enum] 错误 {result.FileName}：{result.ErrorMessage}", LogLevel.Error);
                            else if (result.WasSaved)
                                log($"  [Enum] 已更新：{result.FileName}", LogLevel.Ok);
                        },
                        forceRewrite: true);
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
            int t2HeaderCount = CountHeaderRows(t2, options.HeaderSymbol);

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
            int t3HeaderCount = CountHeaderRows(t3, options.HeaderSymbol);
            if (t3HeaderCount != t2HeaderCount)
                AdjustHeaderRows(t2, t2HeaderCount, t3HeaderCount, t2LastCol);

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
                ExtendHeaderRowStyles(t2, t3HeaderCount, t2LastCol, dataLastCol);

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
                    DoMergeHeaderCells(t2, keywords, dataLastRow, dataLastCol, options.HeaderSymbol);
            }

            // 8. 调整智能表格尺寸以覆盖新的数据范围
            ResizeTables(t2, dataLastRow, dataLastCol);

            // 9. 自动调整列宽（最后执行，确保内容已全部确定）
            if (options.AutoColumnWidth)
            {
                for (int c = 1; c <= dataLastCol; c++)
                    t2.Column(c).AdjustToContents();
            }
        }

        private static int CountHeaderRows(IXLWorksheet ws, string headerSymbol)
        {
            int count = 0;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            for (int r = 1; r <= lastRow; r++)
            {
                if (ws.Cell(r, 1).GetString().Contains(headerSymbol))
                    count++;
                else
                    break;
            }
            return count;
        }

        // 插入或删除表头行使 T2 与 targetCount 匹配。
        // 新插入的行将从原第一行完整复制单元格级样式，
        // 精确保留填充、边框、对齐和字体等模板格式。
        // 已有行不会重新设置样式——保持其模板格式不变。
        private static void AdjustHeaderRows(
            IXLWorksheet t2,
            int t2Count,
            int targetCount,
            int lastCol)
        {
            int diff = targetCount - t2Count;

            if (diff > 0)
            {
                // === 在插入行前，快照 T2 第一行的边框及单元格样式 ===
                // InsertRowsAbove 后行会下移，ClosedXML 对从未赋值的空单元格
                // 样式保留可能不可靠。插入前读取可确保获取到原始样式对象。

                // 优先扫描行级样式获取代表性边框，再逐单元格扫描。
                // 以普通值存储四个方向和颜色，避免持有 IXLStyle 引用。
                var rowBorder = t2.Row(1).Style.Border;
                XLBorderStyleValues bTop    = rowBorder.TopBorder;
                XLBorderStyleValues bBottom = rowBorder.BottomBorder;
                XLBorderStyleValues bLeft   = rowBorder.LeftBorder;
                XLBorderStyleValues bRight  = rowBorder.RightBorder;
                XLColor cTop    = rowBorder.TopBorderColor;
                XLColor cBottom = rowBorder.BottomBorderColor;
                XLColor cLeft   = rowBorder.LeftBorderColor;
                XLColor cRight  = rowBorder.RightBorderColor;

                bool anyBorder = bTop    != XLBorderStyleValues.None ||
                                 bBottom != XLBorderStyleValues.None ||
                                 bLeft   != XLBorderStyleValues.None ||
                                 bRight  != XLBorderStyleValues.None;

                if (!anyBorder)
                {
                    // 行级样式无边框——扫描单元格找到第一个有明确边框的单元格。
                    // 优先使用内部列，避免外边缘的粗边框。
                    int preferCol = lastCol >= 2 ? lastCol - 1 : lastCol;
                    int[] scanOrder = Enumerable.Range(1, lastCol)
                        .OrderBy(c => Math.Abs(c - preferCol))
                        .ToArray();

                    foreach (int c in scanOrder)
                    {
                        var cb = t2.Cell(1, c).Style.Border;
                        if (cb.TopBorder    != XLBorderStyleValues.None ||
                            cb.BottomBorder != XLBorderStyleValues.None ||
                            cb.LeftBorder   != XLBorderStyleValues.None ||
                            cb.RightBorder  != XLBorderStyleValues.None)
                        {
                            bTop    = cb.TopBorder;    cTop    = cb.TopBorderColor;
                            bBottom = cb.BottomBorder; cBottom = cb.BottomBorderColor;
                            bLeft   = cb.LeftBorder;   cLeft   = cb.LeftBorderColor;
                            bRight  = cb.RightBorder;  cRight  = cb.RightBorderColor;
                            break;
                        }
                    }
                }

                // 在插入前快照第一行各单元格的对齐方式和字体。
                var cellAlignFont = new (XLAlignmentHorizontalValues H,
                                         XLAlignmentVerticalValues V,
                                         bool Wrap,
                                         bool Bold,
                                         double FontSize,
                                         string FontName,
                                         XLColor FontColor)[lastCol + 1];
                for (int c = 1; c <= lastCol; c++)
                {
                    var s = t2.Cell(1, c).Style;
                    cellAlignFont[c] = (
                        s.Alignment.Horizontal, s.Alignment.Vertical, s.Alignment.WrapText,
                        s.Font.Bold, s.Font.FontSize, s.Font.FontName, s.Font.FontColor);
                }

                // === 插入行后，将快照的样式应用到新行 ===
                t2.Row(1).InsertRowsAbove(diff);

                int lastHeaderRow = diff + t2Count;

                for (int r = diff; r >= 1; r--)
                {
                    // 通过行级样式应用边框，使该行所有单元格都被覆盖，
                    // 包括当前无数据的 lastCol 之外的单元格。
                    var destRowStyle = t2.Row(r).Style;
                    destRowStyle.Border.TopBorder         = bTop;
                    destRowStyle.Border.TopBorderColor    = cTop;
                    destRowStyle.Border.BottomBorder      = bBottom;
                    destRowStyle.Border.BottomBorderColor = cBottom;
                    destRowStyle.Border.LeftBorder        = bLeft;
                    destRowStyle.Border.LeftBorderColor   = cLeft;
                    destRowStyle.Border.RightBorder       = bRight;
                    destRowStyle.Border.RightBorderColor  = cRight;

                    // 从快照应用各单元格的对齐方式和字体。
                    for (int c = 1; c <= lastCol; c++)
                    {
                        var (H, V, Wrap, Bold, FontSize, FontName, FontColor) = cellAlignFont[c];
                        var d = t2.Cell(r, c).Style;
                        d.Alignment.Horizontal = H;
                        d.Alignment.Vertical   = V;
                        d.Alignment.WrapText   = Wrap;
                        d.Font.Bold      = Bold;
                        d.Font.FontSize  = FontSize;
                        d.Font.FontName  = FontName;
                        d.Font.FontColor = FontColor;
                    }

                    // 填充色：取自下方两行（r+2）的颜色，夹紧在表头范围内。
                    int fillSrc = r + 2;
                    if (fillSrc > lastHeaderRow) fillSrc = lastHeaderRow;
                    if (fillSrc >= 1 && t2Count > 0)
                        CopyRowFillOnly(t2, fillSrc, r, lastCol);
                }
            }
            else if (diff < 0)
            {
                int n = -diff;
                int col = t2.LastColumnUsed()?.ColumnNumber() ?? lastCol;
                t2.Range(1, 1, n, col).Delete(XLShiftDeletedCells.ShiftCellsUp);
            }
        }

        // 将 T2 表头样式扩展到 T3 中有而模板中没有的列。
        // 使用倒数第二列作为内部单元格参考，
        // 避免外边框渗入应为内部的单元格。
        private static void ExtendHeaderRowStyles(
            IXLWorksheet ws,
            int headerCount,
            int templateLastCol,
            int newLastCol)
        {
            // 内部参考列：避免最右列（可能有右外边框）
            int refCol = templateLastCol >= 2 ? templateLastCol - 1 : templateLastCol;

            for (int hr = 1; hr <= headerCount; hr++)
            {
                var refStyle  = ws.Cell(hr, refCol).Style;
                var rowFill   = ws.Row(hr).Style.Fill;

                // 确定有效填充色：行级填充优先于单元格级填充
                var fillColor   = rowFill.BackgroundColor.Equals(XLColor.NoColor)
                                  ? refStyle.Fill.BackgroundColor
                                  : rowFill.BackgroundColor;
                var fillPattern = rowFill.PatternType == XLFillPatternValues.None
                                  ? refStyle.Fill.PatternType
                                  : rowFill.PatternType;

                for (int c = templateLastCol + 1; c <= newLastCol; c++)
                {
                    var d = ws.Cell(hr, c).Style;

                    if (!fillColor.Equals(XLColor.NoColor))
                    {
                        d.Fill.BackgroundColor = fillColor;
                        d.Fill.PatternType     = fillPattern;
                    }

                    d.Border.TopBorder         = refStyle.Border.TopBorder;
                    d.Border.TopBorderColor    = refStyle.Border.TopBorderColor;
                    d.Border.BottomBorder      = refStyle.Border.BottomBorder;
                    d.Border.BottomBorderColor = refStyle.Border.BottomBorderColor;
                    d.Border.LeftBorder        = refStyle.Border.LeftBorder;
                    d.Border.LeftBorderColor   = refStyle.Border.LeftBorderColor;
                    d.Border.RightBorder       = refStyle.Border.RightBorder;
                    d.Border.RightBorderColor  = refStyle.Border.RightBorderColor;

                    d.Alignment.Horizontal = refStyle.Alignment.Horizontal;
                    d.Alignment.Vertical   = refStyle.Alignment.Vertical;
                    d.Alignment.WrapText   = refStyle.Alignment.WrapText;

                    d.Font.Bold      = refStyle.Font.Bold;
                    d.Font.FontSize  = refStyle.Font.FontSize;
                    d.Font.FontName  = refStyle.Font.FontName;
                    d.Font.FontColor = refStyle.Font.FontColor;
                }
            }
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
            }
        }

        // keywords 中每项为从 1 开始的行号（如 "3"）或文本模式（如 "##type"）。
        // 行号匹配或 A 列包含该模式时处理该行。
        private static void DoMergeHeaderCells(
            IXLWorksheet ws,
            string[] keywords,
            int lastRow,
            int lastCol,
            string headerSymbol)
        {
            var rowNums = new HashSet<int>();
            var textKws = new List<string>();
            foreach (var kw in keywords)
            {
                if (int.TryParse(kw, out int n) && n >= 1)
                    rowNums.Add(n);
                else
                    textKws.Add(kw);
            }

            for (int r = 1; r <= lastRow; r++)
            {
                bool match = rowNums.Contains(r);
                if (!match && textKws.Count > 0)
                {
                    string aValue = ws.Cell(r, 1).GetString();
                    match = textKws.Any(kw => aValue.Contains(kw, StringComparison.Ordinal));
                }
                if (!match) continue;

                int c = 1;
                while (c <= lastCol)
                {
                    // 跳过包含表头符号的单元格——它们不应参与合并
                    string cellVal = ws.Cell(r, c).GetString();
                    if (!string.IsNullOrEmpty(headerSymbol) && cellVal.Contains(headerSymbol, StringComparison.Ordinal))
                    {
                        c++;
                        continue;
                    }

                    if (!IsEffectivelyEmpty(ws.Cell(r, c)))
                    {
                        int end = c;
                        while (end + 1 <= lastCol && IsEffectivelyEmpty(ws.Cell(r, end + 1)))
                            end++;

                        if (end > c)
                        {
                            ws.Range(r, c, r, end).Merge();
                            c = end + 1;
                            continue;
                        }
                    }
                    c++;
                }
            }
        }


        // 当边框/对齐/字体来自不同源行时使用。
        private static void CopyRowFillOnly(IXLWorksheet ws, int srcRow, int destRow, int lastCol)
        {
            // 行级填充——覆盖所有没有显式单元格级填充的单元格
            var srcRowFill = ws.Row(srcRow).Style.Fill;
            var dstRowStyle = ws.Row(destRow).Style;
            dstRowStyle.Fill.BackgroundColor = srcRowFill.BackgroundColor;
            dstRowStyle.Fill.PatternType     = srcRowFill.PatternType;

            for (int c = 1; c <= lastCol; c++)
            {
                var s = ws.Cell(srcRow, c).Style;
                var d = ws.Cell(destRow, c).Style;

                // 单元格级填充：仅在单元格有明确填充色（非 NoColor）时复制，
                // 否则以上方已应用的行级填充作为正确回退。
                if (!s.Fill.BackgroundColor.Equals(XLColor.NoColor))
                {
                    d.Fill.BackgroundColor = s.Fill.BackgroundColor;
                    d.Fill.PatternType     = s.Fill.PatternType;
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

        private static bool IsEffectivelyEmpty(IXLCell cell)
            => cell.IsEmpty() || string.IsNullOrWhiteSpace(cell.GetString());

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
