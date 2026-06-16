using System.Text.Json;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 负责将 AppSettings 持久化到应用目录下的 settings.json 文件。
    /// 支持加载、保存和清除操作；JSON 格式化输出便于手动编辑。
    /// <para>
    /// 路径策略：保存时将路径转为相对于项目根目录（<see cref="LocalState.ProjectRoot"/>）的相对路径，
    /// 加载时以同一根目录还原为绝对路径。项目根目录存储于 LocalApplicationData，不进入版本控制。
    /// 若本机未配置项目根目录，则回退到相对于 exe 目录的旧行为以保持向后兼容。
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
                DenormalizePaths(settings, GetProjectRoot());
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
        /// 写入前将路径字段转为相对于项目根目录的相对路径，不修改传入对象。
        /// </summary>
        public static void Save(AppSettings settings)
        {
            var toWrite = ShallowClone(settings);
            NormalizePaths(toWrite, GetProjectRoot());
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
        /// 获取当前有效的路径基准目录：优先使用本机配置的项目根目录，
        /// 未配置时回退到 exe 所在目录（旧行为，向后兼容）。
        /// </summary>
        private static string GetProjectRoot()
        {
            var root = LocalStateManager.Load().ProjectRoot;
            return !string.IsNullOrEmpty(root) && Directory.Exists(root)
                ? root
                : AppContext.BaseDirectory;
        }

        /// <summary>
        /// 将 settings 中所有路径字段转为相对于 <paramref name="baseDir"/> 的相对路径（保存前调用）。
        /// </summary>
        private static void NormalizePaths(AppSettings s, string baseDir)
        {
            s.XmlDirectory               = FunctionLibrary.ToProjectRelative(s.XmlDirectory, baseDir);
            s.ExcelDirectory             = FunctionLibrary.ToProjectRelative(s.ExcelDirectory, baseDir);
            s.GenBatPath                 = FunctionLibrary.ToProjectRelative(s.GenBatPath, baseDir);
            s.TableDesignSourceExcel     = FunctionLibrary.ToProjectRelative(s.TableDesignSourceExcel, baseDir);
            s.TableDesignTargetDirectory = FunctionLibrary.ToProjectRelative(s.TableDesignTargetDirectory, baseDir);
            s.TablesClassPath            = FunctionLibrary.ToProjectRelative(s.TablesClassPath, baseDir);
            s.EnumExcelFiles             = s.EnumExcelFiles.Select(p => FunctionLibrary.ToProjectRelative(p, baseDir)).ToList();
            s.TableDesignTargetFiles     = s.TableDesignTargetFiles.Select(p => FunctionLibrary.ToProjectRelative(p, baseDir)).ToList();
            s.ExcelExportXmlFolder       = FunctionLibrary.ToProjectRelative(s.ExcelExportXmlFolder, baseDir);
            s.ExcelExportDesignTemplate  = FunctionLibrary.ToProjectRelative(s.ExcelExportDesignTemplate, baseDir);
            s.ExcelExportTargetFolder    = FunctionLibrary.ToProjectRelative(s.ExcelExportTargetFolder, baseDir);
            foreach (var cfg in s.ExcelExportClassConfigs)
            {
                cfg.SourceXmlFile   = FunctionLibrary.ToProjectRelative(cfg.SourceXmlFile, baseDir);
                cfg.TargetExcelPath = FunctionLibrary.ToProjectRelative(cfg.TargetExcelPath, baseDir);
            }
            foreach (var job in s.TemplateExportJobs)
            {
                job.JsonFilePath                   = FunctionLibrary.ToProjectRelative(job.JsonFilePath, baseDir);
                job.OutputDirectory                = FunctionLibrary.ToProjectRelative(job.OutputDirectory, baseDir);
                job.IdsOutputDirectory             = FunctionLibrary.ToProjectRelative(job.IdsOutputDirectory, baseDir);
                job.LastExportedIdsOutputDirectory = FunctionLibrary.ToProjectRelative(job.LastExportedIdsOutputDirectory, baseDir);
                job.TypeTemplates = job.TypeTemplates.ToDictionary(
                    kv => kv.Key,
                    kv => FunctionLibrary.ToProjectRelative(kv.Value, baseDir));
            }
        }

        /// <summary>
        /// 将 settings 中所有路径字段以 <paramref name="baseDir"/> 为基准还原为绝对路径（加载后调用）。
        /// 已是绝对路径的字段（旧版 settings.json）原样保留，实现向后兼容。
        /// </summary>
        private static void DenormalizePaths(AppSettings s, string baseDir)
        {
            s.XmlDirectory               = FunctionLibrary.ToAbsoluteFromRoot(s.XmlDirectory, baseDir);
            s.ExcelDirectory             = FunctionLibrary.ToAbsoluteFromRoot(s.ExcelDirectory, baseDir);
            s.GenBatPath                 = FunctionLibrary.ToAbsoluteFromRoot(s.GenBatPath, baseDir);
            s.TableDesignSourceExcel     = FunctionLibrary.ToAbsoluteFromRoot(s.TableDesignSourceExcel, baseDir);
            s.TableDesignTargetDirectory = FunctionLibrary.ToAbsoluteFromRoot(s.TableDesignTargetDirectory, baseDir);
            s.TablesClassPath            = FunctionLibrary.ToAbsoluteFromRoot(s.TablesClassPath, baseDir);
            s.EnumExcelFiles             = s.EnumExcelFiles.Select(p => FunctionLibrary.ToAbsoluteFromRoot(p, baseDir)).ToList();
            s.TableDesignTargetFiles     = s.TableDesignTargetFiles.Select(p => FunctionLibrary.ToAbsoluteFromRoot(p, baseDir)).ToList();
            s.ExcelExportXmlFolder       = FunctionLibrary.ToAbsoluteFromRoot(s.ExcelExportXmlFolder, baseDir);
            s.ExcelExportDesignTemplate  = FunctionLibrary.ToAbsoluteFromRoot(s.ExcelExportDesignTemplate, baseDir);
            s.ExcelExportTargetFolder    = FunctionLibrary.ToAbsoluteFromRoot(s.ExcelExportTargetFolder, baseDir);
            foreach (var cfg in s.ExcelExportClassConfigs)
            {
                cfg.SourceXmlFile   = FunctionLibrary.ToAbsoluteFromRoot(cfg.SourceXmlFile, baseDir);
                cfg.TargetExcelPath = FunctionLibrary.ToAbsoluteFromRoot(cfg.TargetExcelPath, baseDir);
            }
            foreach (var job in s.TemplateExportJobs)
            {
                job.JsonFilePath                   = FunctionLibrary.ToAbsoluteFromRoot(job.JsonFilePath, baseDir);
                job.OutputDirectory                = FunctionLibrary.ToAbsoluteFromRoot(job.OutputDirectory, baseDir);
                job.IdsOutputDirectory             = FunctionLibrary.ToAbsoluteFromRoot(job.IdsOutputDirectory, baseDir);
                job.LastExportedIdsOutputDirectory = FunctionLibrary.ToAbsoluteFromRoot(job.LastExportedIdsOutputDirectory, baseDir);
                job.TypeTemplates = job.TypeTemplates.ToDictionary(
                    kv => kv.Key,
                    kv => FunctionLibrary.ToAbsoluteFromRoot(kv.Value, baseDir));
            }
        }

        /// <summary>
        /// 浅克隆 AppSettings，避免 Save() 修改调用方持有的对象。
        /// </summary>
        private static AppSettings ShallowClone(AppSettings src) => new()
        {
            ProjectName                       = src.ProjectName,
            FuzzyFindProjectRoot              = src.FuzzyFindProjectRoot,
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
            TemplateExportJobs                = src.TemplateExportJobs.Select(j => new TemplateExportJob
            {
                DisplayName                    = j.DisplayName,
                JsonFilePath                   = j.JsonFilePath,
                OutputDirectory                = j.OutputDirectory,
                Namespace                      = j.Namespace,
                UseGeneratedSuffix             = j.UseGeneratedSuffix,
                GenerateIds                    = j.GenerateIds,
                IdsOutputDirectory             = j.IdsOutputDirectory,
                IdsNamespace                   = j.IdsNamespace,
                IdsClassName                   = j.IdsClassName,
                IdsUsePartialClass             = j.IdsUsePartialClass,
                IdsUseGeneratedSuffix          = j.IdsUseGeneratedSuffix,
                TypeTemplates                  = new Dictionary<string, string>(j.TypeTemplates),
                NameField                      = j.NameField,
                IdField                        = j.IdField,
                LastExportedIdsClassName       = j.LastExportedIdsClassName,
                LastExportedIdsOutputDirectory = j.LastExportedIdsOutputDirectory,
                LastExportedOwnedGroups        = new List<string>(j.LastExportedOwnedGroups),
            }).ToList(),
            HomeIncludeEnum                   = src.HomeIncludeEnum,
            ExcelExportXmlFolder              = src.ExcelExportXmlFolder,
            ExcelExportDesignTemplate         = src.ExcelExportDesignTemplate,
            ExcelExportMode                   = src.ExcelExportMode,
            ExcelExportClassConfigs           = src.ExcelExportClassConfigs.Select(c => new ExcelExportClassConfig
            {
                Enabled         = c.Enabled,
                ClassName       = c.ClassName,
                SourceXmlFile   = c.SourceXmlFile,
                TargetExcelPath = c.TargetExcelPath,
            }).ToList(),
            ExcelExportTargetFolder           = src.ExcelExportTargetFolder,
            ExcelExportNamePrefix             = src.ExcelExportNamePrefix,
            ExcelExportNameSuffix             = src.ExcelExportNameSuffix,
            ExcelExportNameConvention         = src.ExcelExportNameConvention,
        };
    }
}
