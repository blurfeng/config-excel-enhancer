using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 导出模板类选项卡。
    /// 读取 Luban 生成的 JSON 配置，结合 Tables.cs 自动推断表访问器，
    /// 生成/更新 C# 模板类和 Ids 文件。
    /// </summary>
    public partial class TemplateTab : TabBase
    {
        private bool _loadingJob;         // 正在从 job 加载 UI，抑制回写
        private TemplateExportJob? _currentJob;

        public TemplateTab()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        protected override RichTextBox? LogBox => txtLog;

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
                chkUseGeneratedSuffix.Checked = job.UseGeneratedSuffix;
                txtNameField.Text = job.NameField;

                chkGenerateIds.Checked = job.GenerateIds;
                txtIdsOutputDir.Text = job.IdsOutputDirectory;
                txtIdsNamespace.Text = job.IdsNamespace;
                txtIdsClassName.Text = job.IdsClassName;
                chkIdsUsePartialClass.Checked = job.IdsUsePartialClass;
                chkIdsUseGeneratedSuffix.Checked = job.IdsUseGeneratedSuffix;

                LoadBindingsToGrid(job);
                UpdateIdsGroupEnabled(job.GenerateIds);
                RefreshTemplatePreview();
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

        private void CommitDisplayName()
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.DisplayName = txtDisplayName.Text;
            int idx = lstJobs.SelectedIndex;
            if (idx >= 0)
                lstJobs.Items[idx] = txtDisplayName.Text.Length > 0 ? txtDisplayName.Text : "(未命名)";
        }

        private void txtDisplayName_Leave(object sender, EventArgs e)
            => CommitDisplayName();

        private void txtDisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitDisplayName();
                e.SuppressKeyPress = true;
            }
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

        private void chkUseGeneratedSuffix_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.UseGeneratedSuffix = chkUseGeneratedSuffix.Checked;
        }

        private void txtNameField_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.NameField = string.IsNullOrWhiteSpace(txtNameField.Text)
                ? "name"
                : txtNameField.Text.Trim();
        }

        private void chkGenerateIds_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.GenerateIds = chkGenerateIds.Checked;
            UpdateIdsGroupEnabled(chkGenerateIds.Checked);
        }

        private void UpdateIdsGroupEnabled(bool enabled)
        {
            txtIdsOutputDir.Enabled = enabled;
            btnBrowseIdsOutputDir.Enabled = enabled;
            txtIdsNamespace.Enabled = enabled;
            txtIdsClassName.Enabled = enabled;
            chkIdsUsePartialClass.Enabled = enabled;
            chkIdsUseGeneratedSuffix.Enabled = enabled;
        }

        private void txtIdsOutputDir_TextChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.IdsOutputDirectory = txtIdsOutputDir.Text.Trim();
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

        private void chkIdsUsePartialClass_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.IdsUsePartialClass = chkIdsUsePartialClass.Checked;
        }

        private void chkIdsUseGeneratedSuffix_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingJob || _currentJob is null) return;
            _currentJob.IdsUseGeneratedSuffix = chkIdsUseGeneratedSuffix.Checked;
        }

        // ── 绑定表格管理 ──────────────────────────────────────────────────

        private void dgvBindings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loadingJob || _currentJob is null || e.RowIndex < 0) return;
            SyncBindingsFromGrid();
            RefreshTemplatePreview();
        }

        private void dgvBindings_SelectionChanged(object sender, EventArgs e)
        {
            RefreshTemplatePreview();
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
            dgvBindings.CurrentCell = dgvBindings.Rows[dgvBindings.Rows.Count - 1].Cells[0];
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
                RefreshTemplatePreview();
            }
        }

        // ── 模板预览 ──────────────────────────────────────────────────────

        private static readonly System.Text.RegularExpressions.Regex ClassDeclPattern =
            new(@"^\s*(public|internal|private|protected)(\s+(sealed|abstract|static|partial))*\s+class\s+\S.*",
                System.Text.RegularExpressions.RegexOptions.Compiled);

        private void RefreshTemplatePreview()
        {
            var row = dgvBindings.CurrentRow;
            if (row is null || row.IsNewRow)
            {
                lblTemplatePreview.Text = "未选中绑定行";
                return;
            }

            string tmplPath = (row.Cells[1].Value as string ?? "").Trim();
            if (string.IsNullOrEmpty(tmplPath) || !File.Exists(tmplPath))
            {
                lblTemplatePreview.Text = string.IsNullOrEmpty(tmplPath) ? "模板路径为空" : "模板文件不存在";
                return;
            }

            try
            {
                string? decl = null;
                foreach (string line in File.ReadLines(tmplPath, System.Text.Encoding.UTF8))
                {
                    if (ClassDeclPattern.IsMatch(line))
                    {
                        decl = line.Trim();
                        break;
                    }
                }
                lblTemplatePreview.Text = decl is not null ? $"类声明：{decl}" : "未检测到类声明";
            }
            catch
            {
                lblTemplatePreview.Text = "读取模板文件失败";
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

        private void btnBrowseIdsOutputDir_Click(object sender, EventArgs e)
        {
            if (_currentJob is null) return;
            string? dir = DialogHelper.BrowseFolder("选择 Ids 文件输出目录", _currentJob.IdsOutputDirectory);
            if (!string.IsNullOrEmpty(dir)) txtIdsOutputDir.Text = dir;
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
            await RunAllAsync();
        }

        /// <summary>
        /// 公开的异步执行入口，供 HomeTab 等外部调用。
        /// 执行所有导出模板任务，返回 Task&lt;bool&gt;：true 表示至少有一个任务成功执行。
        /// </summary>
        public Task<bool> RunAllAsync()
        {
            return RunJobs(Settings.TemplateExportJobs);
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

        private async Task<bool> RunJobs(IReadOnlyList<TemplateExportJob> jobs)
        {
            if (jobs.Count == 0)
            {
                Log("没有可运行的任务，已跳过。", LogLevel.Info);
                return true;
            }

            if (string.IsNullOrWhiteSpace(Settings.TablesClassPath))
            {
                Log("请先配置 Tables.cs 路径（全局设置），工具需要它来推断表访问器。", LogLevel.Error);
                return false;
            }

            if (!File.Exists(Settings.TablesClassPath))
            {
                Log($"Tables.cs 文件不存在：{Settings.TablesClassPath}", LogLevel.Error);
                return false;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SetUILocked(true);
            RaiseExecutionState(true);
            btnRunAll.Enabled = false;
            btnRunSelected.Enabled = false;
            btnCancel.Enabled = true;
            ProgressBarHelper.SetProgressBegin(pbRun);
            LogDivider();

            bool anySuccess = false;

            try
            {

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
                return anySuccess;
            }

            // key = (IdsOutputDirectory, IdsClassName)
            var idsAccumulator = new Dictionary<(string Dir, string ClassName), IdsCollectionResult>(
                EqualityComparer<(string, string)>.Default);

            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                if (string.IsNullOrWhiteSpace(job.JsonFilePath))
                {
                    Log($"任务「{job.DisplayName}」未配置 JSON 文件，跳过", LogLevel.Warn);
                    continue;
                }

                // 检查 Ids 类名是否发生了重命名
                if (job.GenerateIds
                    && !string.IsNullOrWhiteSpace(job.LastExportedIdsClassName)
                    && job.LastExportedIdsClassName != job.IdsClassName)
                {
                    string suffix = job.IdsUseGeneratedSuffix ? ".Generated" : "";
                    string oldFile = Path.Combine(
                        job.LastExportedIdsOutputDirectory,
                        $"{job.LastExportedIdsClassName}{suffix}.cs");
                    Log($"任务「{job.DisplayName}」的 Ids 类名已从 \"{job.LastExportedIdsClassName}\" 改为 \"{job.IdsClassName}\"。" +
                        $"请先手动删除旧文件（{oldFile}），再执行导出，否则会导致数据残留。", LogLevel.Error);
                    continue;
                }

                Log($"── 任务：{job.DisplayName} ──", LogLevel.Section);
                int segStart = jobs.Count > 0 ? 10 + i * 80 / jobs.Count : 10;
                int segEnd   = jobs.Count > 0 ? 10 + (i + 1) * 80 / jobs.Count : 90;
                int processed = 0;
                var progress = new Progress<string>(_ =>
                {
                    processed++;
                    // 在本任务段内线性推进，最多到 segEnd-1（下一任务开始时才到 segEnd）
                    int v = segStart + (segEnd - segStart) * processed / Math.Max(1, processed + 1);
                    ProgressBarHelper.SetProgress(pbRun, Math.Min(v, segEnd - 1));
                });

                IdsCollectionResult? idsResult = null;
                try
                {
                    var options = new TemplateExportOptions(job, tableMappings);
                    idsResult = await TemplateExporter.ExportAsync(options, progress,
                        (msg, lvl) => LogLibrary.Write(txtLog, msg, lvl), token);
                    anySuccess = true;
                }
                catch (OperationCanceledException) { Log("操作已停止。", LogLevel.Warn); return anySuccess; }
                catch (Exception ex) { Log($"未预期的错误：{ex.Message}", LogLevel.Error); }

                // 累积 Ids 数据
                if (idsResult is not null)
                {
                    var key = (idsResult.IdsOutputDirectory, idsResult.IdsClassName);
                    if (idsAccumulator.TryGetValue(key, out var existing))
                    {
                        if (existing.IdsNamespace != idsResult.IdsNamespace)
                            Log($"警告：Ids 类 \"{idsResult.IdsClassName}\" 的命名空间存在冲突，将使用先出现的 \"{existing.IdsNamespace}\"", LogLevel.Warn);
                        idsAccumulator[key] = existing with
                        {
                            Entries = existing.Entries.Concat(idsResult.Entries).ToList(),
                            OwnedGroups = existing.OwnedGroups.Union(idsResult.OwnedGroups).ToHashSet(),
                            TemplateNamespace = existing.TemplateNamespace
                        };
                    }
                    else
                    {
                        idsAccumulator[key] = idsResult;
                    }
                }
            }

            // 统一写 Ids 文件
            if (idsAccumulator.Count > 0)
            {
                Log("── 生成 Ids 文件 ──", LogLevel.Section);
                foreach (var result in idsAccumulator.Values)
                {
                    try
                    {
                        var genOptions = new IdsGenerateOptions(
                            result.IdsOutputDirectory,
                            result.IdsNamespace,
                            result.IdsClassName,
                            result.UsePartialClass,
                            result.UseGeneratedSuffix,
                            result.Entries,
                            result.OwnedGroups,
                            result.TemplateNamespace);
                        TemplateExporter.GenerateIds(genOptions,
                            (msg, lvl) => LogLibrary.Write(txtLog, msg, lvl));

                        // 更新缓存：遍历所有任务，标记成功导出的 Ids 类名、目录和 owned groups
                        foreach (var j in jobs)
                        {
                            if (!j.GenerateIds) continue;
                            var k = (j.IdsOutputDirectory, j.IdsClassName);
                            if (idsAccumulator.ContainsKey(k))
                            {
                                j.LastExportedIdsClassName = j.IdsClassName;
                                j.LastExportedIdsOutputDirectory = j.IdsOutputDirectory;
                                j.LastExportedOwnedGroups = j.TypeTemplates.Count > 0
                                    ? j.TypeTemplates.Keys.ToList()
                                    : new List<string> { j.IdsClassName };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Ids 文件生成失败（{result.IdsClassName}）：{ex.Message}", LogLevel.Error);
                    }
                }

                // 持久化设置，保存缓存字段
                try
                {
                    SettingsManager.Save(Settings);
                }
                catch (Exception ex)
                {
                    Log($"保存设置失败：{ex.Message}", LogLevel.Warn);
                }
            }

            ProgressBarHelper.SetProgress(pbRun, 100);
            Log("全部任务完成。", LogLevel.Ok);

            } // try
            finally
            {
                SetUILocked(false);
                RaiseExecutionState(false);
                btnRunAll.Enabled = true;
                btnRunSelected.Enabled = true;
                btnCancel.Enabled = false;
                _cts?.Dispose();
                _cts = null;
                LogDivider();
            }
            return anySuccess;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnCancel.Enabled = false;
        }

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