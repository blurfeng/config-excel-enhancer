using System.Text;
using System.Text.Json;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    public record TemplateExportOptions(
        TemplateExportJob Job,
        Dictionary<string, TableMapping> TableMappings
    );

    public static class TemplateExporter
    {
        public static async Task ExportAsync(
            TemplateExportOptions options,
            IProgress<string> progress,
            Action<string, LogLevel> log,
            CancellationToken token)
        {
            await Task.Run(() => Export(options, progress, log, token), token);
        }

        private static void Export(
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
                return;
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
                log($"JSON 解析失败：{ex.Message}", LogLevel.Error);
                return;
            }

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                log("JSON 根元素不是数组，跳过", LogLevel.Error);
                return;
            }

            var entries = doc.RootElement.EnumerateArray().ToList();
            log($"读取到 {entries.Count} 条数据", LogLevel.Info);

            // ── 生成 Ids.Generated.cs ────────────────────────────────
            if (job.GenerateIds)
                GenerateIds(job, entries, log, token);

            // ── 创建输出目录 ─────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(job.OutputDirectory))
                Directory.CreateDirectory(job.OutputDirectory);

            // ── 逐条生成模板类 ────────────────────────────────────────
            int created = 0, skipped = 0, overwritten = 0;
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

                string targetPath = Path.Combine(job.OutputDirectory, $"{className}.cs");
                bool hasTemplate = !string.IsNullOrWhiteSpace(typeValue)
                    && job.TypeTemplates.TryGetValue(typeValue, out string? tmplPath)
                    && File.Exists(tmplPath);

                if (hasTemplate)
                {
                    // 有模板：始终覆盖
                    string rendered = TemplateRenderer.Render(
                        job.TypeTemplates[typeValue!]!, entry, mapping,
                        job.Namespace, typeValue!, className, id);
                    var headerSb = new StringBuilder();
                    AppendFileHeader(headerSb, null);
                    string content = headerSb.ToString() + rendered;
                    WriteFile(targetPath, content);
                    log($"{className}.cs（{typeValue}）← 模板生成", LogLevel.Ok);
                    overwritten++;
                }
                else if (!File.Exists(targetPath))
                {
                    // 无模板且文件不存在：写入骨架
                    string scaffold = BuildScaffold(job, className, typeValue, id);
                    WriteFile(targetPath, scaffold);
                    log($"{className}.cs ← 骨架创建", LogLevel.Ok);
                    created++;
                }
                else
                {
                    log($"{className}.cs — 已存在，跳过", LogLevel.Skip);
                    skipped++;
                }
            }

            log("─────────────────────────────────────────────────────────", LogLevel.Section);
            log($"完成：新建 {created}，模板覆盖 {overwritten}，跳过 {skipped}", LogLevel.Ok);
        }

        // ── Ids 文件生成 ─────────────────────────────────────────────

        private static void GenerateIds(
            TemplateExportJob job,
            List<JsonElement> entries,
            Action<string, LogLevel> log,
            CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(job.IdsOutputPath) ||
                string.IsNullOrWhiteSpace(job.IdsClassName))
            {
                log("Ids 输出路径或类名未配置，跳过 Ids 生成", LogLevel.Warn);
                return;
            }

            // 按 $type 分组
            var groups = new Dictionary<string, List<(string className, long id)>>(StringComparer.Ordinal);
            foreach (var entry in entries)
            {
                token.ThrowIfCancellationRequested();
                if (!TryGetString(entry, job.NameField, out string name)) continue;
                TryGetString(entry, "$type", out string typeValue);
                long id = TryGetId(entry, job.IdField);
                string group = string.IsNullOrWhiteSpace(typeValue) ? "(未知)" : typeValue;
                if (!groups.ContainsKey(group)) groups[group] = new();
                groups[group].Add((ToPascalCase(name), id));
            }

            var sb = new StringBuilder();
            AppendFileHeader(sb, null);
            sb.AppendLine($"namespace {job.IdsNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static partial class {job.IdsClassName}");
            sb.AppendLine("    {");

            bool firstGroup = true;
            foreach (var (groupName, items) in groups)
            {
                if (!firstGroup) sb.AppendLine();
                firstGroup = false;
                sb.AppendLine($"        // {groupName}");
                foreach (var (className, id) in items)
                    sb.AppendLine($"        public const int {className} = {id};");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            string dir = Path.GetDirectoryName(job.IdsOutputPath)!;
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
            WriteFile(job.IdsOutputPath, sb.ToString());
            log($"Ids 文件已生成：{Path.GetFileName(job.IdsOutputPath)}（{entries.Count} 条）", LogLevel.Ok);
        }

        // ── 骨架生成 ─────────────────────────────────────────────────

        private static string BuildScaffold(TemplateExportJob job, string className, string typeValue, long id)
        {
            string partial = job.UsePartialClass ? "partial " : "";
            string typeComment = string.IsNullOrWhiteSpace(typeValue) ? "" : $" | $type: {typeValue}";
            var sb = new StringBuilder();
            AppendFileHeader(sb, $"骨架由工具自动生成，请手动填写实现逻辑。id: {id}{typeComment}");
            sb.AppendLine($"namespace {job.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public {partial}class {className}");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        // ── 工具方法 ─────────────────────────────────────────────────

        /// <summary>
        /// 向 StringBuilder 追加统一的规范化文件头部注释。
        /// </summary>
        /// <param name="sb">目标 StringBuilder。</param>
        /// <param name="extraInfo">附加说明行，为 null 则省略。</param>
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
