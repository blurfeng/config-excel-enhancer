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

        /// <summary>
        /// 是否强制重写枚举验证规则（忽略 Schema 是否变化）。
        /// 属临时操作性开关，机器私有，故存于本地而非共享 settings.json。
        /// </summary>
        public bool EnumForceRewrite { get; set; } = false;

        /// <summary>
        /// 各导出任务的运行态缓存，键为 <see cref="TemplateExportJob.Id"/>。
        /// 由导出过程自动写入，仅用于下次导出的差异检测，故不入版本控制。
        /// </summary>
        public Dictionary<string, TemplateExportCache> TemplateExportCaches { get; set; } = new();

        /// <summary>
        /// 单独导出模式：单个 XML 来源文件（覆盖通用 XML 文件夹）；空 = 回退通用文件夹。
        /// 属一对一临时操作的当前选择，机器私有，故存于本地而非共享 settings.json。
        /// </summary>
        public string ExcelExportSingleXmlFile { get; set; } = string.Empty;

        /// <summary>单独导出模式：选中的数据类名。属临时操作的当前选择，机器私有。</summary>
        public string ExcelExportSingleClassName { get; set; } = string.Empty;

        /// <summary>单独导出模式：导出 Excel 目标文件夹（文件名按命名规则自动生成，存在则更新、不存在则新建）。属临时操作的当前选择，机器私有。</summary>
        public string ExcelExportSingleTargetFolder { get; set; } = string.Empty;

        /// <summary>
        /// 导出 Excel 文件的类名转换方式：0 = 类名不变，1 = 驼峰（首字母大写），2 = 全小写_下划线。
        /// 属个人操作习惯的当前选择，机器私有，故存于本地而非共享 settings.json。
        /// </summary>
        public int ExcelExportNameConvention { get; set; } = 0;

        /// <summary>导出文件名前缀（三种模式共用）。属个人操作习惯的当前选择，机器私有。</summary>
        public string ExcelExportNamePrefix { get; set; } = string.Empty;

        /// <summary>导出文件名后缀（三种模式共用）。属个人操作习惯的当前选择，机器私有。</summary>
        public string ExcelExportNameSuffix { get; set; } = string.Empty;

        // ── 主页 ──────────────────────────────────────────

        /// <summary>
        /// 自动查找根目录时是否启用模糊匹配（忽略大小写及连字符/下划线）。
        /// 属个人操作习惯的当前选择，机器私有，故存于本地而非共享 settings.json。
        /// </summary>
        public bool FuzzyFindProjectRoot { get; set; } = true;

        /// <summary>
        /// 主页"一键导出"是否包含 Enum 验证步骤。
        /// 属个人操作习惯的当前选择，机器私有，故存于本地而非共享 settings.json。
        /// </summary>
        public bool HomeIncludeEnum { get; set; } = true;

        // ── 枚举验证 ──────────────────────────────────────

        /// <summary>
        /// 配置 Excel 文件选择模式：0 = 目录扫描，1 = 手动列表。
        /// 属临时操作的当前选择，机器私有。
        /// </summary>
        public int EnumExcelMode { get; set; } = 0;

        /// <summary>
        /// 枚举验证"目标 Excel"目录模式下的目录（绝对路径）。
        /// 属临时操作的当前选择，机器私有，故存于本地而非共享 settings.json。
        /// </summary>
        public string ExcelDirectory { get; set; } = string.Empty;

        /// <summary>
        /// 枚举验证"目标 Excel"列表模式下的文件列表（绝对路径）。属临时操作的当前选择，机器私有。
        /// </summary>
        public List<string> EnumExcelFiles { get; set; } = new();

        // ── 表设计 ────────────────────────────────────────

        /// <summary>
        /// 表设计目标文件选择模式：0 = 目录扫描，1 = 手动列表。属临时操作的当前选择，机器私有。
        /// </summary>
        public int TableDesignTargetMode { get; set; } = 0;

        /// <summary>
        /// 表设计目标文件目录（目录模式下使用，绝对路径）。属临时操作的当前选择，机器私有。
        /// </summary>
        public string TableDesignTargetDirectory { get; set; } = string.Empty;

        /// <summary>
        /// 表设计目标文件列表（列表模式下使用，绝对路径）。属临时操作的当前选择，机器私有。
        /// </summary>
        public List<string> TableDesignTargetFiles { get; set; } = new();

        // ── 导出 Excel ────────────────────────────────────────

        /// <summary>
        /// 导出模式：0 = 按列表，1 = 批量导出。属个人操作习惯的当前选择，机器私有。
        /// </summary>
        public int ExcelExportMode { get; set; } = 0;
    }
}
