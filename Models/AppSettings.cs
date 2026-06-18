namespace ConfigExcelEnhancer.Models
{
    /// <summary>
    /// 应用全局设置，持久化到 settings.json。
    /// 包含枚举验证、Luban 导表和表格设计三个功能模块的配置。
    /// </summary>
    public class AppSettings
    {
        // ── 枚举验证 ──────────────────────────────────────

        /// <summary>XML Schema 文件所在目录，用于扫描 &lt;enum&gt; 定义。</summary>
        public string XmlDirectory { get; set; } = string.Empty;

        /// <summary>Excel 配置文件所在目录，用于批量更新枚举验证规则。</summary>
        public string ExcelDirectory { get; set; } = string.Empty;

        /// <summary>Luban gen.bat 文件路径，用于解析和执行导表命令。</summary>
        public string GenBatPath { get; set; } = string.Empty;

        /// <summary>是否将 __enum_data 工作表设为极度隐藏（VeryHidden），防止用户误操作。</summary>
        public bool HideEnumDataSheet { get; set; } = true;

        /// <summary>是否为 bool 类型列添加 TRUE/FALSE 数据验证下拉框。</summary>
        public bool BoolValidation { get; set; } = true;

        /// <summary>配置 Excel 文件选择模式：0 = 目录扫描，1 = 手动列表。</summary>
        public int EnumExcelMode { get; set; } = 0;

        /// <summary>配置 Excel 文件列表（列表模式下使用）。</summary>
        public List<string> EnumExcelFiles { get; set; } = new();

        // ── 表设计 ────────────────────────────────────────

        /// <summary>表格设计模板 Excel 文件路径（样式来源）。</summary>
        public string TableDesignSourceExcel { get; set; } = string.Empty;

        /// <summary>目标文件选择模式：0 = 目录扫描，1 = 手动列表。</summary>
        public int TableDesignTargetMode { get; set; } = 0;

        /// <summary>目标文件目录（目录模式下使用）。</summary>
        public string TableDesignTargetDirectory { get; set; } = string.Empty;

        /// <summary>目标文件列表（列表模式下使用）。</summary>
        public List<string> TableDesignTargetFiles { get; set; } = new();

        /// <summary>是否跳过文件名以 __ 开头的 Excel 文件（通常为内部用途文件）。</summary>
        public bool TableDesignIgnoreUnderscoreFiles { get; set; } = true;

        /// <summary>应用设计的 Sheet 范围：0 = 所有 Sheet，1 = 仅第一张 Sheet。</summary>
        public int TableDesignSheetScope { get; set; } = 0;

        /// <summary>是否跳过 Sheet 名称以 __ 开头的工作表。</summary>
        public bool TableDesignIgnoreUnderscoreSheets { get; set; } = true;

        /// <summary>表头行的识别符号，A 列包含此符号的行视为表头行（默认 "##"）。</summary>
        public string TableDesignHeaderSymbol { get; set; } = "##";

        /// <summary>是否在应用设计后自动调整各列宽度以适应内容。</summary>
        public bool TableDesignAutoColumnWidth { get; set; } = true;

        /// <summary>是否合并表头行中的连续空白单元格。</summary>
        public bool TableDesignMergeHeaderCells { get; set; } = true;

        /// <summary>
        /// 需要执行单元格合并的行标识关键字，逗号或分号分隔。
        /// 支持行号（如 "3"）或文本模式（如 "##type"）两种形式。
        /// </summary>
        public string TableDesignMergeHeaderKeywords { get; set; } = "##type";

        // ── 导出模板类 ────────────────────────────────────────

        /// <summary>Luban 生成的 Tables.cs 路径（全局一次性配置，用于自动推断表访问器和命名空间）。</summary>
        public string TablesClassPath { get; set; } = string.Empty;

        /// <summary>导出模板任务列表。</summary>
        public List<TemplateExportJob> TemplateExportJobs { get; set; } = new();

        // ── 主页 ──────────────────────────────────────────

        /// <summary>
        /// 项目名称（通用参数，如 "GodsClash"）。
        /// 用于在新机器上自动定位项目根目录：启动时若本机未设置根目录，
        /// 会在工具目录的上层路径中查找同名文件夹并自动设置。
        /// 不含路径信息，可安全提交到版本控制。
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>自动查找根目录时是否启用模糊匹配（忽略大小写及连字符/下划线）。</summary>
        public bool FuzzyFindProjectRoot { get; set; } = true;

        /// <summary>主页"一键导出"是否包含 Enum 验证步骤。</summary>
        public bool HomeIncludeEnum { get; set; } = true;

        // ── 导出 Excel ────────────────────────────────────────

        /// <summary>XML 定义文件夹（供两种模式共用，支持跨文件继承引用）。</summary>
        public string ExcelExportXmlFolder { get; set; } = string.Empty;

        /// <summary>Excel 设计模板路径；空 = 使用表设计分页的模板；两者均空则直接新建。</summary>
        public string ExcelExportDesignTemplate { get; set; } = string.Empty;

        /// <summary>导出模式：0 = 按列表，1 = 批量导出。</summary>
        public int ExcelExportMode { get; set; } = 0;

        /// <summary>列表模式：各数据类的导出配置（类名 → 目标 Excel 路径）。</summary>
        public List<ExcelExportClassConfig> ExcelExportClassConfigs { get; set; } = new();

        /// <summary>列表模式：通用目标文件夹（配置项未指定路径时的回退目标）。</summary>
        public string ExcelExportListTargetFolder { get; set; } = string.Empty;

        /// <summary>列表模式：是否启用“通用导出文件夹”回退（关闭时空路径条目不导出）。</summary>
        public bool ExcelExportListCommonFolderEnabled { get; set; } = true;

        /// <summary>批量导出模式：导出 Excel 的目标文件夹。</summary>
        public string ExcelExportTargetFolder { get; set; } = string.Empty;

        /// <summary>导出文件名前缀（两种模式共用）。</summary>
        public string ExcelExportNamePrefix { get; set; } = string.Empty;

        /// <summary>导出文件名后缀（两种模式共用）。</summary>
        public string ExcelExportNameSuffix { get; set; } = string.Empty;

        /// <summary>导出 Excel 文件的类名转换方式：0 = 类名不变，1 = 驼峰（首字母大写），2 = 全小写_下划线。</summary>
        public int ExcelExportNameConvention { get; set; } = 0;

        /// <summary>导出 Excel 后是否对导出的文件执行 Enum 数据验证（复用枚举验证功能）。</summary>
        public bool ExcelExportRunEnumValidation { get; set; } = true;
    }
}
