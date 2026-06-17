using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    /// <summary>
    /// 表格设计选项卡。从模板 Excel 将样式（表头行、列宽、单元格合并等）应用到目标 Excel 文件，
    /// 并可选地在应用后强制更新枚举验证规则和刷新公式缓存。
    /// </summary>
    public partial class TableDesignTab : TabBase
    {
        public TableDesignTab()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        protected override RichTextBox? LogBox => txtLog;

        /// <summary>将 AppSettings 的值同步到界面控件。</summary>
        public void LoadFromSettings()
        {
            txtSourceExcel.Text = Settings.TableDesignSourceExcel;

            excelPicker.Mode = Settings.TableDesignTargetMode;
            excelPicker.ExcelDirectory = Settings.TableDesignTargetDirectory;
            excelPicker.Files = Settings.TableDesignTargetFiles;

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
        }

        // ── 设置同步 ──────────────────────────────────────────────────────

        private void txtSourceExcel_TextChanged(object sender, EventArgs e)
            => Settings.TableDesignSourceExcel = txtSourceExcel.Text;

        private void excelPicker_ValueChanged(object? sender, EventArgs e)
        {
            Settings.TableDesignTargetMode = excelPicker.Mode;
            Settings.TableDesignTargetDirectory = excelPicker.ExcelDirectory;
            Settings.TableDesignTargetFiles = excelPicker.Files;
        }

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

        private void chkIgnoreUnderscoreFiles_CheckedChanged(object sender, EventArgs e)
            => Settings.TableDesignIgnoreUnderscoreFiles = chkIgnoreUnderscoreFiles.Checked;

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

        // ── 主流程 ────────────────────────────────────────────────────────

        private void SetUILocked(bool locked)
        {
            txtSourceExcel.Enabled = !locked;
            btnBrowseSource.Enabled = !locked;
            excelPicker.Enabled = !locked;
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

            var targetFiles = excelPicker.BuildFileList(sourcePath);
            if (targetFiles.Count == 0)
            {
                Log("未找到任何目标文件。", LogLevel.Warn);
                return;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SetUILocked(true);
            RaiseExecutionState(true);
            btnApply.Enabled = false;
            btnCancel.Enabled = true;
            ProgressBarHelper.SetProgressBegin(pbApply);
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
                ProgressBarHelper.SetProgress(pbApply, ScaledProgress(processed, total));
            });

            try
            {
                await Task.Run(() => TableDesignApplier.Apply(options, progress,
                    (msg, level) => LogLibrary.Write(txtLog, msg, level),
                    token), token);

                ProgressBarHelper.SetProgress(pbApply, 100);
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
                RaiseExecutionState(false);
                btnApply.Enabled = true;
                btnCancel.Enabled = false;
                _cts?.Dispose();
                _cts = null;
                Log("─ 结束 ─", LogLevel.Info);
                LogDivider();
            }
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
    }
}
