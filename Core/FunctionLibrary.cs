using System.Runtime.InteropServices;
using ClosedXML.Excel;

namespace ConfigExcelEnhancer.Core
{
    public static class FunctionLibrary
    {
        // ── 表头样式公共方法（TableDesignApplier 与 ExcelExporter 共用）──────

        /// <summary>
        /// 统计从首行起 A 列包含 <paramref name="headerSymbol"/> 的连续行数（表头行数）。
        /// </summary>
        public static int CountHeaderRows(IXLWorksheet ws, string headerSymbol)
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

        /// <summary>
        /// 插入或删除表头行，使表头行数从 <paramref name="currentCount"/> 变为 <paramref name="targetCount"/>。
        /// 新插入的行从原第一行完整复制单元格级样式（填充、边框、对齐、字体），
        /// 已有行保持其模板格式不变。
        /// </summary>
        public static void AdjustHeaderRows(
            IXLWorksheet ws,
            int currentCount,
            int targetCount,
            int lastCol)
        {
            int diff = targetCount - currentCount;

            if (diff > 0)
            {
                // === 在插入行前，快照第一行的边框及单元格样式 ===
                // InsertRowsAbove 后行会下移，ClosedXML 对从未赋值的空单元格
                // 样式保留可能不可靠。插入前读取可确保获取到原始样式对象。

                // 优先扫描行级样式获取代表性边框，再逐单元格扫描。
                // 以普通值存储四个方向和颜色，避免持有 IXLStyle 引用。
                var rowBorder = ws.Row(1).Style.Border;
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
                        var cb = ws.Cell(1, c).Style.Border;
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
                    var s = ws.Cell(1, c).Style;
                    cellAlignFont[c] = (
                        s.Alignment.Horizontal, s.Alignment.Vertical, s.Alignment.WrapText,
                        s.Font.Bold, s.Font.FontSize, s.Font.FontName, s.Font.FontColor);
                }

                // === 插入行后，将快照的样式应用到新行 ===
                ws.Row(1).InsertRowsAbove(diff);

                int lastHeaderRow = diff + currentCount;

                for (int r = diff; r >= 1; r--)
                {
                    // 通过行级样式应用边框，使该行所有单元格都被覆盖，
                    // 包括当前无数据的 lastCol 之外的单元格。
                    var destRowStyle = ws.Row(r).Style;
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
                        var d = ws.Cell(r, c).Style;
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
                    if (fillSrc >= 1 && currentCount > 0)
                        CopyRowFillOnly(ws, fillSrc, r, lastCol);
                }
            }
            else if (diff < 0)
            {
                int n = -diff;
                int col = ws.LastColumnUsed()?.ColumnNumber() ?? lastCol;
                ws.Range(1, 1, n, col).Delete(XLShiftDeletedCells.ShiftCellsUp);
            }
        }

