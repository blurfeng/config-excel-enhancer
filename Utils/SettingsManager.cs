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

        /// <summary>settings.json 的绝对路径（供「打开所在文件夹」与诊断信息展示）。</summary>
        public static string FilePath => SettingsPath;

        #region 加载 / 保存 / 清除

        /// <summary>
        /// 从 settings.json 加载设置。若文件不存在则返回默认 AppSettings；
        /// 若文件存在但解析失败，弹出警告框并返回默认设置。
        /// 加载后所有路径字段均还原为绝对路径。
        /// </summary>
        public static AppSettings Load()
        {
            // System.Text.Json 遇到缺失字段时保留属性类定义的默认值，
            // 因此 HideEnumDataSheet 等字段即使不在旧版 JSON 中也会正确取 true。
            var settings = JsonFileHelper.Load(
                SettingsPath,
                () => new AppSettings(),
                ex => MessageBox.Show(
                    $"settings.json 加载失败，将使用默认设置。\n\n{ex.Message}",
                    "配置加载失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning));

            // 还原为绝对路径（对默认空路径为无操作）。
            TransformPaths(settings, GetProjectRoot(), FunctionLibrary.ToAbsoluteFromRoot);
            return settings;
        }

        /// <summary>
        /// 将设置序列化为格式化 JSON 并写入 settings.json。
        /// 写入前将路径字段转为相对于项目根目录的相对路径，不修改传入对象。
        /// </summary>
        public static void Save(AppSettings settings)
        {
            var toWrite = ShallowClone(settings);
            TransformPaths(toWrite, GetProjectRoot(), FunctionLibrary.ToProjectRelative);
            JsonFileHelper.Save(SettingsPath, toWrite);
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

        /// <summary>
        /// 读取磁盘上 settings.json 中原始（未还原）的所有路径字段，返回其中的相对路径。
        /// 用于「指纹法」自动定位项目根目录：在候选根目录下验证这些相对路径是否真实存在。
        /// 复用 <see cref="TransformPaths"/> 的字段登记表，避免遗漏新增字段。
        /// </summary>
        public static IReadOnlyList<string> GetConfiguredRelativePaths()
        {
            var raw = JsonFileHelper.Load(SettingsPath, () => new AppSettings());
            var collected = new List<string>();
            TransformPaths(raw, string.Empty, (p, _) =>
            {
                if (!string.IsNullOrWhiteSpace(p) && !Path.IsPathRooted(p))
                    collected.Add(p);
                return p;
            });
            return collected;
        }

        #endregion

        #region 路径规范化辅助

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
        /// 对 settings 中所有路径字段应用统一转换 <paramref name="transform"/>(原路径, baseDir)。
        /// 保存前传入 <see cref="FunctionLibrary.ToProjectRelative"/> 转相对，
        /// 加载后传入 <see cref="FunctionLibrary.ToAbsoluteFromRoot"/> 还原绝对。
        /// 新增路径字段时只需在此一处登记。
        /// </summary>
        private static void TransformPaths(AppSettings s, string baseDir, Func<string, string, string> transform)
        {
            string T(string p) => transform(p, baseDir);

            s.XmlDirectory               = T(s.XmlDirectory);
            s.ExcelDirectory             = T(s.ExcelDirectory);
            s.GenBatPath                 = T(s.GenBatPath);
            s.TableDesignSourceExcel     = T(s.TableDesignSourceExcel);
            s.TableDesignTargetDirectory = T(s.TableDesignTargetDirectory);
            s.TablesClassPath            = T(s.TablesClassPath);
            s.EnumExcelFiles             = s.EnumExcelFiles.Select(T).ToList();
            s.TableDesignTargetFiles     = s.TableDesignTargetFiles.Select(T).ToList();
            s.ExcelExportXmlFolder        = T(s.ExcelExportXmlFolder);
            s.ExcelExportDesignTemplate   = T(s.ExcelExportDesignTemplate);
            s.ExcelExportListTargetFolder = T(s.ExcelExportListTargetFolder);
            s.ExcelExportTargetFolder     = T(s.ExcelExportTargetFolder);
            foreach (var cfg in s.ExcelExportClassConfigs)
            {
                cfg.SourceXmlFile   = T(cfg.SourceXmlFile);
                cfg.TargetExcelPath = T(cfg.TargetExcelPath);
            }
            foreach (var job in s.TemplateExportJobs)
            {
                job.JsonFilePath                   = T(job.JsonFilePath);
                job.OutputDirectory                = T(job.OutputDirectory);
                job.IdsOutputDirectory             = T(job.IdsOutputDirectory);
                job.TypeTemplates = job.TypeTemplates.ToDictionary(kv => kv.Key, kv => T(kv.Value));
            }
        }

        #endregion

        #region 克隆

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
                Id                             = j.Id,
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
            ExcelExportListTargetFolder       = src.ExcelExportListTargetFolder,
            ExcelExportListCommonFolderEnabled = src.ExcelExportListCommonFolderEnabled,
            ExcelExportTargetFolder           = src.ExcelExportTargetFolder,
            ExcelExportNamePrefix             = src.ExcelExportNamePrefix,
            ExcelExportNameSuffix             = src.ExcelExportNameSuffix,
            ExcelExportNameConvention         = src.ExcelExportNameConvention,
            ExcelExportRunEnumValidation      = src.ExcelExportRunEnumValidation,
        };

        #endregion
    }
}
