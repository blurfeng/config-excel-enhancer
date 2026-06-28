using System.ComponentModel;
using ConfigExcelEnhancer.Core;
using ConfigExcelEnhancer.Models;
using ConfigExcelEnhancer.Utils;

namespace ConfigExcelEnhancer.UI
{
    public partial class LubanTab : TabBase
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppSettings Settings { get; set; } = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LocalState LocalState { get; set; } = new();

        protected override RichTextBox? LogBox => txtLog;

        protected override string? GreetingMessage => "Luban 导表已就绪 — 配置并执行 Luban 导出脚本。";

        private LubanConfig? _config;
        private LubanRunner? _runner;

        private const string BrowseMarker = "...";
        // 导出时生成的 gen.bat 临时副本文件名（套用本地覆盖后运行，跑完即删，与 gen.bat 同目录）
        private const string LocalGenBatName = "__cee_local_gen.bat";

        // 当前 gen.bat 的本地路径覆盖工作副本；键见 OverrideKeyForSet/OverrideKeyForXArg，值为注入用的原始路径。
        // 任何改动都即时自动保存到 local_state.json（无需手动保存按钮）。
        private Dictionary<string, string> _overrides = new();
        // 本次导出生成的临时副本路径，结束后清理
        private string? _tempBatPath;

        // 标记正在填充 grid，抑制 CellValueChanged 事件
        private bool _isPopulating;
        // 非路径参数是否有未写入 gen.bat 的修改（一旦有改动就置 true，重置/写入后清除）
        private bool _isDirty;

        private bool _preLockResetEnabled;
        private bool _preLockWriteSharedEnabled;

        // 路径行右键菜单：清除本地覆盖
        private readonly ContextMenuStrip _rowMenu;
        private DataGridView? _ctxGrid;
        private int _ctxRowIndex = -1;

        public LubanTab()
        {
            InitializeComponent();

            _rowMenu = new ContextMenuStrip();
            var clearItem = new ToolStripMenuItem("清除本地覆盖（还原 gen.bat 值）");
            clearItem.Click += OnClearOverrideClick;
            _rowMenu.Items.Add(clearItem);
        }

        public void LoadFromSettings()
        {
            txtBatPath.Text = Settings.GenBatPath;
            if (File.Exists(Settings.GenBatPath))
                LoadBatConfig(Settings.GenBatPath);
        }

        /// <summary>获取当前已加载的 Luban 配置（用于外部检查配置一致性）。</summary>
        public LubanConfig? GetCurrentConfig() => _config;

