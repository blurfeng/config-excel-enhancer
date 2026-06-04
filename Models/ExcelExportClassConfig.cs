namespace ConfigExcelEnhancer.Models
{
    /// <summary>
    /// 列表模式下单个数据类的导出配置。
    /// </summary>
    public class ExcelExportClassConfig
    {
        /// <summary>是否启用该类的导出。</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>叶子类名称（由 XML 扫描得出，只读显示）。</summary>
        public string ClassName { get; set; } = string.Empty;

        /// <summary>该类所在的 .xml 文件路径（只读显示）。</summary>
        public string SourceXmlFile { get; set; } = string.Empty;

        /// <summary>目标 Excel 文件路径；为空时跳过该类的导出。</summary>
        public string TargetExcelPath { get; set; } = string.Empty;
    }
}
