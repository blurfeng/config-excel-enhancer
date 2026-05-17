using System.Diagnostics;

namespace ConfigExcelEnhancer.Core
{
    public class LubanRunner
    {
        public event Action<string>? OutputReceived;
        public event Action<int>? Finished;

        private Process? _process;

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
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
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
            _process.StandardInput.Close();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        public void Cancel()
        {
            try { _process?.Kill(true); } catch { }
        }
    }
}
