using System.Text.Json;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 负责将 AppSettings 持久化到应用目录下的 settings.json 文件。
    /// 支持加载、保存和清除操作；JSON 格式化输出便于手动编辑。
    /// <para>
    /// 路径策略：保存时将路径转为相对于 exe 目录的相对路径（若可能），
    /// 加载时还原为绝对路径。这样仓库克隆到不同机器后路径仍然有效。
    /// </para>
    /// </summary>
    public static class SettingsManager
    {
        // settings.json 固定位于应用程序 exe 所在目录
        private static readonly string SettingsPath = Path.Combine(
            AppContext.BaseDirectory, "settings.json");

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// 从 settings.json 加载设置。若文件不存在则返回默认 AppSettings；
        /// 若文件存在但解析失败，弹出警告框并返回默认设置。
        /// 加载后所有路径字段均还原为绝对路径。
        /// </summary>
        public static AppSettings Load()
        {
            if (!File.Exists(SettingsPath))
                return new AppSettings();

            try
            {
                var json = File.ReadAllText(SettingsPath);
                // System.Text.Json 遇到缺失字段时保留属性类定义的默认值，
                // 因此 HideEnumDataSheet 等字段即使不在旧版 JSON 中也会正确取 true。
                var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                DenormalizePaths(settings);
                return settings;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"settings.json 加载失败，将使用默认设置。\n\n{ex.Message}",
                    "配置加载失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return new AppSettings();
            }
        }

        /// <summary>
        /// 将设置序列化为格式化 JSON 并写入 settings.json。
        /// 写入前将路径字段转为相对路径（相对 exe 目录），不修改传入对象。
        /// </summary>
        public static void Save(AppSettings settings)
        {
            var toWrite = ShallowClone(settings);
            NormalizePaths(toWrite);
            var json = JsonSerializer.Serialize(toWrite, JsonOptions);
            File.WriteAllText(SettingsPath, json);
        }

        /// <summary>
        /// 删除 settings.json，使下次启动时恢复默认设置。
        /// </summary>
        public static void Clear()
        {
            try
            {
                if (File.Exists(SettingsPath))
                    File.Delete(SettingsPath);
            }
            catch { }
        }

        // ── 路径规范化辅助 ─────────────────────────────────

        /// <summary>
        /// 将 settings 中所有路径字段转为相对路径（保存前调用）。
        /// </summary>
        private static void NormalizePaths(AppSettings s)
        {
            s.XmlDirectory               = ToRelativeIfPossible(s.XmlDirectory);
            s.ExcelDirectory             = ToRelativeIfPossible(s.ExcelDirectory);
            s.GenBatPath                 = ToRelativeIfPossible(s.GenBatPath);
            s.TableDesignSourceExcel     = ToRelativeIfPossible(s.TableDesignSourceExcel);
            s.TableDesignTargetDirectory = ToRelativeIfPossible(s.TableDesignTargetDirectory);
            s.TablesClassPath            = ToRelativeIfPossible(s.TablesClassPath);
            s.EnumExcelFiles             = s.EnumExcelFiles.Select(ToRelativeIfPossible).ToList();
            s.TableDesignTargetFiles     = s.TableDesignTargetFiles.Select(ToRelativeIfPossible).ToList();
        }

        /// <summary>
        /// 将 settings 中所有路径字段还原为绝对路径（加载后调用）。
        /// </summary>
        private static void DenormalizePaths(AppSettings s)
        {
            s.XmlDirectory               = ToAbsolute(s.XmlDirectory);
            s.ExcelDirectory             = ToAbsolute(s.ExcelDirectory);
            s.GenBatPath                 = ToAbsolute(s.GenBatPath);
            s.TableDesignSourceExcel     = ToAbsolute(s.TableDesignSourceExcel);
            s.TableDesignTargetDirectory = ToAbsolute(s.TableDesignTargetDirectory);
            s.TablesClassPath            = ToAbsolute(s.TablesClassPath);
            s.EnumExcelFiles             = s.EnumExcelFiles.Select(ToAbsolute).ToList();
            s.TableDesignTargetFiles     = s.TableDesignTargetFiles.Select(ToAbsolute).ToList();
        }

        /// <summary>
        /// 若 <paramref name="path"/> 在 BaseDirectory 下，则返回相对路径；否则原样返回。
        /// 空字符串直接返回。
        /// </summary>
        private static string ToRelativeIfPossible(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            try
            {
                // 统一用完整路径比较，避免大小写 / 斜杠差异
                var baseDir = Path.GetFullPath(AppContext.BaseDirectory)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;

                var fullPath = Path.GetFullPath(path);

                if (fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                {
                    // 转为以 "." 开头的相对路径，跨平台友好
                    var rel = Path.GetRelativePath(AppContext.BaseDirectory, fullPath);
                    return rel;
                }
            }
            catch
            {
                // 路径非法时原样保留
            }

            return path;
        }

        /// <summary>
        /// 若 <paramref name="path"/> 是相对路径，则以 BaseDirectory 为基准还原为绝对路径；
        /// 否则原样返回（向后兼容已保存绝对路径的旧 settings.json）。
        /// 空字符串直接返回。
        /// </summary>
        private static string ToAbsolute(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            try
            {
                if (!Path.IsPathRooted(path))
                    return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
            }
            catch
            {
                // 路径非法时原样保留
            }

            return path;
        }

        /// <summary>
        /// 浅克隆 AppSettings，避免 Save() 修改调用方持有的对象。
        /// </summary>
        private static AppSettings ShallowClone(AppSettings src) => new()
        {
            XmlDirectory                      = src.XmlDirectory,
            ExcelDirectory                    = src.ExcelDirectory,
            GenBatPath                        = src.GenBatPath,
            HideEnumDataSheet                 = src.HideEnumDataSheet,
            EnumForceRewrite                  = src.EnumForceRewrite,
            BoolValidation                    = src.BoolValidation,
            EnumExcelMode                     = src.EnumExcelMode,
            EnumExcelFiles                    = new List<string>(src.EnumExcelFiles),
            TableDesignSourceExcel            = src.TableDesignSourceExcel,
            TableDesignTargetMode             = src.TableDesignTargetMode,
            TableDesignTargetDirectory        = src.TableDesignTargetDirectory,
            TableDesignTargetFiles            = new List<string>(src.TableDesignTargetFiles),
            TableDesignIgnoreUnderscoreFiles  = src.TableDesignIgnoreUnderscoreFiles,
            TableDesignSheetScope             = src.TableDesignSheetScope,
            TableDesignIgnoreUnderscoreSheets = src.TableDesignIgnoreUnderscoreSheets,
            TableDesignHeaderSymbol           = src.TableDesignHeaderSymbol,
            TableDesignAutoColumnWidth        = src.TableDesignAutoColumnWidth,
            TableDesignMergeHeaderCells       = src.TableDesignMergeHeaderCells,
            TableDesignMergeHeaderKeywords    = src.TableDesignMergeHeaderKeywords,
            TablesClassPath                   = src.TablesClassPath,
            TemplateExportJobs                = new List<TemplateExportJob>(src.TemplateExportJobs),
            HomeIncludeEnum                   = src.HomeIncludeEnum,
        };
    }
}
