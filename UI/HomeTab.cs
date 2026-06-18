using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 主页选项卡，提供最常用的快捷操作：一键导出（Enum 验证 + Luban 导表 + 导出模板类）。
    /// </summary>
    public partial class HomeTab : TabBase
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

        // 控件事件是否已订阅。Initialize 可能因 LoadSettings 重载而被多次调用，
        // 用此守卫确保控件事件只订阅一次，避免重复订阅。
        private bool _initialized;

        /// <summary>
        /// 本机项目根目录（ProjectRoot）发生变化时触发（自动探测或手动浏览）。
        /// MainForm 订阅此事件以从磁盘按新根重新还原设置。
        /// </summary>
        public event EventHandler? ProjectRootChanged;

        protected override RichTextBox? LogBox => txtLog;

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

            // 从设置加载勾选状态与项目名称（每次重载都刷新）
            chkIncludeEnum.Checked = settings.HomeIncludeEnum;
            txtProjectName.Text = settings.ProjectName;
            chkFuzzyFindProjectRoot.Checked = settings.FuzzyFindProjectRoot;

            // 控件事件只订阅一次，避免重载时重复订阅
            if (!_initialized)
            {
                chkIncludeEnum.CheckedChanged += chkIncludeEnum_CheckedChanged;
                txtProjectName.Leave += txtProjectName_Leave;
                chkFuzzyFindProjectRoot.CheckedChanged += chkFuzzyFindProjectRoot_CheckedChanged;
                _initialized = true;

                // 自动检测仅在首次初始化（启动）时执行：避免后续重载（尤其是「清空」
                // 触发的重载）把用户显式清空的根目录又自动填回。改名、模糊查找勾选等
                // 显式操作会单独调用 TryAutoDetectProjectRoot，不受此处影响。
                TryAutoDetectProjectRoot();
            }
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

            // 项目根目录状态
            string projectRoot = _localState?.ProjectRoot ?? string.Empty;
            bool rootValid = !string.IsNullOrEmpty(projectRoot) && Directory.Exists(projectRoot);
            if (!rootValid)
            {
                lblProjectRootDot.ForeColor = UITheme.StateInvalid;
                lblProjectRoot.Text = "本地项目根目录：未配置";
                toolTip.SetToolTip(lblProjectRoot, "本地项目根目录：未配置");
            }
            else
            {
                string localFolderName = Path.GetFileName(projectRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                bool nameMatches = string.IsNullOrEmpty(_settings.ProjectName)
                    || string.Equals(localFolderName, _settings.ProjectName, StringComparison.OrdinalIgnoreCase);
                if (nameMatches)
                {
                    lblProjectRootDot.ForeColor = UITheme.StateValid;
                    lblProjectRoot.Text = $"本地项目根目录：{ShortenPath(projectRoot, 60)}";
                    toolTip.SetToolTip(lblProjectRoot, $"本地项目根目录：{projectRoot}");
                }
                else
                {
                    lblProjectRootDot.ForeColor = UITheme.StateWarning;
                    lblProjectRoot.Text = $"本地项目根目录：{ShortenPath(projectRoot, 45)}（本地名称与项目名称不一致，项目名称：{_settings.ProjectName}）";
                    toolTip.SetToolTip(lblProjectRoot, $"本地项目根目录：{projectRoot}\n（本地名称与项目名称不一致，项目名称：{_settings.ProjectName}）");
                }
            }

            // gen.bat 状态
            string genBatPath = _settings.GenBatPath;
            bool genBatValid = !string.IsNullOrEmpty(genBatPath) && File.Exists(genBatPath);
            lblGenBat.Text = genBatValid
                ? $"gen.bat：{ShortenPath(genBatPath, 60)}"
                : "gen.bat：未配置";
            lblGenBatDot.ForeColor = genBatValid ? UITheme.StateValid : UITheme.StateInvalid;

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
                lblTemplateJobsDot.ForeColor = UITheme.StateWarning;
            else if (!tablesValid)
                lblTemplateJobsDot.ForeColor = UITheme.StateInvalid;
            else
                lblTemplateJobsDot.ForeColor = UITheme.StateValid;

            // 上次导出时间（仅显示，不指示配置对错）
            if (_localState?.LastExportTime.HasValue == true)
            {
                lblLastExport.Text = $"本地上次导出：{_localState.LastExportTime.Value:yyyy-MM-dd HH:mm}";
                lblLastExportDot.ForeColor = UITheme.StateValid;
            }
            else
            {
                lblLastExport.Text = "本地上次导出：从未导出";
                lblLastExportDot.ForeColor = Color.Gray;
            }

            // 配置检查（公共根仅用于内部一致性判断，显示的「根目录」统一为本地项目根目录）
            var (issues, _) = BuildConfigIssues();
            if (issues.Count == 0)
            {
                lblCheckDot.ForeColor = UITheme.StateValid;
                lblCheckSummary.Text = "配置检查通过";
                lblCheckIssues.Visible = false;
            }
            else
            {
                lblCheckDot.ForeColor = UITheme.StateWarning;
                lblCheckSummary.Text = $"发现 {issues.Count} 项配置问题：";
                lblCheckIssues.Text = string.Join("\n", issues.Select(s => "• " + s));
                lblCheckIssues.Visible = true;
            }
        }

        /// <summary>设置状态圆点颜色；不修改主 Label 的字体颜色。</summary>
        private static void SetDot(Label dot, bool ok)
            => dot.ForeColor = ok ? UITheme.StateValid : UITheme.StateWarning;

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

            // 检查 0：项目根目录未配置或不存在
            string projectRoot = _localState?.ProjectRoot ?? string.Empty;
            if (string.IsNullOrEmpty(projectRoot) || !Directory.Exists(projectRoot))
                issues.Add("项目根目录未配置或不存在，settings.json 中的相对路径将无法正确解析");

            // 检查 0.1：gen.bat 未配置或文件不存在（与主页红点判定一致）
            if (string.IsNullOrEmpty(_settings.GenBatPath) || !File.Exists(_settings.GenBatPath))
                issues.Add("gen.bat 未配置或文件不存在");

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
            RaiseExecutionState(true);
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
                RaiseExecutionState(false);
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
            RaiseExecutionState(true);
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
            RaiseExecutionState(false);
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
        /// HomeTab 编排子 Tab，重写为向各子 Tab 转发取消。
        /// </summary>
        public override void CancelRunningTask()
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

        // ── 项目根目录 ────────────────────────────────────────

        /// <summary>
        /// 若本机项目根目录未配置或已失效，则自动定位并设置 ProjectRoot（不覆盖已有的有效根目录）。
        /// 定位策略见 <see cref="FindProjectRootHybrid"/>。
        /// </summary>
        private void TryAutoDetectProjectRoot()
        {
            if (_localState == null || _settings == null) return;

            // 已有有效的根目录则不覆盖
            if (!string.IsNullOrEmpty(_localState.ProjectRoot) && Directory.Exists(_localState.ProjectRoot))
                return;

            try
            {
                var found = FindProjectRootHybrid();
                if (found == null) return;

                _localState.ProjectRoot = found;
                LocalStateManager.Save(_localState);
                ProjectRootChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Log($"自动检测项目根目录失败：{ex.Message}", LogLevel.Warn);
            }
        }

        /// <summary>
        /// 混合策略定位项目根目录：优先用 settings.json 中已配置的相对路径做「指纹」验证候选根目录；
        /// 若一个路径都没配置（指纹法无从验证），再回退到按 ProjectName 查找同名文件夹。
        /// 均未命中返回 null。
        /// </summary>
        private string? FindProjectRootHybrid()
        {
            // 指纹法：用已配置的相对路径验证候选根目录
            var relativePaths = SettingsManager.GetConfiguredRelativePaths();
            var found = FunctionLibrary.TryFindProjectRootByPaths(relativePaths, AppContext.BaseDirectory);
            if (found != null) return found;

            // 回退：按项目名称查找同名文件夹
            if (_settings != null && !string.IsNullOrEmpty(_settings.ProjectName))
                return FunctionLibrary.TryFindProjectRoot(_settings.ProjectName, AppContext.BaseDirectory, _settings.FuzzyFindProjectRoot);

            return null;
        }

        private void txtProjectName_Leave(object? sender, EventArgs e)
        {
            if (_settings == null) return;
            var newName = txtProjectName.Text.Trim();
            if (string.Equals(newName, _settings.ProjectName, StringComparison.Ordinal)) return;

            _settings.ProjectName = newName;
            SettingsManager.Save(_settings);
            TryAutoDetectProjectRoot();
            RefreshStatus();
        }

        private void chkFuzzyFindProjectRoot_CheckedChanged(object? sender, EventArgs e)
        {
            if (_settings == null) return;
            _settings.FuzzyFindProjectRoot = chkFuzzyFindProjectRoot.Checked;
            SettingsManager.Save(_settings);
            TryAutoDetectProjectRoot();
            RefreshStatus();
        }

        private void btnBrowseProjectRoot_Click(object sender, EventArgs e)
        {
            if (_localState == null || _settings == null) return;

            using var dlg = new FolderBrowserDialog
            {
                Description = "选择本地项目根目录（如 GodsClash/ 所在文件夹）",
                UseDescriptionForTitle = true,
            };

            if (!string.IsNullOrEmpty(_localState.ProjectRoot) && Directory.Exists(_localState.ProjectRoot))
                dlg.InitialDirectory = _localState.ProjectRoot;

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            _localState.ProjectRoot = dlg.SelectedPath;
            LocalStateManager.Save(_localState);
            // 根目录变化后，由 MainForm 从磁盘按新根重新还原设置（settings.json 的相对路径为唯一事实来源）。
            // 不在此处保存内存中的 _settings，避免把基于旧根的路径固化写回。
            ProjectRootChanged?.Invoke(this, EventArgs.Empty);
            RefreshStatus();
        }

        private void btnClearProjectRoot_Click(object sender, EventArgs e)
        {
            if (_localState == null) return;
            if (string.IsNullOrEmpty(_localState.ProjectRoot)) return;

            _localState.ProjectRoot = string.Empty;
            LocalStateManager.Save(_localState);
            // 清空后同样从磁盘重新还原（此时回退到 exe 目录基准）。
            ProjectRootChanged?.Invoke(this, EventArgs.Empty);
            RefreshStatus();
        }

        private void btnFindProjectRoot_Click(object sender, EventArgs e)
        {
            if (_localState == null || _settings == null) return;

            string? found;
            try { found = FindProjectRootHybrid(); }
            catch (Exception ex)
            {
                Log($"自动查找项目根目录失败：{ex.Message}", LogLevel.Warn);
                return;
            }

            if (found == null)
            {
                MessageBox.Show(
                    "未能自动定位项目根目录。\n\n请确认工具位于项目目录内，且 settings.json 中已配置至少一个相对路径（或已填写项目名称）。\n也可点击「设置根目录...」手动指定。",
                    "自动查找根目录",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            _localState.ProjectRoot = found;
            LocalStateManager.Save(_localState);
            ProjectRootChanged?.Invoke(this, EventArgs.Empty);
            RefreshStatus();
        }

        // ── 日志右键菜单 ──────────────────────────────────────

        private void ctxMenuItemClearLog_Click(object sender, EventArgs e)
            => txtLog.Clear();

        private void ctxMenuItemCopyLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

    }
}
