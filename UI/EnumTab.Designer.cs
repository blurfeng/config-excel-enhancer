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
            lblXmlDir = new Label();
            txtXmlDir = new TextBox();
            btnBrowseXml = new Button();
            lblExcelDir = new Label();
            txtExcelDir = new TextBox();
            btnBrowseExcel = new Button();
            chkHideEnumDataSheet = new CheckBox();
            btnUpdate = new Button();
            btnStop = new Button();
            pbUpdate = new ProgressBar();
            ctxLog = new ContextMenuStrip();
            txtLog = new RichTextBox();
            SuspendLayout();

            // lblXmlDir
            lblXmlDir.AutoSize = true;
            lblXmlDir.Location = new Point(12, 15);
            lblXmlDir.Text = "XML 目录：";

            // txtXmlDir
            txtXmlDir.Location = new Point(90, 12);
            txtXmlDir.Size = new Size(480, 23);
            txtXmlDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtXmlDir.TextChanged += txtXmlDir_TextChanged;

            // btnBrowseXml
            btnBrowseXml.Text = "浏览...";
            btnBrowseXml.Location = new Point(578, 11);
            btnBrowseXml.Size = new Size(75, 25);
            btnBrowseXml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseXml.Click += btnBrowseXml_Click;

            // lblExcelDir
            lblExcelDir.AutoSize = true;
            lblExcelDir.Location = new Point(12, 48);
            lblExcelDir.Text = "Excel 目录：";

            // txtExcelDir
            txtExcelDir.Location = new Point(90, 45);
            txtExcelDir.Size = new Size(480, 23);
            txtExcelDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtExcelDir.TextChanged += txtExcelDir_TextChanged;

            // btnBrowseExcel
            btnBrowseExcel.Text = "浏览...";
            btnBrowseExcel.Location = new Point(578, 44);
            btnBrowseExcel.Size = new Size(75, 25);
            btnBrowseExcel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseExcel.Click += btnBrowseExcel_Click;

            // chkHideEnumDataSheet  （第三行，更新按钮上方）
            chkHideEnumDataSheet.AutoSize = true;
            chkHideEnumDataSheet.Location = new Point(12, 80);
            chkHideEnumDataSheet.Text = "隐藏枚举数据表";
            chkHideEnumDataSheet.CheckedChanged += chkHideEnumDataSheet_CheckedChanged;

            // btnUpdate  （第四行）
            btnUpdate.Text = "更新 Enum 验证";
            btnUpdate.Location = new Point(12, 108);
            btnUpdate.Size = new Size(130, 30);
            btnUpdate.Click += btnUpdate_Click;

            // btnStop
            btnStop.Text = "停止";
            btnStop.Location = new Point(150, 108);
            btnStop.Size = new Size(75, 30);
            btnStop.Enabled = false;
            btnStop.Click += btnStop_Click;

            // pbUpdate
            pbUpdate.Location = new Point(233, 111);
            pbUpdate.Size = new Size(415, 22);
            pbUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbUpdate.Minimum = 0;
            pbUpdate.Maximum = 100;
            pbUpdate.Value = 0;
            pbUpdate.Visible = false;

            // ctxLog
            var ctxClear = ctxLog.Items.Add("清空日志");
            var ctxCopy  = ctxLog.Items.Add("复制全部");
            ctxClear.Click += btnClearLog_Click;
            ctxCopy.Click  += btnCopyLog_Click;

            // txtLog
            txtLog.Location = new Point(12, 147);
            txtLog.Size = new Size(641, 253);
            txtLog.ContextMenuStrip = ctxLog;
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.ReadOnly = true;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Font = new Font("Consolas", 9f);
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;

            // EnumTab
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblXmlDir);
            Controls.Add(txtXmlDir);
            Controls.Add(btnBrowseXml);
            Controls.Add(lblExcelDir);
            Controls.Add(txtExcelDir);
            Controls.Add(btnBrowseExcel);
            Controls.Add(chkHideEnumDataSheet);
            Controls.Add(btnUpdate);
            Controls.Add(btnStop);
            Controls.Add(pbUpdate);
            Controls.Add(txtLog);
            Size = new Size(665, 412);
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
        private Button btnUpdate = null!;
        private Button btnStop = null!;
        private ProgressBar pbUpdate = null!;
        private ContextMenuStrip ctxLog = null!;
        private RichTextBox txtLog = null!;
    }
}
