namespace ConfigExcelEnhancer.Models
{
    /// <summary>
    /// 枚举选项，对应 XML 中 &lt;var&gt; 或 &lt;option&gt; 元素的一条记录。
    /// </summary>
    public class EnumOption
    {
        /// <summary>选项名称（对应 name 属性），填入 Excel 单元格的实际文本。</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>选项数值（对应 value 属性），value="0" 的选项将作为默认值优先填入空单元格。</summary>
        public string Value { get; set; } = string.Empty;
    }
}
