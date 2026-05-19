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

        /// <summary>是否为 partial 类（默认 true）。</summary>
        public bool UsePartialClass { get; set; } = true;

        // ── Ids 文件 ──────────────────────────────────────────────────

        public bool GenerateIds { get; set; } = true;

        /// <summary>Ids.Generated.cs 输出完整路径（含文件名）。</summary>
        public string IdsOutputPath { get; set; } = "";

        public string IdsNamespace { get; set; } = "";

        /// <summary>Ids 类名，如 "UnitIds"。</summary>
        public string IdsClassName { get; set; } = "";

        // ── $type → .tmpl 文件路径绑定 ────────────────────────────────

        /// <summary>key=$type 值，value=.tmpl 文件绝对路径。</summary>
        public Dictionary<string, string> TypeTemplates { get; set; } = new();

        // ── 字段名自定义 ───────────────────────────────────────────────

        /// <summary>作为类名来源的 JSON 字段（默认 "name"）。</summary>
        public string NameField { get; set; } = "name";

        /// <summary>作为 ID 来源的 JSON 字段（默认 "id"）。</summary>
        public string IdField { get; set; } = "id";
    }
}
