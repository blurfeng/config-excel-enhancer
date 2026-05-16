namespace ConfigExcelEnhancer.Models
{
    public class UpdateResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName => Path.GetFileName(FilePath);

        /// <summary>文件中找到的枚举列总数（跨所有 Sheet）</summary>
        public int EnumColumnsFound { get; set; }

        /// <summary>为空单元格填入默认值的数量</summary>
        public int DefaultsFilled { get; set; }

        /// <summary>是否有枚举列被处理（验证规则已同步）</summary>
        public bool WasProcessed => EnumColumnsFound > 0 && !WasSkipped && !HasError;

        /// <summary>是否有实际数据变动（默认值被填入）</summary>
        public bool HasDataChange => DefaultsFilled > 0;

        /// <summary>Enum 定义有变化（选项增删改、新增/移除 Enum）</summary>
        public bool HasSchemaChange { get; set; }

        /// <summary>__enum_data 表的隐藏状态与配置不一致，已被修正</summary>
        public bool HasVisibilityChange { get; set; }

        /// <summary>文件是否实际被写盘（三个变更标志之一为真时触发）</summary>
        public bool WasSaved { get; set; }

        public bool WasSkipped { get; set; }
        public string? SkipReason { get; set; }

        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
