namespace ConfigExcelEnhancer.UI
{
    // UI 尺寸/间距常量，供各 Designer.cs 参照保持一致
    internal static class UITheme
    {
        public static readonly Size BtnMain   = new(110, 32); // 主操作按钮（执行、应用、保存）
        public static readonly Size BtnStop   = new(80,  32); // 停止/取消/重置按钮
        public static readonly Size BtnBrowse = new(75,  28); // 浏览按钮
        public const int PadOuter = 12; // 控件距左/上边缘
        public const int PadRow   = 8;  // 行间距
    }
}
