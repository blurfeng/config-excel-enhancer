namespace ConfigExcelEnhancer.UI
{
    partial class TableDesignTab
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
            pnlConfig = new Panel();
            lblSourceExcel = new Label();
            txtSourceExcel = new TextBox();
            btnBrowseSource = new Button();
            lblTargetMode = new Label();
            pnlModeGroup = new Panel();
            rdoDirectory = new RadioButton();
            rdoList = new RadioButton();
            pnlDirMode = new Panel();
            txtTargetDir = new TextBox();
            btnBrowseTargetDir = new Button();
            pnlListMode = new Panel();
            lstTargetFiles = new ListBox();
            btnAddFiles = new Button();
            btnRemoveFiles = new Button();
            btnClearFiles = new Button();
            grpOptions = new GroupBox();
            chkIgnoreUnderscoreFiles = new CheckBox();
            lblSheetScope = new Label();
            pnlScopeGroup = new Panel();
            rdoScopeAll = new RadioButton();
            rdoScopeFirst = new RadioButton();
            chkIgnoreUnderscoreSheets = new CheckBox();
            lblHeaderSymbol = new Label();
            txtHeaderSymbol = new TextBox();
            chkAutoColumnWidth = new CheckBox();
            chkMergeHeaderCells = new CheckBox();
            lblMergeKeywords = new Label();
            txtMergeKeywords = new TextBox();
            btnApply = new Button();
            btnStop = new Button();
            pbApply = new ProgressBar();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            txtLog = new RichTextBox();
            pnlConfig.SuspendLayout();
            pnlModeGroup.SuspendLayout();
            pnlDirMode.SuspendLayout();
            pnlListMode.SuspendLayout();
            grpOptions.SuspendLayout();
            pnlScopeGroup.SuspendLayout();
            ctxLog.SuspendLayout();
            SuspendLayout();
            // 
            // pnlConfig
            // 
            pnlConfig.Controls.Add(lblSourceExcel);
            pnlConfig.Controls.Add(txtSourceExcel);
            pnlConfig.Controls.Add(btnBrowseSource);
            pnlConfig.Controls.Add(lblTargetMode);
            pnlConfig.Controls.Add(pnlModeGroup);
            pnlConfig.Controls.Add(pnlDirMode);
            pnlConfig.Controls.Add(pnlListMode);
            pnlConfig.Controls.Add(grpOptions);
            pnlConfig.Controls.Add(btnApply);
            pnlConfig.Controls.Add(btnStop);
            pnlConfig.Controls.Add(pbApply);
            pnlConfig.Dock = DockStyle.Top;
            pnlConfig.Location = new Point(0, 0);
            pnlConfig.Name = "pnlConfig";
            pnlConfig.Size = new Size(665, 382);
            pnlConfig.TabIndex = 0;
            // 
            // lblSourceExcel
            // 
            lblSourceExcel.AutoSize = true;
            lblSourceExcel.Location = new Point(12, 17);
            lblSourceExcel.Name = "lblSourceExcel";
            lblSourceExcel.Size = new Size(77, 17);
            lblSourceExcel.TabIndex = 1;
            lblSourceExcel.Text = "模板 Excel：";
            // 
            // txtSourceExcel
            // 
            txtSourceExcel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSourceExcel.Location = new Point(90, 14);
            txtSourceExcel.Name = "txtSourceExcel";
            txtSourceExcel.Size = new Size(480, 23);
            txtSourceExcel.TabIndex = 2;
            txtSourceExcel.TextChanged += txtSourceExcel_TextChanged;
            // 
            // btnBrowseSource
            // 
            btnBrowseSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseSource.Location = new Point(578, 12);
            btnBrowseSource.Name = "btnBrowseSource";
            btnBrowseSource.Size = new Size(75, 28);
            btnBrowseSource.TabIndex = 3;
            btnBrowseSource.Text = "浏览...";
            btnBrowseSource.Click += btnBrowseSource_Click;
            // 
            // lblTargetMode
            // 
            lblTargetMode.AutoSize = true;
            lblTargetMode.Location = new Point(12, 52);
            lblTargetMode.Name = "lblTargetMode";
            lblTargetMode.Size = new Size(56, 17);
            lblTargetMode.TabIndex = 4;
            lblTargetMode.Text = "目标表：";
            // 
            // pnlModeGroup
            // 
            pnlModeGroup.Controls.Add(rdoDirectory);
            pnlModeGroup.Controls.Add(rdoList);
            pnlModeGroup.Location = new Point(78, 45);
            pnlModeGroup.Name = "pnlModeGroup";
            pnlModeGroup.Size = new Size(210, 29);
            pnlModeGroup.TabIndex = 5;
            // 
            // rdoDirectory
            // 
            rdoDirectory.AutoSize = true;
            rdoDirectory.Checked = true;
            rdoDirectory.Location = new Point(4, 3);
            rdoDirectory.Name = "rdoDirectory";
            rdoDirectory.Size = new Size(74, 21);
            rdoDirectory.TabIndex = 0;
            rdoDirectory.TabStop = true;
            rdoDirectory.Text = "目录模式";
            rdoDirectory.CheckedChanged += rdoDirectory_CheckedChanged;
            // 
            // rdoList
            // 
            rdoList.AutoSize = true;
            rdoList.Location = new Point(94, 3);
            rdoList.Name = "rdoList";
            rdoList.Size = new Size(74, 21);
            rdoList.TabIndex = 1;
            rdoList.Text = "列表模式";
            rdoList.CheckedChanged += rdoList_CheckedChanged;
            // 
            // pnlDirMode
            // 
            pnlDirMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDirMode.Controls.Add(txtTargetDir);
            pnlDirMode.Controls.Add(btnBrowseTargetDir);
            pnlDirMode.Location = new Point(12, 82);
            pnlDirMode.Name = "pnlDirMode";
            pnlDirMode.Size = new Size(641, 102);
            pnlDirMode.TabIndex = 6;
            // 
            // txtTargetDir
            // 
            txtTargetDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTargetDir.Location = new Point(0, 4);
            txtTargetDir.Name = "txtTargetDir";
            txtTargetDir.Size = new Size(555, 23);
            txtTargetDir.TabIndex = 0;
            txtTargetDir.TextChanged += txtTargetDir_TextChanged;
            // 
            // btnBrowseTargetDir
            // 
            btnBrowseTargetDir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseTargetDir.Location = new Point(563, 2);
            btnBrowseTargetDir.Name = "btnBrowseTargetDir";
            btnBrowseTargetDir.Size = new Size(75, 28);
            btnBrowseTargetDir.TabIndex = 1;
            btnBrowseTargetDir.Text = "浏览...";
            btnBrowseTargetDir.Click += btnBrowseTargetDir_Click;
            // 
            // pnlListMode
            // 
            pnlListMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlListMode.Controls.Add(lstTargetFiles);
            pnlListMode.Controls.Add(btnAddFiles);
            pnlListMode.Controls.Add(btnRemoveFiles);
            pnlListMode.Controls.Add(btnClearFiles);
            pnlListMode.Location = new Point(12, 82);
            pnlListMode.Name = "pnlListMode";
            pnlListMode.Size = new Size(641, 102);
            pnlListMode.TabIndex = 7;
            pnlListMode.Visible = false;
            // 
            // lstTargetFiles
            // 
            lstTargetFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstTargetFiles.HorizontalScrollbar = true;
            lstTargetFiles.Location = new Point(0, 0);
            lstTargetFiles.Name = "lstTargetFiles";
            lstTargetFiles.SelectionMode = SelectionMode.MultiSimple;
            lstTargetFiles.Size = new Size(553, 89);
            lstTargetFiles.TabIndex = 0;
            // 
            // btnAddFiles
            // 
            btnAddFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddFiles.Location = new Point(561, -4);
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new Size(75, 28);
            btnAddFiles.TabIndex = 1;
            btnAddFiles.Text = "添加...";
            btnAddFiles.Click += btnAddFiles_Click;
            // 
            // btnRemoveFiles
            // 
            btnRemoveFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoveFiles.Location = new Point(561, 28);
            btnRemoveFiles.Name = "btnRemoveFiles";
            btnRemoveFiles.Size = new Size(75, 28);
            btnRemoveFiles.TabIndex = 2;
            btnRemoveFiles.Text = "移除选中";
            btnRemoveFiles.Click += btnRemoveFiles_Click;
            // 
            // btnClearFiles
            // 
            btnClearFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFiles.Location = new Point(561, 62);
            btnClearFiles.Name = "btnClearFiles";
            btnClearFiles.Size = new Size(75, 28);
            btnClearFiles.TabIndex = 3;
            btnClearFiles.Text = "清空";
            btnClearFiles.Click += btnClearFiles_Click;
            // 
            // grpOptions
            // 
            grpOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpOptions.Controls.Add(chkIgnoreUnderscoreFiles);
            grpOptions.Controls.Add(lblSheetScope);
            grpOptions.Controls.Add(pnlScopeGroup);
            grpOptions.Controls.Add(chkIgnoreUnderscoreSheets);
            grpOptions.Controls.Add(lblHeaderSymbol);
            grpOptions.Controls.Add(txtHeaderSymbol);
            grpOptions.Controls.Add(chkAutoColumnWidth);
            grpOptions.Controls.Add(chkMergeHeaderCells);
            grpOptions.Controls.Add(lblMergeKeywords);
            grpOptions.Controls.Add(txtMergeKeywords);
            grpOptions.Location = new Point(12, 190);
            grpOptions.Name = "grpOptions";
            grpOptions.Size = new Size(641, 140);
            grpOptions.TabIndex = 8;
            grpOptions.TabStop = false;
            grpOptions.Text = "选项";
            // 
            // chkIgnoreUnderscoreFiles
            // 
            chkIgnoreUnderscoreFiles.AutoSize = true;
            chkIgnoreUnderscoreFiles.Checked = true;
            chkIgnoreUnderscoreFiles.CheckState = CheckState.Checked;
            chkIgnoreUnderscoreFiles.Location = new Point(10, 22);
            chkIgnoreUnderscoreFiles.Name = "chkIgnoreUnderscoreFiles";
            chkIgnoreUnderscoreFiles.Size = new Size(121, 21);
            chkIgnoreUnderscoreFiles.TabIndex = 8;
            chkIgnoreUnderscoreFiles.Text = "忽略__开头的文件";
            chkIgnoreUnderscoreFiles.CheckedChanged += chkIgnoreUnderscoreFiles_CheckedChanged;
            // 
            // lblSheetScope
            // 
            lblSheetScope.Location = new Point(8, 50);
            lblSheetScope.Name = "lblSheetScope";
            lblSheetScope.Size = new Size(87, 21);
            lblSheetScope.TabIndex = 9;
            lblSheetScope.Text = "Sheet 范围：";
            lblSheetScope.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlScopeGroup
            // 
            pnlScopeGroup.Controls.Add(rdoScopeAll);
            pnlScopeGroup.Controls.Add(rdoScopeFirst);
            pnlScopeGroup.Location = new Point(92, 45);
            pnlScopeGroup.Name = "pnlScopeGroup";
            pnlScopeGroup.Size = new Size(121, 29);
            pnlScopeGroup.TabIndex = 10;
            // 
            // rdoScopeAll
            // 
            rdoScopeAll.AutoSize = true;
            rdoScopeAll.Checked = true;
            rdoScopeAll.Location = new Point(3, 3);
            rdoScopeAll.Name = "rdoScopeAll";
            rdoScopeAll.Size = new Size(50, 21);
            rdoScopeAll.TabIndex = 0;
            rdoScopeAll.TabStop = true;
            rdoScopeAll.Text = "所有";
            rdoScopeAll.CheckedChanged += rdoScopeAll_CheckedChanged;
            // 
            // rdoScopeFirst
            // 
            rdoScopeFirst.AutoSize = true;
            rdoScopeFirst.Location = new Point(54, 3);
            rdoScopeFirst.Name = "rdoScopeFirst";
            rdoScopeFirst.Size = new Size(62, 21);
            rdoScopeFirst.TabIndex = 1;
            rdoScopeFirst.Text = "第一张";
            rdoScopeFirst.CheckedChanged += rdoScopeFirst_CheckedChanged;
            // 
            // chkIgnoreUnderscoreSheets
            // 
            chkIgnoreUnderscoreSheets.AutoSize = true;
            chkIgnoreUnderscoreSheets.Checked = true;
            chkIgnoreUnderscoreSheets.CheckState = CheckState.Checked;
            chkIgnoreUnderscoreSheets.Location = new Point(230, 50);
            chkIgnoreUnderscoreSheets.Name = "chkIgnoreUnderscoreSheets";
            chkIgnoreUnderscoreSheets.Size = new Size(129, 21);
            chkIgnoreUnderscoreSheets.TabIndex = 11;
            chkIgnoreUnderscoreSheets.Text = "忽略__开头的Sheet";
            chkIgnoreUnderscoreSheets.CheckedChanged += chkIgnoreUnderscoreSheets_CheckedChanged;
            // 
            // lblHeaderSymbol
            // 
            lblHeaderSymbol.AutoSize = true;
            lblHeaderSymbol.Location = new Point(10, 78);
            lblHeaderSymbol.Name = "lblHeaderSymbol";
            lblHeaderSymbol.Size = new Size(68, 17);
            lblHeaderSymbol.TabIndex = 12;
            lblHeaderSymbol.Text = "表头符号：";
            // 
            // txtHeaderSymbol
            // 
            txtHeaderSymbol.Location = new Point(82, 75);
            txtHeaderSymbol.Name = "txtHeaderSymbol";
            txtHeaderSymbol.Size = new Size(65, 23);
            txtHeaderSymbol.TabIndex = 13;
            txtHeaderSymbol.Text = "##";
            txtHeaderSymbol.TextChanged += txtHeaderSymbol_TextChanged;
            // 
            // chkAutoColumnWidth
            // 
            chkAutoColumnWidth.AutoSize = true;
            chkAutoColumnWidth.Checked = true;
            chkAutoColumnWidth.CheckState = CheckState.Checked;
            chkAutoColumnWidth.Location = new Point(160, 78);
            chkAutoColumnWidth.Name = "chkAutoColumnWidth";
            chkAutoColumnWidth.Size = new Size(147, 21);
            chkAutoColumnWidth.TabIndex = 14;
            chkAutoColumnWidth.Text = "按列内容自动调整列宽";
            chkAutoColumnWidth.CheckedChanged += chkAutoColumnWidth_CheckedChanged;
            // 
            // chkMergeHeaderCells
            // 
            chkMergeHeaderCells.AutoSize = true;
            chkMergeHeaderCells.Checked = true;
            chkMergeHeaderCells.CheckState = CheckState.Checked;
            chkMergeHeaderCells.Location = new Point(10, 106);
            chkMergeHeaderCells.Name = "chkMergeHeaderCells";
            chkMergeHeaderCells.Size = new Size(135, 21);
            chkMergeHeaderCells.TabIndex = 15;
            chkMergeHeaderCells.Text = "合并表头空白单元格";
            chkMergeHeaderCells.CheckedChanged += chkMergeHeaderCells_CheckedChanged;
            // 
            // lblMergeKeywords
            // 
            lblMergeKeywords.AutoSize = true;
            lblMergeKeywords.Location = new Point(155, 109);
            lblMergeKeywords.Name = "lblMergeKeywords";
            lblMergeKeywords.Size = new Size(121, 17);
            lblMergeKeywords.TabIndex = 16;
            lblMergeKeywords.Text = "合并行标识符/行号：";
            // 
            // txtMergeKeywords
            // 
            txtMergeKeywords.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMergeKeywords.Location = new Point(283, 106);
            txtMergeKeywords.Name = "txtMergeKeywords";
            txtMergeKeywords.PlaceholderText = "##type,1,2（逗号或分号分隔）";
            txtMergeKeywords.Size = new Size(346, 23);
            txtMergeKeywords.TabIndex = 17;
            txtMergeKeywords.Text = "##type";
            txtMergeKeywords.TextChanged += txtMergeKeywords_TextChanged;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(12, 342);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(110, 32);
            btnApply.TabIndex = 18;
            btnApply.Text = "应用表设计";
            btnApply.Click += btnApply_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(130, 342);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 32);
            btnStop.TabIndex = 19;
            btnStop.Text = "停止";
            btnStop.Click += btnStop_Click;
            // 
            // pbApply
            // 
            pbApply.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbApply.Location = new Point(220, 347);
            pbApply.Name = "pbApply";
            pbApply.Size = new Size(433, 23);
            pbApply.TabIndex = 20;
            pbApply.Visible = false;
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
            txtLog.Dock = DockStyle.Fill;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Location = new Point(0, 382);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(665, 148);
            txtLog.TabIndex = 21;
            txtLog.Text = "";
            // 
            // TableDesignTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(txtLog);
            Controls.Add(pnlConfig);
            Name = "TableDesignTab";
            Size = new Size(665, 530);
            pnlConfig.ResumeLayout(false);
            pnlConfig.PerformLayout();
            pnlModeGroup.ResumeLayout(false);
            pnlModeGroup.PerformLayout();
            pnlDirMode.ResumeLayout(false);
            pnlDirMode.PerformLayout();
            pnlListMode.ResumeLayout(false);
            grpOptions.ResumeLayout(false);
            grpOptions.PerformLayout();
            pnlScopeGroup.ResumeLayout(false);
            pnlScopeGroup.PerformLayout();
            ctxLog.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel pnlConfig = null!;
        private Label lblSourceExcel = null!;
        private TextBox txtSourceExcel = null!;
        private Button btnBrowseSource = null!;
        private Label lblTargetMode = null!;
        private Panel pnlModeGroup = null!;
        private RadioButton rdoDirectory = null!;
        private RadioButton rdoList = null!;
        private Panel pnlDirMode = null!;
        private TextBox txtTargetDir = null!;
        private Button btnBrowseTargetDir = null!;
        private Panel pnlListMode = null!;
        private ListBox lstTargetFiles = null!;
        private Button btnAddFiles = null!;
        private Button btnRemoveFiles = null!;
        private Button btnClearFiles = null!;
        private GroupBox grpOptions = null!;
        private CheckBox chkIgnoreUnderscoreFiles = null!;
        private Label lblSheetScope = null!;
        private Panel pnlScopeGroup = null!;
        private RadioButton rdoScopeAll = null!;
        private RadioButton rdoScopeFirst = null!;
        private CheckBox chkIgnoreUnderscoreSheets = null!;
        private Label lblHeaderSymbol = null!;
        private TextBox txtHeaderSymbol = null!;
        private CheckBox chkAutoColumnWidth = null!;
        private CheckBox chkMergeHeaderCells = null!;
        private Label lblMergeKeywords = null!;
        private TextBox txtMergeKeywords = null!;
        private Button btnApply = null!;
        private Button btnStop = null!;
        private ProgressBar pbApply = null!;
        private ContextMenuStrip ctxLog = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog = null!;
        private RichTextBox txtLog = null!;
    }
}
