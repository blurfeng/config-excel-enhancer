using System.ComponentModel;
using System.Diagnostics;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.UI
{
    public partial class EnumTab : UserControl
    {
        // ── 颜色定义 ──────────────────────────────────────────
        private static readonly Color ClrNormal  = Color.LightGreen;
        private static readonly Color ClrGray    = Color.FromArgb(150, 150, 150);
        private static readonly Color ClrWarn    = Color.FromArgb(255, 200, 60);
        private static readonly Color ClrError   = Color.OrangeRed;
        private static readonly Color ClrInfo    = Color.FromArgb(100, 200, 255);
        private static readonly Color ClrSection = Color.FromArgb(80, 160, 80);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        public EnumTab()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Log("Hello! ConfigStudio 已就绪。", ClrInfo);
        }

        // ── 目录选择 ──────────────────────────────────────────

        private void btnBrowseXml_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog { Description = "选择 XML Schema 目录" };
            if (!string.IsNullOrEmpty(Settings.XmlDirectory))
                dlg.SelectedPath = Settings.XmlDirectory;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtXmlDir.Text = dlg.SelectedPath;
                Settings.XmlDirectory = dlg.SelectedPath;
            }
        }

        private void btnBrowseExcel_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog { Description = "选择 Excel 配置目录" };
            if (!string.IsNullOrEmpty(Settings.ExcelDirectory))
                dlg.SelectedPath = Settings.ExcelDirectory;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtExcelDir.Text = dlg.SelectedPath;
                Settings.ExcelDirectory = dlg.SelectedPath;
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

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            var xmlDir   = txtXmlDir.Text.Trim();
            var excelDir = txtExcelDir.Text.Trim();

            if (!Directory.Exists(xmlDir))
            {
                Log("XML 目录不存在。", ClrError);
                return;
            }
            if (!Directory.Exists(excelDir))
            {
                Log("Excel 目录不存在。", ClrError);
                return;
            }

            btnUpdate.Enabled = false;
            LogDivider();

            try
            {
                // ── 步骤1：扫描 XML ──────────────────────────
                Log("扫描 XML Enum 定义...", ClrInfo);
                var sw = Stopwatch.StartNew();

                List<EnumInfo> enums = await Task.Run(() => EnumScanner.ScanDirectory(xmlDir));
                sw.Stop();

                if (enums.Count == 0)
                {
                    Log("未找到任何 Enum 定义，请检查 XML 目录。", ClrWarn);
                    return;
                }

                // 枚举列表（超过 8 个时截断显示）
                const int maxShow = 8;
                var enumNames = enums.Select(e => e.Name).ToList();
                var namesDisplay = enumNames.Count <= maxShow
                    ? string.Join("、", enumNames)
                    : string.Join("、", enumNames.Take(maxShow)) + $" (+{enumNames.Count - maxShow})";
                Log($"找到 {enums.Count} 个 Enum：{namesDisplay}", ClrNormal);

                // 警告没有 value=0 的枚举（默认值将使用第一项）
                foreach (var ei in enums)
                {
                    if (!ei.Options.Any(o => o.Value == "0"))
                        Log($"  ⚠ {ei.Name} 没有 value=0 的选项，默认将使用第一项：{ei.Options.FirstOrDefault()?.Name}", ClrWarn);
                }

                // ── 步骤2：修改 Excel ────────────────────────
                Log("开始修改 Excel...", ClrInfo);
                sw.Restart();

                var results = await Task.Run(() =>
                    ValidationUpdater.UpdateDirectory(
                        excelDir,
                        enums,
                        chkHideEnumDataSheet.Checked,
                        result => BeginInvoke(() => LogFileResult(result))));

                sw.Stop();

                // ── 步骤3：汇总 ─────────────────────────────
                PrintSummary(enums.Count, results, sw.Elapsed);

                // ── 步骤4：Excel COM 刷新公式缓存值 ─────────
                var savedFiles = results.Where(r => r.WasSaved).Select(r => r.FilePath).ToList();
                if (savedFiles.Count > 0)
                {
                    Log($"正在通过 Excel 刷新 {savedFiles.Count} 个文件的公式缓存值...", ClrInfo);
                    bool excelAvailable = false;
                    await Task.Run(() =>
                    {
                        // Excel COM 要求 STA 线程
                        bool result = false;
                        var sta = new Thread(() => result = ValidationUpdater.RefreshFormulasViaExcel(savedFiles));
                        sta.SetApartmentState(ApartmentState.STA);
                        sta.Start();
                        sta.Join();
                        excelAvailable = result;
                    });
                    if (excelAvailable)
                        Log("公式缓存值刷新完成。", ClrNormal);
                    else
                        Log("本机未安装 Excel，已跳过公式缓存刷新。如需刷新，请安装 Microsoft Excel 后重试。", ClrWarn);
                }
            }
            catch (Exception ex)
            {
                Log($"未预期的错误：{ex.Message}", ClrError);
            }
            finally
            {
                btnUpdate.Enabled = true;
            }
        }

        // ── 结果打印 ──────────────────────────────────────────

        private void LogFileResult(UpdateResult r)
        {
            if (r.WasSkipped)
            {
                Log($"⚠  {r.FileName}  —  跳过（文件被占用）", ClrWarn);
                return;
            }
            if (r.HasError)
            {
                Log($"✗  {r.FileName}  —  错误：{r.ErrorMessage}", ClrError);
                return;
            }
            if (!r.WasProcessed)
            {
                // WasSaved=true 但无枚举列：理论上修复后不再出现，保留为兜底警告
                if (r.WasSaved)
                    Log($"⚠  {r.FileName}  —  已写盘但未发现枚举列", ClrWarn);
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
                Log($"OK  {r.FileName}  —  {string.Join(" | ", parts)}  [{r.EnumColumnsFound} 列]", ClrNormal);
            else
                Log($"—  {r.FileName}  —  已同步，无变化  [{r.EnumColumnsFound} 列]", ClrGray);
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
            Log($"枚举定义：{enumCount} 个", ClrInfo);

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
            Log(string.Join("  |  ", statParts), ClrInfo);
            Log($"耗时 {elapsed.TotalSeconds:F1}s", ClrGray);

            if (errors > 0 || skipped > 0)
                Log($"完成（有 {errors + skipped} 个文件处理异常，请检查上方日志）",
                    errors > 0 ? ClrError : ClrWarn);
            else if (anyUpdate)
            {
                var updateDesc = new List<string>();
                if (withSchema     > 0) updateDesc.Add($"Enum 定义变更 {withSchema} 个文件");
                if (withData       > 0) updateDesc.Add($"数据变更 {withData} 个文件");
                if (withVisibility > 0) updateDesc.Add($"可见性修正 {withVisibility} 个文件");
                Log($"完成 | 有更新内容（{string.Join("，", updateDesc)}）", ClrNormal);
            }
            else
                Log("完成 | 无更新内容", ClrGray);
        }

        // ── 日志工具 ──────────────────────────────────────────

        private void Log(string message, Color? color = null)
        {
            txtLog.SelectionStart  = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor  = color ?? ClrNormal;
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }

        private void LogDivider()
        {
            txtLog.SelectionStart  = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor  = ClrSection;
            txtLog.AppendText($"{"─".PadRight(60, '─')}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }
    }
}
