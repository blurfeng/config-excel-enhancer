using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 主页选项卡，提供最常用的快捷操作：一键导出（Enum 验证 + Luban 导表 + 导出模板类）。
    /// </summary>
    public partial class HomeTab : UserControl
    {
        private EnumTab? _enumTab;
        private LubanTab? _lubanTab;
        private TemplateTab? _templateTab;
        private TabControl? _tabControl;
        private TabPage? _tabLuban;
        private TabPage? _tabTemplate;
        private AppSettings? _settings;
        private LocalState? _localState;

        private bool _isExecuting;

        public event EventHandler<bool>? ExecutionStateChanged;

        public HomeTab()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化 HomeTab，注入依赖的 Tab 和 TabControl 引用。
        /// 必须在 MainForm.InitializeComponent() 后调用。
        /// </summary>
        public void Initialize(
            LubanTab lubanTab,
            TemplateTab templateTab,
            EnumTab enumTab,
            TabControl tabControl,
            TabPage tabLuban,
            TabPage tabTemplate,
            AppSettings settings,
            LocalState localState)
        {
            _lubanTab = lubanTab;
            _templateTab = templateTab;
            _enumTab = enumTab;
            _tabControl = tabControl;
            _tabLuban = tabLuban;
            _tabTemplate = tabTemplate;
            _settings = settings;
            _localState = localState;

            // 从设置加载勾选状态
            chkIncludeEnum.Checked = settings.HomeIncludeEnum;
            chkIncludeEnum.CheckedChanged += chkIncludeEnum_CheckedChanged;
        }

        /// <summary>
        /// 每次切换到主页时，刷新配置状态显示。
        /// </summary>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
                RefreshStatus();
        }

        private void RefreshStatus()
        {
            if (_settings == null) return;

            // gen.bat 状态
            string genBatPath = _settings.GenBatPath;
            bool genBatValid = !string.IsNullOrEmpty(genBatPath) && File.Exists(genBatPath);
            lblGenBat.Text = genBatValid
                ? $"gen.bat：{ShortenPath(genBatPath, 60)}"
                : "gen.bat：未配置";
            SetDot(lblGenBatDot, genBatValid);

            // 模板任务数
            int jobCount = _settings.TemplateExportJobs.Count;
            lblTemplateJobs.Text = jobCount > 0
                ? $"模板任务：{jobCount} 个已配置"
                : "模板任务：未配置";
            SetDot(lblTemplateJobsDot, jobCount > 0);

            // Tables.cs 状态
            string tablesPath = _settings.TablesClassPath;
            bool tablesValid = !string.IsNullOrEmpty(tablesPath) && File.Exists(tablesPath);
            lblTablesCs.Text = tablesValid
                ? $"Tables.cs：{ShortenPath(tablesPath, 60)}"
                : "Tables.cs：未配置";
            SetDot(lblTablesCsDot, tablesValid);

            // 上次导出时间（仅显示，不指示配置对错）
            if (_localState?.LastExportTime.HasValue == true)
            {
                lblLastExport.Text = $"本地上次导出：{_localState.LastExportTime.Value:yyyy-MM-dd HH:mm}";
                lblLastExportDot.ForeColor = Color.LightGreen;
            }
            else
            {
                lblLastExport.Text = "本地上次导出：从未导出";
                lblLastExportDot.ForeColor = Color.Gray;
            }
        }

        /// <summary>设置状态圆点颜色；不修改主 Label 的字体颜色。</summary>
        private static void SetDot(Label dot, bool ok)
            => dot.ForeColor = ok ? Color.LightGreen : Color.OrangeRed;

        private static string ShortenPath(string path, int maxLength)
        {
            if (path.Length <= maxLength) return path;
            return "..." + path[(path.Length - maxLength + 3)..];
        }

        // ── 快速操作 ──────────────────────────────────────────

        private async void btnOneClick_Click(object sender, EventArgs e)
        {
            if (_enumTab == null || _lubanTab == null || _templateTab == null || _settings == null)
            {
                Log("HomeTab 未正确初始化，请联系开发人员。", LogLevel.Error);
                return;
            }

            SetUILocked(true);
            ExecutionStateChanged?.Invoke(this, true);
            txtLog.Clear();
            LogDivider();

            int totalSteps = chkIncludeEnum.Checked ? 3 : 2;
            int currentStep = 0;

            void BeginStep(string title)
            {
                currentStep++;
                Log($"【{currentStep}/{totalSteps}】{title}", LogLevel.Section);
            }

            void EndStep(bool ok, string okMsg, string failMsg)
            {
                LogLibrary.UpdateLastLine(txtLog,
                    $"【{currentStep}/{totalSteps}】{(ok ? "✓ " + okMsg : "✗ " + failMsg)}",
                    ok ? LogLevel.Ok : LogLevel.Error);
                SetOverallProgress(currentStep, totalSteps);
            }

            SetOverallProgress(0, totalSteps);

            try
            {
                // 步骤 1：Enum 验证（可选）
                if (chkIncludeEnum.Checked)
                {
                    BeginStep("执行 Enum 验证...");
                    bool enumSuccess = await _enumTab.RunAsync();
                    EndStep(enumSuccess, "Enum 验证完成。", "Enum 验证失败，已终止后续步骤。");
                    if (!enumSuccess) return;
                    LogDivider();
                }

                // 步骤：Luban 导表
                BeginStep("执行 Luban 导表...");
                bool lubanSuccess = await _lubanTab.RunAsync();
                EndStep(lubanSuccess, "Luban 导表完成。", "Luban 导表失败，已终止后续步骤。");
                if (!lubanSuccess) return;
                LogDivider();

                // 步骤：导出模板类
                BeginStep("导出模板类...");
                bool templateSuccess = await _templateTab.RunAllAsync();
                EndStep(templateSuccess, "导出模板类完成。", "导出模板类失败。");
                if (!templateSuccess) return;
                LogDivider();

                // 全部成功：更新导出时间
                if (_localState != null)
                {
                    _localState.LastExportTime = DateTime.Now;
                    try
                    {
                        LocalStateManager.Save(_localState);
                        Log("一键导出全部完成！", LogLevel.Ok);
                        RefreshStatus();
                    }
                    catch (Exception ex)
                    {
                        Log($"保存导出时间失败：{ex.Message}", LogLevel.Warn);
                    }
                }
                else
                {
                    Log("一键导出全部完成！", LogLevel.Ok);
                }
            }
            finally
            {
                SetUILocked(false);
                ExecutionStateChanged?.Invoke(this, false);
                LogDivider();
            }
        }

        private async void btnLubanOnly_Click(object sender, EventArgs e)
        {
            if (_lubanTab == null)
            {
                Log("HomeTab 未正确初始化，请联系开发人员。", LogLevel.Error);
                return;
            }

            SetUILocked(true);
            ExecutionStateChanged?.Invoke(this, true);
            txtLog.Clear();
            LogDivider();

            SetOverallProgress(0, 1);
            Log("【1/1】执行 Luban 导表...", LogLevel.Section);
            bool success = await _lubanTab.RunAsync();
            LogLibrary.UpdateLastLine(txtLog,
                success ? "【1/1】✓ 导表完成。" : "【1/1】✗ 导表失败。",
                success ? LogLevel.Ok : LogLevel.Error);
            SetOverallProgress(1, 1);

            SetUILocked(false);
            ExecutionStateChanged?.Invoke(this, false);
            LogDivider();
        }

        private void chkIncludeEnum_CheckedChanged(object? sender, EventArgs e)
        {
            if (_settings == null) return;
            _settings.HomeIncludeEnum = chkIncludeEnum.Checked;
            SettingsManager.Save(_settings);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _enumTab?.CancelRunningTask();
            _lubanTab?.CancelRunningTask();
            _templateTab?.CancelRunningTask();
            Log("已发送取消信号。", LogLevel.Warn);
        }

        private void SetUILocked(bool locked)
        {
            btnOneClick.Enabled = !locked;
            btnLubanOnly.Enabled = !locked;
            btnCancel.Enabled = locked;
            chkIncludeEnum.Enabled = !locked;
            if (locked) ProgressBarHelper.SetProgressBegin(pbOverall);
            else ProgressBarHelper.SetProgress(pbOverall, 100);
            _isExecuting = locked;
        }

        private void SetOverallProgress(int done, int total)
        {
            if (done <= 0)
            {
                ProgressBarHelper.SetProgressBegin(pbOverall);
                return;
            }
            int t = total > 0 ? total : 1;
            if (done >= t)
            {
                ProgressBarHelper.SetProgress(pbOverall, 100);
                return;
            }
            ProgressBarHelper.SetProgress(pbOverall, 10 + done * 80 / t);
        }

        /// <summary>
        /// 供 MainForm 在窗口关闭时调用，取消当前正在执行的任务。
        /// </summary>
        public void CancelRunningTask()
        {
            if (_isExecuting)
                btnCancel_Click(this, EventArgs.Empty);
        }

        // ── 跳转 Tab ──────────────────────────────────────────

        private void btnGoLuban_Click(object sender, EventArgs e)
        {
            if (_tabControl != null && _tabLuban != null)
                _tabControl.SelectedTab = _tabLuban;
        }

        private void btnGoTemplate_Click(object sender, EventArgs e)
        {
            if (_tabControl != null && _tabTemplate != null)
                _tabControl.SelectedTab = _tabTemplate;
        }

        // ── 日志右键菜单 ──────────────────────────────────────

        private void ctxMenuItemClearLog_Click(object sender, EventArgs e)
            => txtLog.Clear();

        private void ctxMenuItemCopyLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

        // ── 日志工具 ──────────────────────────────────────────

        private void Log(string message, LogLevel level = LogLevel.Ok)
            => LogLibrary.Write(txtLog, message, level);

        private void LogDivider()
            => LogLibrary.Divider(txtLog);
    }
}
