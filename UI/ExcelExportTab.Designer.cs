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
            pnlSettings = new Panel();
            lblXmlFolder = new Label();
            txtXmlFolder = new TextBox();
            btnBrowseXmlFolder = new Button();
            lblTemplate = new Label();
            txtDesignTemplate = new TextBox();
            btnBrowseTemplate = new Button();
            lblMode = new Label();
            pnlModeGroup = new Panel();
            rdoList = new RadioButton();
            rdoBatch = new RadioButton();
            pnlListBar = new Panel();
            btnRefresh = new Button();
            btnSelectAll = new Button();
            btnDeselectAll = new Button();
            pnlBatchExtra = new Panel();
            lblTargetFolder = new Label();
            txtTargetFolder = new TextBox();
            btnBrowseTargetFolder = new Button();
            lblFileName = new Label();
            txtPrefix = new TextBox();
            lblClassName = new Label();
            txtSuffix = new TextBox();
            dgvClasses = new DataGridView();
            colEnabled = new DataGridViewCheckBoxColumn();
            colClassName = new DataGridViewTextBoxColumn();
            colSourceFile = new DataGridViewTextBoxColumn();
            colTargetPath = new DataGridViewTextBoxColumn();
            colBrowse = new DataGridViewButtonColumn();
            pnlAction = new Panel();
            btnExport = new Button();
            btnStop = new Button();
            pbExport = new ProgressBar();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            txtLog = new RichTextBox();
            pnlSettings.SuspendLayout();
            pnlModeGroup.SuspendLayout();
            pnlListBar.SuspendLayout();
            pnlBatchExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClasses).BeginInit();
            pnlAction.SuspendLayout();
            ctxLog.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSettings
            // 
            pnlSettings.Controls.Add(lblXmlFolder);
            pnlSettings.Controls.Add(txtXmlFolder);
            pnlSettings.Controls.Add(btnBrowseXmlFolder);
            pnlSettings.Controls.Add(lblTemplate);
            pnlSettings.Controls.Add(txtDesignTemplate);
            pnlSettings.Controls.Add(btnBrowseTemplate);
            pnlSettings.Controls.Add(lblMode);
            pnlSettings.Controls.Add(pnlModeGroup);
            pnlSettings.Dock = DockStyle.Top;
            pnlSettings.Location = new Point(0, 0);
            pnlSettings.Name = "pnlSettings";
            pnlSettings.Size = new Size(764, 110);
            pnlSettings.TabIndex = 6;
            pnlSettings.Resize += pnlSettings_Resize;
            // 
            // lblXmlFolder
            // 
            lblXmlFolder.AutoSize = true;
            lblXmlFolder.Location = new Point(12, 14);
            lblXmlFolder.Name = "lblXmlFolder";
            lblXmlFolder.Size = new Size(110, 17);
            lblXmlFolder.TabIndex = 0;
            lblXmlFolder.Text = "XML 来源文件夹：";
            // 
            // txtXmlFolder
            // 
            txtXmlFolder.Location = new Point(120, 9);
            txtXmlFolder.Name = "txtXmlFolder";
            txtXmlFolder.Size = new Size(549, 23);
            txtXmlFolder.TabIndex = 1;
            txtXmlFolder.TextChanged += txtXmlFolder_TextChanged;
            // 
            // btnBrowseXmlFolder
            // 
            btnBrowseXmlFolder.Location = new Point(677, 6);
            btnBrowseXmlFolder.Name = "btnBrowseXmlFolder";
            btnBrowseXmlFolder.Size = new Size(75, 28);
            btnBrowseXmlFolder.TabIndex = 2;
            btnBrowseXmlFolder.Text = "浏览...";
            btnBrowseXmlFolder.Click += btnBrowseXmlFolder_Click;
            // 
            // lblTemplate
            // 
            lblTemplate.AutoSize = true;
            lblTemplate.Location = new Point(12, 50);
            lblTemplate.Name = "lblTemplate";
            lblTemplate.Size = new Size(101, 17);
            lblTemplate.TabIndex = 3;
            lblTemplate.Text = "Excel 设计模板：";
            // 
            // txtDesignTemplate
            // 
            txtDesignTemplate.Location = new Point(120, 45);
            txtDesignTemplate.Name = "txtDesignTemplate";
            txtDesignTemplate.Size = new Size(549, 23);
            txtDesignTemplate.TabIndex = 4;
            txtDesignTemplate.TextChanged += txtDesignTemplate_TextChanged;
            // 
            // btnBrowseTemplate
            // 
            btnBrowseTemplate.Location = new Point(677, 44);
            btnBrowseTemplate.Name = "btnBrowseTemplate";
            btnBrowseTemplate.Size = new Size(75, 28);
            btnBrowseTemplate.TabIndex = 5;
            btnBrowseTemplate.Text = "浏览...";
            btnBrowseTemplate.Click += btnBrowseTemplate_Click;
            // 
            // lblMode
            // 
            lblMode.AutoSize = true;
            lblMode.Location = new Point(12, 85);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(44, 17);
            lblMode.TabIndex = 7;
            lblMode.Text = "模式：";
            // 
            // pnlModeGroup
            // 
            pnlModeGroup.Controls.Add(rdoList);
            pnlModeGroup.Controls.Add(rdoBatch);
            pnlModeGroup.Location = new Point(55, 80);
            pnlModeGroup.Name = "pnlModeGroup";
            pnlModeGroup.Size = new Size(200, 26);
            pnlModeGroup.TabIndex = 8;
            // 
            // rdoList
            // 
            rdoList.AutoSize = true;
            rdoList.Checked = true;
            rdoList.Location = new Point(3, 3);
            rdoList.Name = "rdoList";
            rdoList.Size = new Size(62, 21);
            rdoList.TabIndex = 0;
            rdoList.TabStop = true;
            rdoList.Text = "按列表";
            rdoList.CheckedChanged += rdoList_CheckedChanged;
            // 
            // rdoBatch
            // 
            rdoBatch.AutoSize = true;
            rdoBatch.Location = new Point(70, 3);
            rdoBatch.Name = "rdoBatch";
            rdoBatch.Size = new Size(74, 21);
            rdoBatch.TabIndex = 1;
            rdoBatch.Text = "批量导出";
            rdoBatch.CheckedChanged += rdoBatch_CheckedChanged;
            // 
            // pnlListBar
            // 
            pnlListBar.Controls.Add(btnRefresh);
            pnlListBar.Controls.Add(btnSelectAll);
            pnlListBar.Controls.Add(btnDeselectAll);
            pnlListBar.Dock = DockStyle.Top;
            pnlListBar.Location = new Point(0, 180);
            pnlListBar.Name = "pnlListBar";
            pnlListBar.Size = new Size(764, 36);
            pnlListBar.TabIndex = 4;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(12, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 26);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "刷新列表";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnSelectAll
            // 
            btnSelectAll.Location = new Point(100, 4);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(56, 26);
            btnSelectAll.TabIndex = 1;
            btnSelectAll.Text = "全选";
            btnSelectAll.Click += btnSelectAll_Click;
            // 
            // btnDeselectAll
            // 
            btnDeselectAll.Location = new Point(164, 4);
            btnDeselectAll.Name = "btnDeselectAll";
            btnDeselectAll.Size = new Size(72, 26);
            btnDeselectAll.TabIndex = 2;
            btnDeselectAll.Text = "取消全选";
            btnDeselectAll.Click += btnDeselectAll_Click;
            // 
            // pnlBatchExtra
            // 
            pnlBatchExtra.Controls.Add(lblTargetFolder);
            pnlBatchExtra.Controls.Add(txtTargetFolder);
            pnlBatchExtra.Controls.Add(btnBrowseTargetFolder);
            pnlBatchExtra.Controls.Add(lblFileName);
            pnlBatchExtra.Controls.Add(txtPrefix);
            pnlBatchExtra.Controls.Add(lblClassName);
            pnlBatchExtra.Controls.Add(txtSuffix);
            pnlBatchExtra.Dock = DockStyle.Top;
            pnlBatchExtra.Location = new Point(0, 110);
            pnlBatchExtra.Name = "pnlBatchExtra";
            pnlBatchExtra.Size = new Size(764, 70);
            pnlBatchExtra.TabIndex = 5;
            pnlBatchExtra.Visible = false;
            pnlBatchExtra.Resize += pnlBatchExtra_Resize;
            // 
            // lblTargetFolder
            // 
            lblTargetFolder.AutoSize = true;
            lblTargetFolder.Location = new Point(12, 9);
            lblTargetFolder.Name = "lblTargetFolder";
            lblTargetFolder.Size = new Size(104, 17);
            lblTargetFolder.TabIndex = 0;
            lblTargetFolder.Text = "导出目标文件夹：";
            // 
            // txtTargetFolder
            // 
            txtTargetFolder.Location = new Point(120, 4);
            txtTargetFolder.Name = "txtTargetFolder";
            txtTargetFolder.Size = new Size(549, 23);
            txtTargetFolder.TabIndex = 1;
            txtTargetFolder.TextChanged += txtTargetFolder_TextChanged;
            // 
            // btnBrowseTargetFolder
            // 
            btnBrowseTargetFolder.Location = new Point(677, 3);
            btnBrowseTargetFolder.Name = "btnBrowseTargetFolder";
            btnBrowseTargetFolder.Size = new Size(75, 28);
            btnBrowseTargetFolder.TabIndex = 2;
            btnBrowseTargetFolder.Text = "浏览...";
            btnBrowseTargetFolder.Click += btnBrowseTargetFolder_Click;
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(12, 45);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(56, 17);
            lblFileName.TabIndex = 3;
            lblFileName.Text = "文件名：";
            // 
            // txtPrefix
            // 
            txtPrefix.Location = new Point(74, 42);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.PlaceholderText = "前缀（可空）";
            txtPrefix.Size = new Size(100, 23);
            txtPrefix.TabIndex = 4;
            txtPrefix.TextChanged += txtPrefix_TextChanged;
            // 
            // lblClassName
            // 
            lblClassName.AutoSize = true;
            lblClassName.Location = new Point(178, 45);
            lblClassName.Name = "lblClassName";
            lblClassName.Size = new Size(56, 17);
            lblClassName.TabIndex = 5;
            lblClassName.Text = "数据类名";
            // 
            // txtSuffix
            // 
            txtSuffix.Location = new Point(240, 42);
            txtSuffix.Name = "txtSuffix";
            txtSuffix.PlaceholderText = "后缀（可空）";
            txtSuffix.Size = new Size(100, 23);
            txtSuffix.TabIndex = 6;
            txtSuffix.TextChanged += txtSuffix_TextChanged;
            // 
            // dgvClasses
            // 
            dgvClasses.AllowUserToAddRows = false;
            dgvClasses.AllowUserToDeleteRows = false;
            dgvClasses.BackgroundColor = SystemColors.Window;
            dgvClasses.Columns.AddRange(new DataGridViewColumn[] { colEnabled, colClassName, colSourceFile, colTargetPath, colBrowse });
            dgvClasses.Dock = DockStyle.Fill;
            dgvClasses.Location = new Point(0, 216);
            dgvClasses.Name = "dgvClasses";
            dgvClasses.RowHeadersVisible = false;
            dgvClasses.ScrollBars = ScrollBars.Vertical;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.Size = new Size(764, 118);
            dgvClasses.TabIndex = 3;
            dgvClasses.CellContentClick += dgvClasses_CellContentClick;
            dgvClasses.CellValueChanged += dgvClasses_CellValueChanged;
            dgvClasses.CurrentCellDirtyStateChanged += dgvClasses_CurrentCellDirtyStateChanged;
            // 
            // colEnabled
            // 
            colEnabled.HeaderText = "启用";
            colEnabled.Name = "colEnabled";
            colEnabled.Resizable = DataGridViewTriState.False;
            colEnabled.Width = 40;
            // 
            // colClassName
            // 
            colClassName.HeaderText = "数据类名";
            colClassName.Name = "colClassName";
            colClassName.ReadOnly = true;
            colClassName.Width = 160;
            // 
            // colSourceFile
            // 
            colSourceFile.HeaderText = "来源文件";
            colSourceFile.Name = "colSourceFile";
            colSourceFile.ReadOnly = true;
            // 
            // colTargetPath
            // 
            colTargetPath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTargetPath.HeaderText = "目标 Excel 路径";
            colTargetPath.Name = "colTargetPath";
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
            // pnlAction
            // 
            pnlAction.Controls.Add(btnExport);
            pnlAction.Controls.Add(btnStop);
            pnlAction.Controls.Add(pbExport);
            pnlAction.Dock = DockStyle.Bottom;
            pnlAction.Location = new Point(0, 484);
            pnlAction.Name = "pnlAction";
            pnlAction.Size = new Size(764, 46);
            pnlAction.TabIndex = 2;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(12, 7);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(110, 32);
            btnExport.TabIndex = 0;
            btnExport.Text = "▶ 导出";
            btnExport.Click += btnExport_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(130, 7);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 32);
            btnStop.TabIndex = 1;
            btnStop.Text = "■ 停止";
            btnStop.Click += btnStop_Click;
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
            txtLog.Location = new Point(0, 334);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(764, 150);
            txtLog.TabIndex = 1;
            txtLog.Text = "";
            // 
            // ExcelExportTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvClasses);
            Controls.Add(txtLog);
            Controls.Add(pnlAction);
            Controls.Add(pnlListBar);
            Controls.Add(pnlBatchExtra);
            Controls.Add(pnlSettings);
            Name = "ExcelExportTab";
            Size = new Size(764, 530);
            pnlSettings.ResumeLayout(false);
            pnlSettings.PerformLayout();
            pnlModeGroup.ResumeLayout(false);
            pnlModeGroup.PerformLayout();
            pnlListBar.ResumeLayout(false);
            pnlBatchExtra.ResumeLayout(false);
            pnlBatchExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClasses).EndInit();
            pnlAction.ResumeLayout(false);
            ctxLog.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── 字段声明 ──────────────────────────────────────────────────────
        private Panel       pnlSettings        = null!;
        private Label       lblXmlFolder       = null!;
        private TextBox     txtXmlFolder       = null!;
        private Button      btnBrowseXmlFolder = null!;
        private Label       lblTemplate        = null!;
        private TextBox     txtDesignTemplate  = null!;
        private Button      btnBrowseTemplate  = null!;
        private Label       lblMode            = null!;
        private Panel       pnlModeGroup       = null!;
        private RadioButton rdoList            = null!;
        private RadioButton rdoBatch           = null!;

        private Panel  pnlListBar    = null!;
        private Button btnRefresh    = null!;
        private Button btnSelectAll  = null!;
        private Button btnDeselectAll = null!;

        private Panel   pnlBatchExtra         = null!;
        private Label   lblTargetFolder       = null!;
        private TextBox txtTargetFolder       = null!;
        private Button  btnBrowseTargetFolder = null!;
        private Label   lblFileName           = null!;
        private TextBox txtPrefix             = null!;
        private Label   lblClassName          = null!;
        private TextBox txtSuffix             = null!;

        private DataGridView               dgvClasses    = null!;
        private DataGridViewCheckBoxColumn colEnabled    = null!;
        private DataGridViewTextBoxColumn  colClassName  = null!;
        private DataGridViewTextBoxColumn  colSourceFile = null!;
        private DataGridViewTextBoxColumn  colTargetPath = null!;
        private DataGridViewButtonColumn   colBrowse     = null!;

        private Panel       pnlAction = null!;
        private Button      btnExport = null!;
        private Button      btnStop   = null!;
        private ProgressBar pbExport  = null!;

        private ContextMenuStrip  ctxLog              = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog  = null!;
        private RichTextBox       txtLog              = null!;
    }
}
