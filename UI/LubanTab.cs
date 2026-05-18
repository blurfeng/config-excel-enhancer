using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;

namespace ConfigExcelEnhancer.UI
{
    public partial class LubanTab : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        private LubanConfig? _config;
        private LubanRunner? _runner;

        private const string BrowseMarker = "...";
        private static readonly Color DirtyBackColor = Color.FromArgb(255, 255, 200);

        // 标记正在填充 grid，抑制 CellValueChanged 事件
        private bool _isPopulating;
        // 是否有未保存的修改（一旦有改动就置 true，重置/保存后清除）
        private bool _isDirty;

        public event EventHandler<bool>? ExecutionStateChanged;
        private bool _preLockSaveEnabled;
        private bool _preLockResetEnabled;

        public LubanTab()
        {
            InitializeComponent();
        }

        public void LoadFromSettings()
        {
            txtBatPath.Text = Settings.GenBatPath;
            if (File.Exists(Settings.GenBatPath))
                LoadBatConfig(Settings.GenBatPath);
        }

        private void btnBrowseBat_Click(object sender, EventArgs e)
        {
            var files = DialogHelper.BrowseFiles(
                "选择 gen.bat",
                "批处理文件 (*.bat)|*.bat|所有文件 (*.*)|*.*",
                Settings.GenBatPath);
            if (files.Length > 0)
            {
                txtBatPath.Text = files[0];
                Settings.GenBatPath = files[0];
                LoadBatConfig(files[0]);
            }
        }

        private void LoadBatConfig(string batPath)
        {
            try
            {
                _config = LubanBatParser.Parse(batPath);
                PopulateTabs();
                Log($"已加载：{batPath}（{_config.Commands.Count} 个导出命令）", LogLevel.Ok);
            }
            catch (Exception ex)
            {
                Log($"读取 gen.bat 失败：{ex.Message}", LogLevel.Error);
            }
        }

        // ── Tab 构建 ──────────────────────────────────────────

        private void PopulateTabs()
        {
            if (_config == null) return;

            _isPopulating = true;
            tabsCommands.TabPages.Clear();

            var batDir = BatDir();
            var workspaceRaw = _config.SetVariables.TryGetValue("WORKSPACE", out var ws) ? ws : "";
            var workspace = ResolveToAbsPath(workspaceRaw, batDir);

            // 全局变量 tab
            var tabGlobal = new TabPage("全局变量");
            var gridGlobal = CreateGrid(allowEdit: false);
            gridGlobal.Tag = "global";
            foreach (var kv in _config.SetVariables)
            {
                var displayVal = FormatSetVarForDisplay(kv.Key, kv.Value, batDir, workspace);
                var rowIdx = gridGlobal.Rows.Add(kv.Key, displayVal);
                var valCell = gridGlobal.Rows[rowIdx].Cells["colValue"];
                valCell.Tag = displayVal;
                gridGlobal.Rows[rowIdx].Cells["colBrowse"].Value = IsPathLike(kv.Key, kv.Value) ? BrowseMarker : "";
            }
            SubscribeGridEvents(gridGlobal);
            tabGlobal.Controls.Add(gridGlobal);
            tabsCommands.TabPages.Add(tabGlobal);

            // 每个 dotnet 命令一个 tab
            for (int ci = 0; ci < _config.Commands.Count; ci++)
            {
                var cmd = _config.Commands[ci];
                var tabTitle = cmd.Label.Length > 22 ? cmd.Label[..22] + "…" : cmd.Label;
                var tab = new TabPage(tabTitle) { ToolTipText = cmd.Label };

                var pnl = new Panel { Dock = DockStyle.Fill };

                var argsText = cmd.Args.Count > 0
                    ? string.Join("   ", cmd.Args.Select(kv => $"-{(kv.Key.Length > 1 ? "-" : "")}{kv.Key} {kv.Value}"))
                    : "（无标准参数）";
                var lblArgs = new Label
                {
                    Text = argsText,
                    AutoSize = false,
                    Dock = DockStyle.Top,
                    Height = 26,
                    Padding = new Padding(4, 5, 0, 0),
                    ForeColor = Color.Gray
                };

                var grid = CreateGrid(allowEdit: true);
                foreach (var kv in cmd.XArgs)
                {
                    var rowIdx = grid.Rows.Add(kv.Key, kv.Value);
                    var keyCell = grid.Rows[rowIdx].Cells["colKey"];
                    var valCell = grid.Rows[rowIdx].Cells["colValue"];
                    keyCell.Tag = kv.Key;
                    valCell.Tag = kv.Value;
                    grid.Rows[rowIdx].Cells["colBrowse"].Value = IsPathLike(kv.Key, kv.Value) ? BrowseMarker : "";
                }
                SubscribeGridEvents(grid);
                grid.Dock = DockStyle.Fill;

                pnl.Controls.Add(grid);
                pnl.Controls.Add(lblArgs);
                tab.Controls.Add(pnl);
                tabsCommands.TabPages.Add(tab);
            }

            _isPopulating = false;
            _isDirty = false;
            UpdateButtonStates();
        }

