using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ConfigExcelEnhancer.Core
{
    public static class TemplateRenderer
    {
        private static readonly Regex PlaceholderPattern =
            new(@"\{\{([^}]+)\}\}", RegexOptions.Compiled);

        /// <summary>
        /// 读取 templatePath 文件内容，替换所有 {{placeholder}}，返回渲染后字符串。
        /// </summary>
        public static string Render(
            string templatePath,
            JsonElement entry,
            TableMapping? mapping,
            string targetNamespace,
            string typeValue,
            string className,
            long id)
        {
            string template = File.ReadAllText(templatePath, Encoding.UTF8);
            return Replace(template, entry, mapping, targetNamespace, typeValue, className, id);
        }

        /// <summary>
        /// 对任意字符串内容执行占位符替换（供内部和测试使用）。
        /// </summary>
        public static string Replace(
            string template,
            JsonElement entry,
            TableMapping? mapping,
            string targetNamespace,
            string typeValue,
            string className,
            long id)
        {
            return PlaceholderPattern.Replace(template, m =>
            {
                string key = m.Groups[1].Value.Trim();
                return ResolveValue(key, entry, mapping, targetNamespace, typeValue, className, id);
            });
        }

        private static string ResolveValue(
            string key,
            JsonElement entry,
            TableMapping? mapping,
            string ns,
            string typeValue,
            string className,
            long id)
        {
            // Built-in $ placeholders
            switch (key)
            {
                case "$ClassName":      return className;
                case "$Id":             return id.ToString();
                case "$Type":           return typeValue;
                case "$Namespace":      return ns;
                case "$TableAccessor":  return mapping?.TableAccessor ?? "";
                case "$TableProperty":  return mapping?.PropertyName  ?? "";
                case "$LubanNamespace": return mapping?.LubanNamespace ?? "";
                case "$CfgType":        return typeValue;
                case "$FullCfgType":
                    return mapping is null ? typeValue : $"{mapping.LubanNamespace}.{typeValue}";
            }

            // JSON field (supports dot-notation for nested objects)
            return ResolveJsonPath(key, entry);
        }

        private static string ResolveJsonPath(string path, JsonElement element)
        {
            string[] parts = path.Split('.');
            JsonElement current = element;

            foreach (string part in parts)
            {
                if (current.ValueKind != JsonValueKind.Object)
                    return "";
                if (!current.TryGetProperty(part, out current))
                    return "";
            }

            return current.ValueKind switch
            {
                JsonValueKind.String  => current.GetString() ?? "",
                JsonValueKind.Number  => current.GetRawText(),
                JsonValueKind.True    => "true",
                JsonValueKind.False   => "false",
                JsonValueKind.Null    => "",
                _                    => current.GetRawText()
            };
        }
    }
}
