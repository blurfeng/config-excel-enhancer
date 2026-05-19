namespace ConfigExcelEnhancer.UI
{
    internal static class DialogHelper
    {
        /// <summary>
        /// 弹出文件夹浏览对话框。返回用户选择的路径，取消则返回 null。
        /// </summary>
        public static string? BrowseFolder(string description, string? initialPath = null)
        {
            using var dlg = new FolderBrowserDialog { Description = description };
            if (!string.IsNullOrEmpty(initialPath))
                dlg.SelectedPath = initialPath;
            return dlg.ShowDialog() == DialogResult.OK ? dlg.SelectedPath : null;
        }

        /// <summary>
        /// 弹出文件选择对话框。返回选中的文件路径数组，取消则返回空数组。
        /// initialPath 可以是目录路径或文件路径（自动取其目录作为初始位置）。
        /// </summary>
        public static string[] BrowseFiles(string title, string filter,
            string? initialPath = null, bool multiselect = false)
        {
            using var dlg = new OpenFileDialog { Title = title, Filter = filter, Multiselect = multiselect };
            if (!string.IsNullOrEmpty(initialPath))
            {
                var dir = Directory.Exists(initialPath) ? initialPath : Path.GetDirectoryName(initialPath);
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    dlg.InitialDirectory = dir;
            }
            return dlg.ShowDialog() == DialogResult.OK ? dlg.FileNames : Array.Empty<string>();
        }

        /// <summary>
        /// 弹出保存文件对话框。返回用户指定的路径，取消则返回 null。
        /// </summary>
        public static string? BrowseSaveFile(string title, string filter,
            string? initialPath = null, string? defaultFileName = null)
        {
            using var dlg = new SaveFileDialog { Title = title, Filter = filter };
            if (!string.IsNullOrEmpty(defaultFileName))
                dlg.FileName = defaultFileName;
            if (!string.IsNullOrEmpty(initialPath))
            {
                var dir = Directory.Exists(initialPath) ? initialPath : Path.GetDirectoryName(initialPath);
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    dlg.InitialDirectory = dir;
                if (File.Exists(initialPath))
                    dlg.FileName = Path.GetFileName(initialPath);
            }
            return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
        }
    }
}
