namespace ConfigExcelEnhancer.Models
{
    /// <summary>
    /// 单个导出任务（<see cref="TemplateExportJob"/>）的导出运行态快照。
    /// 由导出过程自动写入、仅用于下次导出时的差异检测（如 Ids 改名清理），
    /// 机器私有且频繁变化，故持久化到 local_state.json 而非 settings.json。
    /// 以 <see cref="TemplateExportJob.Id"/> 为键存储于 <see cref="LocalState.TemplateExportCaches"/>。
    /// </summary>
    public class TemplateExportCache
    {
        /// <summary>上次成功导出时使用的 Ids 类名（用于检测重命名）。</summary>
        public string LastExportedIdsClassName { get; set; } = "";

        /// <summary>上次成功导出时使用的 Ids 输出目录。</summary>
        public string LastExportedIdsOutputDirectory { get; set; } = "";

        /// <summary>上次成功导出时拥有的 group 名称列表（用于检测 $type 改名后清除旧 group）。</summary>
        public List<string> LastExportedOwnedGroups { get; set; } = new();
    }
}
