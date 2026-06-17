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
            btnOpenSource = new Button();
            excelPicker = new ExcelPickerControl();
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
            btnCancel = new Button();
            pbApply = new ProgressBar();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            txtLog = new RichTextBox();
            pnlConfig.SuspendLayout();
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
            pnlConfig.Controls.Add(btnOpenSource);
            pnlConfig.Controls.Add(excelPicker);
            pnlConfig.Controls.Add(grpOptions);
            pnlConfig.Controls.Add(btnApply);
            pnlConfig.Controls.Add(btnCancel);
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
            txtSourceExcel.Location = new Point(92, 11);
            txtSourceExcel.Name = "txtSourceExcel";
            txtSourceExcel.Size = new Size(452, 23);
            txtSourceExcel.TabIndex = 2;
            txtSourceExcel.TextChanged += txtSourceExcel_TextChanged;
            // 
            // btnBrowseSource
            // 
            btnBrowseSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseSource.Location = new Point(578, 9);
            btnBrowseSource.Name = "btnBrowseSource";
            btnBrowseSource.Size = new Size(75, 28);
            btnBrowseSource.TabIndex = 3;
            btnBrowseSource.Text = "浏览...";
            btnBrowseSource.Click += btnBrowseSource_Click;
            //
            // btnOpenSource
            //
            btnOpenSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenSource.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenSource.Location = new Point(548, 9);
            btnOpenSource.Name = "btnOpenSource";
            btnOpenSource.Size = new Size(28, 28);
            btnOpenSource.TabIndex = 4;
            btnOpenSource.Text = "📂";
            btnOpenSource.Click += btnOpenSource_Click;
            // 
            // excelPicker
            // 
            excelPicker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            excelPicker.Location = new Point(12, 45);
            excelPicker.Name = "excelPicker";
            excelPicker.Size = new Size(641, 136);
            excelPicker.TabIndex = 4;
            excelPicker.ValueChanged += excelPicker_ValueChanged;
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
            lblSheetScope.Location = new Point(10, 49);
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
            rdoScopeAll.Location = new Point(3, 4);
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
            rdoScopeFirst.Location = new Point(54, 4);
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
            lblHeaderSymbol.Location = new Point(10, 79);
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
            chkMergeHeaderCells.Location = new Point(10, 108);
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
            btnApply.Text = "▶ 应用表设计";
            btnApply.Click += btnApply_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(130, 342);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 19;
            btnCancel.Text = "■ 取消";
            btnCancel.Click += btnCancel_Click;
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
        private Button btnOpenSource = null!;
        private ExcelPickerControl excelPicker = null!;
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
        private Button btnCancel = null!;
        private ProgressBar pbApply = null!;
        private ContextMenuStrip ctxLog = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog = null!;
        private RichTextBox txtLog = null!;
    }
}
