using System.Diagnostics;

namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 在 Windows 资源管理器中打开 / 定位路径的工具。
    /// 供各页“浏览”按钮旁的“打开目录”小按钮统一调用。
    /// </summary>
    public static class ExplorerHelper
    {
        /// <summary>
        /// 在资源管理器中打开 <paramref name="path"/>：
        /// <list type="bullet">
        ///   <item>指向已存在文件时，打开其所在目录并选中该文件；</item>
        ///   <item>指向已存在目录时，直接打开该目录；</item>
        ///   <item>路径尚不存在时，回退打开最近的已存在上级目录。</item>
        /// </list>
        /// 路径为空或找不到任何可用目录时返回 false（不抛异常）。
        /// </summary>
        public static bool RevealInExplorer(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            try
            {
                string full = Path.GetFullPath(path);

                if (File.Exists(full))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{full}\"") { UseShellExecute = true });
                    return true;
                }
                if (Directory.Exists(full))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", $"\"{full}\"") { UseShellExecute = true });
                    return true;
                }

                // 路径尚不存在：回退到最近的已存在上级目录
                var dir = Path.GetDirectoryName(full);
                while (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    dir = Path.GetDirectoryName(dir);

                if (!string.IsNullOrEmpty(dir))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", $"\"{dir}\"") { UseShellExecute = true });
                    return true;
                }
            }
            catch { }

            return false;
        }
    }
}
