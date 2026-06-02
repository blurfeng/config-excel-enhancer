using System.Diagnostics;
using System.Text;

namespace ConfigExcelEnhancer.Core
{
    /// <summary>
    /// 负责以子进程方式执行 .bat 脚本（Luban gen.bat），并通过事件实时转发输出和退出码。
    /// stdout 和 stderr 均以异步方式读取，不会阻塞调用线程。
    /// </summary>
    public class LubanRunner
    {
        /// <summary>每当子进程产生一行输出（stdout 或 stderr）时触发。stderr 行以 "[ERR] " 前缀标识。</summary>
        public event Action<string>? OutputReceived;

        /// <summary>子进程退出后触发，参数为进程退出码（0 表示成功）。</summary>
        public event Action<int>? Finished;

        // 当前正在运行的子进程；未运行时为 null
        private Process? _process;

        /// <summary>
        /// 以无窗口子进程方式启动指定的 .bat 文件。
        /// 工作目录设置为 bat 文件所在目录，stdout/stderr 均被异步重定向。
        /// </summary>
        public void Run(string batFilePath)
        {
            var workingDir = Path.GetDirectoryName(batFilePath) ?? Directory.GetCurrentDirectory();

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = batFilePath,
                    WorkingDirectory = workingDir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,   // 重定向 stdin 以防止子进程等待用户输入
                    CreateNoWindow = true,
                    StandardOutputEncoding = new UTF8Encoding(false),   // Luban 为 .NET 工具，输出 UTF-8
                    StandardErrorEncoding = new UTF8Encoding(false)
                },
                EnableRaisingEvents = true          // 启用 Exited 事件
            };

            _process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null) OutputReceived?.Invoke(e.Data);
            };
            _process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null) OutputReceived?.Invoke($"[ERR] {e.Data}");
            };
            _process.Exited += (_, _) =>
            {
                Finished?.Invoke(_process.ExitCode);
                _process.Dispose();
                _process = null;
            };

            _process.Start();
            _process.StandardInput.Close();     // 立即关闭 stdin，防止子进程阻塞等待输入
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        /// <summary>
        /// 强制终止正在运行的子进程（及其所有子进程树）。
        /// 若进程未运行则无操作。
        /// </summary>
        public void Cancel()
        {
            try { _process?.Kill(true); } catch { }
        }
    }
}
