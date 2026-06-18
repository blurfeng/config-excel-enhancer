using ClosedXML.Excel;

namespace ConfigExcelEnhancer.Core
{
    /// <summary>单个 Excel 重命名计划项的状态。</summary>
    public enum RenameStatus
    {
        /// <summary>需要重命名。</summary>
        Rename,
        /// <summary>已是期望文件名，无需操作。</summary>
        AlreadyNamed,
        /// <summary>目标名冲突（被其它文件占用，或多个类算出同名），跳过。</summary>
        Conflict,
        /// <summary>无法确定类名，跳过。</summary>
        Unresolved,
    }

    /// <summary>单个 Excel 的重命名计划项。</summary>
    public record RenamePlanItem(string OldPath, string NewPath, string ClassName, RenameStatus Status, string? Note);

    /// <summary>
    /// 根据命名设置把磁盘上的目标 Excel 重命名为期望文件名（仅改文件名、同目录）。
    /// 纯逻辑、无 UI 依赖：候选对（类名 → 旧路径）由调用方按模式构造。
    /// </summary>
    public static class ExcelRenamer
    {
        /// <summary>
        /// 读取 Excel 内嵌的数据类名：扫描首个工作表 A 列前若干行找 ##type 标记行，
        /// 返回该行 B 列值（对应 <see cref="ExcelExporter"/> 写入的 className）。
        /// 打不开或找不到返回 null。
        /// </summary>
        public static string? TryReadEmbeddedClassName(string xlsxPath)
        {
            try
            {
                using var wb = new XLWorkbook(xlsxPath);
                var ws = wb.Worksheets.First();
                int lastRow = Math.Min(ws.LastRowUsed()?.RowNumber() ?? 0, 10);
                for (int r = 1; r <= lastRow; r++)
                {
                    if (ws.Cell(r, 1).GetString().Trim() == "##type")
                    {
                        var name = ws.Cell(r, 2).GetString().Trim();
                        return string.IsNullOrEmpty(name) ? null : name;
                    }
                }
            }
            catch
            {
                // 文件损坏/被占用/非法格式：视为无法读取。
            }
            return null;
        }

        /// <summary>
        /// 根据候选对（类名, 旧路径）构建重命名计划。
        /// 旧路径不存在的不入计划；类名为空标 Unresolved；目标名等价（含大小写）标 AlreadyNamed；
        /// 目标名被其它文件占用或计划内重复标 Conflict；其余标 Rename。
        /// </summary>
        public static IReadOnlyList<RenamePlanItem> BuildPlan(
            IEnumerable<(string ClassName, string OldPath)> candidates,
            int convention, string prefix, string suffix)
        {
            var items = new List<RenamePlanItem>();

            foreach (var (className, oldPath) in candidates)
            {
                if (string.IsNullOrEmpty(oldPath) || !File.Exists(oldPath))
                    continue;

                if (string.IsNullOrEmpty(className))
                {
                    items.Add(new RenamePlanItem(oldPath, oldPath, className, RenameStatus.Unresolved, "无法确定数据类名"));
                    continue;
                }

                string dir     = Path.GetDirectoryName(oldPath) ?? string.Empty;
                string newName  = FunctionLibrary.BuildExcelFileName(className, convention, prefix, suffix);
                string newPath  = Path.Combine(dir, newName);

                // 完全等价（含大小写）→ 已是期望名；仅大小写不同仍视为需要重命名。
                if (string.Equals(FullPath(oldPath), FullPath(newPath), StringComparison.Ordinal))
                    items.Add(new RenamePlanItem(oldPath, newPath, className, RenameStatus.AlreadyNamed, null));
                else
                    items.Add(new RenamePlanItem(oldPath, newPath, className, RenameStatus.Rename, null));
            }

            // 冲突检测：仅针对待重命名项。
            // 计划内多项目标同名（忽略大小写）。
            var dupTargets = items
                .Where(i => i.Status == RenameStatus.Rename)
                .GroupBy(i => FullPath(i.NewPath), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            for (int idx = 0; idx < items.Count; idx++)
            {
                var it = items[idx];
                if (it.Status != RenameStatus.Rename) continue;

                bool dup = dupTargets.Contains(FullPath(it.NewPath));
                // 目标名已被磁盘上的其它文件占用（排除仅大小写改名的自身）。
                bool diskTaken = File.Exists(it.NewPath) && !PathEquals(it.NewPath, it.OldPath);

                if (dup || diskTaken)
                    items[idx] = it with
                    {
                        Status = RenameStatus.Conflict,
                        Note   = dup ? "多个数据类算出同名" : "目标文件名已被占用",
                    };
            }

            return items;
        }

        /// <summary>
        /// 执行计划中的 Rename 项（其余跳过）。逐项 try/catch，返回成功重命名的（旧路径, 新路径）对，
        /// 供调用方回写关联路径。
        /// </summary>
        public static List<(string OldPath, string NewPath)> Execute(
            IReadOnlyList<RenamePlanItem> plan, Action<string, LogLevel> log)
        {
            var renamed = new List<(string, string)>();

            foreach (var item in plan)
            {
                if (item.Status != RenameStatus.Rename) continue;

                string oldName = Path.GetFileName(item.OldPath);
                string newName = Path.GetFileName(item.NewPath);
                try
                {
                    // 仅大小写不同：在不分大小写的文件系统上 File.Move 可能报目标已存在，改用两步移动。
                    if (PathEquals(item.OldPath, item.NewPath))
                    {
                        string tmp = item.NewPath + ".rntmp";
                        File.Move(item.OldPath, tmp);
                        File.Move(tmp, item.NewPath);
                    }
                    else
                    {
                        File.Move(item.OldPath, item.NewPath);
                    }

                    log($"重命名：{oldName} → {newName}", LogLevel.Ok);
                    renamed.Add((item.OldPath, item.NewPath));
                }
                catch (IOException)
                {
                    log($"文件被占用，跳过：{oldName}", LogLevel.Warn);
                }
                catch (Exception ex)
                {
                    log($"重命名失败 {oldName}：{LogLibrary.FormatException(ex)}", LogLevel.Error);
                }
            }

            return renamed;
        }

        private static string FullPath(string path)
        {
            try { return Path.GetFullPath(path); }
            catch { return path; }
        }

        /// <summary>规范化后比较两个路径是否指向同一文件（忽略大小写）。</summary>
        private static bool PathEquals(string a, string b)
            => string.Equals(FullPath(a), FullPath(b), StringComparison.OrdinalIgnoreCase);
    }
}
