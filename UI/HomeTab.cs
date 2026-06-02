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
            lblGenBatDot.ForeColor = genBatValid ? Color.LightGreen : Color.Red;

            // 模板任务数 & Tables.cs 状态
            int jobCount = _settings.TemplateExportJobs.Count;
            lblTemplateJobs.Text = jobCount > 0
                ? $"模板任务：{jobCount} 个已配置"
                : "模板任务：未配置";

            string tablesPath = _settings.TablesClassPath;
            bool tablesValid = !string.IsNullOrEmpty(tablesPath) && File.Exists(tablesPath);
            lblTablesCs.Text = tablesValid
                ? "Tables.cs：已配置"
                : "Tables.cs：未配置";

            // 圆点三态：未配置模板任务→黄色；任务已配置但Tables.cs未配置→红色；全部配置→绿色
            if (jobCount == 0)
                lblTemplateJobsDot.ForeColor = Color.Yellow;
            else if (!tablesValid)
                lblTemplateJobsDot.ForeColor = Color.Red;
            else
                lblTemplateJobsDot.ForeColor = Color.LightGreen;

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

            // 配置检查
            var (issues, commonRoot) = BuildConfigIssues();
            if (issues.Count == 0)
            {
                lblCheckDot.ForeColor = Color.LightGreen;
                lblCheckSummary.Text = !string.IsNullOrEmpty(commonRoot)
                    ? $"配置检查通过，根目录：{ShortenPath(commonRoot, 80)}"
                    : "配置检查通过";
                lblCheckIssues.Visible = false;
            }
            else
            {
                lblCheckDot.ForeColor = Color.Yellow;
                lblCheckSummary.Text = $"发现 {issues.Count} 项配置问题：";
                lblCheckIssues.Text = string.Join("\n", issues.Select(s => "• " + s));
                lblCheckIssues.Visible = true;
            }
        }

        /// <summary>设置状态圆点颜色；不修改主 Label 的字体颜色。</summary>
        private static void SetDot(Label dot, bool ok)
            => dot.ForeColor = ok ? Color.LightGreen : Color.Yellow;

        private static string ShortenPath(string path, int maxLength)
        {
            if (path.Length <= maxLength) return path;
            return "..." + path[(path.Length - maxLength + 3)..];
        }

        /// <summary>
        /// 计算一组路径的最长公共根路径（按路径段逐段比对）。
        /// 返回 null 表示路径集为空、无公共前缀或路径无效。
        /// </summary>
        private static string? GetLongestCommonRootPath(IEnumerable<string> paths)
        {
            var validPaths = paths
                .Where(p => !string.IsNullOrWhiteSpace(p) && Path.IsPathRooted(p))
                .Select(p => p.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (validPaths.Count == 0) return null;
            if (validPaths.Count == 1) return validPaths[0];

            var splitPaths = validPaths.Select(p => p.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToList();
            int minSegmentCount = splitPaths.Min(sp => sp.Length);
            var commonSegments = new List<string>();

            for (int i = 0; i < minSegmentCount; i++)
            {
                var segment = splitPaths[0][i];
                if (splitPaths.All(sp => string.Equals(sp[i], segment, StringComparison.OrdinalIgnoreCase)))
                    commonSegments.Add(segment);
                else
                    break;
            }

            return commonSegments.Count > 0 ? string.Join(Path.DirectorySeparatorChar.ToString(), commonSegments) : null;
        }

        /// <summary>
        /// 构建配置检查问题列表。
        /// 返回 (问题列表, 公共根路径)。问题列表为空表示配置正确。
        /// </summary>
        private (List<string> issues, string? commonRoot) BuildConfigIssues()
        {
            var issues = new List<string>();
            if (_settings == null) return (issues, null);

            var allPaths = new List<string>();
            var genBatDir = string.Empty;

            // 收集 gen.bat 路径
            if (!string.IsNullOrEmpty(_settings.GenBatPath) && File.Exists(_settings.GenBatPath))
            {
                genBatDir = Path.GetDirectoryName(_settings.GenBatPath) ?? "";
                if (!string.IsNullOrEmpty(genBatDir))
                    allPaths.Add(genBatDir);
            }

            // 收集 Tables.cs 路径
            var tablesDir = string.Empty;
            if (!string.IsNullOrEmpty(_settings.TablesClassPath) && File.Exists(_settings.TablesClassPath))
            {
                tablesDir = Path.GetDirectoryName(_settings.TablesClassPath) ?? "";
                if (!string.IsNullOrEmpty(tablesDir))
                    allPaths.Add(tablesDir);
            }

            // 收集模板任务输出路径
            var jobPaths = new List<string>();
            foreach (var job in _settings.TemplateExportJobs)
            {
                if (!string.IsNullOrEmpty(job.OutputDirectory) && Directory.Exists(job.OutputDirectory))
                {
                    allPaths.Add(job.OutputDirectory);
                    jobPaths.Add(job.OutputDirectory);
                }
                if (!string.IsNullOrEmpty(job.IdsOutputDirectory) && Directory.Exists(job.IdsOutputDirectory))
                {
                    allPaths.Add(job.IdsOutputDirectory);
                    jobPaths.Add(job.IdsOutputDirectory);
                }
            }

            // 计算公共根路径
            var commonRoot = GetLongestCommonRootPath(allPaths);

            // 检查 1：根目录不一致
            if (allPaths.Count > 1 && commonRoot != null)
            {
                var outliers = allPaths.Where(p =>
                {
                    var normalized = p.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var rootNormalized = commonRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    return !normalized.StartsWith(rootNormalized, StringComparison.OrdinalIgnoreCase);
                }).ToList();

                if (outliers.Any())
                    issues.Add($"根目录不一致：{outliers.Count} 个路径不在公共根 [{commonRoot}] 下");
            }

            // 检查 2：gen.bat 导出路径不在 gen.bat 根目录下
            if (!string.IsNullOrEmpty(genBatDir) && _lubanTab != null)
            {
                var config = _lubanTab.GetCurrentConfig();
                if (config != null)
                {
                    var genOutputPaths = new List<string>();
                    foreach (var cmd in config.Commands)
                    {
                        foreach (var kv in cmd.XArgs)
                        {
                            if (kv.Key.EndsWith("Dir", StringComparison.OrdinalIgnoreCase) ||
                                kv.Key.EndsWith("Path", StringComparison.OrdinalIgnoreCase))
                            {
                                var val = kv.Value;
                                if (string.IsNullOrWhiteSpace(val)) continue;

                                // 解析为绝对路径（相对路径以 genBatDir 为基准）
                                string absPath;
                                if (Path.IsPathRooted(val))
                                    absPath = val;
                                else if (!val.Contains('%'))
                                {
                                    try { absPath = Path.GetFullPath(Path.Combine(genBatDir, val)); }
                                    catch { continue; }
                                }
                                else
                                    continue; // 包含变量引用，跳过

                                genOutputPaths.Add(absPath);
                            }
                        }
                    }

                    var genRootNormalized = genBatDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var outsideGen = genOutputPaths.Where(p =>
                    {
                        var normalized = p.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        return !normalized.StartsWith(genRootNormalized, StringComparison.OrdinalIgnoreCase);
                    }).ToList();

                    if (outsideGen.Any())
                        issues.Add($"Luban 导出路径不在 gen.bat 根目录下：{outsideGen.Count} 个路径在 [{genBatDir}] 外");
                }
            }

            // 检查 3：Tables.cs 与模板任务输出目录根不一致
            if (!string.IsNullOrEmpty(tablesDir) && jobPaths.Any())
            {
                var tablesJobPaths = new List<string> { tablesDir };
                tablesJobPaths.AddRange(jobPaths);
                var tablesCommonRoot = GetLongestCommonRootPath(tablesJobPaths);

                // 如果公共前缀只是驱动符（如 "F:"），认为不一致
                if (tablesCommonRoot == null || tablesCommonRoot.Length <= 3)
                    issues.Add("Tables.cs 与模板任务输出目录根不一致");
            }

            return (issues, commonRoot);
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
