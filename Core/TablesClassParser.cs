using System.Text.RegularExpressions;

namespace ConfigExcelEnhancer.Core
{
    public record TableMapping(
        string JsonBaseName,    // "unit_tbpawn"
        string PropertyName,    // "TbPawn"
        string LubanNamespace,  // "cfg.Unit"
        string TableAccessor    // "Config.Tables.TbPawn"
    );

    public static class TablesClassParser
    {
        // Matches:  TbPawn = new Unit.TbPawn(loader("unit_tbpawn"));
        // Group 1 = property name (TbPawn)
        // Group 2 = namespace segment (Unit)
        // Group 3 = json base name (unit_tbpawn)
        private static readonly Regex AssignmentPattern = new(
            @"(\w+)\s*=\s*new\s+(\w+)\.\w+\s*\(\s*loader\s*\(\s*""([^""]+)""\s*\)",
            RegexOptions.Compiled);

        /// <summary>
        /// 解析 Tables.cs，返回 jsonBaseName → TableMapping 字典。
        /// Tables.cs 中需存在形如 TbPawn = new Unit.TbPawn(loader("unit_tbpawn")); 的赋值。
        /// </summary>
        public static Dictionary<string, TableMapping> Parse(string tablesClassPath)
        {
            var result = new Dictionary<string, TableMapping>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(tablesClassPath) || !File.Exists(tablesClassPath))
                return result;

            string content = File.ReadAllText(tablesClassPath);

            // Detect the Tables class's own namespace prefix (e.g. "cfg" from "namespace cfg")
            string tablesPrefix = DetectTablesPrefix(content);

            foreach (Match m in AssignmentPattern.Matches(content))
            {
                string propertyName  = m.Groups[1].Value;  // TbPawn
                string nsSegment     = m.Groups[2].Value;  // Unit
                string jsonBaseName  = m.Groups[3].Value;  // unit_tbpawn

                // Full Luban namespace: "cfg.Unit"
                string lubanNamespace = string.IsNullOrEmpty(tablesPrefix)
                    ? nsSegment
                    : $"{tablesPrefix}.{nsSegment}";

                // Table accessor: "Config.Tables.TbPawn"
                string tableAccessor = $"Config.Tables.{propertyName}";

                result[jsonBaseName] = new TableMapping(jsonBaseName, propertyName, lubanNamespace, tableAccessor);
            }

            return result;
        }

        // Extracts the outer namespace of the Tables class, e.g. "cfg" from "namespace cfg { ... class Tables"
        private static string DetectTablesPrefix(string content)
        {
            var m = Regex.Match(content, @"namespace\s+([\w.]+)\s*\{[^}]*class\s+Tables");
            return m.Success ? m.Groups[1].Value : string.Empty;
        }
    }
}
