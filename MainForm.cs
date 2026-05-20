using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer
{
    /// <summary>
    /// 应用程序主窗体。负责协调四个功能 Tab 的生命周期，并统一管理应用设置的加载与保存。
    /// </summary>
    public partial class MainForm : Form
    {
        // 当前是否有任务正在执行（任意 Tab 运行中均为 true），用于阻止 Tab 切换
        private bool _isExecuting;

        /// <summary>
        /// 初始化主窗体。订阅各 Tab 的执行状态事件，阻止执行期间切换 Tab，并加载持久化设置。
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            settingsTab.SettingsCleared += (_, _) => LoadSettings();
            enumTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            lubanTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            tableDesignTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            templateTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            homeTab.ExecutionStateChanged += (_, executing) => _isExecuting = executing;
            tabControl.Selecting += (_, e) => { if (_isExecuting) e.Cancel = true; };
            LoadSettings();
        }

        /// <summary>
        /// 将 HomeTab 依赖的引用注入。必须在 LoadSettings 之后调用，确保 Settings 已分发。
        /// </summary>
        private void InitializeHomeTab()
        {
            homeTab.Initialize(
                lubanTab,
                templateTab,
                enumTab,
                tabControl,
                tabLuban,
                tabTemplate,
                enumTab.Settings);
        }

        /// <summary>
        /// 从磁盘加载设置，并将同一 AppSettings 实例分发给所有 Tab。
        /// 三个 Tab 共享同一实例，因此 SaveSettings() 只需保存其中任意一个。
        /// </summary>
        private void LoadSettings()
        {
            var settings = SettingsManager.Load();
            enumTab.Settings = settings;
            lubanTab.Settings = settings;
            tableDesignTab.Settings = settings;
            templateTab.Settings = settings;
            enumTab.LoadFromSettings();
            lubanTab.LoadFromSettings();
            tableDesignTab.LoadFromSettings();
            templateTab.LoadFromSettings();
            InitializeHomeTab();
        }

        /// <summary>
        /// 将当前设置写入磁盘。
        /// 三个 Tab 共享同一 AppSettings 实例，故只需通过任意 Tab 保存。
        /// </summary>
        private void SaveSettings()
        {
            SettingsManager.Save(enumTab.Settings);
        }

        /// <summary>
        /// 窗体关闭前：取消所有正在运行的任务，保存设置。
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            enumTab.CancelRunningTask();
            lubanTab.CancelRunningTask();
            tableDesignTab.CancelRunningTask();
            templateTab.CancelRunningTask();
            homeTab.CancelRunningTask();
            SaveSettings();
            base.OnFormClosing(e);
        }
    }
}
