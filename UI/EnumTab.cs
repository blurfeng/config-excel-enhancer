using System.ComponentModel;
using System.Diagnostics;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    public partial class EnumTab : TabBase
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        protected override RichTextBox? LogBox => txtLog;

        public EnumTab()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Log("Hello! Config Excel Enhancer 已就绪。", LogLevel.Info);
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

        private void txtXmlDir_TextChanged(object sender, EventArgs e)
            => Settings.XmlDirectory = txtXmlDir.Text;

        private void excelPicker_ValueChanged(object? sender, EventArgs e)
        {
            Settings.EnumExcelMode = excelPicker.Mode;
            Settings.ExcelDirectory = excelPicker.ExcelDirectory;
            Settings.EnumExcelFiles = excelPicker.Files;
        }

        private void chkHideEnumDataSheet_CheckedChanged(object sender, EventArgs e)
            => Settings.HideEnumDataSheet = chkHideEnumDataSheet.Checked;

        private void chkForceRewrite_CheckedChanged(object sender, EventArgs e)
            => Settings.EnumForceRewrite = chkForceRewrite.Checked;

        private void chkBoolValidation_CheckedChanged(object sender, EventArgs e)
            => Settings.BoolValidation = chkBoolValidation.Checked;

        public void LoadFromSettings()
        {
            txtXmlDir.Text = Settings.XmlDirectory;
            excelPicker.Mode = Settings.EnumExcelMode;
            excelPicker.ExcelDirectory = Settings.ExcelDirectory;
            excelPicker.Files = Settings.EnumExcelFiles;
            chkHideEnumDataSheet.Checked = Settings.HideEnumDataSheet;
            chkForceRewrite.Checked = Settings.EnumForceRewrite;
            chkBoolValidation.Checked = Settings.BoolValidation;
        }

        // ── 核心流程 ──────────────────────────────────────────

        private void SetUILocked(bool locked)
        {
            txtXmlDir.Enabled = !locked;
            btnBrowseXml.Enabled = !locked;
            excelPicker.Enabled = !locked;
            chkHideEnumDataSheet.Enabled = !locked;
            chkForceRewrite.Enabled = !locked;
            chkBoolValidation.Enabled = !locked;
        }

        /// <summary>
        /// 公开的异步执行入口，供 HomeTab 等外部调用。
        /// 返回 Task&lt;bool&gt;：true 表示成功，false 表示失败或取消。
        /// </summary>
        public async Task<bool> RunAsync()
        {
            var xmlDir = txtXmlDir.Text.Trim();

            if (!Directory.Exists(xmlDir))
            {
                Log("数据定义 XML 目录不存在。", LogLevel.Error);
                return false;
            }

            // 验证 Excel 来源
            bool useListMode = excelPicker.Mode == 1;
            string excelDir = excelPicker.ExcelDirectory.Trim();
            List<string>? listModeFiles = null;

            if (useListMode)
            {
                listModeFiles = excelPicker.BuildFileList();
                if (listModeFiles.Count == 0)
                {
                    Log("列表中没有有效的 Excel 文件。", LogLevel.Error);
                    return false;
                }
            }
            else
            {
                if (!Directory.Exists(excelDir))
                {
                    Log("配置 Excel 目录不存在。", LogLevel.Error);
                    return false;
                }
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SetUILocked(true);
            RaiseExecutionState(true);
            btnUpdate.Enabled = false;
            btnCancel.Enabled = true;
            ProgressBarHelper.SetProgressBegin(pbUpdate);
            LogDivider();

            bool success = false;

            try
            {
                // ── 步骤1：扫描数据定义 XML ──────────────────────────
                Log("扫描数据定义 XML Enum 定义...", LogLevel.Info);
                var sw = Stopwatch.StartNew();

                List<EnumInfo> enums = await Task.Run(() => EnumScanner.ScanDirectory(xmlDir), token);
                sw.Stop();

                token.ThrowIfCancellationRequested();

                if (enums.Count == 0 && !Settings.BoolValidation)
                {
                    Log("未找到任何 Enum 定义，请检查数据定义 XML 目录。", LogLevel.Warn);
                    ProgressBarHelper.SetProgress(pbUpdate, 100);
                    return false;
                }
                else if (enums.Count == 0)
                {
                    Log("未找到任何 Enum 定义，将仅执行布尔值验证。", LogLevel.Info);
                }

                // 枚举列表（超过 8 个时截断显示）
                var enumNameSet = enums.Select(e => e.Name).ToHashSet(StringComparer.Ordinal);
                if (enums.Count > 0)
                {
                    const int maxShow = 8;
                    var enumNames = enumNameSet.ToList();
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
                }

                // 布尔值验证：注入合成 bool 枚举（Name="bool"，选项 FALSE/TRUE）
                var enumsForValidation = enums.ToList();
                if (Settings.BoolValidation)
                {
                    enumNameSet.Add("bool");
                    enumsForValidation.Add(new EnumInfo
                    {
                        Name = "bool",
                        Options =
                        [
                            new EnumOption { Name = "FALSE", Value = "0" },
                            new EnumOption { Name = "TRUE",  Value = "1" }
                        ]
                    });
                    Log("布尔值数据验证已启用（FALSE/TRUE）。", LogLevel.Info);
                }

                // 扫描 bean 字段枚举映射
                var beanFieldEnumMap = await Task.Run(
                    () => EnumScanner.ScanBeanEnumFields(xmlDir, enumNameSet), token);
                if (beanFieldEnumMap.Count > 0)
                    Log($"找到 {beanFieldEnumMap.Count} 个含枚举/布尔字段的 Bean。", LogLevel.Info);

                ProgressBarHelper.SetProgress(pbUpdate, 20);

                // ── 步骤2：修改 Excel ────────────────────────
                Log("开始修改 Excel...", LogLevel.Info);
                sw.Restart();

                int totalFiles;
                int processed = 0;
                List<UpdateResult> results;

                if (useListMode)
                {
                    totalFiles = listModeFiles!.Count;
                    results = await Task.Run(() =>
                        ValidationUpdater.UpdateFiles(
                            listModeFiles,
                            enumsForValidation,
                            chkHideEnumDataSheet.Checked,
                            result =>
                            {
                                processed++;
                                int v = totalFiles > 0 ? 20 + (int)(processed * 50.0 / totalFiles) : 70;
                                BeginInvoke(() =>
                                {
                                    LogFileResult(result);
                                    ProgressBarHelper.SetProgress(pbUpdate, Math.Min(v, 70));
                                });
                            },
                            forceRewrite: chkForceRewrite.Checked,
                            beanFieldEnumMap: beanFieldEnumMap), token);
                }
                else
                {
                    totalFiles = Directory.EnumerateFiles(excelDir, "*.xlsx", SearchOption.AllDirectories)
                        .Count(f => !Path.GetFileName(f).StartsWith("~$"));
                    results = await Task.Run(() =>
                        ValidationUpdater.UpdateDirectory(
                            excelDir,
                            enumsForValidation,
                            chkHideEnumDataSheet.Checked,
                            result =>
                            {
                                processed++;
                                int v = totalFiles > 0 ? 20 + (int)(processed * 50.0 / totalFiles) : 70;
                                BeginInvoke(() =>
                                {
                                    LogFileResult(result);
                                    ProgressBarHelper.SetProgress(pbUpdate, Math.Min(v, 70));
                                });
                            },
                            forceRewrite: chkForceRewrite.Checked,
                            beanFieldEnumMap: beanFieldEnumMap), token);
                }

                sw.Stop();

                token.ThrowIfCancellationRequested();

                ProgressBarHelper.SetProgress(pbUpdate, 75);

                // ── 步骤3：汇总
                PrintSummary(enums.Count, results, sw.Elapsed);

                // ── 步骤4：Excel COM 刷新公式缓存值 ─────────
                var savedFiles = results.Where(r => r.WasSaved).Select(r => r.FilePath).ToList();
                if (savedFiles.Count > 0)
                {
                    ProgressBarHelper.SetProgress(pbUpdate, 85);
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
                    ProgressBarHelper.SetProgress(pbUpdate, 85);
                }

                ProgressBarHelper.SetProgress(pbUpdate, 100);
                success = true;
            }
            catch (OperationCanceledException)
            {
                Log("操作已停止。", LogLevel.Warn);
                ProgressBarHelper.SetProgress(pbUpdate, 100);
            }
            catch (Exception ex)
            {
                Log($"未预期的错误：{ex.Message}", LogLevel.Error);
            }
            finally
            {
                SetUILocked(false);
                RaiseExecutionState(false);
                btnUpdate.Enabled = true;
                btnCancel.Enabled = false;
                _cts?.Dispose();
                _cts = null;
                Log("─ 结束 ─", LogLevel.Info);
                LogDivider();
            }

            return success;
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            await RunAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnCancel.Enabled = false;
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
                if (r.WasSaved)
                    Log($"{r.FileName}  —  已写盘但未发现枚举列", LogLevel.Warn);
                return;
            }

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
            int total = results.Count;
            int withEnum = results.Count(r => r.WasProcessed);
            int withSchema = results.Count(r => r.WasProcessed && r.HasSchemaChange);
            int withData = results.Count(r => r.WasProcessed && r.HasDataChange);
            int withVisibility = results.Count(r => r.HasVisibilityChange);
            int skipped = results.Count(r => r.WasSkipped);
            int errors = results.Count(r => r.HasError);
            bool anyUpdate = withSchema > 0 || withData > 0 || withVisibility > 0;

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
            if (errors > 0) statParts.Add($"错误：{errors} 个");
            Log(string.Join("  |  ", statParts), LogLevel.Info);
            Log($"耗时 {elapsed.TotalSeconds:F1}s", LogLevel.Skip);

            if (errors > 0 || skipped > 0)
                Log($"完成（有 {errors + skipped} 个文件处理异常，请检查上方日志）",
                    errors > 0 ? LogLevel.Error : LogLevel.Warn);
            else if (anyUpdate)
            {
                var updateDesc = new List<string>();
                if (withSchema > 0) updateDesc.Add($"Enum 定义变更 {withSchema} 个文件");
                if (withData > 0) updateDesc.Add($"数据变更 {withData} 个文件");
                if (withVisibility > 0) updateDesc.Add($"可见性修正 {withVisibility} 个文件");
                Log($"完成 | 有更新内容（{string.Join("，", updateDesc)}）", LogLevel.Ok);
            }
            else
                Log("完成 | 无更新内容", LogLevel.Skip);
        }
    }
}
