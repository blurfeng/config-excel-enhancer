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
            progressBar = new ProgressBar();
            ctxLog = new ContextMenuStrip();
            txtLog = new RichTextBox();
            pnlModeGroup.SuspendLayout();
            pnlDirMode.SuspendLayout();
            pnlListMode.SuspendLayout();
            pnlScopeGroup.SuspendLayout();
            SuspendLayout();

            // lblSourceExcel
            lblSourceExcel.AutoSize = true;
            lblSourceExcel.Location = new Point(12, 15);
            lblSourceExcel.Text = "来源 Excel：";

            // txtSourceExcel
            txtSourceExcel.Location = new Point(90, 12);
            txtSourceExcel.Size = new Size(480, 23);
            txtSourceExcel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSourceExcel.TextChanged += txtSourceExcel_TextChanged;

            // btnBrowseSource
            btnBrowseSource.Text = "浏览...";
            btnBrowseSource.Location = new Point(578, 11);
            btnBrowseSource.Size = new Size(75, 25);
            btnBrowseSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseSource.Click += btnBrowseSource_Click;

            // lblTargetMode
            lblTargetMode.AutoSize = true;
            lblTargetMode.Location = new Point(12, 46);
            lblTargetMode.Text = "目标表：";

            // pnlModeGroup — independent group for mode radios
            pnlModeGroup.Location = new Point(78, 40);
            pnlModeGroup.Size = new Size(210, 26);
            pnlModeGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pnlModeGroup.Controls.Add(rdoDirectory);
            pnlModeGroup.Controls.Add(rdoList);

            // rdoDirectory (inside pnlModeGroup; x=4 to avoid dot clipping)
            rdoDirectory.AutoSize = true;
            rdoDirectory.Location = new Point(4, 3);
            rdoDirectory.Text = "目录模式";
            rdoDirectory.Checked = true;
            rdoDirectory.CheckedChanged += rdoDirectory_CheckedChanged;

            // rdoList (inside pnlModeGroup)
            rdoList.AutoSize = true;
            rdoList.Location = new Point(94, 3);
            rdoList.Text = "列表模式";
            rdoList.CheckedChanged += rdoList_CheckedChanged;

            // pnlDirMode
            pnlDirMode.Location = new Point(12, 68);
            pnlDirMode.Size = new Size(641, 90);
            pnlDirMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDirMode.Controls.Add(txtTargetDir);
            pnlDirMode.Controls.Add(btnBrowseTargetDir);

            // txtTargetDir
            txtTargetDir.Location = new Point(0, 3);
            txtTargetDir.Size = new Size(555, 23);
            txtTargetDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTargetDir.TextChanged += txtTargetDir_TextChanged;

            // btnBrowseTargetDir
            btnBrowseTargetDir.Text = "浏览...";
            btnBrowseTargetDir.Location = new Point(563, 2);
            btnBrowseTargetDir.Size = new Size(75, 25);
            btnBrowseTargetDir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseTargetDir.Click += btnBrowseTargetDir_Click;

            // pnlListMode
            pnlListMode.Location = new Point(12, 68);
            pnlListMode.Size = new Size(641, 90);
            pnlListMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlListMode.Visible = false;
            pnlListMode.Controls.Add(lstTargetFiles);
            pnlListMode.Controls.Add(btnAddFiles);
            pnlListMode.Controls.Add(btnRemoveFiles);
            pnlListMode.Controls.Add(btnClearFiles);

            // lstTargetFiles
            lstTargetFiles.Location = new Point(0, 0);
            lstTargetFiles.Size = new Size(553, 87);
            lstTargetFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstTargetFiles.SelectionMode = SelectionMode.MultiSimple;
            lstTargetFiles.HorizontalScrollbar = true;

            // btnAddFiles
            btnAddFiles.Text = "添加...";
            btnAddFiles.Location = new Point(561, 0);
            btnAddFiles.Size = new Size(75, 27);
            btnAddFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddFiles.Click += btnAddFiles_Click;

            // btnRemoveFiles
            btnRemoveFiles.Text = "移除选中";
            btnRemoveFiles.Location = new Point(561, 30);
            btnRemoveFiles.Size = new Size(75, 27);
            btnRemoveFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoveFiles.Click += btnRemoveFiles_Click;

            // btnClearFiles
            btnClearFiles.Text = "清空";
            btnClearFiles.Location = new Point(561, 60);
            btnClearFiles.Size = new Size(75, 27);
            btnClearFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFiles.Click += btnClearFiles_Click;

            // chkIgnoreUnderscoreFiles
            chkIgnoreUnderscoreFiles.AutoSize = true;
            chkIgnoreUnderscoreFiles.Location = new Point(12, 163);
            chkIgnoreUnderscoreFiles.Text = "忽略__开头的文件";
            chkIgnoreUnderscoreFiles.Checked = true;
            chkIgnoreUnderscoreFiles.CheckedChanged += chkIgnoreUnderscoreFiles_CheckedChanged;

            // lblSheetScope — fixed width so it never overlaps pnlScopeGroup
            lblSheetScope.AutoSize = false;
            lblSheetScope.Size = new Size(90, 17);
            lblSheetScope.Location = new Point(12, 191);
            lblSheetScope.Text = "Sheet 范围：";

            // pnlScopeGroup — independent group for scope radios; starts after lblSheetScope
            pnlScopeGroup.Location = new Point(105, 185);
            pnlScopeGroup.Size = new Size(145, 26);
            pnlScopeGroup.Controls.Add(rdoScopeAll);
            pnlScopeGroup.Controls.Add(rdoScopeFirst);

            // rdoScopeAll (inside pnlScopeGroup; x=4 to avoid dot clipping)
            rdoScopeAll.AutoSize = true;
            rdoScopeAll.Location = new Point(4, 3);
            rdoScopeAll.Text = "所有";
            rdoScopeAll.Checked = true;
            rdoScopeAll.CheckedChanged += rdoScopeAll_CheckedChanged;

            // rdoScopeFirst (inside pnlScopeGroup)
            rdoScopeFirst.AutoSize = true;
            rdoScopeFirst.Location = new Point(55, 3);
            rdoScopeFirst.Text = "第一张";
            rdoScopeFirst.CheckedChanged += rdoScopeFirst_CheckedChanged;

            // chkIgnoreUnderscoreSheets
            chkIgnoreUnderscoreSheets.AutoSize = true;
            chkIgnoreUnderscoreSheets.Location = new Point(257, 188);
            chkIgnoreUnderscoreSheets.Text = "忽略__开头的Sheet";
            chkIgnoreUnderscoreSheets.Checked = true;
            chkIgnoreUnderscoreSheets.CheckedChanged += chkIgnoreUnderscoreSheets_CheckedChanged;

            // lblHeaderSymbol
            lblHeaderSymbol.AutoSize = true;
            lblHeaderSymbol.Location = new Point(395, 191);
            lblHeaderSymbol.Text = "表头符号：";

            // txtHeaderSymbol
            txtHeaderSymbol.Location = new Point(465, 188);
            txtHeaderSymbol.Size = new Size(65, 23);
            txtHeaderSymbol.Text = "##";
            txtHeaderSymbol.TextChanged += txtHeaderSymbol_TextChanged;

            // chkAutoColumnWidth
            chkAutoColumnWidth.AutoSize = true;
            chkAutoColumnWidth.Location = new Point(12, 218);
            chkAutoColumnWidth.Text = "按列内容自动调整列宽";
            chkAutoColumnWidth.Checked = true;
            chkAutoColumnWidth.CheckedChanged += chkAutoColumnWidth_CheckedChanged;

            // chkMergeHeaderCells
            chkMergeHeaderCells.AutoSize = true;
            chkMergeHeaderCells.Location = new Point(12, 244);
            chkMergeHeaderCells.Text = "合并表头空白单元格";
            chkMergeHeaderCells.Checked = true;
            chkMergeHeaderCells.CheckedChanged += chkMergeHeaderCells_CheckedChanged;

            // lblMergeKeywords
            lblMergeKeywords.AutoSize = true;
            lblMergeKeywords.Location = new Point(175, 247);
            lblMergeKeywords.Text = "合并行标识符/行号：";

            // txtMergeKeywords
            txtMergeKeywords.Location = new Point(303, 244);
            txtMergeKeywords.Size = new Size(182, 23);
            txtMergeKeywords.Text = "##type";
            txtMergeKeywords.PlaceholderText = "##type,1,2（逗号或分号分隔）";
            txtMergeKeywords.TextChanged += txtMergeKeywords_TextChanged;

            // btnApply
            btnApply.Text = "应用表设计";
            btnApply.Location = new Point(12, 276);
            btnApply.Size = new Size(120, 28);
            btnApply.Click += btnApply_Click;

            // btnStop
            btnStop.Text = "停止";
            btnStop.Location = new Point(140, 276);
            btnStop.Size = new Size(65, 28);
            btnStop.Enabled = false;
            btnStop.Click += btnStop_Click;

            // progressBar
            progressBar.Location = new Point(213, 280);
            progressBar.Size = new Size(436, 20);
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = false;

            // ctxLog
            var ctxClear = ctxLog.Items.Add("清空日志");
            var ctxCopy = ctxLog.Items.Add("复制全部");
            ctxClear.Click += btnClearLog_Click;
            ctxCopy.Click += btnCopyLog_Click;

            // txtLog
            txtLog.Location = new Point(12, 312);
            txtLog.Size = new Size(641, 150);
            txtLog.ContextMenuStrip = ctxLog;
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.ReadOnly = true;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Font = new Font("Consolas", 9f);
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;

            // TableDesignTab
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblSourceExcel);
            Controls.Add(txtSourceExcel);
            Controls.Add(btnBrowseSource);
            Controls.Add(lblTargetMode);
            Controls.Add(pnlModeGroup);
            Controls.Add(pnlDirMode);
            Controls.Add(pnlListMode);
            Controls.Add(chkIgnoreUnderscoreFiles);
            Controls.Add(lblSheetScope);
            Controls.Add(pnlScopeGroup);
            Controls.Add(chkIgnoreUnderscoreSheets);
            Controls.Add(lblHeaderSymbol);
            Controls.Add(txtHeaderSymbol);
            Controls.Add(chkAutoColumnWidth);
            Controls.Add(chkMergeHeaderCells);
            Controls.Add(lblMergeKeywords);
            Controls.Add(txtMergeKeywords);
            Controls.Add(btnApply);
            Controls.Add(btnStop);
            Controls.Add(progressBar);
            Controls.Add(txtLog);
            Size = new Size(665, 462);
            pnlModeGroup.ResumeLayout(false);
            pnlModeGroup.PerformLayout();
            pnlDirMode.ResumeLayout(false);
            pnlDirMode.PerformLayout();
            pnlListMode.ResumeLayout(false);
            pnlScopeGroup.ResumeLayout(false);
            pnlScopeGroup.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

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
        private ProgressBar progressBar = null!;
        private ContextMenuStrip ctxLog = null!;
        private RichTextBox txtLog = null!;
    }
}
