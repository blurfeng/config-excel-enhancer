using System.Security.Cryptography;
using System.Text;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.Utils
{
    /// <summary>
    /// 负责将 LocalState 持久化到用户 LocalApplicationData 目录下的 local_state.json 文件。
    /// 该文件与项目目录隔离，不会被 Git 追踪。
    /// 为支持「同一台机器上多个项目各自放置本软件 .exe」的场景，存储路径按 .exe 所在目录
    /// 派生唯一子目录，使每个 .exe 位置拥有独立的 local_state.json，互不覆盖。
    /// JSON 读写统一委托给 <see cref="JsonFileHelper"/>。
    /// </summary>
    public static class LocalStateManager
    {
        // ConfigExcelEnhancer 在 LocalApplicationData 下的根目录
        private static readonly string BaseDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ConfigExcelEnhancer");

        // 旧版本固定存储路径（全机器唯一），仅用于升级时的兼容回退读取
        private static readonly string LegacyStatePath = Path.Combine(BaseDir, "local_state.json");

        // 当前 .exe 位置对应的存储路径：BaseDir\<exe目录哈希>\local_state.json
        private static readonly string StatePath = Path.Combine(
            BaseDir, GetExeDirHash(), "local_state.json");

        /// <summary>
        /// 当前 .exe 位置实际写入用的 local_state.json 绝对路径（供「打开所在文件夹」与诊断信息展示）。
        /// 注意：<see cref="Load"/> 在该文件缺失时可能回退读取 <see cref="LegacyStatePath"/>，
        /// 但写入与清除始终针对此路径，故诊断展示以此为准。
        /// </summary>
        public static string FilePath => StatePath;

        /// <summary>
        /// 基于 .exe 所在目录（<see cref="AppContext.BaseDirectory"/>）派生一个稳定的子目录名，
        /// 保证「不同 .exe 位置 → 不同子目录」。路径先归一化（去尾部分隔符 + 转小写，因 Windows
        /// 路径大小写不敏感），再取 SHA256 的前 16 个十六进制字符。
        /// </summary>
        private static string GetExeDirHash()
        {
            var normalized = AppContext.BaseDirectory
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToLowerInvariant();
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
            return Convert.ToHexString(hash).Substring(0, 16).ToLowerInvariant();
        }

        /// <summary>
        /// 从 local_state.json 加载状态。若按 .exe 路径的文件不存在，但存在旧版固定路径文件，
        /// 则回退读取旧文件（一次性兼容迁移）。若均不存在或解析失败，静默返回默认 LocalState。
        /// </summary>
        public static LocalState Load()
        {
            if (!File.Exists(StatePath) && File.Exists(LegacyStatePath))
                return JsonFileHelper.Load(LegacyStatePath, () => new LocalState());

            return JsonFileHelper.Load(StatePath, () => new LocalState());
        }

        /// <summary>
        /// 将状态序列化为格式化 JSON 并写入当前 .exe 位置对应的 local_state.json。
        /// 若目录不存在则自动创建。旧版固定路径文件保持不变。
        /// </summary>
        public static void Save(LocalState state)
            => JsonFileHelper.Save(StatePath, state);

        /// <summary>
        /// 删除当前 .exe 位置对应的 local_state.json，使本机状态恢复默认。
        /// 注意：调用方还需重置内存中持有的 <see cref="LocalState"/> 实例，
        /// 否则程序关闭时会被内存值重新写回磁盘。
        /// </summary>
        public static void Clear()
        {
            try
            {
                if (File.Exists(StatePath))
                    File.Delete(StatePath);
            }
            catch { }
        }
    }
}
