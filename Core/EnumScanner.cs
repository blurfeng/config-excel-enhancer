using System.Xml.Linq;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    public class EnumScanner
    {
        /// <summary>
        /// 扫描目录下所有 .xml 文件，读取所有 &lt;enum&gt; 定义。
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

        private static List<EnumInfo> ScanFile(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var enums = new List<EnumInfo>();

            foreach (var enumEl in doc.Descendants("enum"))
            {
                var name = (string?)enumEl.Attribute("name");
                if (string.IsNullOrWhiteSpace(name)) continue;

                var info = new EnumInfo { Name = name };

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
