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

        /// <summary>是否强制重写枚举验证规则（忽略 Schema 是否变化）。</summary>
        public bool EnumForceRewrite { get; set; } = false;

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
    }
}
