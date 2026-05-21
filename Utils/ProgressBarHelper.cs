namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 进度条统一封装工具。
    /// 调用方统一传入 0-100 的逻辑进度值：
    ///   ≤ 0  → 显示进度条并设为 10（防止起始无颜色）
    ///   1-99 → 显示进度条并 Clamp 到 [10, 90]（防止满条误导用户）
    ///   ≥ 100→ 直接隐藏进度条（任务完成，无需手动隐藏）
    /// 内部自动处理跨线程调用，外部可直接从后台线程调用。
    /// </summary>
    public static class ProgressBarHelper
    {
        /// <summary>
        /// 设置进度条逻辑进度（传入 0-100）。
        /// </summary>
        public static void SetProgress(ProgressBar pb, int value)
        {
            if (pb.InvokeRequired)
            {
                pb.BeginInvoke(() => SetProgress(pb, value));
                return;
            }

            if (pb.Maximum != 100)
            {
                pb.Value = pb.Minimum;  // 先归零，防止 Value > 新 Maximum 时抛异常
                pb.Maximum = 100;
            }

            if (value >= 100)
            {
                pb.Visible = false;
                return;
            }

            pb.Visible = true;
            pb.Value = value <= 0 ? 10 : Math.Clamp(value, 10, 90);
        }

        /// <summary>
        /// 标记任务开始，进度条显示并设为 10。
        /// </summary>
        public static void SetProgressBegin(ProgressBar pb) => SetProgress(pb, 0);
    }
}
