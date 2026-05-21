using System.Text.Json;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 负责将 LocalState 持久化到用户 LocalApplicationData 目录下的 local_state.json 文件。
    /// 该文件与项目目录隔离，不会被 Git 追踪。
    /// </summary>
    public static class LocalStateManager
    {
        // local_state.json 存储在用户 LocalApplicationData 目录
        private static readonly string StatePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ConfigExcelEnhancer",
            "local_state.json");

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// 从 local_state.json 加载状态。若文件不存在或解析失败，静默返回默认 LocalState。
        /// </summary>
        public static LocalState Load()
        {
            if (!File.Exists(StatePath))
                return new LocalState();

            try
            {
                var json = File.ReadAllText(StatePath);
                return JsonSerializer.Deserialize<LocalState>(json) ?? new LocalState();
            }
            catch
            {
                return new LocalState();
            }
        }

        /// <summary>
        /// 将状态序列化为格式化 JSON 并写入 local_state.json。
        /// 若目录不存在则自动创建。
        /// </summary>
        public static void Save(LocalState state)
        {
            var directory = Path.GetDirectoryName(StatePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(state, JsonOptions);
            File.WriteAllText(StatePath, json);
        }
    }
}
