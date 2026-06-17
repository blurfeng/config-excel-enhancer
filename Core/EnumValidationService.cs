using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    /// <summary>
    /// 枚举数据验证的公共准备流程：扫描 XML 枚举定义、（可选）注入合成 bool 枚举、
    /// 扫描 bean 字段枚举映射，得到可直接交给 <see cref="ValidationUpdater"/> 的验证数据。
    /// <para>供「枚举验证」页与「导出 Excel」页复用，避免重复实现同一套扫描逻辑。</para>
    /// </summary>
    public static class EnumValidationService
    {
        /// <summary>合成 bool 枚举的名称。</summary>
        public const string BoolEnumName = "bool";

        /// <summary>
        /// 扫描准备结果：用于验证的枚举集合、bean 字段枚举映射、真实枚举数量（不含合成 bool）。
        /// </summary>
        public sealed class PreparedEnums
        {
            /// <summary>交给 ValidationUpdater 的枚举集合（含可选的合成 bool）。</summary>
            public required List<EnumInfo> EnumsForValidation { get; init; }

            /// <summary>bean 类型 → (字段名 → 枚举类型名) 的映射。</summary>
            public required Dictionary<string, Dictionary<string, string>> BeanFieldEnumMap { get; init; }

            /// <summary>实际扫描到的枚举数量（不含合成 bool）。</summary>
            public required int RealEnumCount { get; init; }

            /// <summary>是否存在可用于验证的内容（真实枚举或已注入的 bool）。</summary>
            public bool HasAny => RealEnumCount > 0
                || EnumsForValidation.Any(e => e.Name == BoolEnumName);
        }

        /// <summary>
        /// 扫描 <paramref name="xmlDir"/> 下的枚举定义并准备验证数据。
        /// 通过 <paramref name="log"/> 输出扫描信息（需为线程安全的日志回调）。
        /// 同步执行，调用方可自行包裹到后台任务中。
        /// </summary>
        public static PreparedEnums PrepareEnums(
            string xmlDir,
            bool boolValidation,
            Action<string, LogLevel> log,
            CancellationToken token)
        {
            log("扫描数据定义 XML Enum 定义...", LogLevel.Info);

            var enums = EnumScanner.ScanDirectory(xmlDir);
            token.ThrowIfCancellationRequested();

            var enumNameSet = enums.Select(e => e.Name).ToHashSet(StringComparer.Ordinal);
            if (enums.Count > 0)
            {
                const int maxShow = 8;
                var names = enumNameSet.ToList();
                var display = names.Count <= maxShow
                    ? string.Join("、", names)
                    : string.Join("、", names.Take(maxShow)) + $" (+{names.Count - maxShow})";
                log($"找到 {enums.Count} 个 Enum：{display}", LogLevel.Ok);

                // 警告没有 value=0 的枚举（默认值将使用第一项）
                foreach (var ei in enums)
                    if (!ei.Options.Any(o => o.Value == "0"))
                        log($"  ⚠ {ei.Name} 没有 value=0 的选项，默认将使用第一项：{ei.Options.FirstOrDefault()?.Name}", LogLevel.Warn);
            }

            // 布尔值验证：注入合成 bool 枚举（Name="bool"，选项 FALSE/TRUE）
            var enumsForValidation = enums.ToList();
            if (boolValidation)
            {
                enumNameSet.Add(BoolEnumName);
                enumsForValidation.Add(new EnumInfo
                {
                    Name = BoolEnumName,
                    Options =
                    [
                        new EnumOption { Name = "FALSE", Value = "0" },
                        new EnumOption { Name = "TRUE",  Value = "1" }
                    ]
                });
                log("布尔值数据验证已启用（FALSE/TRUE）。", LogLevel.Info);
            }

            // 扫描 bean 字段枚举映射
            var beanFieldEnumMap = EnumScanner.ScanBeanEnumFields(xmlDir, enumNameSet);
            token.ThrowIfCancellationRequested();
            if (beanFieldEnumMap.Count > 0)
                log($"找到 {beanFieldEnumMap.Count} 个含枚举/布尔字段的 Bean。", LogLevel.Info);

            return new PreparedEnums
            {
                EnumsForValidation = enumsForValidation,
                BeanFieldEnumMap = beanFieldEnumMap,
                RealEnumCount = enums.Count,
            };
        }
    }
}
