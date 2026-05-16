namespace ConfigExcelEnhancer.Models
{
    public class AppSettings
    {
        public string XmlDirectory { get; set; } = string.Empty;
        public string ExcelDirectory { get; set; } = string.Empty;
        public string GenBatPath { get; set; } = string.Empty;
        public bool HideEnumDataSheet { get; set; } = true;
    }
}
