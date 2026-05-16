using System.Text;
using System.Text.RegularExpressions;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Core
{
    public partial class LubanBatParser
    {
        [GeneratedRegex(@"^(\s*set\s+)(\w+)(=)(.*)$", RegexOptions.IgnoreCase)]
        private static partial Regex SetVarRegex();

        // Matches -x key=value; value stops at whitespace or ^ (line continuation)
        [GeneratedRegex(@"-x\s+([\w.]+)=([^\s^]+)")]
        private static partial Regex XArgRegex();

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

                // set VAR=value
                var setMatch = SetVarRegex().Match(raw);
                if (setMatch.Success)
                {
                    config.SetVariables[setMatch.Groups[2].Value] = setMatch.Groups[4].Value.Trim();
                    i++;
                    continue;
                }

                // echo lines: capture non-separator text as label for the next dotnet block
                if (trimmed.StartsWith("echo ", StringComparison.OrdinalIgnoreCase))
                {
                    var echoText = trimmed[5..].Trim();
                    if (!string.IsNullOrEmpty(echoText) && !echoText.All(c => c is '=' or '-' or ' '))
                        pendingLabel = echoText;
                    i++;
                    continue;
                }

                // dotnet command block
                if (trimmed.StartsWith("dotnet ", StringComparison.OrdinalIgnoreCase))
                {
                    var cmd = new LubanDotnetCommand
                    {
                        Label = pendingLabel ?? $"命令 {config.Commands.Count + 1}"
                    };
                    pendingLabel = null;

                    // Collect all continuation lines into one string
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

                // Replace set variable values
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

                // Track which dotnet command block we're in
                if (trimmed.StartsWith("dotnet ", StringComparison.OrdinalIgnoreCase))
                {
                    cmdIdx++;
                    inDotnet = true;
                }

                // Replace -x values within the current command block
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

                    // Last line of the dotnet block has no trailing ^
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
