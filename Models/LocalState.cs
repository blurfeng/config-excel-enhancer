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
    }
}
