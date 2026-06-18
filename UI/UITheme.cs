namespace ConfigExcelEnhancer.UI
{
    // UI 尺寸/间距/颜色常量，供各页与 Designer.cs 参照保持一致
    internal static class UITheme
    {
        public static readonly Size BtnMain   = new(110, 32); // 主操作按钮（执行、应用、保存）
        public static readonly Size BtnStop   = new(80,  32); // 停止/取消/重置按钮
        public static readonly Size BtnBrowse = new(75,  28); // 浏览按钮
        public const int PadOuter = 12; // 控件距左/上边缘
        public const int PadRow   = 8;  // 行间距

        // ── 颜色：状态指示 ──
        public static readonly Color StateValid   = Color.LightGreen;              // 配置有效/通过
        public static readonly Color StateWarning = Color.Yellow;                  // 需注意
        public static readonly Color StateInvalid = Color.Red;                     // 配置无效
        public static readonly Color RowDirty     = Color.FromArgb(255, 255, 200); // 未保存修改行底色
        public static readonly Color RowDirtySel  = Color.FromArgb(200, 195, 120); // 未保存修改行选中底色
        public static readonly Color CellWarnBack = Color.FromArgb(255, 224, 178); // 失效目标路径警示底色（淡橙，区别于脏行淡黄）
    }
}
