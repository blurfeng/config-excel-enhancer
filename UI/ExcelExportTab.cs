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

        protected override string? GreetingMessage => "导出 Excel 已就绪 — 按 XML 定义创建/更新 Excel 文件。";

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
            rdoSingleTargetFolder.Checked = LocalState.ExcelExportSingleTargetMode != 1;
            rdoSingleTargetFile.Checked   = LocalState.ExcelExportSingleTargetMode == 1;
            ApplySingleTargetMode(LocalState.ExcelExportSingleTargetMode);
            txtPrefix.Text             = LocalState.ExcelExportNamePrefix;
            txtSuffix.Text             = LocalState.ExcelExportNameSuffix;

            tabMode.SelectedIndex = LocalState.ExcelExportMode switch { 1 => 1, 2 => 2, _ => 0 };

            rdoNameAsIs.Checked  = LocalState.ExcelExportNameConvention == 0;
            rdoNameCamel.Checked = LocalState.ExcelExportNameConvention == 1;
            rdoNameSnake.Checked = LocalState.ExcelExportNameConvention == 2;

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
        {
            // 切换目标形式时文本框由程序赋值，避免把另一形式的值误写入当前字段。
            if (_loadingSingleTarget) return;
            if (LocalState.ExcelExportSingleTargetMode == 1)
                LocalState.ExcelExportSingleTargetFile = txtSingleTargetPath.Text;
            else
                LocalState.ExcelExportSingleTargetFolder = txtSingleTargetPath.Text;
        }

        private void txtPrefix_TextChanged(object sender, EventArgs e)
            => LocalState.ExcelExportNamePrefix = txtPrefix.Text;

        private void txtSuffix_TextChanged(object sender, EventArgs e)
            => LocalState.ExcelExportNameSuffix = txtSuffix.Text;

        private void tabMode_SelectedIndexChanged(object sender, EventArgs e)
            => LocalState.ExcelExportMode = tabMode.SelectedIndex;

        private void rdoNameAsIs_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNameAsIs.Checked) LocalState.ExcelExportNameConvention = 0;
        }

        private void rdoNameCamel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNameCamel.Checked) LocalState.ExcelExportNameConvention = 1;
        }

        private void rdoNameSnake_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNameSnake.Checked) LocalState.ExcelExportNameConvention = 2;
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
            string? initial = !string.IsNullOrEmpty(txtSingleTargetPath.Text)
                ? txtSingleTargetPath.Text
                : Settings.ExcelExportListTargetFolder;

            if (LocalState.ExcelExportSingleTargetMode == 1)
            {
                // 文件形式：保存文件对话框，默认名按当前命名规则生成（仅作建议，可改）。
                string className   = LocalState.ExcelExportSingleClassName?.Trim() ?? string.Empty;
                string defaultName = string.IsNullOrEmpty(className)
                    ? string.Empty
                    : FunctionLibrary.BuildExcelFileName(
                        className, LocalState.ExcelExportNameConvention, txtPrefix.Text, txtSuffix.Text);

                var file = DialogHelper.BrowseSaveFile(
                    "选择导出 Excel 目标文件", "Excel 文件 (*.xlsx)|*.xlsx", initial, defaultName);
                if (file != null)
                    txtSingleTargetPath.Text = file;
            }
            else
            {
                var path = DialogHelper.BrowseFolder("选择导出 Excel 目标文件夹", initial);
                if (path != null)
                    txtSingleTargetPath.Text = path;
            }
        }

        private void btnClearSingleTarget_Click(object sender, EventArgs e)
            => txtSingleTargetPath.Text = string.Empty;

        /// <summary>切换单独导出目标形式时为 true，避免文本框程序化赋值触发 TextChanged 误写另一字段。</summary>
        private bool _loadingSingleTarget;

        private void rdoSingleTargetFolder_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoSingleTargetFolder.Checked) return;
            LocalState.ExcelExportSingleTargetMode = 0;
            ApplySingleTargetMode(0);
        }

        private void rdoSingleTargetFile_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoSingleTargetFile.Checked) return;
            LocalState.ExcelExportSingleTargetMode = 1;
            ApplySingleTargetMode(1);
        }

        /// <summary>
        /// 按目标形式（0 = 文件夹，1 = 文件）刷新目标行：还原对应形式已保存的路径文本与提示。
        /// 文本框由程序赋值，故包裹 <see cref="_loadingSingleTarget"/> 守卫避免误写。
        /// </summary>
        private void ApplySingleTargetMode(int mode)
        {
            _loadingSingleTarget = true;
            try
            {
                txtSingleTargetPath.Text = mode == 1
                    ? LocalState.ExcelExportSingleTargetFile
                    : LocalState.ExcelExportSingleTargetFolder;
            }
            finally
            {
                _loadingSingleTarget = false;
            }

            string tip = mode == 1
                ? "选中数据类导出到此 Excel 文件；文件名以你选择的为准（不套用“文件命名/文件名”规则），文件存在则更新、不存在则新建（目录自动创建）。"
                : "选中数据类导出到此文件夹；文件名按“文件命名/文件名”规则自动生成，文件存在则更新、不存在则新建（文件夹自动创建）。";
            toolTip.SetToolTip(txtSingleTargetPath, tip);
            toolTip.SetToolTip(btnBrowseSingleTarget, tip);
        }

        // ── 列表模式：刷新列表 ────────────────────────────────────────────

        private void btnRefresh_Click(object sender, EventArgs e)
            => RefreshClassList(rescanXml: true);

        private void btnSelectAll_Click(object sender, EventArgs e)
            => SetAllEnabled(true);

        private void btnDeselectAll_Click(object sender, EventArgs e)
            => SetAllEnabled(false);

        /// <summary>清空所有“目标 Excel 路径”已设置但对应文件不存在的项（执行前弹窗确认）。</summary>
        private void btnClearInvalidPaths_Click(object sender, EventArgs e)
        {
            var invalid = Settings.ExcelExportClassConfigs
                .Where(c => !string.IsNullOrEmpty(c.TargetExcelPath) && !File.Exists(c.TargetExcelPath))
                .ToList();

            if (invalid.Count == 0)
            {
                Log("没有需要清空的无效目标 Excel 路径。", LogLevel.Info);
                return;
            }

            if (MessageBox.Show(
                    $"将清空 {invalid.Count} 个无效的“目标 Excel 路径”（对应文件不存在）。确认执行？",
                    "清空无效目标 Excel 路径",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            foreach (var cfg in invalid)
                cfg.TargetExcelPath = string.Empty;

            foreach (DataGridViewRow row in dgvClasses.Rows)
            {
                if (row.Tag is not ExcelExportClassConfig cfg) continue;
                row.Cells[colTargetPath.Index].Value = cfg.TargetExcelPath;
                RefreshTargetPathWarning(row);
            }

            SettingsManager.Save(Settings);
            Log($"已清空 {invalid.Count} 个无效目标 Excel 路径。", LogLevel.Ok);
        }

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
                    Log($"扫描 XML 失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
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
                Log($"扫描 XML 失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
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
                string  defaultName = FunctionLibrary.ApplyNameConvention(cfg.ClassName, LocalState.ExcelExportNameConvention) + ".xlsx";
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
            btnRename.Enabled          = !locked;
            chkRunEnumValidation.Enabled = !locked;
            txtSingleXmlFile.Enabled       = !locked;
            btnBrowseSingleXml.Enabled     = !locked;
            btnClearSingleXml.Enabled      = !locked;
            btnRefreshSingle.Enabled       = !locked;
            dgvSingleClasses.Enabled       = !locked;
            rdoSingleTargetFolder.Enabled  = !locked;
            rdoSingleTargetFile.Enabled    = !locked;
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
                Log($"扫描 XML 失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
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
                Log($"未预期的错误：{LogLibrary.FormatException(ex, includeStackTrace: true)}", LogLevel.Error);
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
        /// HideEnumDataSheet/BoolValidation 复用全局设置；此处恒为强制重写：导出步骤会按新列
        /// 布局清空旧验证（含 RebuildColumnsInOrder 删除全部验证），故必须无条件按新列重新应用，
        /// 不受 Enum 选项卡的全局 EnumForceRewrite 开关影响。
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

                // 逐文件结果先静默收集；成功时只打印汇总，出现失败/跳过时再转写所有明细，保持 Log 专注。
                var results = ValidationUpdater.UpdateFiles(
                    files,
                    prepared.EnumsForValidation,
                    Settings.HideEnumDataSheet,
                    _ => { },
                    forceRewrite: true,
                    beanFieldEnumMap: prepared.BeanFieldEnumMap);

                int cols      = results.Sum(r => r.EnumColumnsFound);
                int failCount = results.Count(r => r.HasError);
                int skipCount = results.Count(r => r.WasSkipped);

                if (failCount == 0 && skipCount == 0)
                {
                    Log($"Enum 验证完成。处理 {results.Count} 个文件，共 {cols} 个枚举列。", LogLevel.Ok);
                }
                else
                {
                    // 失败时转写全部明细，便于定位问题。
                    foreach (var r in results)
                        Log(
                            r.HasError     ? $"  {r.FileName}  —  错误：{r.ErrorMessage}"
                            : r.WasSkipped ? $"  {r.FileName}  —  跳过（文件被占用）"
                            : $"  {r.FileName}  —  {r.EnumColumnsFound} 个枚举列",
                            r.HasError ? LogLevel.Error : r.WasSkipped ? LogLevel.Warn : LogLevel.Ok);

                    Log($"Enum 验证完成（失败 {failCount}、跳过 {skipCount}）。处理 {results.Count} 个文件，共 {cols} 个枚举列。",
                        failCount > 0 ? LogLevel.Error : LogLevel.Warn);
                }
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

        // ── 重命名 Excel ──────────────────────────────────────────────────

        private async void btnRename_Click(object sender, EventArgs e)
        {
            int    mode             = tabMode.SelectedIndex;
            int    convention       = LocalState.ExcelExportNameConvention;
            string prefix           = txtPrefix.Text;
            string suffix           = txtSuffix.Text;
            string batchFolder      = txtTargetFolder.Text.Trim();

            // 单独导出模式的文件名在导出时已按命名规则自动生成，无需重命名。
            if (mode == 2)
            {
                Log("单独导出模式无需重命名：导出时已按“文件命名/文件名”规则自动生成文件名。", LogLevel.Warn);
                return;
            }

            LogDivider();
            Log("构建重命名计划...", LogLevel.Section);

            // 候选构建（批量模式需读取文件内嵌类名）与计划生成放后台，避免阻塞 UI。
            IReadOnlyList<RenamePlanItem> plan;
            try
            {
                plan = await Task.Run(() =>
                {
                    var candidates = BuildRenameCandidates(mode, batchFolder);
                    return ExcelRenamer.BuildPlan(candidates, convention, prefix, suffix);
                });
            }
            catch (Exception ex)
            {
                Log($"构建重命名计划失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
                return;
            }

            var toRename   = plan.Where(p => p.Status == RenameStatus.Rename).ToList();
            int already    = plan.Count(p => p.Status == RenameStatus.AlreadyNamed);
            var conflicts  = plan.Where(p => p.Status == RenameStatus.Conflict).ToList();
            var unresolved = plan.Where(p => p.Status == RenameStatus.Unresolved).ToList();

            // 冲突 / 无法识别项写入日志便于排查。
            foreach (var c in conflicts)
                Log($"冲突跳过：{Path.GetFileName(c.OldPath)} → {Path.GetFileName(c.NewPath)}（{c.Note}）", LogLevel.Warn);
            foreach (var u in unresolved)
                Log($"无法识别，跳过：{Path.GetFileName(u.OldPath)}（{u.Note}）", LogLevel.Warn);

            if (toRename.Count == 0)
            {
                Log($"没有需要重命名的文件（已正确命名 {already}，冲突 {conflicts.Count}，无法识别 {unresolved.Count}）。", LogLevel.Warn);
                return;
            }

            // 预览确认弹窗。
            if (!ConfirmRename(toRename, already, conflicts.Count, unresolved.Count))
            {
                Log("已取消重命名。", LogLevel.Warn);
                return;
            }

            SetUILocked(true);
            RaiseExecutionState(true);
            btnExport.Enabled = false;
            ProgressBarHelper.SetProgressBegin(pbExport);
            Log($"开始重命名 {toRename.Count} 个文件...", LogLevel.Section);

            try
            {
                int done = 0;
                var renamed = await Task.Run(() =>
                    ExcelRenamer.Execute(plan, (msg, level) =>
                    {
                        LogLibrary.Write(txtLog, msg, level);
                        if (level == LogLevel.Ok)
                        {
                            done++;
                            ProgressBarHelper.SetProgress(pbExport, ScaledProgress(done, toRename.Count));
                        }
                    }));

                // 回写关联路径（UI 线程）：列表配置、单独模式目标，凡指向旧路径者改为新路径。
                ApplyRenamePathSync(renamed);

                // 磁盘与配置均已变化，立即落盘。
                SettingsManager.Save(Settings);
                LocalStateManager.Save(LocalState);

                ProgressBarHelper.SetProgress(pbExport, 100);
                Log($"重命名完成。成功 {renamed.Count} 个，跳过 {toRename.Count - renamed.Count} 个。", LogLevel.Ok);
            }
            catch (Exception ex)
            {
                Log($"未预期的错误：{LogLibrary.FormatException(ex, includeStackTrace: true)}", LogLevel.Error);
            }
            finally
            {
                SetUILocked(false);
                RaiseExecutionState(false);
                btnExport.Enabled = true;
            }
        }

        /// <summary>
        /// 按当前模式构造重命名候选对（类名, 旧路径）。后台线程调用：仅访问数据对象与
        /// 线程安全的 <see cref="TabBase.Log"/>，不触碰 UI 控件（控件文本已由调用方传入）。
        /// </summary>
        private List<(string ClassName, string OldPath)> BuildRenameCandidates(
            int mode, string batchFolder)
        {
            var result = new List<(string, string)>();

            if (mode == 0)
            {
                // 列表模式：启用且已设路径、文件存在的项；类名已知，无需读 Excel。
                foreach (var cfg in Settings.ExcelExportClassConfigs)
                {
                    if (!cfg.Enabled || string.IsNullOrEmpty(cfg.TargetExcelPath)) continue;
                    if (!File.Exists(cfg.TargetExcelPath))
                    {
                        Log($"跳过 [{cfg.ClassName}]：目标 Excel 路径不存在 —— {cfg.TargetExcelPath}", LogLevel.Warn);
                        continue;
                    }
                    result.Add((cfg.ClassName, cfg.TargetExcelPath));
                }
            }
            else
            {
                // 批量模式：枚举目标文件夹下 *.xlsx，先读内嵌类名，读不到则按列表中匹配路径的配置取类名。
                if (string.IsNullOrEmpty(batchFolder) || !Directory.Exists(batchFolder))
                {
                    Log("请设置有效的导出文件夹。", LogLevel.Warn);
                    return result;
                }

                foreach (var file in Directory.EnumerateFiles(batchFolder, "*.xlsx"))
                {
                    if (Path.GetFileName(file).StartsWith("~$")) continue; // 跳过 Excel 临时锁文件

                    string className = ExcelRenamer.TryReadEmbeddedClassName(file) ?? string.Empty;
                    if (string.IsNullOrEmpty(className))
                    {
                        var cfg = Settings.ExcelExportClassConfigs.FirstOrDefault(c =>
                            !string.IsNullOrEmpty(c.TargetExcelPath) && PathEquals(c.TargetExcelPath, file));
                        if (cfg != null) className = cfg.ClassName;
                    }
                    result.Add((className, file)); // 类名仍为空时由 BuildPlan 标记为 Unresolved
                }
            }

            return result;
        }

        /// <summary>弹窗预览将要发生的重命名清单，返回用户是否确认执行。</summary>
        private bool ConfirmRename(
            List<RenamePlanItem> toRename, int already, int conflictCount, int unresolvedCount)
        {
            const int maxLines = 30;
            var lines = toRename
                .Take(maxLines)
                .Select(p => $"{Path.GetFileName(p.OldPath)}  →  {Path.GetFileName(p.NewPath)}");

            string list = string.Join(Environment.NewLine, lines);
            if (toRename.Count > maxLines)
                list += $"{Environment.NewLine}… 其余 {toRename.Count - maxLines} 项";

            string summary =
                $"将重命名 {toRename.Count} 个文件" +
                $"（已正确命名 {already}，冲突跳过 {conflictCount}，无法识别 {unresolvedCount}）。" +
                Environment.NewLine + Environment.NewLine + list +
                Environment.NewLine + Environment.NewLine + "确认执行？";

            return MessageBox.Show(summary, "重命名 Excel 预览",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>把成功重命名的（旧→新）路径回写到列表配置，刷新列表显示。</summary>
        private void ApplyRenamePathSync(List<(string OldPath, string NewPath)> renamed)
        {
            foreach (var (oldPath, newPath) in renamed)
            {
                foreach (var cfg in Settings.ExcelExportClassConfigs)
                    if (!string.IsNullOrEmpty(cfg.TargetExcelPath) && PathEquals(cfg.TargetExcelPath, oldPath))
                        cfg.TargetExcelPath = newPath;
            }

            foreach (DataGridViewRow row in dgvClasses.Rows)
            {
                if (row.Tag is not ExcelExportClassConfig cfg) continue;
                row.Cells[colTargetPath.Index].Value = cfg.TargetExcelPath;
                RefreshTargetPathWarning(row);
            }
        }

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
            int    convention = LocalState.ExcelExportNameConvention;

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

                        string fileName = FunctionLibrary.BuildExcelFileName(cfg.ClassName, convention, prefix, suffix);
                        targetPath = Path.Combine(commonFolder, fileName);
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
                //  - 取选中类（LocalState.ExcelExportSingleClassName）
                //  - 目标形式（ExcelExportSingleTargetMode）：
                //      0 = 文件夹 → 目标文件夹 + 按命名规则生成的文件名
                //      1 = 文件   → 直接使用指定的 .xlsx 路径（不套用命名规则）
                //  - 目标存在 → 更新，不存在 → 新建（ExportAll/CreateExcel 自动处理）
                string className = LocalState.ExcelExportSingleClassName?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(className))
                {
                    Log("请在列表中选择一个数据类。", LogLevel.Warn);
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

                string targetPath;
                if (LocalState.ExcelExportSingleTargetMode == 1)
                {
                    string targetFile = txtSingleTargetPath.Text.Trim();
                    if (string.IsNullOrEmpty(targetFile))
                    {
                        Log("请设置导出 Excel 目标文件。", LogLevel.Warn);
                        return tasks;
                    }
                    targetPath = targetFile;
                }
                else
                {
                    string targetFolder = txtSingleTargetPath.Text.Trim();
                    if (string.IsNullOrEmpty(targetFolder))
                    {
                        Log("请设置导出 Excel 目标文件夹。", LogLevel.Warn);
                        return tasks;
                    }
                    string fileName = FunctionLibrary.BuildExcelFileName(className, convention, prefix, suffix);
                    targetPath = Path.Combine(targetFolder, fileName);
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
                    string fileName = FunctionLibrary.BuildExcelFileName(bean.Name, convention, prefix, suffix);
                    string path     = Path.Combine(targetFolder, fileName);
                    var    allFields = BeanParser.GetAllFields(bean, beanMap);
                    tasks.Add(new ExcelExportTask(bean, allFields, path));
                }
            }

            return tasks;
        }

    }
}
