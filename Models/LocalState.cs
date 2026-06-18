namespace ConfigExcelEnhancer.Models
{
    /// <summary>
    /// 应用本地状态，持久化到用户 LocalApplicationData 目录下的 local_state.json。
    /// 仅存储不应进入版本控制的临时状态信息。
    /// </summary>
    public class LocalState
    {
        /// <summary>上次执行"一键导出"的时间（包含 Luban 导表 + 导出模板类）。</summary>
        public DateTime? LastExportTime { get; set; }

        /// <summary>
        /// 本机的项目根目录绝对路径（如 GodsClash/ 所在的文件夹）。
        /// 此路径机器私有，存储于 LocalApplicationData，不进入版本控制。
        /// settings.json 中的所有相对路径均以此为基准进行还原。
        /// </summary>
        public string ProjectRoot { get; set; } = string.Empty;

        /// <summary>
        /// 上次关闭程序时的窗口宽度（最大化时记录还原后的尺寸）。未保存过时为 null。
        /// </summary>
        public int? WindowWidth { get; set; }

        /// <summary>
        /// 上次关闭程序时的窗口高度（最大化时记录还原后的尺寸）。未保存过时为 null。
        /// </summary>
        public int? WindowHeight { get; set; }
    }
}
