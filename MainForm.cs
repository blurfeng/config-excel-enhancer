using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer
{
    /// <summary>
    /// 应用程序主窗体。负责协调四个功能 Tab 的生命周期，并统一管理应用设置的加载与保存。
    /// </summary>
    public partial class MainForm : Form
    {
        // 正在执行任务的 Tab 计数（>0 表示有任务运行中），用于阻止 Tab 切换。
        // 用计数器而非单一 bool：HomeTab 一键导出会串联触发子 Tab 的状态事件，
        // 单一 bool 会被先结束的子任务误置为 false，导致执行期间仍可切换 Tab。
        private int _executingCount;

        // 本地状态（不进入版本控制）
        private LocalState _localState = LocalStateManager.Load();

        /// <summary>
        /// 初始化主窗体。订阅各 Tab 的执行状态事件，阻止执行期间切换 Tab，并加载持久化设置。
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            settingsTab.SettingsCleared += (_, _) => LoadSettings();
            enumTab.ExecutionStateChanged += OnTabExecutionStateChanged;
            lubanTab.ExecutionStateChanged += OnTabExecutionStateChanged;
            tableDesignTab.ExecutionStateChanged += OnTabExecutionStateChanged;
            templateTab.ExecutionStateChanged += OnTabExecutionStateChanged;
            excelExportTab.ExecutionStateChanged += OnTabExecutionStateChanged;
            homeTab.ExecutionStateChanged += OnTabExecutionStateChanged;
            tabControl.Selecting += (_, e) => { if (_executingCount > 0) e.Cancel = true; };
            LoadSettings();
        }

        /// <summary>
        /// 任一 Tab 执行状态变化时更新计数：开始 +1，结束 -1（钳制非负）。
        /// </summary>
        private void OnTabExecutionStateChanged(object? sender, bool executing)
        {
            if (executing) _executingCount++;
            else _executingCount = Math.Max(0, _executingCount - 1);
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
                enumTab.Settings,
                _localState);
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
            excelExportTab.Settings = settings;
            enumTab.LoadFromSettings();
            lubanTab.LoadFromSettings();
            tableDesignTab.LoadFromSettings();
            templateTab.LoadFromSettings();
            excelExportTab.LoadFromSettings();
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
            excelExportTab.CancelRunningTask();
            homeTab.CancelRunningTask();
            SaveSettings();
            base.OnFormClosing(e);
        }
    }
}
