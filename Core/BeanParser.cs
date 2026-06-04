using System.Xml.Linq;

namespace ConfigExcelEnhancer.Core
{
    /// <summary>
    /// Luban XML 中的 Bean 定义（一个数据类或结构体）。
    /// </summary>
    public class BeanInfo
    {
        public string Name { get; init; } = string.Empty;

        /// <summary>parent 属性原始值，可能含命名空间前缀（如 "Unit.UnitCfg"）。</summary>
        public string? Parent { get; init; }

        /// <summary>sep 属性；非空表示这是结构体 Bean（在 Excel 中展开为多列）。</summary>
        public string? Sep { get; init; }

        public List<BeanField> Fields { get; init; } = new();

        public string SourceFile { get; init; } = string.Empty;

        /// <summary>直接继承此 Bean 的子类名称（由 ParseFolder 填充）。</summary>
        public List<string> Children { get; } = new();
    }

    /// <summary>
    /// Bean 中的一个字段定义（对应 &lt;var&gt; 元素）。
    /// </summary>
    public record BeanField(
        string Name,
        string Type,    // 原始 type 属性（如 "int"、"EMoveType"、"SteeringStats"）
        string Group,   // group 属性（空 = 默认分组 "c,s"）
        string Comment  // comment 属性
    );

    /// <summary>
    /// 解析 Luban XML 文件夹中的 Bean 定义，识别叶子类并收集完整字段列表。
    /// </summary>
    public static class BeanParser
    {
        /// <summary>
        /// 递归扫描文件夹下所有 .xml 文件，返回全部 BeanInfo（含跨文件继承引用）。
        /// 同名 Bean 以后读取的文件为准（后者覆盖前者）。
        /// </summary>
        public static List<BeanInfo> ParseFolder(string xmlFolder)
        {
            var beanMap = new Dictionary<string, BeanInfo>(StringComparer.Ordinal);

            foreach (var file in Directory.EnumerateFiles(xmlFolder, "*.xml", SearchOption.AllDirectories))
            {
                try
                {
                    var doc = XDocument.Load(file);
                    foreach (var beanEl in doc.Descendants("bean"))
                    {
                        var name = (string?)beanEl.Attribute("name");
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        var fields = new List<BeanField>();
                        foreach (var varEl in beanEl.Elements("var"))
                        {
                            var fieldName = (string?)varEl.Attribute("name");
                            var fieldType = (string?)varEl.Attribute("type");
                            if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldType))
                                continue;

                            fields.Add(new BeanField(
                                Name:    fieldName,
                                Type:    fieldType,
                                Group:   (string?)varEl.Attribute("group") ?? string.Empty,
                                Comment: (string?)varEl.Attribute("comment") ?? string.Empty
                            ));
                        }

                        beanMap[name] = new BeanInfo
                        {
                            Name       = name,
                            Parent     = (string?)beanEl.Attribute("parent"),
                            Sep        = (string?)beanEl.Attribute("sep"),
                            Fields     = fields,
                            SourceFile = file,
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"解析 XML 文件失败：{file}", ex);
                }
            }

            // 填充 Children 列表
            foreach (var bean in beanMap.Values)
            {
                if (string.IsNullOrEmpty(bean.Parent)) continue;
                var parentSimpleName = StripNamespace(bean.Parent);
                if (beanMap.TryGetValue(parentSimpleName, out var parentBean))
                    parentBean.Children.Add(bean.Name);
            }

            return [.. beanMap.Values];
        }

        /// <summary>
        /// 从 BeanInfo 列表中识别叶子类（无子类 + 不作为其他 Bean 的字段类型）。
        /// 这些是需要生成独立 Excel 表的数据类。
        /// </summary>
        public static List<BeanInfo> GetLeafBeans(List<BeanInfo> allBeans)
        {
            // 所有在 <var type="..."> 中出现的简单类型名 → 结构体 Bean
            var usedAsField = new HashSet<string>(StringComparer.Ordinal);
            foreach (var bean in allBeans)
            {
                foreach (var field in bean.Fields)
                    usedAsField.Add(StripNamespace(field.Type));
            }

            return allBeans
                .Where(b =>
                    b.Sep == null &&             // 不是显式结构体 Bean
                    b.Children.Count == 0 &&     // 没有子类（叶子节点）
                    !usedAsField.Contains(b.Name)) // 不作为其他 Bean 的字段类型
                .OrderBy(b => b.SourceFile)
                .ThenBy(b => b.Name)
                .ToList();
        }

        /// <summary>
        /// 递归收集指定 Bean 的全部字段（父类字段在前，自身字段在后）。
        /// 自动去重：若同名字段在子类中重新定义，以子类为准。
        /// </summary>
        public static List<BeanField> GetAllFields(BeanInfo bean, Dictionary<string, BeanInfo> beanMap)
        {
            var result = new List<BeanField>();
            CollectFields(bean, beanMap, result, new HashSet<string>(StringComparer.Ordinal));
            return result;
        }

        /// <summary>
        /// 将 allBeans 列表转换为按简单类名索引的字典（忽略命名空间前缀）。
        /// </summary>
        public static Dictionary<string, BeanInfo> BuildBeanMap(List<BeanInfo> allBeans)
            => allBeans.ToDictionary(b => b.Name, StringComparer.Ordinal);

        /// <summary>
        /// 判断给定的类型名（原始 type 属性，可能含命名空间）是否对应一个结构体 Bean。
        /// 结构体 Bean 在 Excel 中展开为多列（有 sep 属性，或作为其他 Bean 的字段类型）。
        /// </summary>
        public static bool IsStructBean(string rawType, Dictionary<string, BeanInfo> beanMap,
            HashSet<string> usedAsField)
        {
            var simpleName = StripNamespace(rawType);
            if (!beanMap.TryGetValue(simpleName, out var bean)) return false;
            return bean.Sep != null || usedAsField.Contains(simpleName);
        }

        /// <summary>
        /// 构建"所有作为字段类型使用的 Bean 名称"集合，用于 IsStructBean 判断。
        /// </summary>
        public static HashSet<string> BuildUsedAsFieldSet(List<BeanInfo> allBeans)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            foreach (var bean in allBeans)
                foreach (var field in bean.Fields)
                    set.Add(StripNamespace(field.Type));
            return set;
        }

        // ── 内部辅助 ──────────────────────────────────────────────────────

        private static void CollectFields(BeanInfo bean, Dictionary<string, BeanInfo> beanMap,
            List<BeanField> result, HashSet<string> seen)
        {
            // 先递归收集父类字段
            if (!string.IsNullOrEmpty(bean.Parent))
            {
                var parentSimple = StripNamespace(bean.Parent);
                if (beanMap.TryGetValue(parentSimple, out var parentBean))
                    CollectFields(parentBean, beanMap, result, seen);
            }

            // 再追加自身字段（跳过同名已存在的字段）
            foreach (var field in bean.Fields)
            {
                if (seen.Add(field.Name))
                    result.Add(field);
            }
        }

        /// <summary>去掉命名空间前缀，如 "Unit.UnitCfg" → "UnitCfg"。</summary>
        public static string StripNamespace(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return typeName;
            var dot = typeName.LastIndexOf('.');
            return dot < 0 ? typeName : typeName[(dot + 1)..];
        }
    }
}
