using System.ComponentModel;
using System.Diagnostics;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.UI
{
    public partial class EnumTab : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        public event EventHandler<bool>? ExecutionStateChanged;

        private CancellationTokenSource? _cts;

        public EnumTab()
        {
            InitializeComponent();
        }

        /// <summary>取消当前正在进行的任务（供窗口关闭时调用）。</summary>
        public void CancelRunningTask()
        {
            _cts?.Cancel();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Log("Hello! ConfigStudio 已就绪。", LogLevel.Info);
        }

        // ── 目录选择 ──────────────────────────────────────────

        private void btnBrowseXml_Click(object sender, EventArgs e)
        {
            if (DialogHelper.BrowseFolder("选择 XML Schema 目录", Settings.XmlDirectory) is { } path)
            {
                txtXmlDir.Text = path;
                Settings.XmlDirectory = path;
            }
        }

        private void btnBrowseExcel_Click(object sender, EventArgs e)
        {
            if (DialogHelper.BrowseFolder("选择 Excel 配置目录", Settings.ExcelDirectory) is { } path)
            {
                txtExcelDir.Text = path;
                Settings.ExcelDirectory = path;
            }
        }

        private void txtXmlDir_TextChanged(object sender, EventArgs e)
            => Settings.XmlDirectory = txtXmlDir.Text;

        private void txtExcelDir_TextChanged(object sender, EventArgs e)
            => Settings.ExcelDirectory = txtExcelDir.Text;

        private void chkHideEnumDataSheet_CheckedChanged(object sender, EventArgs e)
            => Settings.HideEnumDataSheet = chkHideEnumDataSheet.Checked;

        public void LoadFromSettings()
        {
            txtXmlDir.Text = Settings.XmlDirectory;
            txtExcelDir.Text = Settings.ExcelDirectory;
            chkHideEnumDataSheet.Checked = Settings.HideEnumDataSheet;
        }

        // ── 核心流程 ──────────────────────────────────────────

        private void SetUILocked(bool locked)
        {
            txtXmlDir.Enabled = !locked;
            btnBrowseXml.Enabled = !locked;
            txtExcelDir.Enabled = !locked;
            btnBrowseExcel.Enabled = !locked;
            chkHideEnumDataSheet.Enabled = !locked;
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            var xmlDir   = txtXmlDir.Text.Trim();
            var excelDir = txtExcelDir.Text.Trim();

            if (!Directory.Exists(xmlDir))
            {
                Log("XML 目录不存在。", LogLevel.Error);
                return;
            }
            if (!Directory.Exists(excelDir))
            {
                Log("Excel 目录不存在。", LogLevel.Error);
                return;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SetUILocked(true);
            ExecutionStateChanged?.Invoke(this, true);
            btnUpdate.Enabled = false;
            btnStop.Enabled = true;
            pbUpdate.Maximum = 1000;
            pbUpdate.Value = 100;
            pbUpdate.Visible = true;
            LogDivider();

            try
            {
                // ── 步骤1：扫描 XML ──────────────────────────
                Log("扫描 XML Enum 定义...", LogLevel.Info);
                var sw = Stopwatch.StartNew();

                List<EnumInfo> enums = await Task.Run(() => EnumScanner.ScanDirectory(xmlDir), token);
                sw.Stop();

                token.ThrowIfCancellationRequested();

                if (enums.Count == 0)
                {
                    Log("未找到任何 Enum 定义，请检查 XML 目录。", LogLevel.Warn);
                    return;
                }

                // 枚举列表（超过 8 个时截断显示）
                const int maxShow = 8;
                var enumNames = enums.Select(e => e.Name).ToList();
                var namesDisplay = enumNames.Count <= maxShow
                    ? string.Join("、", enumNames)
                    : string.Join("、", enumNames.Take(maxShow)) + $" (+{enumNames.Count - maxShow})";
                Log($"找到 {enums.Count} 个 Enum：{namesDisplay}", LogLevel.Ok);

                // 警告没有 value=0 的枚举（默认值将使用第一项）
                foreach (var ei in enums)
                {
                    if (!ei.Options.Any(o => o.Value == "0"))
                        Log($"  ⚠ {ei.Name} 没有 value=0 的选项，默认将使用第一项：{ei.Options.FirstOrDefault()?.Name}", LogLevel.Warn);
                }

                pbUpdate.Value = 200;

                // ── 步骤2：修改 Excel ────────────────────────
                Log("开始修改 Excel...", LogLevel.Info);
                sw.Restart();

                int totalFiles = Directory.EnumerateFiles(excelDir, "*.xlsx", SearchOption.AllDirectories)
                    .Count(f => !Path.GetFileName(f).StartsWith("~$"));
                int processed = 0;

                var results = await Task.Run(() =>
                    ValidationUpdater.UpdateDirectory(
                        excelDir,
                        enums,
                        chkHideEnumDataSheet.Checked,
                        result =>
                        {
                            processed++;
                            int v = totalFiles > 0 ? 200 + (int)(processed * 600.0 / totalFiles) : 800;
                            BeginInvoke(() =>
                            {
                                LogFileResult(result);
                                pbUpdate.Value = Math.Min(v, 800);
                            });
                        }), token);

                sw.Stop();

                token.ThrowIfCancellationRequested();

                pbUpdate.Value = 850;

                // ── 步骤3：汇总
                PrintSummary(enums.Count, results, sw.Elapsed);

                // ── 步骤4：Excel COM 刷新公式缓存值 ─────────
                var savedFiles = results.Where(r => r.WasSaved).Select(r => r.FilePath).ToList();
                if (savedFiles.Count > 0)
                {
                    pbUpdate.Value = 900;
                    Log($"正在通过 Excel 刷新 {savedFiles.Count} 个文件的公式缓存值...", LogLevel.Info);
                    bool excelAvailable = await Task.Run(
                        () => FunctionLibrary.RefreshFormulasViaSTA(savedFiles), token);
                    if (excelAvailable)
                        Log("公式缓存值刷新完成。", LogLevel.Ok);
                    else
                        Log("本机未安装 Excel，已跳过公式缓存刷新。如需刷新，请安装 Microsoft Excel 后重试。", LogLevel.Warn);
                }
                else
                {
                    pbUpdate.Value = 900;
                }

                pbUpdate.Value = 1000;
            }
            catch (OperationCanceledException)
            {
                Log("操作已停止。", LogLevel.Warn);
                pbUpdate.Visible = false;
            }
            catch (Exception ex)
            {
                Log($"未预期的错误：{ex.Message}", LogLevel.Error);
            }
            finally
            {
                SetUILocked(false);
                ExecutionStateChanged?.Invoke(this, false);
                btnUpdate.Enabled = true;
                btnStop.Enabled = false;
                pbUpdate.Visible = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnStop.Enabled = false;
        }

        private void btnClearLog_Click(object? sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void btnCopyLog_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

        // ── 结果打印 ──────────────────────────────────────────

        private void LogFileResult(UpdateResult r)
        {
            if (r.WasSkipped)
            {
                Log($"{r.FileName}  —  跳过（文件被占用）", LogLevel.Warn);
                return;
            }
            if (r.HasError)
            {
                Log($"{r.FileName}  —  错误：{r.ErrorMessage}", LogLevel.Error);
                return;
            }
            if (!r.WasProcessed)
            {
                // WasSaved=true 但无枚举列：理论上修复后不再出现，保留为兜底警告
                if (r.WasSaved)
                    Log($"{r.FileName}  —  已写盘但未发现枚举列", LogLevel.Warn);
                return;
            }

            // 构建变更描述（三种变更独立显示）
            var parts = new List<string>();
            if (r.HasSchemaChange)
                parts.Add("Enum 定义已变更");
            if (r.HasDataChange)
                parts.Add($"数据已更新（{r.DefaultsFilled} 个默认值填入）");
            if (r.HasVisibilityChange)
                parts.Add("__enum_data 表可见性已修正");

            if (parts.Count > 0)
                Log($"{r.FileName}  —  {string.Join(" | ", parts)}  [{r.EnumColumnsFound} 列]", LogLevel.Ok);
            else
                Log($"{r.FileName}  —  已同步，无变化  [{r.EnumColumnsFound} 列]", LogLevel.Skip);
        }

        private void PrintSummary(int enumCount, List<UpdateResult> results, TimeSpan elapsed)
        {
            int total          = results.Count;
            int withEnum       = results.Count(r => r.WasProcessed);
            int withSchema     = results.Count(r => r.WasProcessed && r.HasSchemaChange);
            int withData       = results.Count(r => r.WasProcessed && r.HasDataChange);
            int withVisibility = results.Count(r => r.HasVisibilityChange);
            int skipped        = results.Count(r => r.WasSkipped);
            int errors         = results.Count(r => r.HasError);
            bool anyUpdate     = withSchema > 0 || withData > 0 || withVisibility > 0;

            LogDivider();
            Log($"枚举定义：{enumCount} 个", LogLevel.Info);

            var statParts = new List<string>
            {
                $"Excel 扫描：{total} 个",
                $"含枚举列：{withEnum} 个",
                $"Enum 定义变更：{withSchema} 个",
                $"数据变更：{withData} 个"
            };
            if (withVisibility > 0) statParts.Add($"可见性修正：{withVisibility} 个");
            if (skipped > 0) statParts.Add($"跳过：{skipped} 个");
            if (errors  > 0) statParts.Add($"错误：{errors} 个");
            Log(string.Join("  |  ", statParts), LogLevel.Info);
            Log($"耗时 {elapsed.TotalSeconds:F1}s", LogLevel.Skip);

            if (errors > 0 || skipped > 0)
                Log($"完成（有 {errors + skipped} 个文件处理异常，请检查上方日志）",
                    errors > 0 ? LogLevel.Error : LogLevel.Warn);
            else if (anyUpdate)
            {
                var updateDesc = new List<string>();
                if (withSchema     > 0) updateDesc.Add($"Enum 定义变更 {withSchema} 个文件");
                if (withData       > 0) updateDesc.Add($"数据变更 {withData} 个文件");
                if (withVisibility > 0) updateDesc.Add($"可见性修正 {withVisibility} 个文件");
                Log($"完成 | 有更新内容（{string.Join("，", updateDesc)}）", LogLevel.Ok);
            }
            else
                Log("完成 | 无更新内容", LogLevel.Skip);
        }

        // ── 日志工具 ──────────────────────────────────────────

        private void Log(string message, LogLevel level = LogLevel.Ok)
            => LogLibrary.Write(txtLog, message, level);

        private void LogDivider()
            => LogLibrary.Divider(txtLog);
    }
}
