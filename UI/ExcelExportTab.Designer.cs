namespace ConfigExcelEnhancer.UI
{
    partial class ExcelExportTab
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
            lblXmlFolder = new Label();
            lblTemplate = new Label();
            lblNaming = new Label();
            rdoNameAsIs = new RadioButton();
            rdoNameCamel = new RadioButton();
            rdoNameSnake = new RadioButton();
            lblFileName = new Label();
            txtPrefix = new TextBox();
            lblClassName = new Label();
            txtSuffix = new TextBox();
            btnRefresh = new Button();
            chkListCommonFolder = new CheckBox();
            lblListTargetFolder = new Label();
            lblTargetFolder = new Label();
            btnExport = new Button();
            lblExportSettings = new Label();
            chkRunEnumValidation = new CheckBox();
            pnlSettings = new Panel();
            txtXmlFolder = new TextBox();
            btnBrowseXmlFolder = new Button();
            btnOpenXmlFolder = new Button();
            txtDesignTemplate = new TextBox();
            btnBrowseTemplate = new Button();
            btnOpenTemplate = new Button();
            pnlNaming = new Panel();
            pnlNamingGroup = new Panel();
            tabMode = new TabControl();
            tabPageList = new TabPage();
            dgvClasses = new DataGridView();
            colEnabled = new DataGridViewCheckBoxColumn();
            colClassName = new DataGridViewTextBoxColumn();
            colSourceFile = new DataGridViewTextBoxColumn();
            colTargetPath = new DataGridViewTextBoxColumn();
            colBrowse = new DataGridViewButtonColumn();
            colClearPath = new DataGridViewButtonColumn();
            pnlListBar = new Panel();
            btnSelectAll = new Button();
            btnDeselectAll = new Button();
            pnlListCommon = new Panel();
            txtListTargetFolder = new TextBox();
            btnBrowseListFolder = new Button();
            btnOpenListFolder = new Button();
            tabPageBatch = new TabPage();
            pnlBatchTarget = new Panel();
            txtTargetFolder = new TextBox();
            btnBrowseTargetFolder = new Button();
            btnOpenTargetFolder = new Button();
            pnlAction = new Panel();
            btnCancel = new Button();
            pbExport = new ProgressBar();
            pnlExportSettings = new Panel();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            txtLog = new RichTextBox();
            pnlSettings.SuspendLayout();
            pnlNaming.SuspendLayout();
            pnlNamingGroup.SuspendLayout();
            tabMode.SuspendLayout();
            tabPageList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClasses).BeginInit();
            pnlListBar.SuspendLayout();
            pnlListCommon.SuspendLayout();
            tabPageBatch.SuspendLayout();
            pnlBatchTarget.SuspendLayout();
            pnlAction.SuspendLayout();
            pnlExportSettings.SuspendLayout();
            ctxLog.SuspendLayout();
            SuspendLayout();
            // 
            // toolTip
            // 
            toolTip.AutoPopDelay = 8000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 200;
            // 
            // lblXmlFolder
            // 
            lblXmlFolder.AutoSize = true;
            lblXmlFolder.Location = new Point(12, 14);
            lblXmlFolder.Name = "lblXmlFolder";
            lblXmlFolder.Size = new Size(110, 17);
            lblXmlFolder.TabIndex = 0;
            lblXmlFolder.Text = "XML 来源文件夹：";
            toolTip.SetToolTip(lblXmlFolder, "存放 Luban .xml 数据定义的文件夹，工具据此识别需要导出为 Excel 的数据类。");
            // 
            // lblTemplate
            // 
            lblTemplate.AutoSize = true;
            lblTemplate.Location = new Point(12, 50);
            lblTemplate.Name = "lblTemplate";
            lblTemplate.Size = new Size(101, 17);
            lblTemplate.TabIndex = 3;
            lblTemplate.Text = "Excel 设计模板：";
            toolTip.SetToolTip(lblTemplate, "导出的 Excel 所使用的样式/格式模板文件（可空，留空则导出无格式的基础表）。");
            // 
            // lblNaming
            // 
            lblNaming.AutoSize = true;
            lblNaming.Location = new Point(12, 10);
            lblNaming.Name = "lblNaming";
            lblNaming.Size = new Size(68, 17);
            lblNaming.TabIndex = 0;
            lblNaming.Text = "文件命名：";
            toolTip.SetToolTip(lblNaming, "导出 Excel 文件名的命名规则，基于数据类名转换。");
            // 
            // rdoNameAsIs
            // 
            rdoNameAsIs.AutoSize = true;
            rdoNameAsIs.Checked = true;
            rdoNameAsIs.Location = new Point(7, 3);
            rdoNameAsIs.Name = "rdoNameAsIs";
            rdoNameAsIs.Size = new Size(74, 21);
            rdoNameAsIs.TabIndex = 0;
            rdoNameAsIs.TabStop = true;
            rdoNameAsIs.Text = "类名不变";
            toolTip.SetToolTip(rdoNameAsIs, "文件名直接使用数据类名，不做大小写转换。");
            rdoNameAsIs.CheckedChanged += rdoNameAsIs_CheckedChanged;
            // 
            // rdoNameCamel
            // 
            rdoNameCamel.AutoSize = true;
            rdoNameCamel.Location = new Point(92, 3);
            rdoNameCamel.Name = "rdoNameCamel";
            rdoNameCamel.Size = new Size(134, 21);
            rdoNameCamel.TabIndex = 1;
            rdoNameCamel.Text = "驼峰（首字母大写）";
            toolTip.SetToolTip(rdoNameCamel, "文件名转为大驼峰（PascalCase），如 UnitInfo。");
            rdoNameCamel.CheckedChanged += rdoNameCamel_CheckedChanged;
            // 
            // rdoNameSnake
            // 
            rdoNameSnake.AutoSize = true;
            rdoNameSnake.Location = new Point(232, 3);
            rdoNameSnake.Name = "rdoNameSnake";
            rdoNameSnake.Size = new Size(103, 21);
            rdoNameSnake.TabIndex = 2;
            rdoNameSnake.Text = "全小写_下划线";
            toolTip.SetToolTip(rdoNameSnake, "文件名转为全小写下划线（snake_case），如 unit_info。");
            rdoNameSnake.CheckedChanged += rdoNameSnake_CheckedChanged;
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(12, 44);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(56, 17);
            lblFileName.TabIndex = 2;
            lblFileName.Text = "文件名：";
            toolTip.SetToolTip(lblFileName, "最终文件名 = 前缀 + 数据类名（按命名规则转换）+ 后缀。");
            // 
            // txtPrefix
            // 
            txtPrefix.Location = new Point(76, 40);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.PlaceholderText = "前缀（可空）";
            txtPrefix.Size = new Size(100, 23);
            txtPrefix.TabIndex = 3;
            toolTip.SetToolTip(txtPrefix, "添加在数据类名之前的文件名前缀（可空）。");
            txtPrefix.TextChanged += txtPrefix_TextChanged;
            // 
            // lblClassName
            // 
            lblClassName.AutoSize = true;
            lblClassName.Location = new Point(182, 44);
            lblClassName.Name = "lblClassName";
            lblClassName.Size = new Size(56, 17);
            lblClassName.TabIndex = 4;
            lblClassName.Text = "数据类名";
            toolTip.SetToolTip(lblClassName, "文件名中部为数据类名（按上方命名规则转换），其前为前缀、其后为后缀。");
            // 
            // txtSuffix
            // 
            txtSuffix.Location = new Point(243, 40);
            txtSuffix.Name = "txtSuffix";
            txtSuffix.PlaceholderText = "后缀（可空）";
            txtSuffix.Size = new Size(100, 23);
            txtSuffix.TabIndex = 5;
            toolTip.SetToolTip(txtSuffix, "添加在数据类名之后的文件名后缀（可空）。");
            txtSuffix.TextChanged += txtSuffix_TextChanged;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(6, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 26);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "刷新列表";
            toolTip.SetToolTip(btnRefresh, "重新扫描 XML 来源文件夹，刷新可导出的数据类列表。");
            btnRefresh.Click += btnRefresh_Click;
            // 
            // chkListCommonFolder
            // 
            chkListCommonFolder.AutoSize = true;
            chkListCommonFolder.Checked = true;
            chkListCommonFolder.CheckState = CheckState.Checked;
            chkListCommonFolder.Location = new Point(7, 11);
            chkListCommonFolder.Name = "chkListCommonFolder";
            chkListCommonFolder.Size = new Size(15, 14);
            chkListCommonFolder.TabIndex = 0;
            toolTip.SetToolTip(chkListCommonFolder, "启用“通用导出文件夹”回退：勾选时未单独指定路径的数据类导出到通用文件夹；取消勾选则禁用此回退，未指定路径的数据类不会被导出。");
            chkListCommonFolder.UseVisualStyleBackColor = true;
            chkListCommonFolder.CheckedChanged += chkListCommonFolder_CheckedChanged;
            // 
            // lblListTargetFolder
            // 
            lblListTargetFolder.AutoSize = true;
            lblListTargetFolder.Location = new Point(26, 9);
            lblListTargetFolder.Name = "lblListTargetFolder";
            lblListTargetFolder.Size = new Size(104, 17);
            lblListTargetFolder.TabIndex = 1;
            lblListTargetFolder.Text = "通用导出文件夹：";
            toolTip.SetToolTip(lblListTargetFolder, "未单独指定目标路径的数据类，统一导出到此文件夹。");
            // 
            // lblTargetFolder
            // 
            lblTargetFolder.AutoSize = true;
            lblTargetFolder.Location = new Point(6, 9);
            lblTargetFolder.Name = "lblTargetFolder";
            lblTargetFolder.Size = new Size(80, 17);
            lblTargetFolder.TabIndex = 0;
            lblTargetFolder.Text = "导出文件夹：";
            toolTip.SetToolTip(lblTargetFolder, "批量导出模式下，所有数据类 Excel 统一输出到此文件夹。");
            // 
            // btnExport
            // 
            btnExport.Location = new Point(12, 7);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(110, 32);
            btnExport.TabIndex = 0;
            btnExport.Text = "▶ 导出";
            toolTip.SetToolTip(btnExport, "根据当前模式与设置，将数据类导出为 Excel 文件。");
            btnExport.Click += btnExport_Click;
            // 
            // lblExportSettings
            // 
            lblExportSettings.AutoSize = true;
            lblExportSettings.Location = new Point(12, 9);
            lblExportSettings.Name = "lblExportSettings";
            lblExportSettings.Size = new Size(68, 17);
            lblExportSettings.TabIndex = 0;
            lblExportSettings.Text = "导出设置：";
            toolTip.SetToolTip(lblExportSettings, "导出过程中的附加处理选项。");
            // 
            // chkRunEnumValidation
            // 
            chkRunEnumValidation.AutoSize = true;
            chkRunEnumValidation.Checked = true;
            chkRunEnumValidation.CheckState = CheckState.Checked;
            chkRunEnumValidation.Location = new Point(86, 8);
            chkRunEnumValidation.Name = "chkRunEnumValidation";
            chkRunEnumValidation.Size = new Size(115, 21);
            chkRunEnumValidation.TabIndex = 1;
            chkRunEnumValidation.Text = "执行 Enum 验证";
            toolTip.SetToolTip(chkRunEnumValidation, "导出 Excel 后，自动将 .xml 中定义的 enum 和 bool 设为数据验证下拉。");
            chkRunEnumValidation.UseVisualStyleBackColor = true;
            chkRunEnumValidation.CheckedChanged += chkRunEnumValidation_CheckedChanged;
            // 
            // pnlSettings
            // 
            pnlSettings.Controls.Add(lblXmlFolder);
            pnlSettings.Controls.Add(txtXmlFolder);
            pnlSettings.Controls.Add(btnBrowseXmlFolder);
            pnlSettings.Controls.Add(btnOpenXmlFolder);
            pnlSettings.Controls.Add(lblTemplate);
            pnlSettings.Controls.Add(txtDesignTemplate);
            pnlSettings.Controls.Add(btnBrowseTemplate);
            pnlSettings.Controls.Add(btnOpenTemplate);
            pnlSettings.Dock = DockStyle.Top;
            pnlSettings.Location = new Point(0, 0);
            pnlSettings.Name = "pnlSettings";
            pnlSettings.Size = new Size(764, 80);
            pnlSettings.TabIndex = 0;
            // 
            // txtXmlFolder
            // 
            txtXmlFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtXmlFolder.Location = new Point(125, 9);
            txtXmlFolder.Name = "txtXmlFolder";
            txtXmlFolder.Size = new Size(518, 23);
            txtXmlFolder.TabIndex = 1;
            txtXmlFolder.TextChanged += txtXmlFolder_TextChanged;
            // 
            // btnBrowseXmlFolder
            // 
            btnBrowseXmlFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseXmlFolder.Location = new Point(677, 6);
            btnBrowseXmlFolder.Name = "btnBrowseXmlFolder";
            btnBrowseXmlFolder.Size = new Size(75, 28);
            btnBrowseXmlFolder.TabIndex = 2;
            btnBrowseXmlFolder.Text = "浏览...";
            btnBrowseXmlFolder.Click += btnBrowseXmlFolder_Click;
            // 
            // btnOpenXmlFolder
            // 
            btnOpenXmlFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenXmlFolder.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenXmlFolder.Location = new Point(647, 6);
            btnOpenXmlFolder.Name = "btnOpenXmlFolder";
            btnOpenXmlFolder.Size = new Size(28, 28);
            btnOpenXmlFolder.TabIndex = 6;
            btnOpenXmlFolder.Text = "📂";
            btnOpenXmlFolder.Click += btnOpenXmlFolder_Click;
            // 
            // txtDesignTemplate
            // 
            txtDesignTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDesignTemplate.Location = new Point(125, 45);
            txtDesignTemplate.Name = "txtDesignTemplate";
            txtDesignTemplate.Size = new Size(518, 23);
            txtDesignTemplate.TabIndex = 4;
            txtDesignTemplate.TextChanged += txtDesignTemplate_TextChanged;
            // 
            // btnBrowseTemplate
            // 
            btnBrowseTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseTemplate.Location = new Point(677, 44);
            btnBrowseTemplate.Name = "btnBrowseTemplate";
            btnBrowseTemplate.Size = new Size(75, 28);
            btnBrowseTemplate.TabIndex = 5;
            btnBrowseTemplate.Text = "浏览...";
            btnBrowseTemplate.Click += btnBrowseTemplate_Click;
            // 
            // btnOpenTemplate
            // 
            btnOpenTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenTemplate.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenTemplate.Location = new Point(647, 44);
            btnOpenTemplate.Name = "btnOpenTemplate";
            btnOpenTemplate.Size = new Size(28, 28);
            btnOpenTemplate.TabIndex = 7;
            btnOpenTemplate.Text = "📂";
            btnOpenTemplate.Click += btnOpenTemplate_Click;
            // 
            // pnlNaming
            // 
            pnlNaming.Controls.Add(lblNaming);
            pnlNaming.Controls.Add(pnlNamingGroup);
            pnlNaming.Controls.Add(lblFileName);
            pnlNaming.Controls.Add(txtPrefix);
            pnlNaming.Controls.Add(lblClassName);
            pnlNaming.Controls.Add(txtSuffix);
            pnlNaming.Dock = DockStyle.Top;
            pnlNaming.Location = new Point(0, 80);
            pnlNaming.Name = "pnlNaming";
            pnlNaming.Size = new Size(764, 70);
            pnlNaming.TabIndex = 1;
            // 
            // pnlNamingGroup
            // 
            pnlNamingGroup.Controls.Add(rdoNameAsIs);
            pnlNamingGroup.Controls.Add(rdoNameCamel);
            pnlNamingGroup.Controls.Add(rdoNameSnake);
            pnlNamingGroup.Location = new Point(76, 5);
            pnlNamingGroup.Name = "pnlNamingGroup";
            pnlNamingGroup.Size = new Size(339, 26);
            pnlNamingGroup.TabIndex = 1;
            // 
            // tabMode
            // 
            tabMode.Controls.Add(tabPageList);
            tabMode.Controls.Add(tabPageBatch);
            tabMode.Dock = DockStyle.Fill;
            tabMode.Location = new Point(0, 150);
            tabMode.Name = "tabMode";
            tabMode.SelectedIndex = 0;
            tabMode.Size = new Size(764, 150);
            tabMode.TabIndex = 2;
            tabMode.SelectedIndexChanged += tabMode_SelectedIndexChanged;
            // 
            // tabPageList
            // 
            tabPageList.Controls.Add(dgvClasses);
            tabPageList.Controls.Add(pnlListBar);
            tabPageList.Controls.Add(pnlListCommon);
            tabPageList.Location = new Point(4, 26);
            tabPageList.Name = "tabPageList";
            tabPageList.Padding = new Padding(3);
            tabPageList.Size = new Size(756, 120);
            tabPageList.TabIndex = 0;
            tabPageList.Text = "按列表";
            tabPageList.ToolTipText = "逐项勾选要导出的数据类，并可为每项单独指定目标 Excel 路径。";
            // 
            // dgvClasses
            // 
            dgvClasses.AllowUserToAddRows = false;
            dgvClasses.AllowUserToDeleteRows = false;
            dgvClasses.AllowUserToResizeRows = false;
            dgvClasses.BackgroundColor = SystemColors.Window;
            dgvClasses.Columns.AddRange(new DataGridViewColumn[] { colEnabled, colClassName, colSourceFile, colTargetPath, colBrowse, colClearPath });
            dgvClasses.Dock = DockStyle.Fill;
            dgvClasses.Location = new Point(3, 75);
            dgvClasses.Name = "dgvClasses";
            dgvClasses.RowHeadersVisible = false;
            dgvClasses.ScrollBars = ScrollBars.Vertical;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.Size = new Size(750, 42);
            dgvClasses.TabIndex = 2;
            dgvClasses.CellContentClick += dgvClasses_CellContentClick;
            dgvClasses.CellValueChanged += dgvClasses_CellValueChanged;
            dgvClasses.CurrentCellDirtyStateChanged += dgvClasses_CurrentCellDirtyStateChanged;
            // 
            // colEnabled
            // 
            colEnabled.HeaderText = "启用";
            colEnabled.Name = "colEnabled";
            colEnabled.Resizable = DataGridViewTriState.False;
            colEnabled.ToolTipText = "勾选后该数据类才会被导出。";
            colEnabled.Width = 40;
            // 
            // colClassName
            // 
            colClassName.HeaderText = "数据类名";
            colClassName.Name = "colClassName";
            colClassName.ReadOnly = true;
            colClassName.ToolTipText = "从 XML 定义中识别出的数据类名。";
            colClassName.Width = 160;
            // 
            // colSourceFile
            // 
            colSourceFile.HeaderText = "来源文件";
            colSourceFile.Name = "colSourceFile";
            colSourceFile.ReadOnly = true;
            colSourceFile.ToolTipText = "定义该数据类的 .xml 来源文件。";
            colSourceFile.Width = 150;
            // 
            // colTargetPath
            // 
            colTargetPath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTargetPath.HeaderText = "目标 Excel 路径";
            colTargetPath.Name = "colTargetPath";
            colTargetPath.ToolTipText = "该数据类导出 Excel 的目标路径；留空则使用上方“通用导出文件夹”。";
            // 
            // colBrowse
            // 
            colBrowse.HeaderText = "...";
            colBrowse.Name = "colBrowse";
            colBrowse.Resizable = DataGridViewTriState.False;
            colBrowse.Text = "...";
            colBrowse.UseColumnTextForButtonValue = true;
            colBrowse.Width = 32;
            // 
            // colClearPath
            // 
            colClearPath.HeaderText = "清空";
            colClearPath.Name = "colClearPath";
            colClearPath.Resizable = DataGridViewTriState.False;
            colClearPath.Text = "清空";
            colClearPath.ToolTipText = "清空该数据类的“目标 Excel 路径”，使其下次导出时改走通用导出文件夹。";
            colClearPath.UseColumnTextForButtonValue = true;
            colClearPath.Width = 60;
            // 
            // pnlListBar
            // 
            pnlListBar.Controls.Add(btnRefresh);
            pnlListBar.Controls.Add(btnSelectAll);
            pnlListBar.Controls.Add(btnDeselectAll);
            pnlListBar.Dock = DockStyle.Top;
            pnlListBar.Location = new Point(3, 39);
            pnlListBar.Name = "pnlListBar";
            pnlListBar.Size = new Size(750, 36);
            pnlListBar.TabIndex = 1;
            // 
            // btnSelectAll
            // 
            btnSelectAll.Location = new Point(94, 4);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(56, 26);
            btnSelectAll.TabIndex = 1;
            btnSelectAll.Text = "全选";
            btnSelectAll.Click += btnSelectAll_Click;
            // 
            // btnDeselectAll
            // 
            btnDeselectAll.Location = new Point(158, 4);
            btnDeselectAll.Name = "btnDeselectAll";
            btnDeselectAll.Size = new Size(72, 26);
            btnDeselectAll.TabIndex = 2;
            btnDeselectAll.Text = "取消全选";
            btnDeselectAll.Click += btnDeselectAll_Click;
            // 
            // pnlListCommon
            // 
            pnlListCommon.Controls.Add(chkListCommonFolder);
            pnlListCommon.Controls.Add(lblListTargetFolder);
            pnlListCommon.Controls.Add(txtListTargetFolder);
            pnlListCommon.Controls.Add(btnBrowseListFolder);
            pnlListCommon.Controls.Add(btnOpenListFolder);
            pnlListCommon.Dock = DockStyle.Top;
            pnlListCommon.Location = new Point(3, 3);
            pnlListCommon.Name = "pnlListCommon";
            pnlListCommon.Size = new Size(750, 36);
            pnlListCommon.TabIndex = 0;
            // 
            // txtListTargetFolder
            // 
            txtListTargetFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtListTargetFolder.Location = new Point(138, 5);
            txtListTargetFolder.Name = "txtListTargetFolder";
            txtListTargetFolder.Size = new Size(498, 23);
            txtListTargetFolder.TabIndex = 2;
            txtListTargetFolder.TextChanged += txtListTargetFolder_TextChanged;
            // 
            // btnBrowseListFolder
            // 
            btnBrowseListFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseListFolder.Location = new Point(670, 3);
            btnBrowseListFolder.Name = "btnBrowseListFolder";
            btnBrowseListFolder.Size = new Size(75, 28);
            btnBrowseListFolder.TabIndex = 2;
            btnBrowseListFolder.Text = "浏览...";
            btnBrowseListFolder.Click += btnBrowseListFolder_Click;
            // 
            // btnOpenListFolder
            // 
            btnOpenListFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenListFolder.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenListFolder.Location = new Point(640, 3);
            btnOpenListFolder.Name = "btnOpenListFolder";
            btnOpenListFolder.Size = new Size(28, 28);
            btnOpenListFolder.TabIndex = 3;
            btnOpenListFolder.Text = "📂";
            btnOpenListFolder.Click += btnOpenListFolder_Click;
            // 
            // tabPageBatch
            // 
            tabPageBatch.Controls.Add(pnlBatchTarget);
            tabPageBatch.Location = new Point(4, 26);
            tabPageBatch.Name = "tabPageBatch";
            tabPageBatch.Padding = new Padding(3);
            tabPageBatch.Size = new Size(756, 154);
            tabPageBatch.TabIndex = 1;
            tabPageBatch.Text = "批量导出";
            tabPageBatch.ToolTipText = "将所有数据类一次性导出到同一个目标文件夹。";
            // 
            // pnlBatchTarget
            // 
            pnlBatchTarget.Controls.Add(lblTargetFolder);
            pnlBatchTarget.Controls.Add(txtTargetFolder);
            pnlBatchTarget.Controls.Add(btnBrowseTargetFolder);
            pnlBatchTarget.Controls.Add(btnOpenTargetFolder);
            pnlBatchTarget.Dock = DockStyle.Top;
            pnlBatchTarget.Location = new Point(3, 3);
            pnlBatchTarget.Name = "pnlBatchTarget";
            pnlBatchTarget.Size = new Size(750, 36);
            pnlBatchTarget.TabIndex = 0;
            // 
            // txtTargetFolder
            // 
            txtTargetFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTargetFolder.Location = new Point(118, 5);
            txtTargetFolder.Name = "txtTargetFolder";
            txtTargetFolder.Size = new Size(518, 23);
            txtTargetFolder.TabIndex = 1;
            txtTargetFolder.TextChanged += txtTargetFolder_TextChanged;
            // 
            // btnBrowseTargetFolder
            // 
            btnBrowseTargetFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseTargetFolder.Location = new Point(670, 3);
            btnBrowseTargetFolder.Name = "btnBrowseTargetFolder";
            btnBrowseTargetFolder.Size = new Size(75, 28);
            btnBrowseTargetFolder.TabIndex = 2;
            btnBrowseTargetFolder.Text = "浏览...";
            btnBrowseTargetFolder.Click += btnBrowseTargetFolder_Click;
            // 
            // btnOpenTargetFolder
            // 
            btnOpenTargetFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenTargetFolder.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenTargetFolder.Location = new Point(640, 3);
            btnOpenTargetFolder.Name = "btnOpenTargetFolder";
            btnOpenTargetFolder.Size = new Size(28, 28);
            btnOpenTargetFolder.TabIndex = 3;
            btnOpenTargetFolder.Text = "📂";
            btnOpenTargetFolder.Click += btnOpenTargetFolder_Click;
            // 
            // pnlAction
            // 
            pnlAction.Controls.Add(btnExport);
            pnlAction.Controls.Add(btnCancel);
            pnlAction.Controls.Add(pbExport);
            pnlAction.Dock = DockStyle.Bottom;
            pnlAction.Location = new Point(0, 334);
            pnlAction.Name = "pnlAction";
            pnlAction.Size = new Size(764, 46);
            pnlAction.TabIndex = 3;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(130, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "■ 取消";
            btnCancel.Click += btnCancel_Click;
            // 
            // pbExport
            // 
            pbExport.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbExport.Location = new Point(218, 12);
            pbExport.Name = "pbExport";
            pbExport.Size = new Size(534, 22);
            pbExport.TabIndex = 2;
            pbExport.Visible = false;
            // 
            // pnlExportSettings
            // 
            pnlExportSettings.Controls.Add(chkRunEnumValidation);
            pnlExportSettings.Controls.Add(lblExportSettings);
            pnlExportSettings.Dock = DockStyle.Bottom;
            pnlExportSettings.Location = new Point(0, 300);
            pnlExportSettings.Name = "pnlExportSettings";
            pnlExportSettings.Size = new Size(764, 34);
            pnlExportSettings.TabIndex = 5;
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
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ContextMenuStrip = ctxLog;
            txtLog.Dock = DockStyle.Bottom;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Location = new Point(0, 380);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(764, 150);
            txtLog.TabIndex = 4;
            txtLog.Text = "";
            // 
            // ExcelExportTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabMode);
            Controls.Add(pnlExportSettings);
            Controls.Add(pnlAction);
            Controls.Add(txtLog);
            Controls.Add(pnlNaming);
            Controls.Add(pnlSettings);
            Name = "ExcelExportTab";
            Size = new Size(764, 530);
            pnlSettings.ResumeLayout(false);
            pnlSettings.PerformLayout();
            pnlNaming.ResumeLayout(false);
            pnlNaming.PerformLayout();
            pnlNamingGroup.ResumeLayout(false);
            pnlNamingGroup.PerformLayout();
            tabMode.ResumeLayout(false);
            tabPageList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvClasses).EndInit();
            pnlListBar.ResumeLayout(false);
            pnlListCommon.ResumeLayout(false);
            pnlListCommon.PerformLayout();
            tabPageBatch.ResumeLayout(false);
            pnlBatchTarget.ResumeLayout(false);
            pnlBatchTarget.PerformLayout();
            pnlAction.ResumeLayout(false);
            pnlExportSettings.ResumeLayout(false);
            pnlExportSettings.PerformLayout();
            ctxLog.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── 字段声明 ──────────────────────────────────────────────────────
        private ToolTip     toolTip               = null!;
        private Panel       pnlSettings           = null!;
        private Label       lblXmlFolder          = null!;
        private TextBox     txtXmlFolder          = null!;
        private Button      btnBrowseXmlFolder    = null!;
        private Button      btnOpenXmlFolder      = null!;
        private Label       lblTemplate           = null!;
        private TextBox     txtDesignTemplate     = null!;
        private Button      btnBrowseTemplate     = null!;
        private Button      btnOpenTemplate       = null!;

        private Panel       pnlNaming             = null!;
        private Label       lblNaming             = null!;
        private Panel       pnlNamingGroup        = null!;
        private RadioButton rdoNameAsIs           = null!;
        private RadioButton rdoNameCamel          = null!;
        private RadioButton rdoNameSnake          = null!;
        private Label       lblFileName           = null!;
        private TextBox     txtPrefix             = null!;
        private Label       lblClassName          = null!;
        private TextBox     txtSuffix             = null!;

        private TabControl  tabMode               = null!;
        private TabPage     tabPageList           = null!;
        private Panel       pnlListCommon         = null!;
        private CheckBox    chkListCommonFolder   = null!;
        private Label       lblListTargetFolder   = null!;
        private TextBox     txtListTargetFolder   = null!;
        private Button      btnBrowseListFolder   = null!;
        private Button      btnOpenListFolder     = null!;
        private Panel       pnlListBar            = null!;
        private Button      btnRefresh            = null!;
        private Button      btnSelectAll          = null!;
        private Button      btnDeselectAll        = null!;
        private DataGridView               dgvClasses    = null!;
        private DataGridViewCheckBoxColumn colEnabled    = null!;
        private DataGridViewTextBoxColumn  colClassName  = null!;
        private DataGridViewTextBoxColumn  colSourceFile = null!;
        private DataGridViewTextBoxColumn  colTargetPath = null!;
        private DataGridViewButtonColumn   colBrowse     = null!;
        private DataGridViewButtonColumn   colClearPath  = null!;

        private TabPage     tabPageBatch          = null!;
        private Panel       pnlBatchTarget        = null!;
        private Label       lblTargetFolder       = null!;
        private TextBox     txtTargetFolder       = null!;
        private Button      btnBrowseTargetFolder = null!;
        private Button      btnOpenTargetFolder   = null!;

        private Panel       pnlAction             = null!;
        private Button      btnExport             = null!;
        private Button      btnCancel               = null!;
        private ProgressBar pbExport              = null!;
        private Panel       pnlExportSettings     = null!;
        private Label       lblExportSettings     = null!;
        private CheckBox    chkRunEnumValidation  = null!;

        private ContextMenuStrip  ctxLog              = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog  = null!;
        private RichTextBox       txtLog              = null!;
    }
}
