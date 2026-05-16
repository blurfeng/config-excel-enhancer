namespace ConfigExcelEnhancer.Models
{
    public class LubanConfig
    {
        public string BatFilePath { get; set; } = string.Empty;

        /// <summary>set VAR=value 形式的变量，如 WORKSPACE, CONF_ROOT</summary>
        public Dictionary<string, string> SetVariables { get; set; } = [];

        /// <summary>bat 文件中每个 dotnet 命令块</summary>
        public List<LubanDotnetCommand> Commands { get; set; } = [];
    }
}
