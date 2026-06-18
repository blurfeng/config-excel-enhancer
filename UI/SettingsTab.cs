using System.Diagnostics;
using System.Reflection;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    public partial class SettingsTab : UserControl
    {
        // Luban 官方仓库与本项目仓库地址。
        private const string LubanRepoUrl = "https://github.com/focus-creative-games/luban";
        private const string ProjectRepoUrl = "https://github.com/blurfeng/config-excel-enhancer";

        /// <summary>清空 settings.json 并立即生效后触发，宿主窗体应重新加载设置。</summary>
        public event EventHandler? SettingsCleared;

        /// <summary>
        /// 清空 local_state.json 后触发。宿主窗体须重置内存中持有的 LocalState 实例，
        /// 否则程序关闭时会被旧值重新写回磁盘。
        /// </summary>
        public event EventHandler? LocalStateCleared;

        public SettingsTab()
        {
            InitializeComponent();
            FillAboutAndDiagnostics();
        }

        #region 配置文件管理

        private void BtnClearSettings_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "确定要清空 settings.json 吗？\n所有设置将立即恢复为默认值。",
                "清空确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                SettingsManager.Clear();
                SettingsCleared?.Invoke(this, EventArgs.Empty);
                FillAboutAndDiagnostics();
                MessageBox.Show("settings.json 已清空，设置已恢复为默认值。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnOpenSettingsFolder_Click(object? sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(SettingsManager.FilePath);

        private void BtnClearLocalState_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "确定要清空 local_state.json 吗？\n\n" +
                "本机状态将恢复默认，包括：项目根目录、窗口尺寸、导出缓存等。\n" +
                "清空后 settings.json 中的相对路径将失去基准，需要重新设置项目根目录。",
                "清空确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                LocalStateManager.Clear();
                LocalStateCleared?.Invoke(this, EventArgs.Empty);
                FillAboutAndDiagnostics();
                MessageBox.Show("local_state.json 已清空，本机状态已恢复为默认值。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnOpenLocalStateFolder_Click(object? sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(LocalStateManager.FilePath);

        #endregion

        #region 关于 / 诊断

        /// <summary>
        /// 读取应用版本号（来自 csproj 的 InformationalVersion，如 0.2.22-beta）。
        /// 防御性截断 '+' 之后的 git 源码修订后缀（已在 csproj 关闭该追加，此处仅作兜底）。
        /// </summary>
        private static string GetAppVersion()
        {
            var version = Assembly.GetEntryAssembly()
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "未知";
            int plus = version.IndexOf('+');
            return plus >= 0 ? version[..plus] : version;
        }

        /// <summary>填充版本标签与诊断信息文本（清空操作后亦会刷新）。</summary>
        private void FillAboutAndDiagnostics()
        {
            lblVersion.Text = $"ConfigExcelEnhancer  v{GetAppVersion()}";

            var projectRoot = LocalStateManager.Load().ProjectRoot;
            txtDiagnostics.Text = string.Join(Environment.NewLine, new[]
            {
                $"版本：{GetAppVersion()}",
                $"项目根目录：{(string.IsNullOrEmpty(projectRoot) ? "(未设置)" : projectRoot)}",
                $"settings.json：{SettingsManager.FilePath}",
                $"local_state.json：{LocalStateManager.FilePath}",
            });
        }

        private void BtnCopyDiagnostics_Click(object? sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtDiagnostics.Text);
                MessageBox.Show("诊断信息已复制到剪贴板。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"复制失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LinkLuban_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
            => OpenUrl(LubanRepoUrl);

        private void LinkProject_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
            => OpenUrl(ProjectRepoUrl);

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch { }
        }

        #endregion
    }
}