        private void btnOpenBat_Click(object sender, EventArgs e)
            => ExplorerHelper.RevealInExplorer(Settings.GenBatPath);

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
                Log($"读取 gen.bat 失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
            }
        }

        // ── Tab 构建 ──────────────────────────────────────────

        private void PopulateTabs()
        {
            if (_config == null) return;

            _isPopulating = true;
            tabsCommands.TabPages.Clear();
            LoadOverridesForCurrentBat();

            var batDir = BatDir();
            var workspace = ResolveToAbsPath(EffectiveSetVar("WORKSPACE"), batDir);

            // 全局变量 tab
            var tabGlobal = new TabPage("全局变量");
            var gridGlobal = CreateGrid(allowEdit: false);
            gridGlobal.Tag = "global";
            foreach (var kv in _config.SetVariables)
            {
                var ovKey = OverrideKeyForSet(kv.Key);
                bool hasOv = _overrides.TryGetValue(ovKey, out var ovRaw);
                bool pathLike = IsPathLike(kv.Key, kv.Value);
                var displayVal = hasOv ? ovRaw! : FormatSetVarForDisplay(kv.Key, kv.Value, batDir, workspace);
                var rowIdx = gridGlobal.Rows.Add(false, kv.Key, displayVal);
                var valCell = gridGlobal.Rows[rowIdx].Cells["colValue"];
                valCell.Tag = displayVal;
                gridGlobal.Rows[rowIdx].Cells["colBrowse"].Value = pathLike ? BrowseMarker : "";
                if (pathLike) MarkOverrideRow(gridGlobal.Rows[rowIdx], hasOv);
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
                grid.Tag = ci; // 命令序号，供浏览/覆盖定位
                foreach (var kv in cmd.XArgs)
                {
                    var ovKey = OverrideKeyForXArg(ci, kv.Key);
                    bool hasOv = _overrides.TryGetValue(ovKey, out var ovRaw);
                    bool pathLike = IsPathLike(kv.Key, kv.Value);
                    var displayVal = hasOv ? ovRaw! : kv.Value;
                    var rowIdx = grid.Rows.Add(false, kv.Key, displayVal);
                    var keyCell = grid.Rows[rowIdx].Cells["colKey"];
                    var valCell = grid.Rows[rowIdx].Cells["colValue"];
                    keyCell.Tag = kv.Key;
                    valCell.Tag = displayVal;
                    grid.Rows[rowIdx].Cells["colBrowse"].Value = pathLike ? BrowseMarker : "";
                    if (pathLike) MarkOverrideRow(grid.Rows[rowIdx], hasOv);
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
            ValidateAllPathCells();
            UpdateButtonStates();
        }

        private void SubscribeGridEvents(DataGridView grid)
        {
            grid.CellClick += OnGridCellClick;
            grid.CellValueChanged += OnGridCellValueChanged;
            grid.UserDeletedRow += OnGridUserDeletedRow;
            grid.UserAddedRow += OnGridUserAddedRow;
            grid.MouseDown += OnGridMouseDown;
        }

        /// <summary>右键已覆盖的路径行 → 弹出「清除本地覆盖」菜单。</summary>
        private void OnGridMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || sender is not DataGridView grid) return;
            var hit = grid.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0 || grid.Rows[hit.RowIndex].IsNewRow) return;

            var row = grid.Rows[hit.RowIndex];
            var key = row.Cells["colKey"].Value?.ToString() ?? "";
            bool isGlobal = grid.Tag?.ToString() == "global";
            var (ovKey, _, _) = ResolveRowBaseline(grid, key, isGlobal);
            if (!_overrides.ContainsKey(ovKey)) return; // 仅对存在覆盖的行显示

            _ctxGrid = grid;
            _ctxRowIndex = hit.RowIndex;
            grid.ClearSelection();
            row.Selected = true;
            _rowMenu.Show(grid, e.Location);
        }

        private void OnClearOverrideClick(object? sender, EventArgs e)
        {
            if (_ctxGrid == null || _ctxRowIndex < 0 || _ctxRowIndex >= _ctxGrid.Rows.Count) return;
            var grid = _ctxGrid;
            var row = grid.Rows[_ctxRowIndex];
            var key = row.Cells["colKey"].Value?.ToString() ?? "";
            bool isGlobal = grid.Tag?.ToString() == "global";
            var (ovKey, _, baselineDisplay) = ResolveRowBaseline(grid, key, isGlobal);

            _overrides.Remove(ovKey);
            _isPopulating = true; // 避免编程赋值触发 CellValueChanged
            row.Cells["colValue"].Value = baselineDisplay;
            _isPopulating = false;
            MarkOverrideRow(row, false);
            PersistOverrides();
            ValidateAllPathCells();
            UpdateButtonStates();
        }

        private static DataGridView CreateGrid(bool allowEdit)
        {
            var colDirty = new DataGridViewCheckBoxColumn
            {
                HeaderText = "本地配置",
                Name = "colDirty",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 64,
                Resizable = DataGridViewTriState.False,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
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
            grid.Columns.AddRange(colDirty, colKey, colValue, colBrowse);
            return grid;
        }

        // ── 本地路径覆盖 ──────────────────────────────────────

        private static string OverrideKeyForSet(string varName) => $"set:{varName}";
        private static string OverrideKeyForXArg(int cmdIndex, string xKey) => $"x:{cmdIndex}:{xKey}";

        /// <summary>从 LocalState 载入当前 gen.bat 的覆盖工作副本。</summary>
        private void LoadOverridesForCurrentBat()
        {
            _overrides = LocalState.LubanBatOverrides.TryGetValue(Settings.GenBatPath, out var d)
                ? new Dictionary<string, string>(d)
                : new Dictionary<string, string>();
        }

        /// <summary>取 set 变量的有效原始值（有本地覆盖用覆盖，否则用 gen.bat 基线）。</summary>
        private string EffectiveSetVar(string name)
        {
            if (_overrides.TryGetValue(OverrideKeyForSet(name), out var ov)) return ov;
            return _config != null && _config.SetVariables.TryGetValue(name, out var v) ? v : "";
        }

        /// <summary>当前有效 WORKSPACE 的绝对路径（考虑本地覆盖），用于解析其它路径与校验。</summary>
        private string EffectiveWorkspaceAbs() => ResolveToAbsPath(EffectiveSetVar("WORKSPACE"), BatDir());

        /// <summary>
        /// 判断解析后的路径在本机是否可用：文件按是否存在判定；目录存在、或其父目录存在（输出目录可由导出时创建）即视为可用。
        /// </summary>
        private static bool IsPathAvailable(string absPath)
        {
            if (string.IsNullOrWhiteSpace(absPath)) return false;
            if (absPath.Contains('%')) return false; // 仍含未展开的变量引用 → 视为不可用
            try
            {
                bool looksFile = !string.IsNullOrEmpty(Path.GetExtension(absPath));
                if (looksFile) return File.Exists(absPath);
                if (Directory.Exists(absPath)) return true;
                var parent = Path.GetDirectoryName(absPath);
                return !string.IsNullOrEmpty(parent) && Directory.Exists(parent);
            }
            catch { return false; }
        }

        /// <summary>按「值」列当前显示值解析路径，不可用时单元格底色标红。</summary>
        private void ApplyPathCellValidation(DataGridViewRow row)
        {
            var valCell = row.Cells["colValue"];
            var disp = valCell.Value?.ToString() ?? "";
            var abs = ResolveRaw(disp, BatDir(), EffectiveWorkspaceAbs());
            bool ok = IsPathAvailable(abs);
            valCell.Style.BackColor = ok ? SystemColors.Window : UITheme.CellErrorBack;
            valCell.Style.SelectionBackColor = ok ? SystemColors.Highlight : UITheme.CellErrorSel;
        }

        /// <summary>校验所有路径行（值变更可能影响 WORKSPACE，从而影响其它行，故整体重算）。</summary>
        private void ValidateAllPathCells()
        {
            for (int t = 0; t < tabsCommands.TabPages.Count; t++)
            {
                var grid = FindGridInTab(t);
                if (grid == null) continue;
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells["colBrowse"].Value?.ToString() == BrowseMarker)
                        ApplyPathCellValidation(row);
                }
            }
        }

        /// <summary>解析某路径行的覆盖键、gen.bat 基线原始值与基线显示值。</summary>
        private (string ovKey, string baselineRaw, string baselineDisplay) ResolveRowBaseline(
            DataGridView grid, string key, bool isGlobal)
        {
            var batDir = BatDir();
            var workspace = GetWorkspacePath();
            if (isGlobal)
            {
                var raw = _config != null && _config.SetVariables.TryGetValue(key, out var b) ? b : "";
                return (OverrideKeyForSet(key), raw, FormatSetVarForDisplay(key, raw, batDir, workspace));
            }
            int ci = Convert.ToInt32(grid.Tag);
            var rawx = _config != null && ci >= 0 && ci < _config.Commands.Count
                       && _config.Commands[ci].XArgs.TryGetValue(key, out var bx) ? bx : "";
            return (OverrideKeyForXArg(ci, key), rawx, rawx);
        }

        /// <summary>标记路径行是否处于本地覆盖状态（仅勾选「本地配置」列，不改背景色）。</summary>
        private static void MarkOverrideRow(DataGridViewRow row, bool hasOverride)
        {
            if (row.Cells["colDirty"] is DataGridViewCheckBoxCell c) c.Value = hasOverride;
        }

        /// <summary>将 %WORKSPACE% 展开并解析为绝对路径（用于和浏览所选路径比对是否等于基线）。</summary>
        private static string ResolveRaw(string raw, string batDir, string workspace)
        {
            if (string.IsNullOrEmpty(raw)) return raw;
            var v = string.IsNullOrEmpty(workspace)
                ? raw
                : raw.Replace("%WORKSPACE%", workspace, StringComparison.OrdinalIgnoreCase);
            return ResolveToAbsPath(v, batDir);
        }

        /// <summary>深拷贝配置并套用覆盖（set 变量名 / 命令序号+键），用于生成导出临时副本。</summary>
        private static LubanConfig CloneConfigWithOverrides(LubanConfig src, Dictionary<string, string> overrides)
        {
            var clone = new LubanConfig
            {
                BatFilePath = src.BatFilePath,
                SetVariables = new Dictionary<string, string>(src.SetVariables),
                Commands = src.Commands.Select(cmd => new LubanDotnetCommand
                {
                    Label = cmd.Label,
                    Args = new Dictionary<string, string>(cmd.Args),
                    XArgs = new Dictionary<string, string>(cmd.XArgs)
                }).ToList()
            };

            foreach (var kv in overrides)
            {
                if (kv.Key.StartsWith("set:", StringComparison.Ordinal))
                {
                    var name = kv.Key[4..];
                    if (clone.SetVariables.ContainsKey(name)) clone.SetVariables[name] = kv.Value;
                }
                else if (kv.Key.StartsWith("x:", StringComparison.Ordinal))
                {
                    // 格式 x:{序号}:{键}；键为 [\w.]，不含冒号
                    var rest = kv.Key[2..];
                    var sep = rest.IndexOf(':');
                    if (sep > 0 && int.TryParse(rest[..sep], out var ci)
                        && ci >= 0 && ci < clone.Commands.Count)
                    {
                        var xKey = rest[(sep + 1)..];
                        if (clone.Commands[ci].XArgs.ContainsKey(xKey))
                            clone.Commands[ci].XArgs[xKey] = kv.Value;
                    }
                }
            }
            return clone;
        }

        // ── 非路径参数改动追踪（写回共享 gen.bat 用）────────────

        private void OnGridCellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isPopulating || e.RowIndex < 0 || sender is not DataGridView grid) return;
            var colName = grid.Columns[e.ColumnIndex].Name;
            if (colName != "colKey" && colName != "colValue") return;

            var row = grid.Rows[e.RowIndex];
            bool isPathRow = row.Cells["colBrowse"].Value?.ToString() == BrowseMarker;
            if (isPathRow && colName == "colValue")
            {
                // 路径行手动输入 → 作为本地覆盖处理（与「浏览」一致）
                var key = row.Cells["colKey"].Value?.ToString() ?? "";
                bool isGlobal = grid.Tag?.ToString() == "global";
                var typed = row.Cells["colValue"].Value?.ToString() ?? "";
                ApplyTypedPathOverride(grid, e.RowIndex, isGlobal, key, typed);
                return;
            }

            // 非路径参数的改动不再做行高亮；是否有待写入共享的改动由「写入 gen.bat（共享）」按钮可用状态提示
            _isDirty = true;
            UpdateButtonStates();
        }

        /// <summary>将路径行手动输入的值设为本地覆盖；为空或等于 gen.bat 基线则取消覆盖。</summary>
        private void ApplyTypedPathOverride(DataGridView grid, int rowIndex, bool isGlobal, string key, string typed)
        {
            if (_config == null) return;
            var row = grid.Rows[rowIndex];
            var (ovKey, baselineRaw, baselineDisplay) = ResolveRowBaseline(grid, key, isGlobal);
            var batDir = BatDir();
            var workspace = EffectiveWorkspaceAbs();
            var baselineAbs = ResolveRaw(baselineRaw, batDir, workspace);
            var typedAbs = ResolveRaw(typed, batDir, workspace);

            bool revert = string.IsNullOrWhiteSpace(typed)
                || string.Equals(typed, baselineDisplay, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrEmpty(baselineAbs) && PathEquals(typedAbs, baselineAbs));

            bool hasOv;
            if (revert)
            {
                _overrides.Remove(ovKey);
                hasOv = false;
                _isPopulating = true; // 还原显示为基线值
                row.Cells["colValue"].Value = baselineDisplay;
                _isPopulating = false;
            }
            else
            {
                _overrides[ovKey] = typed;
                hasOv = true;
            }

            MarkOverrideRow(row, hasOv);
            PersistOverrides();
            ValidateAllPathCells();
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

        /// <summary>本地配置是否与 gen.bat 共享配置存在差异（有路径覆盖或未写入的非路径改动）。</summary>
        private bool HasLocalDifferences() => _overrides.Count > 0 || _isDirty;

        private void UpdateButtonStates()
        {
            bool diff = HasLocalDifferences();
            btnReset.Enabled = diff;
            btnWriteShared.Enabled = diff;
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
            var workspace = GetWorkspacePath();
            var batDir = BatDir();

            // 解析当前值为绝对路径，仅用作对话框初始位置
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

                SetOverrideFromBrowse(grid, e.RowIndex, isGlobal, key, Path.GetFullPath(fileDlg.FileName));
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

            SetOverrideFromBrowse(grid, e.RowIndex, isGlobal, key, Path.GetFullPath(dlg.SelectedPath));
        }

        /// <summary>
        /// 将浏览所选的绝对路径设为该路径行的本地覆盖（接受 WORKSPACE 之外的任意路径——这正是要解决的场景）；
        /// 若所选等于 gen.bat 基线值则取消覆盖。覆盖值存绝对路径，作为导出时注入的原始值。
        /// </summary>
        private void SetOverrideFromBrowse(DataGridView grid, int rowIndex, bool isGlobal, string key, string selectedAbs)
        {
            if (_config == null) return;
            var batDir = BatDir();
            var workspace = GetWorkspacePath();
            var row = grid.Rows[rowIndex];

            var (ovKey, baselineRaw, baselineDisplay) = ResolveRowBaseline(grid, key, isGlobal);
            var baselineAbs = ResolveRaw(baselineRaw, batDir, workspace);
            bool sameAsBaseline = !string.IsNullOrEmpty(baselineAbs) && PathEquals(selectedAbs, baselineAbs);

            bool hasOv;
            if (sameAsBaseline)
            {
                _overrides.Remove(ovKey);
                hasOv = false;
            }
            else
            {
                _overrides[ovKey] = selectedAbs;
                hasOv = true;
            }

            _isPopulating = true; // 避免编程赋值触发 CellValueChanged 误置基线 dirty
            row.Cells["colValue"].Value = hasOv ? selectedAbs : baselineDisplay;
            _isPopulating = false;

            MarkOverrideRow(row, hasOv);
            PersistOverrides();
            ValidateAllPathCells();
            UpdateButtonStates();
        }

        private static bool PathEquals(string a, string b)
        {
            try
            {
                return string.Equals(
                    Path.GetFullPath(a).TrimEnd(Path.DirectorySeparatorChar),
                    Path.GetFullPath(b).TrimEnd(Path.DirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            }
        }

        // ── 自动保存 / 重置 / 写入共享 ─────────────────────────

        /// <summary>把当前本地路径覆盖即时写入 local_state.json（值改动时自动调用，静默无日志）。</summary>
        private void PersistOverrides()
        {
            if (string.IsNullOrEmpty(Settings.GenBatPath)) return;
            try
            {
                if (_overrides.Count > 0)
                    LocalState.LubanBatOverrides[Settings.GenBatPath] = new Dictionary<string, string>(_overrides);
                else
                    LocalState.LubanBatOverrides.Remove(Settings.GenBatPath);
                LocalStateManager.Save(LocalState);
            }
            catch (Exception ex)
            {
                Log($"自动保存本地路径失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
            }
        }

        private void btnWriteShared_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "将把当前的本地配置（含路径覆盖）写入共享的 gen.bat。\n" +
                    "gen.bat 进版本控制，此修改会影响团队其他成员，且本机的绝对路径可能在他人机器上失效。\n\n确定继续？",
                    "写入 gen.bat（共享）", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                != DialogResult.OK)
                return;

            if (_config == null)
            {
                Log("未加载 gen.bat。", LogLevel.Warn);
                return;
            }

            ReadEffectiveFromGrids();
            try
            {
                LubanBatParser.Save(_config);
                // 当前有效值已固化为 gen.bat 基线，本机覆盖不再需要 → 清空并持久化
                _overrides.Clear();
                PersistOverrides();
                PopulateTabs();
                Log("本地配置已写入共享 gen.bat。", LogLevel.Ok);
            }
            catch (Exception ex)
            {
                Log($"写入 gen.bat 失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
            }
        }

        /// <summary>按当前磁盘状态重新校验所有路径行的红/绿显示（不重读 gen.bat，保留 grid 当前编辑与本地覆盖）。</summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (_config == null)
            {
                Log("未加载 gen.bat，无可刷新的配置。", LogLevel.Warn);
                return;
            }
            ValidateAllPathCells();
            Log("已按当前磁盘状态刷新路径显示。", LogLevel.Ok);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "将清除本机的所有路径覆盖与未写入的改动，还原为 gen.bat 的共享配置。\n\n确定继续？",
                    "重置", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                != DialogResult.OK)
                return;

            _overrides.Clear();
            PersistOverrides();
            PopulateTabs(); // 重新载入（覆盖已空）→ 回到 gen.bat 基线
            Log("已清除本地覆盖，还原为 gen.bat 共享配置。", LogLevel.Ok);
        }

        /// <summary>
        /// 从 grid 读回到 _config，用于写入共享 gen.bat。
        /// 采用各行当前显示的有效值（含本机覆盖），即把本地配置固化为共享基线。
        /// </summary>
        private void ReadEffectiveFromGrids()
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
                _preLockResetEnabled = btnReset.Enabled;
                _preLockWriteSharedEnabled = btnWriteShared.Enabled;
                pnlTop.Enabled = false;
                pnlConfigActions.Enabled = false;
                tabsCommands.Enabled = false;
                btnReset.Enabled = false;
                btnWriteShared.Enabled = false;
            }
            else
            {
                pnlTop.Enabled = true;
                pnlConfigActions.Enabled = true;
                tabsCommands.Enabled = true;
                btnReset.Enabled = _preLockResetEnabled;
                btnWriteShared.Enabled = _preLockWriteSharedEnabled;
            }
        }

        /// <summary>
        /// 公开的异步执行入口，供 HomeTab 等外部调用。
        /// 返回 Task&lt;bool&gt;：true 表示成功，false 表示失败或取消。
        /// </summary>
        public Task<bool> RunAsync()
        {
            if (string.IsNullOrEmpty(Settings.GenBatPath) || !File.Exists(Settings.GenBatPath))
            {
                Log($"请先选择有效的 gen.bat 路径，当前路径无效或不存在：{(string.IsNullOrEmpty(Settings.GenBatPath) ? "(未配置)" : Settings.GenBatPath)}", LogLevel.Error);
                return Task.FromResult(false);
            }

            // 本地路径覆盖已在改动时即时保存，此处无需再存
            if (_isDirty)
                Log("提示：有未写入的非路径参数改动（grid 中），本次按 gen.bat 现有内容执行；如需生效请点「写入 gen.bat（共享）」。", LogLevel.Warn);

            var tcs = new TaskCompletionSource<bool>();

            SetUILocked(true);
            RaiseExecutionState(true);
            btnRun.Enabled = false;
            btnCancel.Enabled = true;
            int cmdCount = _config?.Commands.Count ?? 1;
            ProgressBarHelper.SetProgressBegin(pbRun);
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
                // Luban 结构化错误日志（含 |ERROR|），或任何写到 stderr 的输出
                //（LubanRunner 已为 stderr 行加 "[ERR] " 前缀，如 cmd 的「无法执行，因为找不到指定的命令或文件」）都视为失败信号
                bool isError = msg.Contains("|ERROR|") || msg.StartsWith("[ERR] ", StringComparison.Ordinal);
                if (isError) _hasError = true;
                LogRaw(msg, isError ? LogLibrary.ClrError : LogLibrary.ClrOk, isError ? LogLevel.Error : LogLevel.Ok);
                _lineCount++;
                int v = totalEstLines > 0 ? 10 + (int)(_lineCount * 80.0 / totalEstLines) : 90;
                v = Math.Min(v, 90);
                if (v > pbRun.Value)
                    ProgressBarHelper.SetProgress(pbRun, v);
            });
            _runner.Finished += code => BeginInvoke(() =>
            {
                bool success = code == 0 && !_hasError;
                if (success)
                    Log("导表完成。", LogLevel.Ok);
                else
                    Log($"导表失败，退出码：{code}", LogLevel.Error);
                ProgressBarHelper.SetProgress(pbRun, 100);
                SetUILocked(false);
                RaiseExecutionState(false);
                btnRun.Enabled = true;
                btnCancel.Enabled = false;
                _runner = null;
                CleanupTempBat();
                // 导表过程中可能已自动创建原本不存在的文件夹 → 按当前磁盘状态刷新路径红/绿显示
                ValidateAllPathCells();
                Log("─ 结束 ─", LogLevel.Info);
                LogDivider();
                tcs.TrySetResult(success);
            });

            try
            {
                _runner.Run(BuildRunBatPath());
            }
            catch (Exception ex)
            {
                Log($"启动失败：{LogLibrary.FormatException(ex)}", LogLevel.Error);
                SetUILocked(false);
                RaiseExecutionState(false);
                btnRun.Enabled = true;
                btnCancel.Enabled = false;
                ProgressBarHelper.SetProgress(pbRun, 100);
                CleanupTempBat();
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }

        /// <summary>
        /// 确定本次实际运行的 bat 路径：有本地覆盖时，生成套用覆盖的临时副本（与 gen.bat 同目录，
        /// 使 %~dp0 与相对路径仍正确解析），否则直接运行原 gen.bat。临时副本路径记于 _tempBatPath，结束后清理。
        /// </summary>
        private string BuildRunBatPath()
        {
            _tempBatPath = null;
            if (_overrides.Count == 0 || _config == null)
                return Settings.GenBatPath;

            try
            {
                var eff = CloneConfigWithOverrides(_config, _overrides);
                var tempPath = Path.Combine(BatDir(), LocalGenBatName);
                if (File.Exists(tempPath)) File.Delete(tempPath);
                LubanBatParser.Save(eff, tempPath);
                _tempBatPath = tempPath;
                Log($"已套用 {_overrides.Count} 项本地路径覆盖（临时副本运行，不改动 gen.bat）。", LogLevel.Info);
                return tempPath;
            }
            catch (Exception ex)
            {
                Log($"生成本地副本失败，改用原 gen.bat 执行：{LogLibrary.FormatException(ex)}", LogLevel.Warn);
                _tempBatPath = null;
                return Settings.GenBatPath;
            }
        }

        /// <summary>删除本次导出生成的 gen.bat 临时副本。</summary>
        private void CleanupTempBat()
        {
            if (string.IsNullOrEmpty(_tempBatPath)) return;
            try { if (File.Exists(_tempBatPath)) File.Delete(_tempBatPath); }
            catch { /* 临时文件清理失败不影响主流程 */ }
            _tempBatPath = null;
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            await RunAsync();
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
                    // 跨驱动器时 GetRelativePath 会抛出，fallback 到原始绝对路径
                    try { return Path.GetRelativePath(batDir, displayValue); }
                    catch { }
                }
                return displayValue;
            }

            if (displayValue.StartsWith("%WORKSPACE%", StringComparison.OrdinalIgnoreCase))
                return displayValue;

            if (Path.IsPathRooted(displayValue) && !string.IsNullOrEmpty(batDir))
            {
                // 跨驱动器时 GetRelativePath 会抛出，fallback 到原始绝对路径
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
            // 路径格式非法时 GetFullPath 会抛出，fallback 到原始值
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

        private void btnClearLog_Click(object? sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void btnCopyLog_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

        /// <summary>取消当前正在进行的任务（供窗口关闭时调用）。Luban 为事件驱动型，重写为通过取消按钮停止。</summary>
        public override void CancelRunningTask()
        {
            if (btnCancel.Enabled)
                btnCancel_Click(this, EventArgs.Empty);
        }
    }
}
