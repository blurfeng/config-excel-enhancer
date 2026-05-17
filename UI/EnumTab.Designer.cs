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
            lblXmlDir = new Label();
            txtXmlDir = new TextBox();
            btnBrowseXml = new Button();
            lblExcelDir = new Label();
            txtExcelDir = new TextBox();
            btnBrowseExcel = new Button();
            chkHideEnumDataSheet = new CheckBox();
            chkForceRewrite = new CheckBox();
            btnUpdate = new Button();
            btnStop = new Button();
            pbUpdate = new ProgressBar();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            txtLog = new RichTextBox();
            saveFileDialog1 = new SaveFileDialog();
            SuspendLayout();
            // 
            // lblXmlDir
            // 
            lblXmlDir.AutoSize = true;
            lblXmlDir.Location = new Point(12, 15);
            lblXmlDir.Name = "lblXmlDir";
            lblXmlDir.Size = new Size(74, 17);
            lblXmlDir.TabIndex = 1;
            lblXmlDir.Text = "XML 目录：";
            // 
            // txtXmlDir
            // 
            txtXmlDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtXmlDir.Location = new Point(90, 12);
            txtXmlDir.Name = "txtXmlDir";
            txtXmlDir.Size = new Size(480, 23);
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
            lblExcelDir.Size = new Size(77, 17);
            lblExcelDir.TabIndex = 4;
            lblExcelDir.Text = "Excel 目录：";
            // 
            // txtExcelDir
            // 
            txtExcelDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtExcelDir.Location = new Point(90, 45);
            txtExcelDir.Name = "txtExcelDir";
            txtExcelDir.Size = new Size(480, 23);
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
            // chkHideEnumDataSheet
            // 
            chkHideEnumDataSheet.AutoSize = true;
            chkHideEnumDataSheet.Location = new Point(12, 80);
            chkHideEnumDataSheet.Name = "chkHideEnumDataSheet";
            chkHideEnumDataSheet.Size = new Size(111, 21);
            chkHideEnumDataSheet.TabIndex = 7;
            chkHideEnumDataSheet.Text = "隐藏枚举数据表";
            chkHideEnumDataSheet.CheckedChanged += chkHideEnumDataSheet_CheckedChanged;
            // 
            // chkForceRewrite
            // 
            chkForceRewrite.AutoSize = true;
            chkForceRewrite.Location = new Point(12, 107);
            chkForceRewrite.Name = "chkForceRewrite";
            chkForceRewrite.Size = new Size(111, 21);
            chkForceRewrite.TabIndex = 12;
            chkForceRewrite.Text = "强制更新验证规则";
            chkForceRewrite.CheckedChanged += chkForceRewrite_CheckedChanged;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(12, 135);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(130, 32);
            btnUpdate.TabIndex = 8;
            btnUpdate.Text = "更新 Enum 验证";
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(150, 135);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 32);
            btnStop.TabIndex = 9;
            btnStop.Text = "停止";
            btnStop.Click += btnStop_Click;
            // 
            // pbUpdate
            // 
            pbUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbUpdate.Location = new Point(233, 139);
            pbUpdate.Name = "pbUpdate";
            pbUpdate.Size = new Size(415, 23);
            pbUpdate.TabIndex = 10;
            pbUpdate.Visible = false;
            // 
            // ctxLog
            //
            ctxLog.Items.AddRange(new ToolStripItem[] { ctxMenuItemClearLog, ctxMenuItemCopyLog });
            ctxLog.Name = "ctxLog";
            ctxLog.Size = new Size(100, 48);
            //
            // ctxMenuItemClearLog
            //
            ctxMenuItemClearLog.Name = "ctxMenuItemClearLog";
            ctxMenuItemClearLog.Size = new Size(99, 22);
            ctxMenuItemClearLog.Text = "清空";
            ctxMenuItemClearLog.Click += new EventHandler(btnClearLog_Click);
            //
            // ctxMenuItemCopyLog
            //
            ctxMenuItemCopyLog.Name = "ctxMenuItemCopyLog";
            ctxMenuItemCopyLog.Size = new Size(99, 22);
            ctxMenuItemCopyLog.Text = "复制";
            ctxMenuItemCopyLog.Click += new EventHandler(btnCopyLog_Click);
            //
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ContextMenuStrip = ctxLog;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Location = new Point(0, 178);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(665, 310);
            txtLog.TabIndex = 11;
            txtLog.Text = "";
            // 
            // EnumTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblXmlDir);
            Controls.Add(txtXmlDir);
            Controls.Add(btnBrowseXml);
            Controls.Add(lblExcelDir);
            Controls.Add(txtExcelDir);
            Controls.Add(btnBrowseExcel);
            Controls.Add(chkHideEnumDataSheet);
            Controls.Add(chkForceRewrite);
            Controls.Add(btnUpdate);
            Controls.Add(btnStop);
            Controls.Add(pbUpdate);
            Controls.Add(txtLog);
            Name = "EnumTab";
            Size = new Size(665, 470);
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblXmlDir = null!;
        private TextBox txtXmlDir = null!;
        private Button btnBrowseXml = null!;
        private Label lblExcelDir = null!;
        private TextBox txtExcelDir = null!;
        private Button btnBrowseExcel = null!;
        private CheckBox chkHideEnumDataSheet = null!;
        private CheckBox chkForceRewrite = null!;
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
