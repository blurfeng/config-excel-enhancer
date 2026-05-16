namespace ConfigExcelEnhancer.Models
{
    public class EnumInfo
    {
        public string Name { get; set; } = string.Empty;
        public List<EnumOption> Options { get; set; } = [];

        /// <summary>
        /// 返回 value="0" 对应的 option name，若不存在则返回第一个 option name，否则返回空字符串。
        /// </summary>
        public string DefaultOptionName
        {
            get
            {
                var byValue = Options.FirstOrDefault(o => o.Value == "0");
                if (byValue != null) return byValue.Name;
                return Options.Count > 0 ? Options[0].Name : string.Empty;
            }
        }
    }
}
