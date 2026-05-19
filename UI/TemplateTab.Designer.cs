namespace ConfigExcelEnhancer.UI
{
    partial class TemplateTab
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTip = new ToolTip(components);
            txtTablesClassPath = new TextBox();
            btnBrowseTablesClass = new Button();
            btnRunAll = new Button();
            btnRunSelected = new Button();
            btnStop = new Button();
            txtDisplayName = new TextBox();
            txtJsonFile = new TextBox();
            btnBrowseJsonFile = new Button();
            txtOutputDir = new TextBox();
            btnBrowseOutputDir = new Button();
            txtNamespace = new TextBox();
            chkPartialClass = new CheckBox();
            chkGenerateIds = new CheckBox();
            txtIdsOutputPath = new TextBox();
            btnBrowseIdsOutput = new Button();
            txtIdsNamespace = new TextBox();
            txtIdsClassName = new TextBox();
            dgvBindings = new DataGridView();
            colType = new DataGridViewTextBoxColumn();
            colTemplatePath = new DataGridViewTextBoxColumn();
            btnAddBinding = new Button();
            btnRemoveBinding = new Button();
            btnBrowseBinding = new Button();
            pnlTablesPath = new Panel();
            lblTablesClass = new Label();
            pnlLeft = new Panel();
            lstJobs = new ListBox();
            pnlJobButtons = new Panel();
            btnAddJob = new Button();
            btnRemoveJob = new Button();
            pnlDetail = new Panel();
            grpBindings = new GroupBox();
            pnlBindingButtons = new Panel();
            grpIds = new GroupBox();
            lblIdsOutputPath = new Label();
            lblIdsNamespace = new Label();
            lblIdsClassName = new Label();
            grpBasic = new GroupBox();
            lblDisplayName = new Label();
            lblJsonFile = new Label();
            lblOutputDir = new Label();
            lblNamespace = new Label();
            pnlRunArea = new Panel();
            txtLog = new RichTextBox();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            pnlRunButtons = new Panel();
            pbRun = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)dgvBindings).BeginInit();
            pnlTablesPath.SuspendLayout();
            pnlLeft.SuspendLayout();
            pnlJobButtons.SuspendLayout();
            pnlDetail.SuspendLayout();
            grpBindings.SuspendLayout();
            pnlBindingButtons.SuspendLayout();
            grpIds.SuspendLayout();
            grpBasic.SuspendLayout();
            pnlRunArea.SuspendLayout();
            ctxLog.SuspendLayout();
            pnlRunButtons.SuspendLayout();
            SuspendLayout();
            // 
            // toolTip
            // 
            toolTip.AutoPopDelay = 8000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 200;
            // 
            // txtTablesClassPath
            // 
            txtTablesClassPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTablesClassPath.Location = new Point(90, 10);
            txtTablesClassPath.Name = "txtTablesClassPath";
            txtTablesClassPath.PlaceholderText = "Luban 生成的 Tables.cs 路径（全局一次性配置）";
            txtTablesClassPath.Size = new Size(612, 23);
            txtTablesClassPath.TabIndex = 1;
            toolTip.SetToolTip(txtTablesClassPath, "Luban 生成的 Tables.cs 绝对路径。\n工具通过解析该文件自动推断每张表的命名空间和访问器属性，\n无需手动填写 $TableAccessor、$LubanNamespace 等占位符。\n同一个项目只需配置一次，所有导出任务共用。");
            txtTablesClassPath.TextChanged += txtTablesClassPath_TextChanged;
            // 
            // btnBrowseTablesClass
            // 
            btnBrowseTablesClass.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseTablesClass.Location = new Point(710, 8);
            btnBrowseTablesClass.Name = "btnBrowseTablesClass";
            btnBrowseTablesClass.Size = new Size(75, 28);
            btnBrowseTablesClass.TabIndex = 2;
            btnBrowseTablesClass.Text = "浏览...";
            toolTip.SetToolTip(btnBrowseTablesClass, "点击选择 Tables.cs 文件（通常在 Luban 输出目录下）。");
            btnBrowseTablesClass.Click += btnBrowseTablesClass_Click;
            // 
            // btnRunAll
            // 
            btnRunAll.Location = new Point(8, 5);
            btnRunAll.Name = "btnRunAll";
            btnRunAll.Size = new Size(110, 28);
            btnRunAll.TabIndex = 0;
            btnRunAll.Text = "▶ 运行全部";
            toolTip.SetToolTip(btnRunAll, "运行左侧列表中的全部任务。");
            btnRunAll.Click += btnRunAll_Click;
            // 
            // btnRunSelected
            // 
            btnRunSelected.Location = new Point(124, 5);
            btnRunSelected.Name = "btnRunSelected";
            btnRunSelected.Size = new Size(110, 28);
            btnRunSelected.TabIndex = 1;
            btnRunSelected.Text = "▶ 运行选中";
            toolTip.SetToolTip(btnRunSelected, "仅运行当前在左侧列表中选中的任务。");
            btnRunSelected.Click += btnRunSelected_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(240, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 28);
            btnStop.TabIndex = 2;
            btnStop.Text = "■ 停止";
            toolTip.SetToolTip(btnStop, "取消正在执行的任务（当前文件写完后停止）。");
            btnStop.Click += btnStop_Click;
            // 
            // txtDisplayName
            // 
            txtDisplayName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDisplayName.Location = new Point(90, 24);
            txtDisplayName.Name = "txtDisplayName";
            txtDisplayName.Size = new Size(455, 23);
            txtDisplayName.TabIndex = 0;
            toolTip.SetToolTip(txtDisplayName, "任务的显示名称，仅用于左侧列表的标识，不影响生成结果。");
            txtDisplayName.TextChanged += txtDisplayName_TextChanged;
            // 
            // txtJsonFile
            // 
            txtJsonFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJsonFile.Location = new Point(90, 51);
            txtJsonFile.Name = "txtJsonFile";
            txtJsonFile.Size = new Size(373, 23);
            txtJsonFile.TabIndex = 1;
            toolTip.SetToolTip(txtJsonFile, "Luban 导出的 JSON 配置文件路径。\n工具通过该文件名在 Tables.cs 中查找对应的表映射。\n例如：...\\output\\unit_tbpawn.json");
            txtJsonFile.TextChanged += txtJsonFile_TextChanged;
            // 
            // btnBrowseJsonFile
            // 
            btnBrowseJsonFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseJsonFile.Location = new Point(469, 49);
            btnBrowseJsonFile.Name = "btnBrowseJsonFile";
            btnBrowseJsonFile.Size = new Size(76, 26);
            btnBrowseJsonFile.TabIndex = 2;
            btnBrowseJsonFile.Text = "浏览...";
            toolTip.SetToolTip(btnBrowseJsonFile, "点击选择 Luban 导出的 JSON 文件。");
            btnBrowseJsonFile.Click += btnBrowseJsonFile_Click;
            // 
            // txtOutputDir
            // 
            txtOutputDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtOutputDir.Location = new Point(90, 78);
            txtOutputDir.Name = "txtOutputDir";
            txtOutputDir.Size = new Size(373, 23);
            txtOutputDir.TabIndex = 3;
            toolTip.SetToolTip(txtOutputDir, "生成的模板类 .cs 文件的输出目录（绝对路径）。\n目录不存在时会自动创建。");
            txtOutputDir.TextChanged += txtOutputDir_TextChanged;
            // 
            // btnBrowseOutputDir
            // 
            btnBrowseOutputDir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseOutputDir.Location = new Point(469, 76);
            btnBrowseOutputDir.Name = "btnBrowseOutputDir";
            btnBrowseOutputDir.Size = new Size(76, 26);
            btnBrowseOutputDir.TabIndex = 4;
            btnBrowseOutputDir.Text = "浏览...";
            toolTip.SetToolTip(btnBrowseOutputDir, "点击选择模板类的输出目录。");
            btnBrowseOutputDir.Click += btnBrowseOutputDir_Click;
            // 
            // txtNamespace
            // 
            txtNamespace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtNamespace.Location = new Point(90, 105);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.Size = new Size(455, 23);
            txtNamespace.TabIndex = 5;
            toolTip.SetToolTip(txtNamespace, "生成的 C# 类的命名空间，将填入模板中的 {{$Namespace}} 占位符。");
            txtNamespace.TextChanged += txtNamespace_TextChanged;
            // 
            // chkPartialClass
            // 
            chkPartialClass.AutoSize = true;
            chkPartialClass.Checked = true;
            chkPartialClass.CheckState = CheckState.Checked;
            chkPartialClass.Location = new Point(10, 132);
            chkPartialClass.Name = "chkPartialClass";
            chkPartialClass.Size = new Size(172, 21);
            chkPartialClass.TabIndex = 6;
            chkPartialClass.Text = "生成 partial class（推荐）";
            toolTip.SetToolTip(chkPartialClass, "勾选后，生成的类声明带有 partial 关键字，\n可在同目录的同名手写文件中追加方法，工具不会覆盖手写部分。");
            chkPartialClass.CheckedChanged += chkPartialClass_CheckedChanged;
            // 
            // chkGenerateIds
            // 
            chkGenerateIds.AutoSize = true;
            chkGenerateIds.Checked = true;
            chkGenerateIds.CheckState = CheckState.Checked;
            chkGenerateIds.Location = new Point(10, 22);
            chkGenerateIds.Name = "chkGenerateIds";
            chkGenerateIds.Size = new Size(224, 21);
            chkGenerateIds.TabIndex = 0;
            chkGenerateIds.Text = "生成 Ids.Generated.cs（每次覆盖）";
            toolTip.SetToolTip(chkGenerateIds, "生成 {IdsClassName}.Generated.cs，包含 JSON 中所有条目的 ID 常量。\n该文件每次运行均会全量覆盖，保持与 JSON 数据同步。");
            chkGenerateIds.CheckedChanged += chkGenerateIds_CheckedChanged;
            // 
            // txtIdsOutputPath
            // 
            txtIdsOutputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtIdsOutputPath.Location = new Point(90, 51);
            txtIdsOutputPath.Name = "txtIdsOutputPath";
            txtIdsOutputPath.PlaceholderText = "如 ...\\UnitIds.Generated.cs";
            txtIdsOutputPath.Size = new Size(373, 23);
            txtIdsOutputPath.TabIndex = 1;
            toolTip.SetToolTip(txtIdsOutputPath, "Ids.Generated.cs 的完整输出路径（含文件名）。");
            txtIdsOutputPath.TextChanged += txtIdsOutputPath_TextChanged;
            // 
            // btnBrowseIdsOutput
            // 
            btnBrowseIdsOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseIdsOutput.Location = new Point(469, 49);
            btnBrowseIdsOutput.Name = "btnBrowseIdsOutput";
            btnBrowseIdsOutput.Size = new Size(76, 26);
            btnBrowseIdsOutput.TabIndex = 2;
            btnBrowseIdsOutput.Text = "浏览...";
            toolTip.SetToolTip(btnBrowseIdsOutput, "点击指定 Ids.Generated.cs 的保存位置和文件名。");
            btnBrowseIdsOutput.Click += btnBrowseIdsOutput_Click;
            // 
            // txtIdsNamespace
            // 
            txtIdsNamespace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtIdsNamespace.Location = new Point(90, 78);
            txtIdsNamespace.Name = "txtIdsNamespace";
            txtIdsNamespace.Size = new Size(455, 23);
            txtIdsNamespace.TabIndex = 3;
            toolTip.SetToolTip(txtIdsNamespace, "Ids.Generated.cs 使用的命名空间。");
            txtIdsNamespace.TextChanged += txtIdsNamespace_TextChanged;
            // 
            // txtIdsClassName
            // 
            txtIdsClassName.Location = new Point(90, 105);
            txtIdsClassName.Name = "txtIdsClassName";
            txtIdsClassName.PlaceholderText = "如 UnitIds";
            txtIdsClassName.Size = new Size(200, 23);
            txtIdsClassName.TabIndex = 4;
            toolTip.SetToolTip(txtIdsClassName, "ID 常量类的类名，例如 UnitIds。\n生成文件名为 {类名}.Generated.cs。");
            txtIdsClassName.TextChanged += txtIdsClassName_TextChanged;
            // 
            // dgvBindings
            // 
            dgvBindings.AllowUserToAddRows = false;
            dgvBindings.AllowUserToDeleteRows = false;
            dgvBindings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvBindings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBindings.Columns.AddRange(new DataGridViewColumn[] { colType, colTemplatePath });
            dgvBindings.Location = new Point(8, 22);
            dgvBindings.Name = "dgvBindings";
            dgvBindings.RowHeadersVisible = false;
            dgvBindings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBindings.Size = new Size(555, 112);
            dgvBindings.TabIndex = 0;
            toolTip.SetToolTip(dgvBindings, "$type 小一了千9 JSON 每条记录的类型字段。\n有绑定 -> 用模板渲染，每次运行均覆盖写入。\n无绑定 -> 目标文件不存在时生成最简骨架，已有文件不修改。");
            dgvBindings.CellValueChanged += dgvBindings_CellValueChanged;
            dgvBindings.RowsRemoved += dgvBindings_RowsRemoved;
            // 
            // colType
            // 
            colType.HeaderText = "$type";
            colType.Name = "colType";
            colType.SortMode = DataGridViewColumnSortMode.NotSortable;
            colType.Width = 120;
            // 
            // colTemplatePath
            // 
            colTemplatePath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTemplatePath.HeaderText = "模板文件路径 (.tmpl)";
            colTemplatePath.Name = "colTemplatePath";
            colTemplatePath.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // btnAddBinding
            // 
            btnAddBinding.Location = new Point(0, 3);
            btnAddBinding.Name = "btnAddBinding";
            btnAddBinding.Size = new Size(80, 26);
            btnAddBinding.TabIndex = 0;
            btnAddBinding.Text = "+ 添加";
            toolTip.SetToolTip(btnAddBinding, "添加一条新的 $type -> 模板文件绑定。");
            btnAddBinding.Click += btnAddBinding_Click;
            // 
            // btnRemoveBinding
            // 
            btnRemoveBinding.Location = new Point(86, 3);
            btnRemoveBinding.Name = "btnRemoveBinding";
            btnRemoveBinding.Size = new Size(80, 26);
            btnRemoveBinding.TabIndex = 1;
            btnRemoveBinding.Text = "- 删除行";
            toolTip.SetToolTip(btnRemoveBinding, "删除当前选中行的绑定。");
            btnRemoveBinding.Click += btnRemoveBinding_Click;
            // 
            // btnBrowseBinding
            // 
            btnBrowseBinding.Location = new Point(172, 3);
            btnBrowseBinding.Name = "btnBrowseBinding";
            btnBrowseBinding.Size = new Size(120, 26);
            btnBrowseBinding.TabIndex = 2;
            btnBrowseBinding.Text = "浏览模板文件...";
            toolTip.SetToolTip(btnBrowseBinding, "为当前选中行选择 .tmpl 模板文件。");
            btnBrowseBinding.Click += btnBrowseBinding_Click;
            // 
            // pnlTablesPath
            // 
            pnlTablesPath.Controls.Add(lblTablesClass);
            pnlTablesPath.Controls.Add(txtTablesClassPath);
            pnlTablesPath.Controls.Add(btnBrowseTablesClass);
            pnlTablesPath.Dock = DockStyle.Top;
            pnlTablesPath.Location = new Point(0, 0);
            pnlTablesPath.Name = "pnlTablesPath";
            pnlTablesPath.Size = new Size(800, 43);
            pnlTablesPath.TabIndex = 0;
            // 
            // lblTablesClass
            // 
            lblTablesClass.AutoSize = true;
            lblTablesClass.Location = new Point(12, 13);
            lblTablesClass.Name = "lblTablesClass";
            lblTablesClass.Size = new Size(64, 17);
            lblTablesClass.TabIndex = 0;
            lblTablesClass.Text = "Tables.cs:";
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(lstJobs);
            pnlLeft.Controls.Add(pnlJobButtons);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 43);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(200, 617);
            pnlLeft.TabIndex = 1;
            // 
            // lstJobs
            // 
            lstJobs.Dock = DockStyle.Fill;
            lstJobs.Location = new Point(0, 0);
            lstJobs.Name = "lstJobs";
            lstJobs.Size = new Size(200, 583);
            lstJobs.TabIndex = 0;
            lstJobs.SelectedIndexChanged += lstJobs_SelectedIndexChanged;
            // 
            // pnlJobButtons
            // 
            pnlJobButtons.Controls.Add(btnAddJob);
            pnlJobButtons.Controls.Add(btnRemoveJob);
            pnlJobButtons.Dock = DockStyle.Bottom;
            pnlJobButtons.Location = new Point(0, 583);
            pnlJobButtons.Name = "pnlJobButtons";
            pnlJobButtons.Size = new Size(200, 34);
            pnlJobButtons.TabIndex = 1;
            // 
            // btnAddJob
            // 
            btnAddJob.Location = new Point(4, 4);
            btnAddJob.Name = "btnAddJob";
            btnAddJob.Size = new Size(88, 26);
            btnAddJob.TabIndex = 0;
            btnAddJob.Text = "+ 添加";
            btnAddJob.Click += btnAddJob_Click;
            // 
            // btnRemoveJob
            // 
            btnRemoveJob.Location = new Point(98, 4);
            btnRemoveJob.Name = "btnRemoveJob";
            btnRemoveJob.Size = new Size(88, 26);
            btnRemoveJob.TabIndex = 1;
            btnRemoveJob.Text = "- 删除";
            btnRemoveJob.Click += btnRemoveJob_Click;
            // 
            // pnlDetail
            // 
            pnlDetail.AutoScroll = true;
            pnlDetail.Controls.Add(grpBindings);
            pnlDetail.Controls.Add(grpIds);
            pnlDetail.Controls.Add(grpBasic);
            pnlDetail.Dock = DockStyle.Fill;
            pnlDetail.Enabled = false;
            pnlDetail.Location = new Point(200, 43);
            pnlDetail.Name = "pnlDetail";
            pnlDetail.Padding = new Padding(6, 4, 6, 4);
            pnlDetail.Size = new Size(600, 457);
            pnlDetail.TabIndex = 3;
            // 
            // grpBindings
            // 
            grpBindings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpBindings.Controls.Add(dgvBindings);
            grpBindings.Controls.Add(pnlBindingButtons);
            grpBindings.Location = new Point(6, 310);
            grpBindings.Name = "grpBindings";
            grpBindings.Size = new Size(571, 180);
            grpBindings.TabIndex = 2;
            grpBindings.TabStop = false;
            grpBindings.Text = "$type 模板绑定（有绑定->覆盖；无绑定->仅新建骨架）";
            // 
            // pnlBindingButtons
            // 
            pnlBindingButtons.Controls.Add(btnAddBinding);
            pnlBindingButtons.Controls.Add(btnRemoveBinding);
            pnlBindingButtons.Controls.Add(btnBrowseBinding);
            pnlBindingButtons.Dock = DockStyle.Bottom;
            pnlBindingButtons.Location = new Point(3, 146);
            pnlBindingButtons.Name = "pnlBindingButtons";
            pnlBindingButtons.Size = new Size(565, 31);
            pnlBindingButtons.TabIndex = 1;
            // 
            // grpIds
            // 
            grpIds.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpIds.Controls.Add(chkGenerateIds);
            grpIds.Controls.Add(lblIdsOutputPath);
            grpIds.Controls.Add(txtIdsOutputPath);
            grpIds.Controls.Add(btnBrowseIdsOutput);
            grpIds.Controls.Add(lblIdsNamespace);
            grpIds.Controls.Add(txtIdsNamespace);
            grpIds.Controls.Add(lblIdsClassName);
            grpIds.Controls.Add(txtIdsClassName);
            grpIds.Location = new Point(6, 167);
            grpIds.Name = "grpIds";
            grpIds.Size = new Size(571, 140);
            grpIds.TabIndex = 1;
            grpIds.TabStop = false;
            grpIds.Text = "Ids 文件（*.Generated.cs）";
            // 
            // lblIdsOutputPath
            // 
            lblIdsOutputPath.AutoSize = true;
            lblIdsOutputPath.Location = new Point(10, 54);
            lblIdsOutputPath.Name = "lblIdsOutputPath";
            lblIdsOutputPath.Size = new Size(68, 17);
            lblIdsOutputPath.TabIndex = 1;
            lblIdsOutputPath.Text = "输出路径：";
            // 
            // lblIdsNamespace
            // 
            lblIdsNamespace.AutoSize = true;
            lblIdsNamespace.Location = new Point(10, 81);
            lblIdsNamespace.Name = "lblIdsNamespace";
            lblIdsNamespace.Size = new Size(68, 17);
            lblIdsNamespace.TabIndex = 3;
            lblIdsNamespace.Text = "命名空间：";
            // 
            // lblIdsClassName
            // 
            lblIdsClassName.AutoSize = true;
            lblIdsClassName.Location = new Point(10, 108);
            lblIdsClassName.Name = "lblIdsClassName";
            lblIdsClassName.Size = new Size(44, 17);
            lblIdsClassName.TabIndex = 4;
            lblIdsClassName.Text = "类名：";
            // 
            // grpBasic
            // 
            grpBasic.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpBasic.Controls.Add(lblDisplayName);
            grpBasic.Controls.Add(txtDisplayName);
            grpBasic.Controls.Add(lblJsonFile);
            grpBasic.Controls.Add(txtJsonFile);
            grpBasic.Controls.Add(btnBrowseJsonFile);
            grpBasic.Controls.Add(lblOutputDir);
            grpBasic.Controls.Add(txtOutputDir);
            grpBasic.Controls.Add(btnBrowseOutputDir);
            grpBasic.Controls.Add(lblNamespace);
            grpBasic.Controls.Add(txtNamespace);
            grpBasic.Controls.Add(chkPartialClass);
            grpBasic.Location = new Point(6, 4);
            grpBasic.Name = "grpBasic";
            grpBasic.Size = new Size(571, 157);
            grpBasic.TabIndex = 0;
            grpBasic.TabStop = false;
            grpBasic.Text = "基本设置";
            // 
            // lblDisplayName
            // 
            lblDisplayName.AutoSize = true;
            lblDisplayName.Location = new Point(10, 27);
            lblDisplayName.Name = "lblDisplayName";
            lblDisplayName.Size = new Size(68, 17);
            lblDisplayName.TabIndex = 0;
            lblDisplayName.Text = "显示名称：";
            // 
            // lblJsonFile
            // 
            lblJsonFile.AutoSize = true;
            lblJsonFile.Location = new Point(10, 54);
            lblJsonFile.Name = "lblJsonFile";
            lblJsonFile.Size = new Size(80, 17);
            lblJsonFile.TabIndex = 1;
            lblJsonFile.Text = "JSON 文件：";
            // 
            // lblOutputDir
            // 
            lblOutputDir.AutoSize = true;
            lblOutputDir.Location = new Point(10, 81);
            lblOutputDir.Name = "lblOutputDir";
            lblOutputDir.Size = new Size(68, 17);
            lblOutputDir.TabIndex = 3;
            lblOutputDir.Text = "输出目录：";
            // 
            // lblNamespace
            // 
            lblNamespace.AutoSize = true;
            lblNamespace.Location = new Point(10, 108);
            lblNamespace.Name = "lblNamespace";
            lblNamespace.Size = new Size(68, 17);
            lblNamespace.TabIndex = 5;
            lblNamespace.Text = "命名空间：";
            // 
            // pnlRunArea
            // 
            pnlRunArea.Controls.Add(txtLog);
            pnlRunArea.Controls.Add(pnlRunButtons);
            pnlRunArea.Dock = DockStyle.Bottom;
            pnlRunArea.Location = new Point(200, 500);
            pnlRunArea.Name = "pnlRunArea";
            pnlRunArea.Size = new Size(600, 160);
            pnlRunArea.TabIndex = 2;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ContextMenuStrip = ctxLog;
            txtLog.Dock = DockStyle.Fill;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Location = new Point(0, 38);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(600, 122);
            txtLog.TabIndex = 1;
            txtLog.Text = "";
            // 
            // ctxLog
            // 
            ctxLog.Items.AddRange(new ToolStripItem[] { ctxMenuItemClearLog, ctxMenuItemCopyLog });
            ctxLog.Name = "ctxLog";
            ctxLog.Size = new Size(101, 48);
            // 
            // ctxMenuItemClearLog
            // 
            ctxMenuItemClearLog.Name = "ctxMenuItemClearLog";
            ctxMenuItemClearLog.Size = new Size(100, 22);
            ctxMenuItemClearLog.Text = "清空";
            ctxMenuItemClearLog.Click += btnClearLog_Click;
            // 
            // ctxMenuItemCopyLog
            // 
            ctxMenuItemCopyLog.Name = "ctxMenuItemCopyLog";
            ctxMenuItemCopyLog.Size = new Size(100, 22);
            ctxMenuItemCopyLog.Text = "复制";
            ctxMenuItemCopyLog.Click += btnCopyLog_Click;
            // 
            // pnlRunButtons
            // 
            pnlRunButtons.Controls.Add(btnRunAll);
            pnlRunButtons.Controls.Add(btnRunSelected);
            pnlRunButtons.Controls.Add(btnStop);
            pnlRunButtons.Controls.Add(pbRun);
            pnlRunButtons.Dock = DockStyle.Top;
            pnlRunButtons.Location = new Point(0, 0);
            pnlRunButtons.Name = "pnlRunButtons";
            pnlRunButtons.Size = new Size(600, 38);
            pnlRunButtons.TabIndex = 0;
            // 
            // pbRun
            // 
            pbRun.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbRun.Location = new Point(330, 9);
            pbRun.Name = "pbRun";
            pbRun.Size = new Size(260, 20);
            pbRun.TabIndex = 3;
            pbRun.Visible = false;
            // 
            // TemplateTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlDetail);
            Controls.Add(pnlRunArea);
            Controls.Add(pnlLeft);
            Controls.Add(pnlTablesPath);
            Name = "TemplateTab";
            Size = new Size(800, 660);
            ((System.ComponentModel.ISupportInitialize)dgvBindings).EndInit();
            pnlTablesPath.ResumeLayout(false);
            pnlTablesPath.PerformLayout();
            pnlLeft.ResumeLayout(false);
            pnlJobButtons.ResumeLayout(false);
            pnlDetail.ResumeLayout(false);
            grpBindings.ResumeLayout(false);
            pnlBindingButtons.ResumeLayout(false);
            grpIds.ResumeLayout(false);
            grpIds.PerformLayout();
            grpBasic.ResumeLayout(false);
            grpBasic.PerformLayout();
            pnlRunArea.ResumeLayout(false);
            ctxLog.ResumeLayout(false);
            pnlRunButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel pnlTablesPath = null!;
        private Label lblTablesClass = null!;
        private TextBox txtTablesClassPath = null!;
        private Button btnBrowseTablesClass = null!;
        private Panel pnlLeft = null!;
        private ListBox lstJobs = null!;
        private Panel pnlJobButtons = null!;
        private Button btnAddJob = null!;
        private Button btnRemoveJob = null!;
        private Panel pnlDetail = null!;
        private GroupBox grpBasic = null!;
        private Label lblDisplayName = null!;
        private TextBox txtDisplayName = null!;
        private Label lblJsonFile = null!;
        private TextBox txtJsonFile = null!;
        private Button btnBrowseJsonFile = null!;
        private Label lblOutputDir = null!;
        private TextBox txtOutputDir = null!;
        private Button btnBrowseOutputDir = null!;
        private Label lblNamespace = null!;
        private TextBox txtNamespace = null!;
        private CheckBox chkPartialClass = null!;
        private GroupBox grpIds = null!;
        private CheckBox chkGenerateIds = null!;
        private Label lblIdsOutputPath = null!;
        private TextBox txtIdsOutputPath = null!;
        private Button btnBrowseIdsOutput = null!;
        private Label lblIdsNamespace = null!;
        private TextBox txtIdsNamespace = null!;
        private Label lblIdsClassName = null!;
        private TextBox txtIdsClassName = null!;
        private GroupBox grpBindings = null!;
        private DataGridView dgvBindings = null!;
        private DataGridViewTextBoxColumn colType = null!;
        private DataGridViewTextBoxColumn colTemplatePath = null!;
        private Panel pnlBindingButtons = null!;
        private Button btnAddBinding = null!;
        private Button btnRemoveBinding = null!;
        private Button btnBrowseBinding = null!;
        private Panel pnlRunArea = null!;
        private Panel pnlRunButtons = null!;
        private Button btnRunAll = null!;
        private Button btnRunSelected = null!;
        private Button btnStop = null!;
        private ProgressBar pbRun = null!;
        private ContextMenuStrip ctxLog = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog = null!;
        private RichTextBox txtLog = null!;
        private ToolTip toolTip = null!;
    }
}
