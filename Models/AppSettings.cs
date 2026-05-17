namespace ConfigExcelEnhancer.Models
{
    public class AppSettings
    {
        public string XmlDirectory { get; set; } = string.Empty;
        public string ExcelDirectory { get; set; } = string.Empty;
        public string GenBatPath { get; set; } = string.Empty;
        public bool HideEnumDataSheet { get; set; } = true;
        public bool EnumForceRewrite { get; set; } = false;

        // ── 表设计 ────────────────────────────────────────
        public string TableDesignSourceExcel { get; set; } = string.Empty;
        public int TableDesignTargetMode { get; set; } = 0; // 0=目录, 1=列表
        public string TableDesignTargetDirectory { get; set; } = string.Empty;
        public List<string> TableDesignTargetFiles { get; set; } = new();
        public bool TableDesignIgnoreUnderscoreFiles { get; set; } = true;
        public int TableDesignSheetScope { get; set; } = 0; // 0=所有, 1=第一张
        public bool TableDesignIgnoreUnderscoreSheets { get; set; } = true;
        public string TableDesignHeaderSymbol { get; set; } = "##";
        public bool TableDesignAutoColumnWidth { get; set; } = true;
        public bool TableDesignMergeHeaderCells { get; set; } = true;
        public string TableDesignMergeHeaderKeywords { get; set; } = "##type";
    }
}
