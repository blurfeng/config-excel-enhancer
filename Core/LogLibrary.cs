namespace ConfigExcelEnhancer.Core
{
    public enum LogLevel { Info, Ok, Skip, Warn, Error, Section }

    public static class LogLibrary
    {
        // 颜色定义（唯一出处）
        public static readonly Color ClrOk      = Color.LightGreen;
        public static readonly Color ClrSkip    = Color.FromArgb(150, 150, 150);
        public static readonly Color ClrWarn    = Color.FromArgb(255, 200, 60);
        public static readonly Color ClrError   = Color.OrangeRed;
        public static readonly Color ClrInfo    = Color.FromArgb(100, 200, 255);
        public static readonly Color ClrSection = Color.FromArgb(80, 160, 80);

        private static readonly Dictionary<LogLevel, (Color color, string tag)> LevelMeta = new()
        {
            [LogLevel.Info]    = (ClrInfo,    "INFO"),
            [LogLevel.Ok]      = (ClrOk,      " OK "),
            [LogLevel.Skip]    = (ClrSkip,    "SKIP"),
            [LogLevel.Warn]    = (ClrWarn,    "WARN"),
            [LogLevel.Error]   = (ClrError,   "ERR "),
            [LogLevel.Section] = (ClrSection, "----"),
        };

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

        public static Color LevelToColor(LogLevel level) => LevelMeta[level].color;
    }
}
