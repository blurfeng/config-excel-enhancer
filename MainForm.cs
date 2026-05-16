using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = SettingsManager.Load();
            enumTab.Settings = settings;
            lubanTab.Settings = settings;
            enumTab.LoadFromSettings();
            lubanTab.LoadFromSettings();
        }

        private void SaveSettings()
        {
            SettingsManager.Save(enumTab.Settings);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSettings();
            base.OnFormClosing(e);
        }
    }
}