        private void SubscribeGridEvents(DataGridView grid)
        {
            grid.CellClick += OnGridCellClick;
            grid.CellValueChanged += OnGridCellValueChanged;
            grid.UserDeletedRow += OnGridUserDeletedRow;
            grid.UserAddedRow += OnGridUserAddedRow;
        }

        private static DataGridView CreateGrid(bool allowEdit)
        {
            var colKey = new DataGridViewTextBoxColumn
            {
                HeaderText = "键",
                Name = "colKey",
                ReadOnly = !allowEdit,
                FillWeight = 38f
            };
            var colValue = new DataGridViewTextBoxColumn
            {
                HeaderText = "值",
                Name = "colValue",
                FillWeight = 54f
            };
            var colBrowse = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Name = "colBrowse",
                UseColumnTextForButtonValue = false,
                MinimumWidth = 38,
                FillWeight = 8f
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = allowEdit,
                AllowUserToDeleteRows = allowEdit,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2
            };
            grid.Columns.AddRange(colKey, colValue, colBrowse);
            return grid;
        }

        // ── Dirty 追踪 ────────────────────────────────────────

        private void OnGridCellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isPopulating || e.RowIndex < 0 || sender is not DataGridView grid) return;
            var colName = grid.Columns[e.ColumnIndex].Name;
            if (colName != "colKey" && colName != "colValue") return;

            var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            RefreshCellDirtyVisual(cell);
            _isDirty = true;
            UpdateButtonStates();
        }

        private void OnGridUserDeletedRow(object? sender, DataGridViewRowEventArgs e)
        {
            if (_isPopulating) return;
            _isDirty = true;
            UpdateButtonStates();
        }

        private void OnGridUserAddedRow(object? sender, DataGridViewRowEventArgs e)
        {
            if (_isPopulating) return;
            _isDirty = true;
            UpdateButtonStates();
        }

        private static void RefreshCellDirtyVisual(DataGridViewCell cell)
        {
            var original = cell.Tag?.ToString();
            var current = cell.Value?.ToString() ?? "";
            bool isDirty = original == null ? !string.IsNullOrEmpty(current) : current != original;
            cell.Style.BackColor = isDirty ? DirtyBackColor : SystemColors.Window;
        }

        private void UpdateButtonStates()
        {
            btnSave.Enabled = _isDirty;
            btnReset.Enabled = _isDirty;
        }

        // ── 目录 / 文件浏览 ───────────────────────────────────

        private void OnGridCellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || sender is not DataGridView grid) return;
            if (grid.Columns[e.ColumnIndex].Name != "colBrowse") return;
            if (grid.Rows[e.RowIndex].Cells["colBrowse"].Value?.ToString() != BrowseMarker) return;

            var keyCell = grid.Rows[e.RowIndex].Cells["colKey"];
            var valCell = grid.Rows[e.RowIndex].Cells["colValue"];
            var key = keyCell.Value?.ToString() ?? "";
            var currentVal = valCell.Value?.ToString() ?? "";

            bool isGlobal = grid.Tag?.ToString() == "global";
            bool isWorkspaceRow = isGlobal && key.Equals("WORKSPACE", StringComparison.OrdinalIgnoreCase);
            // 含 %WORKSPACE% 变量引用的行（LUBAN_DLL、CONF_ROOT 及 -x 参数）→ 校验后用 %WORKSPACE%\... 表示
            bool useWorkspaceNotation = !isGlobal
                || currentVal.StartsWith("%WORKSPACE%", StringComparison.OrdinalIgnoreCase);

            var workspace = GetWorkspacePath();
            var batDir = BatDir();

            // 解析当前值为绝对路径
            var resolvedVal = currentVal;
            if (!string.IsNullOrEmpty(workspace))
                resolvedVal = resolvedVal.Replace("%WORKSPACE%", workspace, StringComparison.OrdinalIgnoreCase);
            resolvedVal = ResolveToAbsPath(resolvedVal, batDir);

            // 如果解析结果是文件路径（有扩展名），用 OpenFileDialog 选文件
            var ext = Path.GetExtension(resolvedVal);
            if (!string.IsNullOrEmpty(ext) && !Directory.Exists(resolvedVal))
            {
                using var fileDlg = new OpenFileDialog
                {
                    Title = $"选择文件：{key}",
                    Filter = $"{ext.TrimStart('.').ToUpper()} 文件 (*{ext})|*{ext}|所有文件 (*.*)|*.*"
                };
                var fileDir = Path.GetDirectoryName(resolvedVal) ?? batDir;
                if (Directory.Exists(fileDir)) fileDlg.InitialDirectory = fileDir;
                if (File.Exists(resolvedVal)) fileDlg.FileName = Path.GetFileName(resolvedVal);

                if (fileDlg.ShowDialog() != DialogResult.OK) return;

                var selectedFile = Path.GetFullPath(fileDlg.FileName);
                valCell.Value = ToWorkspaceRelative(selectedFile, workspace) ?? selectedFile;
                ApplyCellChange(valCell);
                return;
            }

            // 文件夹浏览
            using var dlg = new FolderBrowserDialog { Description = $"选择目录：{key}" };
            var dialogStart = Directory.Exists(resolvedVal)
                ? resolvedVal
                : Path.GetDirectoryName(resolvedVal) ?? "";
            if (Directory.Exists(dialogStart))
                dlg.SelectedPath = dialogStart;
            else if (!string.IsNullOrEmpty(workspace) && Directory.Exists(workspace))
                dlg.SelectedPath = workspace;
            else if (Directory.Exists(batDir))
                dlg.SelectedPath = batDir;

            if (dlg.ShowDialog() != DialogResult.OK) return;

            var selected = Path.GetFullPath(dlg.SelectedPath);

            if (!useWorkspaceNotation)
            {
                // WORKSPACE 行：直接写绝对路径，保存时转回相对路径
                valCell.Value = selected;
            }
            else
            {
                // 其余行：验证在 WORKSPACE 下，存为 %WORKSPACE%\... 格式
                var wsPath = ToWorkspaceRelative(selected, workspace);
                if (wsPath == null)
                {
                    Log($"所选目录不在 WORKSPACE（{workspace}）下，已取消。", LogLevel.Warn);
                    return;
                }
                valCell.Value = wsPath;
            }

            ApplyCellChange(valCell);
        }

        /// <summary>将绝对路径转为 %WORKSPACE%\子路径；不在 workspace 下则返回 null。</summary>
        private static string? ToWorkspaceRelative(string absPath, string workspace)
        {
            if (string.IsNullOrEmpty(workspace)) return null;
            var wsFull = Path.GetFullPath(workspace).TrimEnd(Path.DirectorySeparatorChar);
            if (absPath.TrimEnd(Path.DirectorySeparatorChar).Equals(wsFull, StringComparison.OrdinalIgnoreCase))
                return "%WORKSPACE%";
            if (absPath.StartsWith(wsFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                var rel = Path.GetRelativePath(wsFull, absPath);
                return $"%WORKSPACE%\\{rel}";
            }
            return null;
        }

        private void ApplyCellChange(DataGridViewCell cell)
        {
            RefreshCellDirtyVisual(cell);
            _isDirty = true;
            UpdateButtonStates();
        }

        // ── 保存 / 重置 ───────────────────────────────────────

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_config == null) { Log("未加载 gen.bat。", LogLevel.Warn); return; }

            ReadFromGrids();
            try
            {
                LubanBatParser.Save(_config);
                PopulateTabs(); // 重置 dirty 状态
                Log("配置已写回 gen.bat。", LogLevel.Ok);
            }
            catch (Exception ex)
            {
                Log($"保存失败：{ex.Message}", LogLevel.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            PopulateTabs();
            Log("已重置所有修改。", LogLevel.Ok);
        }

        private void ReadFromGrids()
        {
            if (_config == null || tabsCommands.TabPages.Count == 0) return;

            var batDir = BatDir();
            var globalGrid = FindGridInTab(0);

            // 先读 WORKSPACE 当前显示值（绝对路径）
            var workspace = string.Empty;
            if (globalGrid != null)
            {
                foreach (DataGridViewRow row in globalGrid.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (string.Equals(row.Cells["colKey"].Value?.ToString(), "WORKSPACE",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        workspace = row.Cells["colValue"].Value?.ToString() ?? "";
                        break;
                    }
                }
            }

            // 全局变量
            if (globalGrid != null)
            {
                _config.SetVariables.Clear();
                foreach (DataGridViewRow row in globalGrid.Rows)
                {
                    if (row.IsNewRow) continue;
                    var key = row.Cells["colKey"].Value?.ToString() ?? "";
                    var val = row.Cells["colValue"].Value?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(key))
                        _config.SetVariables[key] = ConvertSetVarForSave(key, val, batDir, workspace);
                }
            }

            // dotnet 命令块
            for (int ci = 0; ci < _config.Commands.Count && ci + 1 < tabsCommands.TabPages.Count; ci++)
            {
                var grid = FindGridInTab(ci + 1);
                if (grid == null) continue;

                _config.Commands[ci].XArgs.Clear();
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    var key = row.Cells["colKey"].Value?.ToString() ?? "";
                    var val = row.Cells["colValue"].Value?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(key))
                        _config.Commands[ci].XArgs[key] = val;
                }
            }
        }

        private DataGridView? FindGridInTab(int tabIndex)
        {
            if (tabIndex >= tabsCommands.TabPages.Count) return null;
            var tab = tabsCommands.TabPages[tabIndex];
            return tab.Controls.OfType<DataGridView>().FirstOrDefault()
                ?? tab.Controls.OfType<Panel>().FirstOrDefault()
                       ?.Controls.OfType<DataGridView>().FirstOrDefault();
        }

        // ── 执行导表 ──────────────────────────────────────────

        private void SetUILocked(bool locked)
        {
            if (locked)
            {
                _preLockSaveEnabled = btnSave.Enabled;
                _preLockResetEnabled = btnReset.Enabled;
                pnlTop.Enabled = false;
                tabsCommands.Enabled = false;
                btnSave.Enabled = false;
                btnReset.Enabled = false;
            }
            else
            {
                pnlTop.Enabled = true;
                tabsCommands.Enabled = true;
                btnSave.Enabled = _preLockSaveEnabled;
                btnReset.Enabled = _preLockResetEnabled;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.GenBatPath) || !File.Exists(Settings.GenBatPath))
            {
                Log("请先选择有效的 gen.bat 路径。", LogLevel.Error);
                return;
            }

            SetUILocked(true);
            ExecutionStateChanged?.Invoke(this, true);
            btnRun.Enabled = false;
            btnCancel.Enabled = true;
            int cmdCount = _config?.Commands.Count ?? 1;
            pbRun.Maximum = 1000;
            pbRun.Value = 100;
            pbRun.Visible = true;
            txtLog.Clear();
            Log("开始执行导表...", LogLevel.Info);

            // 按行数将 10%→90% 线性分配给所有命令的输出
            const int avgLines = 20;
            int totalEstLines = cmdCount * avgLines;
            int _lineCount = 0;
            bool _hasError = false;
            _runner = new LubanRunner();
            _runner.OutputReceived += msg => BeginInvoke(() =>
            {
                bool isError = msg.Contains("|ERROR|");
                if (isError) _hasError = true;
                LogLibrary.WriteRaw(txtLog, msg, isError ? LogLibrary.ClrError : LogLibrary.ClrOk);
                _lineCount++;
                int v = totalEstLines > 0 ? 100 + (int)(_lineCount * 800.0 / totalEstLines) : 900;
                v = Math.Min(v, 900);
                if (v > pbRun.Value)
                    pbRun.Value = v;
            });
            _runner.Finished += code => BeginInvoke(() =>
            {
                if (code == 0 && !_hasError)
                    Log("导表完成。", LogLevel.Ok);
                else
                    Log($"导表失败，退出码：{code}", LogLevel.Error);
                pbRun.Value = 1000;
                SetUILocked(false);
                ExecutionStateChanged?.Invoke(this, false);
                btnRun.Enabled = true;
                btnCancel.Enabled = false;
                pbRun.Visible = false;
                _runner = null;
                Log("─ 结束 ─", LogLevel.Info);
                LogDivider();
            });

            try { _runner.Run(Settings.GenBatPath); }
            catch (Exception ex)
            {
                Log($"启动失败：{ex.Message}", LogLevel.Error);
                SetUILocked(false);
                ExecutionStateChanged?.Invoke(this, false);
                btnRun.Enabled = true;
                btnCancel.Enabled = false;
                pbRun.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _runner?.Cancel();
            Log("已发送终止信号。", LogLevel.Warn);
        }

        // ── 工具方法 ──────────────────────────────────────────

        private string GetWorkspacePath()
        {
            var globalGrid = FindGridInTab(0);
            if (globalGrid != null)
            {
                foreach (DataGridViewRow row in globalGrid.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (string.Equals(row.Cells["colKey"].Value?.ToString(), "WORKSPACE",
                            StringComparison.OrdinalIgnoreCase))
                        return row.Cells["colValue"].Value?.ToString() ?? "";
                }
            }
            if (_config?.SetVariables.TryGetValue("WORKSPACE", out var raw) == true)
                return ResolveToAbsPath(raw, BatDir());
            return string.Empty;
        }

        private string BatDir() =>
            string.IsNullOrEmpty(_config?.BatFilePath) ? "" : Path.GetDirectoryName(_config.BatFilePath) ?? "";

        /// <summary>
        /// set 变量显示格式：WORKSPACE → 绝对路径；其余 → 已含 %WORKSPACE% 原样，否则转为 %WORKSPACE%\子路径。
        /// </summary>
        private static string FormatSetVarForDisplay(string key, string value, string batDir, string workspace)
        {
            if (key.Equals("WORKSPACE", StringComparison.OrdinalIgnoreCase))
                return ResolveToAbsPath(value, batDir);

            if (value.StartsWith("%WORKSPACE%", StringComparison.OrdinalIgnoreCase))
                return value;

            // 将相对路径（如 . 或 %VAR% 以外的形式）解析为绝对路径，再转为 %WORKSPACE%\子路径
            if (!string.IsNullOrEmpty(workspace))
            {
                var abs = ResolveToAbsPath(value, batDir);
                var wsFull = workspace.TrimEnd(Path.DirectorySeparatorChar);
                if (abs.TrimEnd(Path.DirectorySeparatorChar).Equals(wsFull, StringComparison.OrdinalIgnoreCase))
                    return "%WORKSPACE%";
                if (abs.StartsWith(wsFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    var rel = Path.GetRelativePath(wsFull, abs);
                    return $"%WORKSPACE%\\{rel}";
                }
            }

            return ResolveToAbsPath(value, batDir);
        }

        /// <summary>
        /// 保存前将 UI 值转回 bat 惯用格式：WORKSPACE → 相对路径；含 %WORKSPACE% → 原样；其余绝对路径 → 相对 bat 目录。
        /// </summary>
        private static string ConvertSetVarForSave(string key, string displayValue, string batDir, string workspace)
        {
            if (key.Equals("WORKSPACE", StringComparison.OrdinalIgnoreCase))
            {
                if (Path.IsPathRooted(displayValue) && !string.IsNullOrEmpty(batDir))
                {
                    try { return Path.GetRelativePath(batDir, displayValue); }
                    catch { }
                }
                return displayValue;
            }

            if (displayValue.StartsWith("%WORKSPACE%", StringComparison.OrdinalIgnoreCase))
                return displayValue;

            if (Path.IsPathRooted(displayValue) && !string.IsNullOrEmpty(batDir))
            {
                try { return Path.GetRelativePath(batDir, displayValue); }
                catch { }
            }

            return displayValue;
        }

        private static string ResolveToAbsPath(string value, string basePath)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(basePath)) return value;
            if (Path.IsPathRooted(value)) return value;
            if (value.Contains('%')) return value;
            try { return Path.GetFullPath(Path.Combine(basePath, value)); }
            catch { return value; }
        }

        private static bool IsPathLike(string key, string value) =>
            key.EndsWith("Dir", StringComparison.OrdinalIgnoreCase) ||
            key.EndsWith("Directory", StringComparison.OrdinalIgnoreCase) ||
            key.EndsWith("Path", StringComparison.OrdinalIgnoreCase) ||
            key.Equals("WORKSPACE", StringComparison.OrdinalIgnoreCase) ||
            key.Equals("CONF_ROOT", StringComparison.OrdinalIgnoreCase) ||
            key.EndsWith("DLL", StringComparison.OrdinalIgnoreCase) ||
            value.Contains('%') ||
            value.Contains('\\') || value.Contains('/') ||
            value == "." || value.StartsWith("..");

        private void Log(string message, LogLevel level = LogLevel.Ok)
            => LogLibrary.Write(txtLog, message, level);

        private void LogDivider()
            => LogLibrary.Divider(txtLog);

        private void btnClearLog_Click(object? sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void btnCopyLog_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

        /// <summary>取消当前正在进行的任务（供窗口关闭时调用）。</summary>
        public void CancelRunningTask()
        {
            if (btnCancel.Enabled)
                btnCancel_Click(this, EventArgs.Empty);
        }
    }
}
