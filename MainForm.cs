using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer
{
    public partial class MainForm : Form
    {
        private bool _isExecuting;

        public MainForm()
        {
            InitializeComponent();
            settingsTab.SettingsCleared += (_, _) => LoadSettings();
            enumTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            lubanTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            tableDesignTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            tabControl.Selecting += (_, e) => { if (_isExecuting) e.Cancel = true; };
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = SettingsManager.Load();
            enumTab.Settings = settings;
            lubanTab.Settings = settings;
            tableDesignTab.Settings = settings;
            enumTab.LoadFromSettings();
            lubanTab.LoadFromSettings();
            tableDesignTab.LoadFromSettings();
        }

        private void SaveSettings()
        {
            SettingsManager.Save(enumTab.Settings);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            enumTab.CancelRunningTask();
            lubanTab.CancelRunningTask();
            tableDesignTab.CancelRunningTask();
            SaveSettings();
            base.OnFormClosing(e);
        }
    }
}
