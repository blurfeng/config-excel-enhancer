using System.Xml.Linq;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    /// <summary>
    /// 从 XML Schema 文件中扫描枚举（enum）定义。
    /// 支持 Luban 格式的 &lt;var&gt; 和 &lt;option&gt; 两种子元素写法。
    /// </summary>
    public class EnumScanner
    {
        /// <summary>
        /// 递归扫描目录下所有 .xml 文件，提取其中的 &lt;enum&gt; 定义。
        /// 同名枚举以后读取的文件为准（后者覆盖前者）。
        /// </summary>
        public static List<EnumInfo> ScanDirectory(string xmlDirectory)
        {
            var result = new Dictionary<string, EnumInfo>(StringComparer.Ordinal);

            foreach (var file in Directory.EnumerateFiles(xmlDirectory, "*.xml", SearchOption.AllDirectories))
            {
                try
                {
                    var enums = ScanFile(file);
                    foreach (var e in enums)
                        result[e.Name] = e; // 同名后者覆盖前者
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"解析 XML 文件失败：{file}", ex);
                }
            }

            return [.. result.Values];
        }

        /// <summary>
        /// 递归扫描目录下所有 .xml 文件，提取 &lt;bean&gt; 中使用枚举类型的字段。
        /// 返回：beanName → (fieldName → enumTypeName)。
        /// 只收录字段类型在 enumNames 中的字段；同名 bean 以后读取的文件为准。
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> ScanBeanEnumFields(
            string xmlDirectory, HashSet<string> enumNames)
        {
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

            foreach (var file in Directory.EnumerateFiles(xmlDirectory, "*.xml", SearchOption.AllDirectories))
            {
                try
                {
                    var doc = XDocument.Load(file);
                    foreach (var beanEl in doc.Descendants("bean"))
                    {
                        var beanName = (string?)beanEl.Attribute("name");
                        if (string.IsNullOrWhiteSpace(beanName)) continue;

                        Dictionary<string, string>? fieldMap = null;
                        foreach (var varEl in beanEl.Elements("var"))
                        {
                            var fieldName = (string?)varEl.Attribute("name");
                            var fieldType = (string?)varEl.Attribute("type");
                            if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldType)) continue;
                            if (!enumNames.Contains(fieldType)) continue;

                            fieldMap ??= new Dictionary<string, string>(StringComparer.Ordinal);
                            fieldMap[fieldName] = fieldType;
                        }

                        if (fieldMap != null)
                            result[beanName] = fieldMap; // 同名后者覆盖前者
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"解析 XML 文件失败：{file}", ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 解析单个 XML 文件，提取所有 &lt;enum&gt; 元素及其选项。
        /// 每个枚举的选项从 &lt;var&gt; 或 &lt;option&gt; 子元素的 name/value 属性读取；
        /// 无选项的枚举（空 enum）会被忽略。
        /// </summary>
        private static List<EnumInfo> ScanFile(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var enums = new List<EnumInfo>();

            foreach (var enumEl in doc.Descendants("enum"))
            {
                var name = (string?)enumEl.Attribute("name");
                if (string.IsNullOrWhiteSpace(name)) continue;

                var info = new EnumInfo { Name = name };

                // 兼容两种子元素写法：Luban 新版用 <var>，旧版用 <option>
                foreach (var optionEl in enumEl.Elements("var").Concat(enumEl.Elements("option")))
                {
                    var optName = (string?)optionEl.Attribute("name");
                    if (string.IsNullOrWhiteSpace(optName)) continue;

                    info.Options.Add(new EnumOption
                    {
                        Name = optName,
                        Value = (string?)optionEl.Attribute("value") ?? string.Empty
                    });
                }

                if (info.Options.Count > 0)
                    enums.Add(info);
            }

            return enums;
        }
    }
}
