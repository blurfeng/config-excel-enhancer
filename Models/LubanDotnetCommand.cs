namespace ConfigExcelEnhancer.Models
{
    public class LubanDotnetCommand
    {
        public string Label { get; set; } = string.Empty;

        /// <summary>标准参数：t, d, c, conf（只读展示，不写回 bat）</summary>
        public Dictionary<string, string> Args { get; set; } = [];

        /// <summary>-x 扩展参数：key → value（可编辑，写回 bat）</summary>
        public Dictionary<string, string> XArgs { get; set; } = [];
    }
}
