using ClosedXML.Excel;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    public record TableDesignOptions(
        string SourceExcelPath,
        IReadOnlyList<string> TargetFiles,
        bool IgnoreUnderscoreFiles,
        int SheetScope,            // 0=all, 1=first only
        bool IgnoreUnderscoreSheets,
        string HeaderSymbol,
        bool AutoColumnWidth,
        bool MergeHeaderCells,
        string MergeHeaderKeywords
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
                log($"来源文件不存在：{options.SourceExcelPath}", LogLevel.Error);
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

            // Excel COM refresh — recalculates formula cache so external tools read correct values
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

            // A. Remove __ sheets from workingWb (source internals, never in output)
            foreach (var sheet in workingWb.Worksheets.ToList())
                if (sheet.Name.StartsWith("__"))
                    sheet.Delete();

            var srcSheets = workingWb.Worksheets.OrderBy(s => s.Position).ToList();
            if (srcSheets.Count == 0)
            {
                log($"来源表中无有效Sheet，跳过：{fileName}", LogLevel.Warn);
                return;
            }

            // Determine which target sheets receive design (filtered by scope/options)
            var sheetsToProcess = targetWb.Worksheets.OrderBy(s => s.Position).ToList();
            if (options.SheetScope == 1)
                sheetsToProcess = sheetsToProcess.Take(1).ToList();
            if (options.IgnoreUnderscoreSheets)
                sheetsToProcess = sheetsToProcess.Where(s => !s.Name.StartsWith("__")).ToList();

            // B. Temp rename all source sheets to avoid name collisions with target names
            for (int i = 0; i < srcSheets.Count; i++)
                srcSheets[i].Name = $"__td_tmp_{i}__";

            // C. Match by position — apply design to each pair (no name check needed)
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

            // D. Delete extra source sheets that have no target counterpart
            for (int i = pairCount; i < srcSheets.Count; i++)
                srcSheets[i].Delete();

            // E. Copy target sheets not covered by design (__ sheets, out-of-range sheets)
            foreach (var t3 in targetWb.Worksheets.OrderBy(s => s.Position))
            {
                if (!workingWb.TryGetWorksheet(t3.Name, out _))
                {
                    t3.CopyTo(workingWb, t3.Name);
                    log($"  → 保留 [{t3.Name}]", LogLevel.Skip);
                }
            }

            // F. Restore sheet ordering to match target
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
            // 1. Count T2 headers BEFORE clearing (text content still present)
            int t2HeaderCount = CountHeaderRows(t2, options.HeaderSymbol);

            // 2. Unmerge all existing merged ranges (ensures CopyDataOnly writes correctly)
            foreach (var mr in t2.MergedRanges.ToList())
                mr.Unmerge();

            // 3. Clear T2 data, keep styles
            int t2LastRow = t2.LastRowUsed()?.RowNumber() ?? 0;
            int t2LastCol = t2.LastColumnUsed()?.ColumnNumber() ?? 0;
            if (t2LastRow > 0)
                t2.Range(1, 1, t2LastRow, t2LastCol).Clear(XLClearOptions.Contents);

            // 4. Adjust header row count to match T3
            // Cell-level styles (fill, border, alignment, font) survive the Clear above.
            // AdjustHeaderRows copies styles from the original first header row to any new rows.
            int t3HeaderCount = CountHeaderRows(t3, options.HeaderSymbol);
            if (t3HeaderCount != t2HeaderCount)
                AdjustHeaderRows(t2, t2HeaderCount, t3HeaderCount, t2LastCol);

            // Capture T2 row extent before data copy so we know which rows are "extra"
            int t2RowExtent = t2.LastRowUsed()?.RowNumber() ?? t3HeaderCount;

            // 5. Copy T3 data into T2 (values/formulas only, no styles)
            CopyDataOnly(t3, t2);

            // 6. Reset freeze pane to cover header rows
            t2.SheetView.FreezeRows(t3HeaderCount);

            // Determine data extent after copy
            int dataLastRow = t2.LastRowUsed()?.RowNumber() ?? 1;
            int dataLastCol = t2.LastColumnUsed()?.ColumnNumber() ?? 1;

            // Extend header row styles to columns that exist in T3 but not in the T2 template.
            // Uses a representative inner cell (not the edge cell) as the style source.
            if (dataLastCol > t2LastCol && t2LastCol > 0)
                ExtendHeaderRowStyles(t2, t3HeaderCount, t2LastCol, dataLastCol);

            // Propagate first-data-row alignment to any rows beyond the template extent
            int firstDataRow = t3HeaderCount + 1;
            if (dataLastRow > t2RowExtent && firstDataRow <= t2RowExtent)
            {
                var templateRow = t2.Row(firstDataRow);
                for (int r = t2RowExtent + 1; r <= dataLastRow; r++)
                    CopyRowAlignment(templateRow, t2.Row(r));
            }

            // 7. Merge header cells based on keyword rows / explicit row numbers
            // Supports comma and semicolon as separators, e.g. "##type,1;2"
            if (options.MergeHeaderCells && !string.IsNullOrWhiteSpace(options.MergeHeaderKeywords))
            {
                var keywords = options.MergeHeaderKeywords
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (keywords.Length > 0)
                    DoMergeHeaderCells(t2, keywords, dataLastRow, dataLastCol);
            }

            // 8. Resize smart tables to cover new data extent
            ResizeTables(t2, dataLastRow, dataLastCol);

            // 9. Auto-fit column widths (last, after all content is finalized)
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

        // Inserts or removes header rows so T2 matches targetCount.
        // Newly inserted rows receive a full cell-level style copy from the original first header row,
        // preserving fill, borders, alignment and font exactly as they appear in the template.
        // Existing rows are never re-styled — their template formatting is kept as-is.
        private static void AdjustHeaderRows(
            IXLWorksheet t2,
            int t2Count,
            int targetCount,
            int lastCol)
        {
            int diff = targetCount - t2Count;

            if (diff > 0)
            {
                t2.Row(1).InsertRowsAbove(diff);
                // Process from the bottom-most new row upward so that r+2 (which may itself be a
                // newly-inserted row) already has its fill set before we read from it.
                int lastHeaderRow = diff + t2Count;   // last row of the original header block
                for (int r = diff; r >= 1; r--)
                {
                    // Copy borders, alignment and font from the original first header row.
                    CopyRowCellStylesExceptFill(t2, diff + 1, r, lastCol);

                    // Fill color: take from the row two positions below (r+2), clamped to header range.
                    int fillSrc = r + 2;
                    if (fillSrc > lastHeaderRow) fillSrc = lastHeaderRow;
                    if (fillSrc >= 1 && t2Count > 0)
                        CopyRowFillOnly(t2, fillSrc, r, lastCol);
                    // else: no original header rows exist → leave fill unset
                }
            }
            else if (diff < 0)
            {
                int n = -diff;
                int col = t2.LastColumnUsed()?.ColumnNumber() ?? lastCol;
                t2.Range(1, 1, n, col).Delete(XLShiftDeletedCells.ShiftCellsUp);
            }
        }

        // Extends styles from the T2 header rows to columns that exist in T3 but not in the template.
        // Uses the second-to-last template column as the inner-cell reference so that edge borders
        // don't bleed into what should be interior cells.
        private static void ExtendHeaderRowStyles(
            IXLWorksheet ws,
            int headerCount,
            int templateLastCol,
            int newLastCol)
        {
            // Inner reference: avoid the rightmost column (may have a right-edge border)
            int refCol = templateLastCol >= 2 ? templateLastCol - 1 : templateLastCol;

            for (int hr = 1; hr <= headerCount; hr++)
            {
                var refStyle  = ws.Cell(hr, refCol).Style;
                var rowFill   = ws.Row(hr).Style.Fill;

                // Determine effective fill: row-level fill takes priority over cell-level
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

        // Each entry in keywords is either a 1-based row number (e.g. "3") or a text pattern
        // (e.g. "##type"). A row is processed if its number matches OR its A-column contains a pattern.
        private static void DoMergeHeaderCells(
            IXLWorksheet ws,
            string[] keywords,
            int lastRow,
            int lastCol)
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

        // Copies borders, alignment and font from srcRow to destRow, but skips fill color.
        // Used when fill should come from a different source row.
        private static void CopyRowCellStylesExceptFill(IXLWorksheet ws, int srcRow, int destRow, int lastCol)
        {
            for (int c = 1; c <= lastCol; c++)
            {
                var s = ws.Cell(srcRow, c).Style;
                var d = ws.Cell(destRow, c).Style;

                d.Border.TopBorder          = s.Border.TopBorder;
                d.Border.TopBorderColor     = s.Border.TopBorderColor;
                d.Border.BottomBorder       = s.Border.BottomBorder;
                d.Border.BottomBorderColor  = s.Border.BottomBorderColor;
                d.Border.LeftBorder         = s.Border.LeftBorder;
                d.Border.LeftBorderColor    = s.Border.LeftBorderColor;
                d.Border.RightBorder        = s.Border.RightBorder;
                d.Border.RightBorderColor   = s.Border.RightBorderColor;

                d.Alignment.Horizontal = s.Alignment.Horizontal;
                d.Alignment.Vertical   = s.Alignment.Vertical;
                d.Alignment.WrapText   = s.Alignment.WrapText;

                d.Font.Bold      = s.Font.Bold;
                d.Font.FontSize  = s.Font.FontSize;
                d.Font.FontName  = s.Font.FontName;
                d.Font.FontColor = s.Font.FontColor;
            }
        }

        // Copies only fill color (row-level and cell-level) from srcRow to destRow.
        // Used when borders/alignment/font come from a different source row.
        private static void CopyRowFillOnly(IXLWorksheet ws, int srcRow, int destRow, int lastCol)
        {
            // Row-level fill — covers every cell that has no explicit cell-level fill
            var srcRowFill = ws.Row(srcRow).Style.Fill;
            var dstRowStyle = ws.Row(destRow).Style;
            dstRowStyle.Fill.BackgroundColor = srcRowFill.BackgroundColor;
            dstRowStyle.Fill.PatternType     = srcRowFill.PatternType;

            for (int c = 1; c <= lastCol; c++)
            {
                var s = ws.Cell(srcRow, c).Style;
                var d = ws.Cell(destRow, c).Style;

                // Cell-level fill: only copy when the cell has an explicit fill (not NoColor),
                // otherwise the row-level fill applied above is the correct fallback.
                if (!s.Fill.BackgroundColor.Equals(XLColor.NoColor))
                {
                    d.Fill.BackgroundColor = s.Fill.BackgroundColor;
                    d.Fill.PatternType     = s.Fill.PatternType;
                }
            }
        }

        // Copies row-level alignment (used for extra data rows beyond the template extent).
        private static void CopyRowAlignment(IXLRow source, IXLRow dest)
        {
            var src = source.Style.Alignment;
            dest.Style.Alignment.Horizontal = src.Horizontal;
            dest.Style.Alignment.Vertical   = src.Vertical;
            dest.Style.Alignment.WrapText    = src.WrapText;
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
