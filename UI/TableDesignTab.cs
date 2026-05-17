using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.UI
{
    public partial class TableDesignTab : UserControl
    {
        private static readonly Color ClrNormal  = Color.LightGreen;
        private static readonly Color ClrGray    = Color.FromArgb(150, 150, 150);
        private static readonly Color ClrWarn    = Color.FromArgb(255, 200, 60);
        private static readonly Color ClrError   = Color.OrangeRed;
        private static readonly Color ClrInfo    = Color.FromArgb(100, 200, 255);
        private static readonly Color ClrSection = Color.FromArgb(80, 160, 80);

        private CancellationTokenSource? _cts;

        public TableDesignTab()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        public void CancelRunningTask() => _cts?.Cancel();

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

        private void UpdateTargetModeVisibility()
        {
            pnlDirMode.Visible = rdoDirectory.Checked;
            pnlListMode.Visible = rdoList.Checked;
        }

        // ── Settings sync ─────────────────────────────────────────────────

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

        // ── Browse handlers ───────────────────────────────────────────────

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "选择来源 Excel",
                Filter = "Excel 文件 (*.xlsx)|*.xlsx"
            };
            if (!string.IsNullOrEmpty(Settings.TableDesignSourceExcel))
                dlg.InitialDirectory = Path.GetDirectoryName(Settings.TableDesignSourceExcel);
            if (dlg.ShowDialog() == DialogResult.OK)
                txtSourceExcel.Text = dlg.FileName;
        }

        private void btnBrowseTargetDir_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog { Description = "选择目标 Excel 目录" };
            if (!string.IsNullOrEmpty(Settings.TableDesignTargetDirectory))
                dlg.SelectedPath = Settings.TableDesignTargetDirectory;
            if (dlg.ShowDialog() == DialogResult.OK)
                txtTargetDir.Text = dlg.SelectedPath;
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "选择目标 Excel 文件",
                Filter = "Excel 文件 (*.xlsx)|*.xlsx",
                Multiselect = true
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (var f in dlg.FileNames)
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

        private void SyncTargetFilesToSettings()
            => Settings.TableDesignTargetFiles = lstTargetFiles.Items.Cast<string>().ToList();

        // ── Main action ───────────────────────────────────────────────────

        private async void btnApply_Click(object sender, EventArgs e)
        {
            string sourcePath = txtSourceExcel.Text.Trim();
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                Log("请选择有效的来源 Excel 文件。", ClrError);
                return;
            }

            var targetFiles = BuildTargetFileList();
            if (targetFiles.Count == 0)
            {
                Log("未找到任何目标文件。", ClrWarn);
                return;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            btnApply.Enabled = false;
            btnStop.Enabled = true;
            progressBar.Value = 0;
            progressBar.Visible = true;
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
                MergeHeaderKeywords: txtMergeKeywords.Text.Trim()
            );

            var progress = new Progress<(int current, int total, string fileName)>(p =>
            {
                progressBar.Value = p.total > 0 ? (int)(p.current * 100.0 / p.total) : 0;
            });

            try
            {
                await Task.Run(() => TableDesignApplier.Apply(options, progress,
                    (msg, sev) => BeginInvoke(() => Log(msg, SeverityToColor(sev))),
                    token), token);

                Log("完成。", ClrNormal);
            }
            catch (OperationCanceledException)
            {
                Log("操作已停止。", ClrWarn);
            }
            catch (Exception ex)
            {
                Log($"未预期的错误：{ex.Message}", ClrError);
            }
            finally
            {
                btnApply.Enabled = true;
                btnStop.Enabled = false;
                progressBar.Value = 0;
                progressBar.Visible = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnStop.Enabled = false;
        }

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

        // ── Log helpers ───────────────────────────────────────────────────

        private void Log(string message, Color? color = null)
        {
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = color ?? ClrNormal;
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }

        private void LogDivider()
        {
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = ClrSection;
            txtLog.AppendText($"{"─".PadRight(60, '─')}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }

        private static Color SeverityToColor(LogSeverity s) => s switch
        {
            LogSeverity.Warning => ClrWarn,
            LogSeverity.Error   => ClrError,
            LogSeverity.Info    => ClrInfo,
            LogSeverity.Section => ClrSection,
            LogSeverity.Gray    => ClrGray,
            _                   => ClrNormal
        };

        private void btnClearLog_Click(object? sender, EventArgs e) => txtLog.Clear();

        private void btnCopyLog_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }
    }
}
