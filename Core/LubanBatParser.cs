using System.Text;
using System.Text.RegularExpressions;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    /// <summary>
    /// 解析和写回 Luban gen.bat 文件。
    /// 支持提取 set 变量、echo 标签和 dotnet 命令块（含多行续行符 ^）；
    /// 保存时保留原始文件的格式和注释，只替换已知字段的值。
    /// </summary>
    public partial class LubanBatParser
    {
        // 匹配 set VAR=value 行，捕获前缀空白、变量名、等号和值四个分组
        [GeneratedRegex(@"^(\s*set\s+)(\w+)(=)(.*)$", RegexOptions.IgnoreCase)]
        private static partial Regex SetVarRegex();

        // 匹配 -x key=value；值在空白字符或 ^（续行符）处截止
        [GeneratedRegex(@"-x\s+([\w.]+)=([^\s^]+)")]
        private static partial Regex XArgRegex();

        /// <summary>
        /// 解析 gen.bat 文件，提取全局 set 变量和所有 dotnet 命令块。
        /// echo 行中的非分隔符文本会被捕获为紧随其后的 dotnet 命令块的标签。
        /// </summary>
        public static LubanConfig Parse(string batFilePath)
        {
            var config = new LubanConfig { BatFilePath = batFilePath };
            var lines = File.ReadAllLines(batFilePath);
            string? pendingLabel = null;
            int i = 0;

            while (i < lines.Length)
            {
                var raw = lines[i];
                var trimmed = raw.Trim();

                // 处理 set VAR=value 语句
                var setMatch = SetVarRegex().Match(raw);
                if (setMatch.Success)
                {
                    config.SetVariables[setMatch.Groups[2].Value] = setMatch.Groups[4].Value.Trim();
                    i++;
                    continue;
                }

                // echo 行：将非分隔符文本捕获为下一个 dotnet 块的标签
                if (trimmed.StartsWith("echo ", StringComparison.OrdinalIgnoreCase))
                {
                    var echoText = trimmed[5..].Trim();
                    if (!string.IsNullOrEmpty(echoText) && !echoText.All(c => c is '=' or '-' or ' '))
                        pendingLabel = echoText;
                    i++;
                    continue;
                }

                // dotnet 命令块
                if (trimmed.StartsWith("dotnet ", StringComparison.OrdinalIgnoreCase))
                {
                    var cmd = new LubanDotnetCommand
                    {
                        Label = pendingLabel ?? $"命令 {config.Commands.Count + 1}"
                    };
                    pendingLabel = null;

                    // 将所有续行合并为一个字符串
                    var sb = new StringBuilder();
                    while (i < lines.Length)
                    {
                        var cmdLine = lines[i].TrimEnd();
                        var hasCont = cmdLine.EndsWith('^');
                        sb.Append(' ').Append(hasCont ? cmdLine[..^1] : cmdLine);
                        i++;
                        if (!hasCont) break;
                    }

                    ParseCommandArgs(sb.ToString(), cmd);
                    config.Commands.Add(cmd);
                    continue;
                }

                i++;
            }

            return config;
        }

        /// <summary>
        /// 从合并后的 dotnet 命令行字符串中提取标准参数（-t/-d/-c/--conf）和扩展参数（-x key=value）。
        /// </summary>
        private static void ParseCommandArgs(string cmdLine, LubanDotnetCommand cmd)
        {
            var tokens = cmdLine.Split((char[])[' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                var tok = tokens[i];
                if (tok is "-t" or "-d" or "-c" && i + 1 < tokens.Length)
                    cmd.Args[tok.TrimStart('-')] = tokens[++i];
                else if (tok == "--conf" && i + 1 < tokens.Length)
                    cmd.Args["conf"] = tokens[++i];
                else if (tok == "-x" && i + 1 < tokens.Length)
                {
                    var kv = tokens[++i].Trim('"');
                    var eq = kv.IndexOf('=');
                    if (eq > 0)
                        cmd.XArgs[kv[..eq]] = kv[(eq + 1)..];
                }
            }
        }

        /// <summary>
        /// 将修改后的配置写回 bat 文件，保留原始格式和注释。
        /// 只更新 set 变量值和各命令块内的 -x 参数值。
        /// </summary>
        public static void Save(LubanConfig config)
        {
            var lines = File.ReadAllLines(config.BatFilePath);
            var result = new List<string>(lines.Length);
            int cmdIdx = -1;
            bool inDotnet = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // 替换 set 变量值
                var setMatch = SetVarRegex().Match(line);
                if (setMatch.Success)
                {
                    var varName = setMatch.Groups[2].Value;
                    if (config.SetVariables.TryGetValue(varName, out var newVal))
                    {
                        result.Add($"{setMatch.Groups[1].Value}{varName}{setMatch.Groups[3].Value}{newVal}");
                        continue;
                    }
                }

                // 追踪当前所在的 dotnet 命令块
                if (trimmed.StartsWith("dotnet ", StringComparison.OrdinalIgnoreCase))
                {
                    cmdIdx++;
                    inDotnet = true;
                }

                // 替换当前命令块内的 -x 参数值
                if (inDotnet && cmdIdx >= 0 && cmdIdx < config.Commands.Count)
                {
                    var cmd = config.Commands[cmdIdx];
                    var newLine = XArgRegex().Replace(line, m =>
                    {
                        var key = m.Groups[1].Value;
                        return cmd.XArgs.TryGetValue(key, out var val)
                            ? $"-x {key}={val}"
                            : m.Value;
                    });

                    // dotnet 块的最后一行末尾无 ^
                    if (!line.TrimEnd().EndsWith('^'))
                        inDotnet = false;

                    result.Add(newLine);
                    continue;
                }

                if (inDotnet && !line.TrimEnd().EndsWith('^'))
                    inDotnet = false;

                result.Add(line);
            }

            File.WriteAllLines(config.BatFilePath, result);
        }
    }
}
