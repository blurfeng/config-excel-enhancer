namespace ConfigExcelEnhancer.Core
{
    /// <summary>
    /// 日志输出级别。决定日志行的颜色和标签前缀。
    /// </summary>
    public enum LogLevel
    {
        Info,       // 普通信息（蓝色）
        Ok,         // 成功结果（绿色）
        Skip,       // 跳过项（灰色）
        Warn,       // 警告（黄色）
        Error,      // 错误（红色）
        Section     // 分节标题 / 分隔线（深绿色）
    }

    /// <summary>
    /// 向 RichTextBox 写入带颜色和时间戳日志的工具类。
    /// 所有方法均支持跨线程调用（内部自动通过 BeginInvoke 派发到 UI 线程）。
    /// </summary>
    public static class LogLibrary
    {
        // 颜色定义（唯一出处，供外部直接引用）
        public static readonly Color ClrOk      = Color.LightGreen;
        public static readonly Color ClrSkip    = Color.FromArgb(150, 150, 150);
        public static readonly Color ClrWarn    = Color.FromArgb(255, 200, 60);
        public static readonly Color ClrError   = Color.OrangeRed;
        public static readonly Color ClrInfo    = Color.FromArgb(100, 200, 255);
        public static readonly Color ClrSection = Color.FromArgb(80, 160, 80);

        // 将 LogLevel 映射到（颜色, 标签文本）
        private static readonly Dictionary<LogLevel, (Color color, string tag)> LevelMeta = new()
        {
            [LogLevel.Info]    = (ClrInfo,    "INFO"),
            [LogLevel.Ok]      = (ClrOk,      " OK "),
            [LogLevel.Skip]    = (ClrSkip,    "SKIP"),
            [LogLevel.Warn]    = (ClrWarn,    "WARN"),
            [LogLevel.Error]   = (ClrError,   "ERR "),
            [LogLevel.Section] = (ClrSection, "----"),
        };

        /// <summary>
        /// 向 RichTextBox 追加一行带时间戳和级别标签的日志。线程安全。
        /// 格式：[HH:mm:ss] [TAG] message
        /// </summary>
        public static void Write(RichTextBox rtb, string message, LogLevel level = LogLevel.Ok)
        {
            void DoWrite()
            {
                var (color, tag) = LevelMeta[level];
                rtb.SelectionStart  = rtb.TextLength;
                rtb.SelectionLength = 0;
                rtb.SelectionColor  = color;
                rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] [{tag}] {message}{Environment.NewLine}");
                rtb.ScrollToCaret();
            }

            if (rtb.InvokeRequired)
                rtb.BeginInvoke(DoWrite);
            else
                DoWrite();
        }

        /// <summary>
        /// 向 RichTextBox 追加一条 60 字符宽的横线分隔符。线程安全。
        /// </summary>
        public static void Divider(RichTextBox rtb)
        {
            void DoWrite()
            {
                rtb.SelectionStart  = rtb.TextLength;
                rtb.SelectionLength = 0;
                rtb.SelectionColor  = ClrSection;
                rtb.AppendText($"{"─".PadRight(60, '─')}{Environment.NewLine}");
                rtb.ScrollToCaret();
            }

            if (rtb.InvokeRequired)
                rtb.BeginInvoke(DoWrite);
            else
                DoWrite();
        }

        /// <summary>
        /// 向 RichTextBox 追加一行原始文本（无时间戳/标签），可指定颜色。线程安全。
        /// 主要用于直接转发子进程的 stdout/stderr 输出。
        /// </summary>
        public static void WriteRaw(RichTextBox rtb, string message, Color? color = null)
        {
            void DoWrite()
            {
                rtb.SelectionStart  = rtb.TextLength;
                rtb.SelectionLength = 0;
                rtb.SelectionColor  = color ?? ClrOk;
                rtb.AppendText($"{message}{Environment.NewLine}");
                rtb.ScrollToCaret();
            }
            if (rtb.InvokeRequired) rtb.BeginInvoke(DoWrite);
            else DoWrite();
        }

        /// <summary>
        /// 原地替换 RichTextBox 最后一行内容（不新增行），用于进度刷新。线程安全。
        /// 若日志为空则等同于 Write。
        /// </summary>
        public static void UpdateLastLine(RichTextBox rtb, string message, LogLevel level = LogLevel.Ok)
        {
            void DoUpdate()
            {
                var (color, tag) = LevelMeta[level];
                string newLine = $"[{DateTime.Now:HH:mm:ss}] [{tag}] {message}";

                // 找最后一行的起始位置（最后一个换行符之后）
                string text = rtb.Text;
                // 末尾可能有 \n，去掉后找上一个换行
                string trimmed = text.TrimEnd('\n').TrimEnd('\r');
                int lastNl = trimmed.LastIndexOf('\n');
                int lineStart = lastNl < 0 ? 0 : lastNl + 1;

                rtb.SelectionStart  = lineStart;
                rtb.SelectionLength = rtb.TextLength - lineStart;
                rtb.SelectionColor  = color;
                rtb.SelectedText    = newLine;

                // 确保末尾有换行
                if (!rtb.Text.EndsWith(Environment.NewLine))
                {
                    rtb.SelectionStart  = rtb.TextLength;
                    rtb.SelectionLength = 0;
                    rtb.AppendText(Environment.NewLine);
                }
                rtb.ScrollToCaret();
            }

            if (rtb.InvokeRequired)
                rtb.BeginInvoke(DoUpdate);
            else
                DoUpdate();
        }

        /// <summary>返回指定日志级别对应的显示颜色。</summary>
        public static Color LevelToColor(LogLevel level) => LevelMeta[level].color;
    }
}
