using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 导出模板类选项卡。
    /// 读取 Luban 生成的 JSON 配置，结合 Tables.cs 自动推断表访问器，
    /// 生成/更新 C# 模板类骨架和 Ids.Generated.cs。
    /// </summary>
    public partial class TemplateTab : UserControl
    {
        private CancellationTokenSource? _cts;
        private bool _loadingJob;         // 正在从 job 加载 UI，抑制回写
        private TemplateExportJob? _currentJob;

        public event EventHandler<bool>? ExecutionStateChanged;

        public TemplateTab()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        public void CancelRunningTask() => _cts?.Cancel();

        // ── 设置同步 ──────────────────────────────────────────────────────

        public void LoadFromSettings()
        {
            txtTablesClassPath.Text = Settings.TablesClassPath;
            RefreshJobList();
        }

        private void txtTablesClassPath_TextChanged(object sender, EventArgs e)
            => Settings.TablesClassPath = txtTablesClassPath.Text.Trim();

        // ── 任务列表管理 ──────────────────────────────────────────────────

        private void RefreshJobList()
        {
            lstJobs.Items.Clear();
            foreach (var job in Settings.TemplateExportJobs)
                lstJobs.Items.Add(job.DisplayName.Length > 0 ? job.DisplayName : "(未命名)");
        }

        private void lstJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lstJobs.SelectedIndex;
            if (idx < 0 || idx >= Settings.TemplateExportJobs.Count)
            {
                _currentJob = null;
                SetDetailPanelEnabled(false);
                return;
            }
            _currentJob = Settings.TemplateExportJobs[idx];
            LoadJobToUI(_currentJob);
            SetDetailPanelEnabled(true);
        }

        private void btnAddJob_Click(object sender, EventArgs e)
        {
            var job = new TemplateExportJob { DisplayName = $"任务 {Settings.TemplateExportJobs.Count + 1}" };
            Settings.TemplateExportJobs.Add(job);
            RefreshJobList();
            lstJobs.SelectedIndex = lstJobs.Items.Count - 1;
        }

        private void btnRemoveJob_Click(object sender, EventArgs e)
        {
            int idx = lstJobs.SelectedIndex;
            if (idx < 0) return;
            Settings.TemplateExportJobs.RemoveAt(idx);
            _currentJob = null;
            RefreshJobList();
            if (lstJobs.Items.Count > 0)
                lstJobs.SelectedIndex = Math.Min(idx, lstJobs.Items.Count - 1);
            else
                SetDetailPanelEnabled(false);
        }

        // ── 任务详情：加载到 UI ───────────────────────────────────────────

        private void LoadJobToUI(TemplateExportJob job)
        {
            _loadingJob = true;
            try
            {
                txtDisplayName.Text = job.DisplayName;
                txtJsonFile.Text = job.JsonFilePath;
                txtOutputDir.Text = job.OutputDirectory;
                txtNamespace.Text = job.Namespace;
                chkPartialClass.Checked = job.UsePartialClass;

                chkGenerateIds.Checked = job.GenerateIds;
                txtIdsOutputPath.Text = job.IdsOutputPath;
                txtIdsNamespace.Text = job.IdsNamespace;
                txtIdsClassName.Text = job.IdsClassName;

                LoadBindingsToGrid(job);
                UpdateIdsGroupEnabled(job.GenerateIds);
            }
            finally { _loadingJob = false; }
        }

        private void LoadBindingsToGrid(TemplateExportJob job)
        {
            dgvBindings.Rows.Clear();
            foreach (var kv in job.TypeTemplates)
                dgvBindings.Rows.Add(kv.Key, kv.Value);
        }

        // ── 任务详情：UI → job 回写 ──────────────────────────────────────

        private void txtDisplayName_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.DisplayName = txtDisplayName.Text;
            // 同步刷新列表项
            int idx = lstJobs.SelectedIndex;
            if (idx >= 0)
                lstJobs.Items[idx] = txtDisplayName.Text.Length > 0 ? txtDisplayName.Text : "(未命名)";
        }

        private void txtJsonFile_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.JsonFilePath = txtJsonFile.Text.Trim();
        }

        private void txtOutputDir_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.OutputDirectory = txtOutputDir.Text.Trim();
        }

        private void txtNamespace_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.Namespace = txtNamespace.Text.Trim();
        }

        private void chkPartialClass_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.UsePartialClass = chkPartialClass.Checked;
        }

        private void chkGenerateIds_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.GenerateIds = chkGenerateIds.Checked;
            UpdateIdsGroupEnabled(chkGenerateIds.Checked);
        }

        private void UpdateIdsGroupEnabled(bool enabled)
        {
            txtIdsOutputPath.Enabled = enabled;
            btnBrowseIdsOutput.Enabled = enabled;
            txtIdsNamespace.Enabled = enabled;
            txtIdsClassName.Enabled = enabled;
        }

        private void txtIdsOutputPath_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.IdsOutputPath = txtIdsOutputPath.Text.Trim();
        }

        private void txtIdsNamespace_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.IdsNamespace = txtIdsNamespace.Text.Trim();
        }

        private void txtIdsClassName_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.IdsClassName = txtIdsClassName.Text.Trim();
        }

        // ── 绑定表格管理 ──────────────────────────────────────────────────

        private void dgvBindings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loadingJob || _currentJob is null || e.RowIndex < 0) return;
            SyncBindingsFromGrid();
        }

        private void dgvBindings_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            SyncBindingsFromGrid();
        }

        private void SyncBindingsFromGrid()
        {
            if (_currentJob is null) return;
            _currentJob.TypeTemplates.Clear();
            foreach (DataGridViewRow row in dgvBindings.Rows)
            {
                if (row.IsNewRow) continue;
                string type = (row.Cells[0].Value as string ?? "").Trim();
                string path = (row.Cells[1].Value as string ?? "").Trim();
                if (!string.IsNullOrEmpty(type))
                    _currentJob.TypeTemplates[type] = path;
            }
        }

        private void btnAddBinding_Click(object sender, EventArgs e)
        {
            dgvBindings.Rows.Add("", "");
            if (dgvBindings.Rows.Count > 1)
                dgvBindings.CurrentCell = dgvBindings.Rows[dgvBindings.Rows.Count - 2].Cells[0];
        }

        private void btnRemoveBinding_Click(object sender, EventArgs e)
        {
            if (dgvBindings.SelectedRows.Count == 0) return;
            foreach (DataGridViewRow row in dgvBindings.SelectedRows)
                if (!row.IsNewRow) dgvBindings.Rows.Remove(row);
            SyncBindingsFromGrid();
        }

        private void btnBrowseBinding_Click(object sender, EventArgs e)
        {
            if (dgvBindings.CurrentRow is null || dgvBindings.CurrentRow.IsNewRow) return;
            string current = (dgvBindings.CurrentRow.Cells[1].Value as string) ?? "";
            var files = DialogHelper.BrowseFiles("选择模板文件", "模板文件 (*.tmpl;*.cs)|*.tmpl;*.cs|所有文件 (*.*)|*.*", current);
            if (files.Length > 0)
            {
                dgvBindings.CurrentRow.Cells[1].Value = files[0];
                SyncBindingsFromGrid();
            }
        }

        // ── 浏览按钮 ─────────────────────────────────────────────────────

        private void btnBrowseTablesClass_Click(object sender, EventArgs e)
        {
            var files = DialogHelper.BrowseFiles("选择 Tables.cs", "C# 文件 (*.cs)|*.cs", Settings.TablesClassPath);
            if (files.Length > 0) txtTablesClassPath.Text = files[0];
        }

        private void btnBrowseJsonFile_Click(object sender, EventArgs e)
        {
            if (_currentJob is null) return;
            var files = DialogHelper.BrowseFiles("选择 JSON 配置文件", "JSON 文件 (*.json)|*.json", _currentJob.JsonFilePath);
            if (files.Length > 0) txtJsonFile.Text = files[0];
        }

        private void btnBrowseOutputDir_Click(object sender, EventArgs e)
        {
            if (_currentJob is null) return;
            string? dir = DialogHelper.BrowseFolder("选择模板类输出目录", _currentJob.OutputDirectory);
            if (!string.IsNullOrEmpty(dir)) txtOutputDir.Text = dir;
        }

        private void btnBrowseIdsOutput_Click(object sender, EventArgs e)
        {
            if (_currentJob is null) return;
            var files = DialogHelper.BrowseSaveFile("Ids Generated 输出路径", "C# 文件 (*.cs)|*.cs",
                _currentJob.IdsOutputPath, "UnitIds.Generated.cs");
            if (!string.IsNullOrEmpty(files)) txtIdsOutputPath.Text = files;
        }

        // ── 执行 ─────────────────────────────────────────────────────────

        private void SetUILocked(bool locked)
        {
            lstJobs.Enabled = !locked;
            btnAddJob.Enabled = !locked;
            btnRemoveJob.Enabled = !locked;
            pnlDetail.Enabled = !locked;
            txtTablesClassPath.Enabled = !locked;
            btnBrowseTablesClass.Enabled = !locked;
        }

        private async void btnRunAll_Click(object sender, EventArgs e)
        {
            await RunJobs(Settings.TemplateExportJobs);
        }

        private async void btnRunSelected_Click(object sender, EventArgs e)
        {
            if (_currentJob is null)
            {
                Log("请先在左侧选择一个任务。", LogLevel.Warn);
                return;
            }
            await RunJobs(new List<TemplateExportJob> { _currentJob });
        }

        private async Task RunJobs(IReadOnlyList<TemplateExportJob> jobs)
        {
            if (jobs.Count == 0)
            {
                Log("没有可运行的任务。", LogLevel.Warn);
                return;
            }

            if (string.IsNullOrWhiteSpace(Settings.TablesClassPath))
            {
                Log("请先配置 Tables.cs 路径（全局设置），工具需要它来推断表访问器。", LogLevel.Error);
                return;
            }

            if (!File.Exists(Settings.TablesClassPath))
            {
                Log($"Tables.cs 文件不存在：{Settings.TablesClassPath}", LogLevel.Error);
                return;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SetUILocked(true);
            ExecutionStateChanged?.Invoke(this, true);
            btnRunAll.Enabled = false;
            btnRunSelected.Enabled = false;
            btnStop.Enabled = true;
            pbRun.Visible = true;
            pbRun.Maximum = jobs.Count * 100;
            pbRun.Value = 0;
            LogDivider();

            // 解析 Tables.cs（一次）
            Dictionary<string, TableMapping> tableMappings;
            try
            {
                tableMappings = TablesClassParser.Parse(Settings.TablesClassPath);
                Log($"Tables.cs 解析完成，找到 {tableMappings.Count} 个表映射", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Log($"Tables.cs 解析失败：{ex.Message}", LogLevel.Error);
                goto Cleanup;
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                if (string.IsNullOrWhiteSpace(job.JsonFilePath))
                {
                    Log($"任务「{job.DisplayName}」未配置 JSON 文件，跳过", LogLevel.Warn);
                    continue;
                }

                Log($"── 任务：{job.DisplayName} ──", LogLevel.Section);
                int jobBase = i * 100;
                int processed = 0;
                var progress = new Progress<string>(name =>
                {
                    processed++;
                    pbRun.Value = Math.Min(jobBase + processed, pbRun.Maximum);
                });

                try
                {
                    var options = new TemplateExportOptions(job, tableMappings);
                    await TemplateExporter.ExportAsync(options, progress,
                        (msg, lvl) => LogLibrary.Write(txtLog, msg, lvl), token);
                }
                catch (OperationCanceledException) { Log("操作已停止。", LogLevel.Warn); goto Cleanup; }
                catch (Exception ex) { Log($"未预期的错误：{ex.Message}", LogLevel.Error); }
            }

            pbRun.Value = pbRun.Maximum;
            Log("全部任务完成。", LogLevel.Ok);

            Cleanup:
            SetUILocked(false);
            ExecutionStateChanged?.Invoke(this, false);
            btnRunAll.Enabled = true;
            btnRunSelected.Enabled = true;
            btnStop.Enabled = false;
            pbRun.Visible = false;
            _cts?.Dispose();
            _cts = null;
            LogDivider();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnStop.Enabled = false;
        }

        // ── 日志工具 ──────────────────────────────────────────────────────

        private void Log(string msg, LogLevel level = LogLevel.Ok)
            => LogLibrary.Write(txtLog, msg, level);

        private void LogDivider()
            => LogLibrary.Divider(txtLog);

        private void btnClearLog_Click(object? sender, EventArgs e) => txtLog.Clear();

        private void btnCopyLog_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

        // ── 辅助 ─────────────────────────────────────────────────────────

        private void SetDetailPanelEnabled(bool enabled)
        {
            pnlDetail.Enabled = enabled;
        }
    }
}
