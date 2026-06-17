using System.Text.Json;

namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 统一的 JSON 文件读写工具。集中处理序列化选项、UTF-8（无 BOM）编码与异常容错，
    /// 供 <see cref="SettingsManager"/> / <see cref="LocalStateManager"/> 等持久化类复用，
    /// 避免各处重复维护 <see cref="JsonSerializerOptions"/> 与 Load/Save 模板代码。
    /// </summary>
    public static class JsonFileHelper
    {
        // 所有持久化文件统一采用缩进格式，便于手动编辑。
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// 从 JSON 文件加载对象。
        /// <list type="bullet">
        ///   <item>文件不存在时返回 <paramref name="defaultFactory"/>() 的结果。</item>
        ///   <item>读取或解析失败时调用 <paramref name="onError"/>（若提供）后返回默认值。</item>
        /// </list>
        /// </summary>
        public static T Load<T>(string path, Func<T> defaultFactory, Action<Exception>? onError = null)
        {
            if (!File.Exists(path))
                return defaultFactory();

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<T>(json) ?? defaultFactory();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return defaultFactory();
            }
        }

        /// <summary>
        /// 将对象序列化为格式化 JSON 并写入文件（<see cref="File.WriteAllText(string, string)"/> 默认 UTF-8 无 BOM）。
        /// 目标目录不存在时自动创建。
        /// </summary>
        public static void Save<T>(string path, T value)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(value, JsonOptions);
            File.WriteAllText(path, json);
        }
    }
}
