using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 导出 Excel 功能选项卡。
    /// 根据 Luban XML 定义自动创建或更新 Excel 文件，支持列表模式和批量导出两种模式。
    /// </summary>
    public partial class ExcelExportTab : TabBase
    {
        public ExcelExportTab()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LocalState LocalState { get; set; } = new();

        protected override RichTextBox? LogBox => txtLog;

        /// <summary>本次导出中走了通用文件夹的条目及其计算出的目标路径，导出成功后回写以建立关联。</summary>
        private readonly List<(ExcelExportClassConfig cfg, string computedPath)> _pendingAssociations = new();

        // ── 设置加载 ──────────────────────────────────────────────────────

        public void LoadFromSettings()
        {
            txtXmlFolder.Text          = Settings.ExcelExportXmlFolder;
            txtDesignTemplate.Text     = Settings.ExcelExportDesignTemplate;
            txtListTargetFolder.Text   = Settings.ExcelExportListTargetFolder;
            chkListCommonFolder.Checked = Settings.ExcelExportListCommonFolderEnabled;
            UpdateListCommonFolderEnabled(Settings.ExcelExportListCommonFolderEnabled);
            txtTargetFolder.Text       = Settings.ExcelExportTargetFolder;
            txtSingleXmlFile.Text      = LocalState.ExcelExportSingleXmlFile;
            txtSingleTargetPath.Text   = LocalState.ExcelExportSingleTargetPath;
            txtPrefix.Text             = Settings.ExcelExportNamePrefix;
            txtSuffix.Text             = Settings.ExcelExportNameSuffix;

            tabMode.SelectedIndex = Settings.ExcelExportMode switch { 1 => 1, 2 => 2, _ => 0 };

            rdoNameAsIs.Checked  = Settings.ExcelExportNameConvention == 0;
            rdoNameCamel.Checked = Settings.ExcelExportNameConvention == 1;
            rdoNameSnake.Checked = Settings.ExcelExportNameConvention == 2;

            chkRunEnumValidation.Checked = Settings.ExcelExportRunEnumValidation;

            if (!string.IsNullOrEmpty(Settings.ExcelExportXmlFolder))
            {
                RefreshClassList(rescanXml: false);
                RefreshSingleClassList(rescanXml: true);
            }
        }

        // ── 设置同步 ──────────────────────────────────────────────────────

        private void txtXmlFolder_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportXmlFolder = txtXmlFolder.Text;

        private void txtDesignTemplate_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportDesignTemplate = txtDesignTemplate.Text;

        private void txtListTargetFolder_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportListTargetFolder = txtListTargetFolder.Text;

        private void chkListCommonFolder_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ExcelExportListCommonFolderEnabled = chkListCommonFolder.Checked;
            UpdateListCommonFolderEnabled(chkListCommonFolder.Checked);
        }

        /// <summary>统一控制“通用导出文件夹”一组控件的启用状态。</summary>
        private void UpdateListCommonFolderEnabled(bool enabled)
        {
            txtListTargetFolder.Enabled = enabled;
            btnBrowseListFolder.Enabled = enabled;
            btnOpenListFolder.Enabled   = enabled;
        }

        private void txtTargetFolder_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportTargetFolder = txtTargetFolder.Text;

        private void txtSingleXmlFile_TextChanged(object sender, EventArgs e)
            => LocalState.ExcelExportSingleXmlFile = txtSingleXmlFile.Text;

        private void txtSingleTargetPath_TextChanged(object sender, EventArgs e)
            => LocalState.ExcelExportSingleTargetPath = txtSingleTargetPath.Text;

        private void txtPrefix_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportNamePrefix = txtPrefix.Text;

        private void txtSuffix_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportNameSuffix = txtSuffix.Text;

        private void tabMode_SelectedIndexChanged(object sender, EventArgs e)
            => Settings.ExcelExportMode = tabMode.SelectedIndex;

        private void rdoNameAsIs_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNameAsIs.Checked) Settings.ExcelExportNameConvention = 0;
        }

        private void rdoNameCamel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNameCamel.Checked) Settings.ExcelExportNameConvention = 1;
        }

        private void rdoNameSnake_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNameSnake.Checked) Settings.ExcelExportNameConvention = 2;
        }

        private void chkRunEnumValidation_CheckedChanged(object sender, EventArgs e)
            => Settings.ExcelExportRunEnumValidation = chkRunEnumValidation.Checked;

        // ── 浏览按钮 ──────────────────────────────────────────────────────

        private void btnOpenXmlFolder_Click(object sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(txtXmlFolder.Text);

        private void btnOpenTemplate_Click(object sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(txtDesignTemplate.Text);

        private void btnOpenListFolder_Click(object sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(txtListTargetFolder.Text);

        private void btnOpenTargetFolder_Click(object sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(txtTargetFolder.Text);

        private void btnBrowseXmlFolder_Click(object sender, EventArgs e)
        {
            var path = DialogHelper.BrowseFolder("选择 XML 定义文件夹", txtXmlFolder.Text);
            if (path != null)
            {
                txtXmlFolder.Text = path;
                RefreshClassList(rescanXml: true);
            }
        }

        private void btnBrowseTemplate_Click(object sender, EventArgs e)
        {
            var files = DialogHelper.BrowseFiles("选择 Excel 设计模板", "Excel 文件 (*.xlsx)|*.xlsx",
                txtDesignTemplate.Text);
            if (files.Length > 0)
                txtDesignTemplate.Text = files[0];
        }

        private void btnClearTemplate_Click(object sender, EventArgs e)
            => txtDesignTemplate.Text = string.Empty;

        private void btnBrowseListFolder_Click(object sender, EventArgs e)
        {
            var path = DialogHelper.BrowseFolder("选择通用目标文件夹", txtListTargetFolder.Text);
            if (path != null)
                txtListTargetFolder.Text = path;
        }

        private void btnBrowseTargetFolder_Click(object sender, EventArgs e)
        {
            var path = DialogHelper.BrowseFolder("选择导出 Excel 目标文件夹", txtTargetFolder.Text);
            if (path != null)
                txtTargetFolder.Text = path;
        }

        // ── 单独导出模式：来源/目标按钮 ────────────────────────────────────

        private void btnOpenSingleXml_Click(object sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(txtSingleXmlFile.Text);

        private void btnOpenSingleTarget_Click(object sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(txtSingleTargetPath.Text);

        private void btnBrowseSingleXml_Click(object sender, EventArgs e)
        {
            var files = DialogHelper.BrowseFiles("选择 XML 来源文件", "XML 文件 (*.xml)|*.xml",
                txtSingleXmlFile.Text);
            if (files.Length > 0)
            {
                txtSingleXmlFile.Text = files[0];
                RefreshSingleClassList(rescanXml: true);
            }
        }

        private void btnClearSingleXml_Click(object sender, EventArgs e)
        {
            txtSingleXmlFile.Text = string.Empty;
            RefreshSingleClassList(rescanXml: true);
        }

        private void btnBrowseSingleTarget_Click(object sender, EventArgs e)
        {
            string className   = LocalState.ExcelExportSingleClassName?.Trim() ?? string.Empty;
            string defaultName  = string.IsNullOrEmpty(className)
                ? "export.xlsx"
                : FunctionLibrary.ApplyNameConvention(className, Settings.ExcelExportNameConvention) + ".xlsx";

            string? initial = !string.IsNullOrEmpty(txtSingleTargetPath.Text)
                ? txtSingleTargetPath.Text
                : Settings.ExcelExportListTargetFolder;

            var path = DialogHelper.BrowseSaveFile(
                "选择导出 Excel 目标文件",
                "Excel 文件 (*.xlsx)|*.xlsx",
                initial,
                defaultName);

            if (path != null)
                txtSingleTargetPath.Text = path;
        }

        private void btnClearSingleTarget_Click(object sender, EventArgs e)
            => txtSingleTargetPath.Text = string.Empty;

        // ── 列表模式：刷新列表 ────────────────────────────────────────────

        private void btnRefresh_Click(object sender, EventArgs e)
            => RefreshClassList(rescanXml: true);

        private void btnSelectAll_Click(object sender, EventArgs e)
            => SetAllEnabled(true);

        private void btnDeselectAll_Click(object sender, EventArgs e)
            => SetAllEnabled(false);

        private void SetAllEnabled(bool enabled)
        {
            foreach (var cfg in Settings.ExcelExportClassConfigs)
                cfg.Enabled = enabled;

            foreach (DataGridViewRow row in dgvClasses.Rows)
            {
                row.Cells[colEnabled.Index].Value = enabled;
                if (row.Tag is ExcelExportClassConfig cfg)
                    cfg.Enabled = enabled;
            }
        }

        /// <summary>
        /// 刷新叶子类列表。
        /// <paramref name="rescanXml"/> 为 true 时重新扫描 XML；为 false 时仅用保存的配置填充列表。
        /// </summary>
        private void RefreshClassList(bool rescanXml)
        {
            string folder = txtXmlFolder.Text.Trim();

            if (rescanXml)
            {
                if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                {
                    dgvClasses.Rows.Clear();
                    return;
                }

                try
                {
                    var allBeans  = BeanParser.ParseFolder(folder);
                    var leafBeans = BeanParser.GetLeafBeans(allBeans);

                    var existing = Settings.ExcelExportClassConfigs
                        .ToDictionary(c => c.ClassName, StringComparer.Ordinal);

                    var newConfigs = leafBeans.Select(b => new ExcelExportClassConfig
                    {
                        Enabled         = existing.TryGetValue(b.Name, out var e) ? e.Enabled : true,
                        ClassName       = b.Name,
                        SourceXmlFile   = b.SourceFile,
                        TargetExcelPath = existing.TryGetValue(b.Name, out var ec) ? ec.TargetExcelPath : string.Empty,
                    }).ToList();

                    Settings.ExcelExportClassConfigs = newConfigs;
                }
                catch (Exception ex)
                {
                    Log($"扫描 XML 失败：{ex.Message}", LogLevel.Error);
                    return;
                }
            }

            dgvClasses.Rows.Clear();
            foreach (var cfg in Settings.ExcelExportClassConfigs)
            {
                int rowIdx = dgvClasses.Rows.Add(
                    cfg.Enabled,
                    cfg.ClassName,
                    Path.GetFileName(cfg.SourceXmlFile),
                    cfg.TargetExcelPath);
                dgvClasses.Rows[rowIdx].Tag = cfg;
                RefreshTargetPathWarning(dgvClasses.Rows[rowIdx]);
            }

            Log($"找到 {Settings.ExcelExportClassConfigs.Count} 个可导出数据类。", LogLevel.Info);
        }

        // ── 单独导出模式：刷新列表 ────────────────────────────────────────

        /// <summary>填充列表时为 true，避免 SelectionChanged 把临时选中行误写入设置。</summary>
        private bool _loadingSingleList;

        /// <summary>正在同步单选状态时为 true，避免 ApplySingleSelection 与 SelectionChanged 互相递归。</summary>
        private bool _syncingSingleSelection;

        private void btnRefreshSingle_Click(object sender, EventArgs e)
            => RefreshSingleClassList(rescanXml: true);

        /// <summary>
        /// 刷新单独导出模式的数据类列表。
        /// 始终解析整个通用 XML 文件夹以保证跨文件继承上下文；
        /// 若指定了单个 XML 来源文件，则仅显示该文件中定义的叶子类，否则显示全部。
        /// </summary>
        private void RefreshSingleClassList(bool rescanXml)
        {
            string folder     = txtXmlFolder.Text.Trim();
            string singleFile = txtSingleXmlFile.Text.Trim();

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                dgvSingleClasses.Rows.Clear();
                return;
            }

            List<BeanInfo> leafBeans;
            try
            {
                var allBeans = BeanParser.ParseFolder(folder);
                leafBeans = BeanParser.GetLeafBeans(allBeans);
            }
            catch (Exception ex)
            {
                Log($"扫描 XML 失败：{ex.Message}", LogLevel.Error);
                return;
            }

            if (!string.IsNullOrEmpty(singleFile) && File.Exists(singleFile))
            {
                leafBeans = leafBeans
                    .Where(b => PathEquals(b.SourceFile, singleFile))
                    .ToList();
            }

            _loadingSingleList = true;
            try
            {
                dgvSingleClasses.ClearSelection();
                dgvSingleClasses.Rows.Clear();
                foreach (var bean in leafBeans)
                {
                    int rowIdx = dgvSingleClasses.Rows.Add(false, bean.Name, Path.GetFileName(bean.SourceFile));
                    dgvSingleClasses.Rows[rowIdx].Tag = bean.Name;
                }
            }
            finally
            {
                _loadingSingleList = false;
            }

            // 还原上次选中的数据类（若仍在列表中），否则清空选中。
            string saved = LocalState.ExcelExportSingleClassName?.Trim() ?? string.Empty;
            bool exists = !string.IsNullOrEmpty(saved) &&
                dgvSingleClasses.Rows.Cast<DataGridViewRow>()
                    .Any(r => r.Tag is string n && n == saved);
            ApplySingleSelection(exists ? saved : null);

            if (rescanXml)
                Log($"单独导出：找到 {dgvSingleClasses.Rows.Count} 个可选数据类。", LogLevel.Info);
        }

        private void dgvSingleClasses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvSingleClasses.Rows[e.RowIndex].Tag is not string name) return;

            bool isChecked = dgvSingleClasses.Rows[e.RowIndex].Cells[colSingleSelected.Index].Value is true;

            // 点击“选中”列且该行已勾选 → 取消选中；其余情况（点击该列未勾选行，或点击行其他单元格）→ 选中该行。
            if (e.ColumnIndex == colSingleSelected.Index && isChecked)
                ApplySingleSelection(null);
            else
                ApplySingleSelection(name);
        }

        private void dgvSingleClasses_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingSingleList || _syncingSingleSelection) return;
            if (dgvSingleClasses.SelectedRows.Count > 0 &&
                dgvSingleClasses.SelectedRows[0].Tag is string name)
                ApplySingleSelection(name);
        }

        /// <summary>
        /// 应用单选：同步“选中”勾选列（仅目标行勾选）、行选中状态与设置中的选中类名。
        /// <paramref name="className"/> 为 null 表示清空选中。
        /// </summary>
        private void ApplySingleSelection(string? className)
        {
            if (_syncingSingleSelection) return;
            _syncingSingleSelection = true;
            try
            {
                DataGridViewRow? target = null;
                foreach (DataGridViewRow row in dgvSingleClasses.Rows)
                {
                    bool match = row.Tag is string n && n == className;
                    row.Cells[colSingleSelected.Index].Value = match;
                    if (match) target = row;
                }

                if (target != null)
                {
                    target.Selected = true;
                    dgvSingleClasses.CurrentCell = target.Cells[colSingleClassName.Index];
                    LocalState.ExcelExportSingleClassName = className!;
                }
                else
                {
                    dgvSingleClasses.ClearSelection();
                    LocalState.ExcelExportSingleClassName = string.Empty;
                }
            }
            finally
            {
                _syncingSingleSelection = false;
            }
        }

        /// <summary>规范化后比较两个路径是否指向同一文件（忽略大小写）。</summary>
        private static bool PathEquals(string a, string b)
        {
            try
            {
                return string.Equals(Path.GetFullPath(a), Path.GetFullPath(b),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            }
        }

        // ── DataGridView 交互 ─────────────────────────────────────────────

        private void dgvClasses_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvClasses.Rows[e.RowIndex];
            if (row.Tag is not ExcelExportClassConfig cfg) return;

            if (e.ColumnIndex == colEnabled.Index)
                cfg.Enabled = (bool)(row.Cells[colEnabled.Index].Value ?? false);
            else if (e.ColumnIndex == colTargetPath.Index)
            {
                cfg.TargetExcelPath = (string)(row.Cells[colTargetPath.Index].Value ?? string.Empty);
                RefreshTargetPathWarning(row);
            }
        }

        /// <summary>
        /// 重新评估所有行的“目标 Excel 路径”警示底色。
        /// 供窗口重新获得焦点时调用，以反映用户在程序外删除/移动目标文件后的最新状态。
        /// </summary>
        public void RefreshTargetPathWarnings()
        {
            foreach (DataGridViewRow row in dgvClasses.Rows)
                RefreshTargetPathWarning(row);
        }

        /// <summary>
        /// 刷新某行“目标 Excel 路径”单元格的警示底色：
        /// 当路径已设置但对应文件不存在时标记警示色，否则恢复默认。
        /// </summary>
        private void RefreshTargetPathWarning(DataGridViewRow row)
        {
            if (row.Tag is not ExcelExportClassConfig cfg) return;

            bool invalid = !string.IsNullOrEmpty(cfg.TargetExcelPath) && !File.Exists(cfg.TargetExcelPath);
            row.Cells[colTargetPath.Index].Style.BackColor = invalid ? UITheme.CellWarnBack : SystemColors.Window;
        }

        private void dgvClasses_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvClasses.IsCurrentCellDirty)
                dgvClasses.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvClasses_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == colBrowse.Index)
            {
                var row = dgvClasses.Rows[e.RowIndex];
                if (row.Tag is not ExcelExportClassConfig cfg) return;

                string? existing    = string.IsNullOrEmpty(cfg.TargetExcelPath) ? null : cfg.TargetExcelPath;
                string  defaultName = FunctionLibrary.ApplyNameConvention(cfg.ClassName, Settings.ExcelExportNameConvention) + ".xlsx";
                var path = DialogHelper.BrowseSaveFile(
                    $"选择 [{cfg.ClassName}] 的导出 Excel 文件",
                    "Excel 文件 (*.xlsx)|*.xlsx",
                    existing ?? Settings.ExcelExportListTargetFolder ?? Settings.ExcelExportTargetFolder,
                    defaultName);

                if (path != null)
                {
                    cfg.TargetExcelPath = path;
                    row.Cells[colTargetPath.Index].Value = path;
                    RefreshTargetPathWarning(row);
                }
            }
            else if (e.ColumnIndex == colClearPath.Index)
            {
                var row = dgvClasses.Rows[e.RowIndex];
                if (row.Tag is not ExcelExportClassConfig cfg) return;

                cfg.TargetExcelPath = string.Empty;
                row.Cells[colTargetPath.Index].Value = string.Empty;
                RefreshTargetPathWarning(row);
            }
        }

        // ── 主流程 ────────────────────────────────────────────────────────

        private void SetUILocked(bool locked)
        {
            txtXmlFolder.Enabled       = !locked;
            btnBrowseXmlFolder.Enabled = !locked;
            txtDesignTemplate.Enabled  = !locked;
            btnBrowseTemplate.Enabled  = !locked;
            tabMode.Enabled            = !locked;
            chkRunEnumValidation.Enabled = !locked;
            txtSingleXmlFile.Enabled       = !locked;
            btnBrowseSingleXml.Enabled     = !locked;
            btnClearSingleXml.Enabled      = !locked;
            btnRefreshSingle.Enabled       = !locked;
            dgvSingleClasses.Enabled       = !locked;
            txtSingleTargetPath.Enabled    = !locked;
            btnBrowseSingleTarget.Enabled  = !locked;
            btnClearSingleTarget.Enabled   = !locked;
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            string xmlFolder = txtXmlFolder.Text.Trim();
            if (string.IsNullOrEmpty(xmlFolder) || !Directory.Exists(xmlFolder))
            {
                Log("请设置有效的 XML 来源文件夹。", LogLevel.Error);
                return;
            }

            List<ExcelExportTask>? tasks = null;
            try
            {
                tasks = BuildTasks(xmlFolder);
            }
            catch (Exception ex)
            {
                Log($"扫描 XML 失败：{ex.Message}", LogLevel.Error);
                return;
            }

            if (tasks == null || tasks.Count == 0)
            {
                Log("没有需要导出的数据类（请检查设置是否完整）。", LogLevel.Warn);
                return;
            }

            // 批量导出模式：检查重名
            if (tabMode.SelectedIndex == 1)
            {
                var duplicates = tasks
                    .GroupBy(t => t.TargetExcelPath, StringComparer.OrdinalIgnoreCase)
                    .Where(g => g.Count() > 1)
                    .Select(g => Path.GetFileName(g.Key))
                    .ToList();

                if (duplicates.Count > 0)
                {
                    Log($"批量导出存在重名文件（请检查数据类是否跨 XML 文件重名）：{string.Join(", ", duplicates)}", LogLevel.Error);
                    return;
                }
            }

            var allBeans    = BeanParser.ParseFolder(xmlFolder);
            var beanMap     = BeanParser.BuildBeanMap(allBeans);
            var usedAsField = BeanParser.BuildUsedAsFieldSet(allBeans);

            string? templatePath = txtDesignTemplate.Text.Trim();
            if (string.IsNullOrEmpty(templatePath))
                templatePath = Settings.TableDesignSourceExcel;
            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                templatePath = null;

            var options = new ExcelExportOptions(templatePath, tasks, beanMap, usedAsField);

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SetUILocked(true);
            RaiseExecutionState(true);
            btnExport.Enabled = false;
            btnCancel.Enabled   = true;
            ProgressBarHelper.SetProgressBegin(pbExport);
            LogDivider();
            Log($"开始导出 {tasks.Count} 个数据类...", LogLevel.Section);

            try
            {
                int done = 0;
                await Task.Run(() =>
                {
                    ExcelExporter.ExportAll(
                        options,
                        (msg, level) =>
                        {
                            LogLibrary.Write(txtLog, msg, level);
                            if (level == LogLevel.Ok)
                            {
                                done++;
                                ProgressBarHelper.SetProgress(pbExport, ScaledProgress(done, tasks.Count));
                            }
                        },
                        token);
                }, token);

                Log($"导出完成。共处理 {tasks.Count} 个数据类。", LogLevel.Ok);

                // 将走了通用文件夹的条目回写目标路径，建立固定关联。
                WriteBackAssociations();

                // 导出后可选地对导出的 Excel 执行 Enum 数据验证（复用枚举验证功能）。
                if (chkRunEnumValidation.Checked)
                    await RunEnumValidationOnExportAsync(xmlFolder, tasks, token);

                ProgressBarHelper.SetProgress(pbExport, 100);
            }
            catch (OperationCanceledException)
            {
                Log("操作已停止。", LogLevel.Warn);
            }
            catch (Exception ex)
            {
                Log($"未预期的错误：{ex.Message}", LogLevel.Error);
            }
            finally
            {
                SetUILocked(false);
                RaiseExecutionState(false);
                btnExport.Enabled = true;
                btnCancel.Enabled   = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        /// <summary>
        /// 对刚导出的 Excel 文件执行 Enum 数据验证：复用 <see cref="EnumValidationService"/> 扫描
        /// 本页 XML 文件夹的枚举定义，再交给 <see cref="ValidationUpdater"/> 写入验证规则。
        /// 验证开关与 HideEnumDataSheet/BoolValidation/EnumForceRewrite 均复用全局设置。
        /// </summary>
        private async Task RunEnumValidationOnExportAsync(
            string xmlFolder, List<ExcelExportTask> tasks, CancellationToken token)
        {
            LogDivider();
            Log("对导出的 Excel 执行 Enum 验证...", LogLevel.Section);

            var files = tasks
                .Select(t => t.TargetExcelPath)
                .Where(p => !string.IsNullOrEmpty(p) && File.Exists(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (files.Count == 0)
            {
                Log("没有可验证的导出文件。", LogLevel.Warn);
                return;
            }

            await Task.Run(() =>
            {
                var prepared = EnumValidationService.PrepareEnums(
                    xmlFolder, Settings.BoolValidation, (m, l) => Log(m, l), token);

                if (!prepared.HasAny)
                {
                    Log("未找到任何 Enum 定义，跳过验证。", LogLevel.Warn);
                    return;
                }

                var results = ValidationUpdater.UpdateFiles(
                    files,
                    prepared.EnumsForValidation,
                    Settings.HideEnumDataSheet,
                    r => Log(
                        r.HasError      ? $"  {r.FileName}  —  错误：{r.ErrorMessage}"
                        : r.WasSkipped  ? $"  {r.FileName}  —  跳过（文件被占用）"
                        : $"  {r.FileName}  —  {r.EnumColumnsFound} 个枚举列",
                        r.HasError ? LogLevel.Error : r.WasSkipped ? LogLevel.Warn : LogLevel.Ok),
                    forceRewrite: LocalState.EnumForceRewrite,
                    beanFieldEnumMap: prepared.BeanFieldEnumMap);

                int cols = results.Sum(r => r.EnumColumnsFound);
                Log($"Enum 验证完成。处理 {results.Count} 个文件，共 {cols} 个枚举列。", LogLevel.Ok);
            }, token);
        }

        /// <summary>导出成功后，把走了通用文件夹的条目计算路径回写到配置与列表，建立固定关联。</summary>
        private void WriteBackAssociations()
        {
            if (_pendingAssociations.Count == 0) return;

            foreach (var (cfg, computedPath) in _pendingAssociations)
            {
                cfg.TargetExcelPath = computedPath;
                foreach (DataGridViewRow row in dgvClasses.Rows)
                {
                    if (!ReferenceEquals(row.Tag, cfg)) continue;
                    row.Cells[colTargetPath.Index].Value = computedPath;
                    RefreshTargetPathWarning(row);
                    break;
                }
            }
            _pendingAssociations.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e) => _cts?.Cancel();

        private void btnClearLog_Click(object sender, EventArgs e) => txtLog.Clear();

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

        // ── 任务构建 ──────────────────────────────────────────────────────

        private List<ExcelExportTask> BuildTasks(string xmlFolder)
        {
            var allBeans  = BeanParser.ParseFolder(xmlFolder);
            var beanMap   = BeanParser.BuildBeanMap(allBeans);
            var leafBeans = BeanParser.GetLeafBeans(allBeans);
            var tasks     = new List<ExcelExportTask>();

            string prefix     = txtPrefix.Text;
            string suffix     = txtSuffix.Text;
            int    convention = Settings.ExcelExportNameConvention;

            _pendingAssociations.Clear();

            if (tabMode.SelectedIndex == 0)
            {
                // 列表模式：
                //  - 已设路径且文件存在 → 更新该文件
                //  - 已设路径但文件不存在 → 跳过并警示（非破坏性，不新建、不回退）
                //  - 未设路径 → 启用通用文件夹时导出到通用文件夹并记录回写关联，否则跳过
                bool   commonEnabled = Settings.ExcelExportListCommonFolderEnabled;
                string commonFolder  = txtListTargetFolder.Text.Trim();

                foreach (var cfg in Settings.ExcelExportClassConfigs)
                {
                    if (!cfg.Enabled) continue;

                    var bean = leafBeans.FirstOrDefault(b => b.Name == cfg.ClassName);
                    if (bean == null) continue;

                    string targetPath;
                    bool   isAssociation = false;

                    if (!string.IsNullOrEmpty(cfg.TargetExcelPath))
                    {
                        if (!File.Exists(cfg.TargetExcelPath))
                        {
                            Log($"跳过 [{cfg.ClassName}]：目标 Excel 路径不存在 —— {cfg.TargetExcelPath}", LogLevel.Warn);
                            continue;
                        }
                        targetPath = cfg.TargetExcelPath;
                    }
                    else
                    {
                        if (!commonEnabled || string.IsNullOrEmpty(commonFolder))
                            continue;

                        string baseName = FunctionLibrary.ApplyNameConvention(cfg.ClassName, convention);
                        targetPath = Path.Combine(commonFolder, $"{prefix}{baseName}{suffix}.xlsx");
                        isAssociation = true;
                    }

                    var allFields = BeanParser.GetAllFields(bean, beanMap);
                    tasks.Add(new ExcelExportTask(bean, allFields, targetPath));

                    if (isAssociation)
                        _pendingAssociations.Add((cfg, targetPath));
                }
            }
            else if (tabMode.SelectedIndex == 2)
            {
                // 单独导出模式：
                //  - 来源 = 单文件（若设）否则整个文件夹；继承上下文始终来自整个文件夹的 beanMap
                //  - 取选中类（LocalState.ExcelExportSingleClassName），目标 = 统一目标路径
                //  - 目标存在 → 更新，不存在 → 新建（ExportAll/CreateExcel 自动处理）
                string className  = LocalState.ExcelExportSingleClassName?.Trim() ?? string.Empty;
                string targetPath = txtSingleTargetPath.Text.Trim();
                if (string.IsNullOrEmpty(className))
                {
                    Log("请在列表中选择一个数据类。", LogLevel.Warn);
                    return tasks;
                }
                if (string.IsNullOrEmpty(targetPath))
                {
                    Log("请设置导出 Excel 目标路径。", LogLevel.Warn);
                    return tasks;
                }

                var bean = leafBeans.FirstOrDefault(b => b.Name == className);
                string singleFile = txtSingleXmlFile.Text.Trim();
                if (bean == null ||
                    (!string.IsNullOrEmpty(singleFile) && File.Exists(singleFile) &&
                     !PathEquals(bean.SourceFile, singleFile)))
                {
                    Log($"选中的数据类 [{className}] 不在当前 XML 来源中，请刷新列表后重选。", LogLevel.Warn);
                    return tasks;
                }

                var allFields = BeanParser.GetAllFields(bean, beanMap);
                tasks.Add(new ExcelExportTask(bean, allFields, targetPath));
            }
            else
            {
                // 批量导出模式
                string targetFolder = txtTargetFolder.Text.Trim();
                if (string.IsNullOrEmpty(targetFolder))
                    return tasks;

                foreach (var bean in leafBeans)
                {
                    string baseName = FunctionLibrary.ApplyNameConvention(bean.Name, convention);
                    string fileName = $"{prefix}{baseName}{suffix}.xlsx";
                    string path     = Path.Combine(targetFolder, fileName);
                    var    allFields = BeanParser.GetAllFields(bean, beanMap);
                    tasks.Add(new ExcelExportTask(bean, allFields, path));
                }
            }

            return tasks;
        }

    }
}
