using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    public partial class SettingsTab : UserControl
    {
        /// <summary>清空并立即生效后触发，宿主窗体应重新加载设置。</summary>
        public event EventHandler? SettingsCleared;

        public SettingsTab()
        {
            InitializeComponent();
        }

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
                MessageBox.Show("settings.json 已清空，设置已恢复为默认值。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
