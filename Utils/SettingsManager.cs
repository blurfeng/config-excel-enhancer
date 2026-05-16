using System.Text.Json;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Utils
{
    public static class SettingsManager
    {
        private static readonly string SettingsPath = Path.Combine(
            AppContext.BaseDirectory, "settings.json");

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                    if (!json.Contains("\"HideEnumDataSheet\"", StringComparison.Ordinal))
                        settings.HideEnumDataSheet = true;
                    return settings;
                }
            }
            catch { }
            return new AppSettings();
        }

        public static void Save(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(SettingsPath, json);
        }
    }
}
