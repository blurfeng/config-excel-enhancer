namespace ConfigExcelEnhancer.UI
{
    partial class EnumTab
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
            lblXmlDir = new Label();
            txtXmlDir = new TextBox();
            btnBrowseXml = new Button();
            lblExcelDir = new Label();
            txtExcelDir = new TextBox();
            btnBrowseExcel = new Button();
            grpOptions = new GroupBox();
            chkHideEnumDataSheet = new CheckBox();
            chkForceRewrite = new CheckBox();
            chkBoolValidation = new CheckBox();
            btnUpdate = new Button();
            btnStop = new Button();
            pbUpdate = new ProgressBar();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            txtLog = new RichTextBox();
            saveFileDialog1 = new SaveFileDialog();
            pnlConfig.SuspendLayout();
            grpOptions.SuspendLayout();
            ctxLog.SuspendLayout();
            SuspendLayout();
            // 
            // pnlConfig
            // 
            pnlConfig.Controls.Add(lblXmlDir);
            pnlConfig.Controls.Add(txtXmlDir);
            pnlConfig.Controls.Add(btnBrowseXml);
            pnlConfig.Controls.Add(lblExcelDir);
            pnlConfig.Controls.Add(txtExcelDir);
            pnlConfig.Controls.Add(btnBrowseExcel);
            pnlConfig.Controls.Add(grpOptions);
            pnlConfig.Controls.Add(btnUpdate);
            pnlConfig.Controls.Add(btnStop);
            pnlConfig.Controls.Add(pbUpdate);
            pnlConfig.Dock = DockStyle.Top;
            pnlConfig.Location = new Point(0, 0);
            pnlConfig.Name = "pnlConfig";
            pnlConfig.Size = new Size(665, 202);
            pnlConfig.TabIndex = 0;
            // 
            // lblXmlDir
            // 
            lblXmlDir.AutoSize = true;
            lblXmlDir.Location = new Point(12, 15);
            lblXmlDir.Name = "lblXmlDir";
            lblXmlDir.Size = new Size(126, 17);
            lblXmlDir.TabIndex = 1;
            lblXmlDir.Text = "数据定义 XML 目录：";
            // 
            // txtXmlDir
            // 
            txtXmlDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtXmlDir.Location = new Point(144, 12);
            txtXmlDir.Name = "txtXmlDir";
            txtXmlDir.Size = new Size(426, 23);
            txtXmlDir.TabIndex = 2;
            txtXmlDir.TextChanged += txtXmlDir_TextChanged;
            // 
            // btnBrowseXml
            // 
            btnBrowseXml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseXml.Location = new Point(578, 12);
            btnBrowseXml.Name = "btnBrowseXml";
            btnBrowseXml.Size = new Size(75, 28);
            btnBrowseXml.TabIndex = 3;
            btnBrowseXml.Text = "浏览...";
            btnBrowseXml.Click += btnBrowseXml_Click;
            // 
            // lblExcelDir
            // 
            lblExcelDir.AutoSize = true;
            lblExcelDir.Location = new Point(12, 48);
            lblExcelDir.Name = "lblExcelDir";
            lblExcelDir.Size = new Size(126, 17);
            lblExcelDir.TabIndex = 4;
            lblExcelDir.Text = "配置 Excel 目录：";
            // 
            // txtExcelDir
            // 
            txtExcelDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtExcelDir.Location = new Point(144, 45);
            txtExcelDir.Name = "txtExcelDir";
            txtExcelDir.Size = new Size(426, 23);
            txtExcelDir.TabIndex = 5;
            txtExcelDir.TextChanged += txtExcelDir_TextChanged;
            // 
            // btnBrowseExcel
            // 
            btnBrowseExcel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseExcel.Location = new Point(578, 45);
            btnBrowseExcel.Name = "btnBrowseExcel";
            btnBrowseExcel.Size = new Size(75, 28);
            btnBrowseExcel.TabIndex = 6;
            btnBrowseExcel.Text = "浏览...";
            btnBrowseExcel.Click += btnBrowseExcel_Click;
            // 
            // grpOptions
            // 
            grpOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpOptions.Controls.Add(chkHideEnumDataSheet);
            grpOptions.Controls.Add(chkForceRewrite);
            grpOptions.Controls.Add(chkBoolValidation);
            grpOptions.Location = new Point(12, 78);
            grpOptions.Name = "grpOptions";
            grpOptions.Size = new Size(641, 80);
            grpOptions.TabIndex = 13;
            grpOptions.TabStop = false;
            grpOptions.Text = "选项";
            // 
            // chkHideEnumDataSheet
            // 
            chkHideEnumDataSheet.AutoSize = true;
            chkHideEnumDataSheet.Location = new Point(10, 22);
            chkHideEnumDataSheet.Name = "chkHideEnumDataSheet";
            chkHideEnumDataSheet.Size = new Size(111, 21);
            chkHideEnumDataSheet.TabIndex = 7;
            chkHideEnumDataSheet.Text = "隐藏枚举数据表";
            chkHideEnumDataSheet.CheckedChanged += chkHideEnumDataSheet_CheckedChanged;
            // 
            // chkForceRewrite
            // 
            chkForceRewrite.AutoSize = true;
            chkForceRewrite.Location = new Point(10, 46);
            chkForceRewrite.Name = "chkForceRewrite";
            chkForceRewrite.Size = new Size(123, 21);
            chkForceRewrite.TabIndex = 12;
            chkForceRewrite.Text = "强制更新验证规则";
            chkForceRewrite.CheckedChanged += chkForceRewrite_CheckedChanged;
            // 
            // chkBoolValidation
            // 
            chkBoolValidation.AutoSize = true;
            chkBoolValidation.Checked = true;
            chkBoolValidation.CheckState = CheckState.Checked;
            chkBoolValidation.Location = new Point(310, 22);
            chkBoolValidation.Name = "chkBoolValidation";
            chkBoolValidation.Size = new Size(111, 21);
            chkBoolValidation.TabIndex = 14;
            chkBoolValidation.Text = "布尔值数据验证";
            chkBoolValidation.CheckedChanged += chkBoolValidation_CheckedChanged;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(12, 168);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(110, 32);
            btnUpdate.TabIndex = 8;
            btnUpdate.Text = "更新 Enum 验证";
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(130, 168);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 32);
            btnStop.TabIndex = 9;
            btnStop.Text = "停止";
            btnStop.Click += btnStop_Click;
            // 
            // pbUpdate
            // 
            pbUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbUpdate.Location = new Point(220, 172);
            pbUpdate.Name = "pbUpdate";
            pbUpdate.Size = new Size(433, 23);
            pbUpdate.TabIndex = 10;
            pbUpdate.Visible = false;
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
            txtLog.Location = new Point(0, 202);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(665, 268);
            txtLog.TabIndex = 11;
            txtLog.Text = "";
            // 
            // EnumTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(txtLog);
            Controls.Add(pnlConfig);
            Name = "EnumTab";
            Size = new Size(665, 470);
            pnlConfig.ResumeLayout(false);
            pnlConfig.PerformLayout();
            grpOptions.ResumeLayout(false);
            grpOptions.PerformLayout();
            ctxLog.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel pnlConfig = null!;
        private Label lblXmlDir = null!;
        private TextBox txtXmlDir = null!;
        private Button btnBrowseXml = null!;
        private Label lblExcelDir = null!;
        private TextBox txtExcelDir = null!;
        private Button btnBrowseExcel = null!;
        private GroupBox grpOptions = null!;
        private CheckBox chkHideEnumDataSheet = null!;
        private CheckBox chkForceRewrite = null!;
        private CheckBox chkBoolValidation = null!;
        private Button btnUpdate = null!;
        private Button btnStop = null!;
        private ProgressBar pbUpdate = null!;
        private ContextMenuStrip ctxLog = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog = null!;
        private RichTextBox txtLog = null!;
        private SaveFileDialog saveFileDialog1;
    }
}
