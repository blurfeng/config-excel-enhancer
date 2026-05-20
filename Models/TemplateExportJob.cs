namespace ConfigExcelEnhancer.Models
{
    public class TemplateExportJob
    {
        public string DisplayName { get; set; } = "";

        /// <summary>输入 JSON 文件绝对路径。</summary>
        public string JsonFilePath { get; set; } = "";

        /// <summary>模板类输出目录。</summary>
        public string OutputDirectory { get; set; } = "";

        /// <summary>生成类的 C# 命名空间。</summary>
        public string Namespace { get; set; } = "";

        /// <summary>生成的模板类文件名是否带 .Generated 后缀（默认 false）。</summary>
        public bool UseGeneratedSuffix { get; set; } = false;

        // ── Ids 文件 ──────────────────────────────────────────────────

        public bool GenerateIds { get; set; } = true;

        /// <summary>Ids 文件输出目录（不含文件名）。</summary>
        public string IdsOutputDirectory { get; set; } = "";

        public string IdsNamespace { get; set; } = "";

        /// <summary>Ids 类名，如 "UnitIds"。</summary>
        public string IdsClassName { get; set; } = "";

        /// <summary>Ids 类是否声明为 partial（默认 true）。</summary>
        public bool IdsUsePartialClass { get; set; } = true;

        /// <summary>Ids 文件名是否带 .Generated 后缀（默认 false）。</summary>
        public bool IdsUseGeneratedSuffix { get; set; } = false;

        // ── $type → .tmpl 文件路径绑定 ────────────────────────────────

        /// <summary>key=$type 值，value=.tmpl 文件绝对路径。</summary>
        public Dictionary<string, string> TypeTemplates { get; set; } = new();

        // ── 字段名自定义 ───────────────────────────────────────────────

        /// <summary>作为类名来源的 JSON 字段（默认 "name"）。</summary>
        public string NameField { get; set; } = "name";

        /// <summary>作为 ID 来源的 JSON 字段（默认 "id"）。</summary>
        public string IdField { get; set; } = "id";

        // ── 导出缓存 ───────────────────────────────────────────────────

        /// <summary>上次成功导出时使用的 Ids 类名（用于检测重命名）。</summary>
        public string LastExportedIdsClassName { get; set; } = "";

        /// <summary>上次成功导出时使用的 Ids 输出目录。</summary>
        public string LastExportedIdsOutputDirectory { get; set; } = "";

        /// <summary>上次成功导出时拥有的 group 名称列表（用于检测 $type 改名后清除旧 group）。</summary>
        public List<string> LastExportedOwnedGroups { get; set; } = new();
    }
}
