using ConfigExcelEnhancer.Core;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 各功能 Tab 的公共基类，集中承载所有 Tab 共享的执行基础设施：
    /// 取消令牌、执行状态事件、日志转发与进度换算，消除各 Tab 中重复的样板代码。
    /// <para>
    /// 刻意设计为非抽象类，以保证派生的 UserControl 仍能在 WinForms 设计器中正常打开；
    /// 日志输出控件由派生类通过重写 <see cref="LogBox"/> 提供。
    /// </para>
    /// </summary>
    public class TabBase : UserControl
    {
        /// <summary>
        /// 当前任务的取消令牌源。基于 CancellationToken 的 Tab 共用此字段；
        /// 事件驱动型 Tab（Luban / Home）可不使用并重写 <see cref="CancelRunningTask"/>。
        /// </summary>
        protected CancellationTokenSource? _cts;

        /// <summary>
        /// 任务执行状态变化时触发（true = 开始执行，false = 执行结束）。
        /// MainForm 监听此事件以在执行期间阻止切换 Tab。
        /// </summary>
        public event EventHandler<bool>? ExecutionStateChanged;

        /// <summary>触发 <see cref="ExecutionStateChanged"/> 事件。</summary>
        protected void RaiseExecutionState(bool executing) => ExecutionStateChanged?.Invoke(this, executing);

        /// <summary>派生类重写以提供日志输出控件；返回 null 表示该 Tab 无日志输出。</summary>
        protected virtual RichTextBox? LogBox => null;

        /// <summary>
        /// 可选的日志镜像回调：非 null 时，本 Tab 的每条日志在写入自身 <see cref="LogBox"/> 的同时
        /// 也转发给该回调。供一键导出等外部编排者收集子 Tab 的详细输出。
        /// </summary>
        private Action<string, LogLevel>? _logSink;

        /// <summary>设置（或清除，传 null）日志镜像回调。</summary>
        public void SetLogSink(Action<string, LogLevel>? sink) => _logSink = sink;

        /// <summary>向日志控件写入一行带级别标签的日志（<see cref="LogBox"/> 为 null 时忽略）。</summary>
        protected void Log(string message, LogLevel level = LogLevel.Ok)
        {
            if (LogBox is { } box) LogLibrary.Write(box, message, level);
            _logSink?.Invoke(message, level);
        }

        /// <summary>
        /// 写入一行不带时间戳/级别标签的原始日志（用于子进程输出），并按需镜像到 <see cref="_logSink"/>。
        /// </summary>
        protected void LogRaw(string message, Color color, LogLevel sinkLevel)
        {
            if (LogBox is { } box) LogLibrary.WriteRaw(box, message, color);
            _logSink?.Invoke(message, sinkLevel);
        }

        /// <summary>向日志控件追加一条分隔线（<see cref="LogBox"/> 为 null 时忽略）。</summary>
        protected void LogDivider()
        {
            if (LogBox is { } box) LogLibrary.Divider(box);
        }

        /// <summary>
        /// 将「已处理 <paramref name="processed"/> / 共 <paramref name="total"/>」换算为进度百分比：
        /// 处理阶段线性映射到 [10, 90]；<paramref name="total"/> 为 0 时返回 90。
        /// 各 Tab 的进度回调统一复用，避免重复书写同一公式。
        /// </summary>
        protected static int ScaledProgress(int processed, int total)
            => total > 0 ? Math.Min(10 + (int)(processed * 80.0 / total), 90) : 90;

        /// <summary>
        /// 取消当前正在进行的任务（供窗口关闭时调用）。默认取消 <see cref="_cts"/>；
        /// 事件驱动型 Tab（如 Luban / Home）应重写为各自的取消逻辑。
        /// </summary>
        public virtual void CancelRunningTask() => _cts?.Cancel();
    }
}