        /// <summary>
        /// 将表头样式扩展到 <paramref name="refLastCol"/> 之外、直到 <paramref name="newLastCol"/> 的新列。
        /// 使用倒数第二列作为内部单元格参考，避免外边框渗入应为内部的单元格。
        /// </summary>
        public static void ExtendHeaderRowStyles(
            IXLWorksheet ws,
            int headerCount,
            int refLastCol,
            int newLastCol)
        {
            // 内部参考列：避免最右列（可能有右外边框）
            int refCol = refLastCol >= 2 ? refLastCol - 1 : refLastCol;

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

                for (int c = refLastCol + 1; c <= newLastCol; c++)
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

        /// <summary>
        /// 合并表头中的空白单元格。<paramref name="keywords"/> 中每项为从 1 开始的行号
        /// （如 "3"）或文本模式（如 "##type"）。行号匹配或 A 列包含该模式时处理该行：
        /// 从某个非空单元格起，向右合并其后连续的空单元格。
        /// </summary>
        public static void MergeHeaderEmptyCells(
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

        /// <summary>
        /// 按列内容自动调整 1..<paramref name="lastCol"/> 的列宽。
        /// </summary>
        public static void AutoFitColumns(IXLWorksheet ws, int lastCol)
        {
            for (int c = 1; c <= lastCol; c++)
                ws.Column(c).AdjustToContents();
        }

        // 当边框/对齐/字体来自不同源行时使用：仅复制填充色。
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

        private static bool IsEffectivelyEmpty(IXLCell cell)
            => cell.IsEmpty() || string.IsNullOrWhiteSpace(cell.GetString());

        /// <summary>
        /// 用 Excel COM 批量刷新公式缓存值。必须在 STA 线程上调用。
        /// </summary>
        /// <returns>true = 刷新已执行；false = 本机未安装 Excel，已跳过。</returns>
        public static bool RefreshFormulasViaExcel(IReadOnlyList<string> filePaths)
        {
            var excelType = Type.GetTypeFromProgID("Excel.Application");
            if (excelType == null) return false;

            dynamic? excel = null;
            try
            {
                excel = Activator.CreateInstance(excelType)!;
                excel.Visible = false;
                excel.DisplayAlerts = false;
                excel.ScreenUpdating = false;

                foreach (var path in filePaths)
                {
                    dynamic workbook = excel.Workbooks.Open(path);
                    workbook.Save();
                    workbook.Close(SaveChanges: false);
                    Marshal.ReleaseComObject(workbook);
                }
            }
            finally
            {
                try { excel?.Quit(); } catch { }
                if (excel != null) Marshal.ReleaseComObject(excel);
            }
            return true;
        }

        /// <summary>
        /// 在任意线程调用：内部自动创建 STA 线程执行 COM 刷新。
        /// </summary>
        public static bool RefreshFormulasViaSTA(IReadOnlyList<string> filePaths)
        {
            bool result = false;
            var sta = new Thread(() => result = RefreshFormulasViaExcel(filePaths));
            sta.SetApartmentState(ApartmentState.STA);
            sta.Start();
            sta.Join();
            return result;
        }

        // ── 路径工具 ─────────────────────────────────────────────────────────

        /// <summary>
        /// 从 <paramref name="startDir"/> 开始逐级向上查找名为 <paramref name="projectName"/> 的兄弟目录。
        /// 最多向上查找 4 层，找到则返回其完整路径，否则返回 null。
        /// <para>
        /// <paramref name="fuzzy"/> 为 true 时启用模糊匹配：忽略大小写，并将连字符、下划线、空格
        /// 统一移除后再比较（如 "GodsClash" 可匹配 "gods-clash"、"gods_clash"）。
        /// </para>
        /// </summary>
        public static string? TryFindProjectRoot(string projectName, string startDir, bool fuzzy = false)
        {
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(startDir))
                return null;

            var normalizedTarget = fuzzy ? NormalizeProjectName(projectName) : null;

            var dir = startDir;
            for (int i = 0; i < 4; i++)
            {
                var parent = Path.GetDirectoryName(dir);
                if (parent == null) break;

                foreach (var sibling in Directory.EnumerateDirectories(parent))
                {
                    var siblingName = Path.GetFileName(sibling);
                    bool match = fuzzy
                        ? NormalizeProjectName(siblingName) == normalizedTarget
                        : string.Equals(siblingName, projectName, StringComparison.OrdinalIgnoreCase);
                    if (match)
                        return sibling;
                }

                dir = parent;
            }
            return null;
        }

        private static string NormalizeProjectName(string name)
            => name.Replace("-", "").Replace("_", "").Replace(" ", "").ToLowerInvariant();

        /// <summary>
        /// 将 <paramref name="absolutePath"/> 转为相对于 <paramref name="baseDir"/> 的相对路径。
        /// 跨盘符或转换失败时原样返回绝对路径。
        /// <paramref name="baseDir"/> 为空时直接返回原路径（无法计算相对关系）。
        /// </summary>
        public static string ToProjectRelative(string absolutePath, string baseDir)
        {
            if (string.IsNullOrEmpty(absolutePath)) return absolutePath;
            if (string.IsNullOrEmpty(baseDir)) return absolutePath;
            try
            {
                var rel = Path.GetRelativePath(baseDir, Path.GetFullPath(absolutePath));
                return Path.IsPathRooted(rel) ? absolutePath : rel;
            }
            catch { return absolutePath; }
        }

        /// <summary>
        /// 将 <paramref name="path"/> 还原为绝对路径。
        /// <list type="bullet">
        ///   <item>若 path 已是绝对路径（旧版 settings.json 兼容）则原样返回。</item>
        ///   <item>若 <paramref name="baseDir"/> 为空则以 <see cref="AppContext.BaseDirectory"/> 为基准（旧行为回退）。</item>
        /// </list>
        /// </summary>
        public static string ToAbsoluteFromRoot(string path, string baseDir)
        {
            if (string.IsNullOrEmpty(path)) return path;
            if (Path.IsPathRooted(path)) return path;
            var effectiveBase = string.IsNullOrEmpty(baseDir) ? AppContext.BaseDirectory : baseDir;
            try { return Path.GetFullPath(Path.Combine(effectiveBase, path)); }
            catch { return path; }
        }

        // ── 命名规范转换 ──────────────────────────────────────────────────────

        /// <summary>
        /// 按命名规范转换名称：0 = 类名不变，1 = 驼峰（首字母大写），2 = 全小写_下划线。
        /// </summary>
        public static string ApplyNameConvention(string name, int convention) => convention switch
        {
            1 => ToCamelCase(name),
            2 => ToSnakeCase(name),
            _ => name,
        };

        private static string ToCamelCase(string name)
            => string.IsNullOrEmpty(name) ? name : char.ToUpper(name[0]) + name[1..];

        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c) && i > 0)
                {
                    bool prevIsLower = char.IsLower(name[i - 1]);
                    bool nextIsLower = i + 1 < name.Length && char.IsLower(name[i + 1]);
                    if (prevIsLower || nextIsLower)
                        sb.Append('_');
                }
                sb.Append(char.ToLower(c));
            }
            return sb.ToString();
        }
    }
}
