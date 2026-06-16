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
    public partial class ExcelExportTab : UserControl
    {
        private CancellationTokenSource? _cts;

        public event EventHandler<bool>? ExecutionStateChanged;

        public ExcelExportTab()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        public void CancelRunningTask() => _cts?.Cancel();

        // ── 设置加载 ──────────────────────────────────────────────────────

        public void LoadFromSettings()
        {
            txtXmlFolder.Text          = Settings.ExcelExportXmlFolder;
            txtDesignTemplate.Text     = Settings.ExcelExportDesignTemplate;
            txtListTargetFolder.Text   = Settings.ExcelExportListTargetFolder;
            txtTargetFolder.Text       = Settings.ExcelExportTargetFolder;
            txtPrefix.Text             = Settings.ExcelExportNamePrefix;
            txtSuffix.Text             = Settings.ExcelExportNameSuffix;

            tabMode.SelectedIndex = Settings.ExcelExportMode == 1 ? 1 : 0;

            rdoNameAsIs.Checked  = Settings.ExcelExportNameConvention == 0;
            rdoNameCamel.Checked = Settings.ExcelExportNameConvention == 1;
            rdoNameSnake.Checked = Settings.ExcelExportNameConvention == 2;

            if (!string.IsNullOrEmpty(Settings.ExcelExportXmlFolder))
                RefreshClassList(rescanXml: false);
        }

        // ── 设置同步 ──────────────────────────────────────────────────────

        private void txtXmlFolder_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportXmlFolder = txtXmlFolder.Text;

        private void txtDesignTemplate_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportDesignTemplate = txtDesignTemplate.Text;

        private void txtListTargetFolder_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportListTargetFolder = txtListTargetFolder.Text;

        private void txtTargetFolder_TextChanged(object sender, EventArgs e)
            => Settings.ExcelExportTargetFolder = txtTargetFolder.Text;

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

        // ── 浏览按钮 ──────────────────────────────────────────────────────

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
            }

            Log($"找到 {Settings.ExcelExportClassConfigs.Count} 个可导出数据类。", LogLevel.Info);
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
                cfg.TargetExcelPath = (string)(row.Cells[colTargetPath.Index].Value ?? string.Empty);
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
                }
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
            ExecutionStateChanged?.Invoke(this, true);
            btnExport.Enabled = false;
            btnStop.Enabled   = true;
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
                                ProgressBarHelper.SetProgress(pbExport,
                                    10 + (int)(done * 80.0 / tasks.Count));
                            }
                        },
                        token);
                }, token);

                ProgressBarHelper.SetProgress(pbExport, 100);
                Log($"导出完成。共处理 {tasks.Count} 个数据类。", LogLevel.Ok);
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
                ExecutionStateChanged?.Invoke(this, false);
                btnExport.Enabled = true;
                btnStop.Enabled   = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private void btnStop_Click(object sender, EventArgs e) => _cts?.Cancel();

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

            if (tabMode.SelectedIndex == 0)
            {
                // 列表模式：优先使用各自配置的路径，未配置时回退到通用目标文件夹
                string commonFolder = txtListTargetFolder.Text.Trim();

                foreach (var cfg in Settings.ExcelExportClassConfigs)
                {
                    if (!cfg.Enabled) continue;

                    string targetPath = cfg.TargetExcelPath;
                    if (string.IsNullOrEmpty(targetPath))
                    {
                        if (string.IsNullOrEmpty(commonFolder))
                            continue;

                        string baseName = FunctionLibrary.ApplyNameConvention(cfg.ClassName, convention);
                        targetPath = Path.Combine(commonFolder, $"{prefix}{baseName}{suffix}.xlsx");
                    }

                    var bean = leafBeans.FirstOrDefault(b => b.Name == cfg.ClassName);
                    if (bean == null) continue;

                    var allFields = BeanParser.GetAllFields(bean, beanMap);
                    tasks.Add(new ExcelExportTask(bean, allFields, targetPath));
                }
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

        // ── 日志辅助 ──────────────────────────────────────────────────────

        private void Log(string msg, LogLevel level = LogLevel.Info)
            => LogLibrary.Write(txtLog, msg, level);

        private void LogDivider()
            => LogLibrary.Divider(txtLog);
    }
}
