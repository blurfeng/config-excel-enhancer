using System.Text;
using System.Text.Json;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    public record TemplateExportOptions(
        TemplateExportJob Job,
        Dictionary<string, TableMapping> TableMappings
    );

    /// <summary>单个任务收集到的 Ids 数据，供 RunJobs 合并后统一写文件。</summary>
    public record IdsCollectionResult(
        string IdsOutputDirectory,
        string IdsNamespace,
        string IdsClassName,
        bool UsePartialClass,
        bool UseGeneratedSuffix,
        List<(string ClassName, long Id, string Group)> Entries,
        string TemplateNamespace
    );

    /// <summary>合并后的 Ids 生成参数，传给 GenerateIds。</summary>
    public record IdsGenerateOptions(
        string IdsOutputDirectory,
        string IdsNamespace,
        string IdsClassName,
        bool UsePartialClass,
        bool UseGeneratedSuffix,
        List<(string ClassName, long Id, string Group)> Entries,
        string TemplateNamespace
    );

    public static class TemplateExporter
    {
        public static async Task<IdsCollectionResult?> ExportAsync(
            TemplateExportOptions options,
            IProgress<string> progress,
            Action<string, LogLevel> log,
            CancellationToken token)
        {
            return await Task.Run(() => Export(options, progress, log, token), token);
        }

        private static IdsCollectionResult? Export(
            TemplateExportOptions options,
            IProgress<string> progress,
            Action<string, LogLevel> log,
            CancellationToken token)
        {
            var job = options.Job;

            // ── 校验基本配置 ──────────────────────────────────────────
            if (!File.Exists(job.JsonFilePath))
            {
                log($"JSON 文件不存在：{job.JsonFilePath}", LogLevel.Error);
                return null;
            }

            // ── 推断 TableMapping ────────────────────────────────────
            string jsonBaseName = Path.GetFileNameWithoutExtension(job.JsonFilePath);
            options.TableMappings.TryGetValue(jsonBaseName, out TableMapping? mapping);
            if (mapping is null)
                log($"Tables.cs 中未找到 \"{jsonBaseName}\" 的映射，{{$TableAccessor}} 等占位符将为空", LogLevel.Warn);

            // ── 解析 JSON ────────────────────────────────────────────
            string jsonText = File.ReadAllText(job.JsonFilePath, Encoding.UTF8);
            JsonDocument doc;
            try { doc = JsonDocument.Parse(jsonText); }
            catch (Exception ex)
            {
                log($"JSON 解析失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
                return null;
            }

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                log("JSON 根元素不是数组，跳过", LogLevel.Error);
                return null;
            }

            var entries = doc.RootElement.EnumerateArray().ToList();
            log($"读取到 {entries.Count} 条数据", LogLevel.Info);

            // ── 创建输出目录 ─────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(job.OutputDirectory))
                Directory.CreateDirectory(job.OutputDirectory);

            // ── 逐条生成模板类 ────────────────────────────────────────
            int rendered = 0, skipped = 0;
            var idsEntries = new List<(string ClassName, long Id, string Group)>();

            foreach (var entry in entries)
            {
                token.ThrowIfCancellationRequested();

                if (!TryGetString(entry, job.NameField, out string name) || string.IsNullOrWhiteSpace(name))
                {
                    log($"条目缺少字段 \"{job.NameField}\"，跳过", LogLevel.Warn);
                    continue;
                }

                string className = ToPascalCase(name);
                long id = TryGetId(entry, job.IdField);
                TryGetString(entry, "$type", out string typeValue);

                progress?.Report(className);

                // 收集 Ids 数据
                // 无 $type 时用唯一模板的 Key（即配置的类型名）作为分组注释，无模板则用 IdsClassName
                string group = !string.IsNullOrWhiteSpace(typeValue)
                    ? typeValue
                    : (job.TypeTemplates.Count == 1 ? job.TypeTemplates.First().Key : job.IdsClassName);
                idsEntries.Add((className, id, group));

                // 有模板才生成类文件
                // 若无 $type（非多态表），且模板列表只有一条，则直接使用该唯一模板
                string? resolvedTemplateKey = null;
                string? resolvedTemplatePath = null;

                if (!string.IsNullOrWhiteSpace(typeValue))
                {
                    // 有 $type，按类型匹配
                    if (job.TypeTemplates.TryGetValue(typeValue, out string? matchedPath) && File.Exists(matchedPath))
                    {
                        resolvedTemplateKey = typeValue;
                        resolvedTemplatePath = matchedPath;
                    }
                }
                else if (job.TypeTemplates.Count == 1)
                {
                    // 非多态：只有唯一模板，直接使用
                    var sole = job.TypeTemplates.First();
                    if (File.Exists(sole.Value))
                    {
                        resolvedTemplateKey = sole.Key;
                        resolvedTemplatePath = sole.Value;
                    }
                }
                else if (job.TypeTemplates.Count == 0)
                {
                    log($"[跳过] {className}：未配置任何模板", LogLevel.Warn);
                    skipped++;
                    continue;
                }
                else
                {
                    // 多模板但缺少 $type，无法匹配
                    log($"[跳过] {className}：条目缺少 \"$type\" 字段，无法匹配模板", LogLevel.Warn);
                    skipped++;
                    continue;
                }

                if (resolvedTemplatePath is null)
                {
                    skipped++;
                    continue;
                }

                string suffix = job.UseGeneratedSuffix ? ".Generated" : "";
                string targetPath = Path.Combine(job.OutputDirectory, $"{className}{suffix}.cs");

                string renderedContent = TemplateRenderer.Render(
                    resolvedTemplatePath, entry, mapping,
                    job.Namespace, resolvedTemplateKey!, className, id, job.IdsClassName);
                var headerSb = new StringBuilder();
                AppendFileHeader(headerSb, null);
                string content = headerSb.ToString() + renderedContent;

                bool written = WriteFileIfChanged(targetPath, content);
                if (written)
                {
                    log($"{className}{suffix}.cs（{resolvedTemplateKey}）← 模板生成", LogLevel.Ok);
                    rendered++;
                }
                else
                {
                    log($"{className}{suffix}.cs（{resolvedTemplateKey}）← 内容未变，已跳过", LogLevel.Info);
                    skipped++;
                }
            }

            log("─────────────────────────────────────────────────────────", LogLevel.Section);
            log($"完成：模板生成 {rendered}，跳过 {skipped}", LogLevel.Ok);

            // ── 返回 Ids 收集结果（由调用方合并写文件）──────────────
            if (!job.GenerateIds ||
                string.IsNullOrWhiteSpace(job.IdsOutputDirectory) ||
                string.IsNullOrWhiteSpace(job.IdsClassName))
                return null;

            return new IdsCollectionResult(
                job.IdsOutputDirectory,
                job.IdsNamespace,
                job.IdsClassName,
                job.IdsUsePartialClass,
                job.IdsUseGeneratedSuffix,
                idsEntries,
                job.Namespace);
        }

        // ── Ids 文件生成（由 RunJobs 合并后统一调用）────────────────

        public static void GenerateIds(
            IdsGenerateOptions options,
            Action<string, LogLevel> log)
        {
            string partial = options.UsePartialClass ? "partial " : "";
            string suffix = options.UseGeneratedSuffix ? ".Generated" : "";
            string fileName = $"{options.IdsClassName}{suffix}.cs";
            string outputPath = Path.Combine(options.IdsOutputDirectory, fileName);

            // 总是用本次运行收集的数据完整重写：不读取/合并旧文件内容，
            // 避免旧 Id 残留（如数据被删除、Id 变更、$type 改名等情况）。
            // 按 Group 分组
            var groups = new Dictionary<string, List<(string ClassName, long Id)>>(StringComparer.Ordinal);
            foreach (var (className, id, group) in options.Entries)
            {
                if (!groups.ContainsKey(group)) groups[group] = new();
                groups[group].Add((className, id));
            }

            var sb = new StringBuilder();
            AppendFileHeader(sb, null);
            sb.AppendLine($"namespace {options.IdsNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static {partial}class {options.IdsClassName}");
            sb.AppendLine("    {");

            bool firstGroup = true;
            foreach (var (groupName, items) in groups)
            {
                if (!firstGroup) sb.AppendLine();
                firstGroup = false;
                sb.AppendLine($"        #region {groupName}");
                foreach (var (className, id) in items)
                {
                    sb.AppendLine($"        /// <summary><see cref=\"{options.TemplateNamespace}.{className}\"/></summary>");
                    sb.AppendLine($"        public const int {className} = {id};");
                }
                sb.AppendLine($"        #endregion");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            Directory.CreateDirectory(options.IdsOutputDirectory);
            WriteFile(outputPath, sb.ToString());
            log($"Ids 文件已生成（覆盖）：{fileName}（共 {options.Entries.Count} 条）", LogLevel.Ok);
        }

        // ── 工具方法 ─────────────────────────────────────────────────

        private static void AppendFileHeader(StringBuilder sb, string? extraInfo)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("//   此文件由 ConfigExcelEnhancer 自动生成，请勿手动修改。");
            sb.AppendLine("//   手动修改将在下次导出时被覆盖。");
            if (!string.IsNullOrWhiteSpace(extraInfo))
                sb.AppendLine($"//   {extraInfo}");
            sb.AppendLine($"//   生成时间：{timestamp}");
            sb.AppendLine("//   来源工具：ConfigExcelEnhancer (https://github.com/blurfeng/config-excel-enhancer)");
            sb.AppendLine("// </auto-generated>");
        }

        private static string StripHeader(string content)
        {
            const string startMarker = "// <auto-generated>";
            const string endMarker = "// </auto-generated>";

            int startIdx = content.IndexOf(startMarker, StringComparison.Ordinal);
            if (startIdx == -1) return content;

            int endIdx = content.IndexOf(endMarker, startIdx, StringComparison.Ordinal);
            if (endIdx == -1) return content;

            // 跳过 endMarker 和其后的换行符
            int removeEnd = endIdx + endMarker.Length;
            if (removeEnd < content.Length && content[removeEnd] == '\r') removeEnd++;
            if (removeEnd < content.Length && content[removeEnd] == '\n') removeEnd++;

            return content[removeEnd..];
        }

        private static bool WriteFileIfChanged(string path, string newContent)
        {
            // 文件不存在，直接写入
            if (!File.Exists(path))
            {
                WriteFile(path, newContent);
                return true;
            }

            // 文件存在，比较内容（去除头部时间戳）
            try
            {
                string oldContent = File.ReadAllText(path, Encoding.UTF8);
                string oldStripped = StripHeader(oldContent);
                string newStripped = StripHeader(newContent);

                if (string.Equals(oldStripped, newStripped, StringComparison.Ordinal))
                {
                    // 内容未变，跳过写入
                    return false;
                }
            }
            catch
            {
                // 读取失败，覆盖写入
            }

            WriteFile(path, newContent);
            return true;
        }

        private static void WriteFile(string path, string content)
        {
            File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        private static bool TryGetString(JsonElement element, string field, out string value)
        {
            if (element.TryGetProperty(field, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                value = prop.GetString() ?? "";
                return !string.IsNullOrEmpty(value);
            }
            value = "";
            return false;
        }

        private static long TryGetId(JsonElement element, string field)
        {
            if (element.TryGetProperty(field, out var prop))
            {
                if (prop.TryGetInt64(out long v)) return v;
                if (prop.ValueKind == JsonValueKind.String
                    && long.TryParse(prop.GetString(), out long sv)) return sv;
            }
            return 0;
        }

        private static string ToPascalCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpperInvariant(s[0]) + s[1..];
        }
    }
}