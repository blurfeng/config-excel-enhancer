using System.Text.Json;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 负责将 AppSettings 持久化到应用目录下的 settings.json 文件。
    /// 支持加载、保存和清除操作；JSON 格式化输出便于手动编辑。
    /// </summary>
    public static class SettingsManager
    {
        // settings.json 固定位于应用程序 exe 所在目录
        private static readonly string SettingsPath = Path.Combine(
            AppContext.BaseDirectory, "settings.json");

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// 从 settings.json 加载设置。若文件不存在则返回默认 AppSettings；
        /// 若文件存在但解析失败，弹出警告框并返回默认设置。
        /// </summary>
        public static AppSettings Load()
        {
            if (!File.Exists(SettingsPath))
                return new AppSettings();

            try
            {
                var json = File.ReadAllText(SettingsPath);
                // System.Text.Json 遇到缺失字段时保留属性类定义的默认值，
                // 因此 HideEnumDataSheet 等字段即使不在旧版 JSON 中也会正确取 true。
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"settings.json 加载失败，将使用默认设置。\n\n{ex.Message}",
                    "配置加载失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return new AppSettings();
            }
        }

        /// <summary>
        /// 将设置序列化为格式化 JSON 并写入 settings.json。
        /// </summary>
        public static void Save(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(SettingsPath, json);
        }

        /// <summary>
        /// 删除 settings.json，使下次启动时恢复默认设置。
        /// </summary>
        public static void Clear()
        {
            try
            {
                if (File.Exists(SettingsPath))
                    File.Delete(SettingsPath);
            }
            catch { }
        }
    }
}
