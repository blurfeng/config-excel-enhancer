using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 表格设计选项卡。从模板 Excel 将样式（表头行、列宽、单元格合并等）应用到目标 Excel 文件，
    /// 并可选地在应用后强制更新枚举验证规则和刷新公式缓存。
    /// </summary>
    public partial class TableDesignTab : UserControl
    {
        // 当前运行任务的取消令牌源；未运行时为 null
        private CancellationTokenSource? _cts;

        /// <summary>任务执行状态变化时触发（true = 开始执行，false = 执行结束）。</summary>
        public event EventHandler<bool>? ExecutionStateChanged;

        public TableDesignTab()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        /// <summary>取消当前正在进行的任务（供窗口关闭时调用）。</summary>
        public void CancelRunningTask() => _cts?.Cancel();

        /// <summary>将 AppSettings 的值同步到界面控件。</summary>
        public void LoadFromSettings()
        {
            txtSourceExcel.Text = Settings.TableDesignSourceExcel;

            if (Settings.TableDesignTargetMode == 1)
                rdoList.Checked = true;
            else
                rdoDirectory.Checked = true;

            txtTargetDir.Text = Settings.TableDesignTargetDirectory;

            lstTargetFiles.Items.Clear();
            foreach (var f in Settings.TableDesignTargetFiles)
                lstTargetFiles.Items.Add(f);

            chkIgnoreUnderscoreFiles.Checked = Settings.TableDesignIgnoreUnderscoreFiles;

            if (Settings.TableDesignSheetScope == 1)
                rdoScopeFirst.Checked = true;
            else
                rdoScopeAll.Checked = true;

            chkIgnoreUnderscoreSheets.Checked = Settings.TableDesignIgnoreUnderscoreSheets;
            txtHeaderSymbol.Text = string.IsNullOrEmpty(Settings.TableDesignHeaderSymbol)
                ? "##" : Settings.TableDesignHeaderSymbol;

            chkAutoColumnWidth.Checked = Settings.TableDesignAutoColumnWidth;
            chkMergeHeaderCells.Checked = Settings.TableDesignMergeHeaderCells;
            txtMergeKeywords.Text = string.IsNullOrEmpty(Settings.TableDesignMergeHeaderKeywords)
                ? "##type" : Settings.TableDesignMergeHeaderKeywords;

            UpdateTargetModeVisibility();
        }

        // 根据当前目标模式单选钮切换显示目录面板或列表面板
        private void UpdateTargetModeVisibility()
        {
            pnlDirMode.Visible = rdoDirectory.Checked;
            pnlListMode.Visible = rdoList.Checked;
        }

        // ── 设置同步 ──────────────────────────────────────────────────────

        private void txtSourceExcel_TextChanged(object sender, EventArgs e)
            => Settings.TableDesignSourceExcel = txtSourceExcel.Text;

        private void rdoDirectory_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDirectory.Checked)
            {
                Settings.TableDesignTargetMode = 0;
                UpdateTargetModeVisibility();
            }
        }

        private void rdoList_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoList.Checked)
            {
                Settings.TableDesignTargetMode = 1;
                UpdateTargetModeVisibility();
            }
        }

        private void txtTargetDir_TextChanged(object sender, EventArgs e)
            => Settings.TableDesignTargetDirectory = txtTargetDir.Text;

        private void chkIgnoreUnderscoreFiles_CheckedChanged(object sender, EventArgs e)
            => Settings.TableDesignIgnoreUnderscoreFiles = chkIgnoreUnderscoreFiles.Checked;

        private void rdoScopeAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoScopeAll.Checked)
                Settings.TableDesignSheetScope = 0;
        }

        private void rdoScopeFirst_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoScopeFirst.Checked)
                Settings.TableDesignSheetScope = 1;
        }

        private void chkIgnoreUnderscoreSheets_CheckedChanged(object sender, EventArgs e)
            => Settings.TableDesignIgnoreUnderscoreSheets = chkIgnoreUnderscoreSheets.Checked;

        private void txtHeaderSymbol_TextChanged(object sender, EventArgs e)
            => Settings.TableDesignHeaderSymbol = txtHeaderSymbol.Text;

        private void chkAutoColumnWidth_CheckedChanged(object sender, EventArgs e)
            => Settings.TableDesignAutoColumnWidth = chkAutoColumnWidth.Checked;

        private void chkMergeHeaderCells_CheckedChanged(object sender, EventArgs e)
            => Settings.TableDesignMergeHeaderCells = chkMergeHeaderCells.Checked;

        private void txtMergeKeywords_TextChanged(object sender, EventArgs e)
            => Settings.TableDesignMergeHeaderKeywords = txtMergeKeywords.Text;

        // ── 文件 / 目录浏览 ───────────────────────────────────────────────

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            var files = DialogHelper.BrowseFiles(
                "选择模板 Excel",
                "Excel 文件 (*.xlsx)|*.xlsx",
                Settings.TableDesignSourceExcel);
            if (files.Length > 0)
                txtSourceExcel.Text = files[0];
        }

        private void btnBrowseTargetDir_Click(object sender, EventArgs e)
        {
            if (DialogHelper.BrowseFolder("选择目标配置 Excel 目录", Settings.TableDesignTargetDirectory) is { } path)
                txtTargetDir.Text = path;
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            var files = DialogHelper.BrowseFiles(
                "选择目标 Excel 文件",
                "Excel 文件 (*.xlsx)|*.xlsx",
                multiselect: true);
            if (files.Length > 0)
            {
                foreach (var f in files)
                    if (!lstTargetFiles.Items.Contains(f))
                        lstTargetFiles.Items.Add(f);
                SyncTargetFilesToSettings();
            }
        }

        private void btnRemoveFiles_Click(object sender, EventArgs e)
        {
            foreach (var item in lstTargetFiles.SelectedItems.Cast<string>().ToList())
                lstTargetFiles.Items.Remove(item);
            SyncTargetFilesToSettings();
        }

        private void btnClearFiles_Click(object sender, EventArgs e)
        {
            lstTargetFiles.Items.Clear();
            SyncTargetFilesToSettings();
        }

        // 将列表框当前内容同步回 Settings，确保持久化时包含最新文件列表
        private void SyncTargetFilesToSettings()
            => Settings.TableDesignTargetFiles = lstTargetFiles.Items.Cast<string>().ToList();

        // ── 主流程 ────────────────────────────────────────────────────────

        // 执行期间锁定所有输入控件，防止用户在任务运行时修改参数
        private void SetUILocked(bool locked)
        {
            txtSourceExcel.Enabled = !locked;
            btnBrowseSource.Enabled = !locked;
            pnlModeGroup.Enabled = !locked;
            pnlDirMode.Enabled = !locked;
            pnlListMode.Enabled = !locked;
            chkIgnoreUnderscoreFiles.Enabled = !locked;
            pnlScopeGroup.Enabled = !locked;
            chkIgnoreUnderscoreSheets.Enabled = !locked;
            txtHeaderSymbol.Enabled = !locked;
            chkAutoColumnWidth.Enabled = !locked;
            chkMergeHeaderCells.Enabled = !locked;
            txtMergeKeywords.Enabled = !locked;
        }

        private async void btnApply_Click(object sender, EventArgs e)
        {
            string sourcePath = txtSourceExcel.Text.Trim();
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                Log("请选择有效的模板 Excel 文件。", LogLevel.Error);
                return;
            }

            var targetFiles = BuildTargetFileList();
            if (targetFiles.Count == 0)
            {
                Log("未找到任何目标文件。", LogLevel.Warn);
                return;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SetUILocked(true);
            ExecutionStateChanged?.Invoke(this, true);
            btnApply.Enabled = false;
            btnStop.Enabled = true;
            pbApply.Maximum = 1000;
            pbApply.Value = 100;
            pbApply.Visible = true;
            LogDivider();

            var options = new TableDesignOptions(
                SourceExcelPath: sourcePath,
                TargetFiles: targetFiles,
                IgnoreUnderscoreFiles: chkIgnoreUnderscoreFiles.Checked,
                SheetScope: rdoScopeAll.Checked ? 0 : 1,
                IgnoreUnderscoreSheets: chkIgnoreUnderscoreSheets.Checked,
                HeaderSymbol: string.IsNullOrWhiteSpace(txtHeaderSymbol.Text) ? "##" : txtHeaderSymbol.Text.Trim(),
                AutoColumnWidth: chkAutoColumnWidth.Checked,
                MergeHeaderCells: chkMergeHeaderCells.Checked,
                MergeHeaderKeywords: txtMergeKeywords.Text.Trim(),
                XmlDirectory: Settings.XmlDirectory,
                HideEnumDataSheet: Settings.HideEnumDataSheet
            );

            int processed = 0;
            int total = targetFiles.Count;
            var progress = new Progress<string>(_ =>
            {
                processed++;
                int v = total > 0 ? 100 + (int)(processed * 800.0 / total) : 900;
                pbApply.Value = Math.Min(v, 900);
            });

            try
            {
                await Task.Run(() => TableDesignApplier.Apply(options, progress,
                    (msg, level) => LogLibrary.Write(txtLog, msg, level),
                    token), token);

                pbApply.Value = 1000;
                Log("完成。", LogLevel.Ok);
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
                btnApply.Enabled = true;
                btnStop.Enabled = false;
                pbApply.Visible = false;
                _cts?.Dispose();
                _cts = null;
                Log("─ 结束 ─", LogLevel.Info);
                LogDivider();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnStop.Enabled = false;
        }

        /// <summary>
        /// 根据当前目标模式构建目标文件列表。
        /// 目录模式：枚举目录顶层的 xlsx（排除临时文件和模板文件本身）；
        /// 列表模式：过滤掉已被删除的文件后返回。
        /// </summary>
        private List<string> BuildTargetFileList()
        {
            if (rdoDirectory.Checked)
            {
                string dir = txtTargetDir.Text.Trim();
                if (!Directory.Exists(dir))
                    return new List<string>();

                string sourceFull = Path.GetFullPath(txtSourceExcel.Text.Trim());
                return Directory.EnumerateFiles(dir, "*.xlsx", SearchOption.TopDirectoryOnly)
                    .Where(f => !Path.GetFileName(f).StartsWith("~$"))
                    .Where(f => !string.Equals(Path.GetFullPath(f), sourceFull,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                return lstTargetFiles.Items.Cast<string>()
                    .Where(File.Exists)
                    .ToList();
            }
        }

        // ── 日志工具 ──────────────────────────────────────────────────────

        private void Log(string message, LogLevel level = LogLevel.Ok)
            => LogLibrary.Write(txtLog, message, level);

        private void LogDivider()
            => LogLibrary.Divider(txtLog);

        private void btnClearLog_Click(object? sender, EventArgs e) => txtLog.Clear();

        private void btnCopyLog_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }
    }
}
