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

            // chkHideEnumDataSheet
            chkHideEnumDataSheet.AutoSize = true;
            chkHideEnumDataSheet.Location = new Point(160, 86);
            chkHideEnumDataSheet.Text = "隐藏枚举数据表";
            chkHideEnumDataSheet.CheckedChanged += chkHideEnumDataSheet_CheckedChanged;

            // btnUpdate
            btnUpdate.Text = "更新 Enum 验证";
            btnUpdate.Location = new Point(12, 80);
            btnUpdate.Size = new Size(130, 30);
            btnUpdate.Click += btnUpdate_Click;

            // txtLog
            txtLog.Location = new Point(12, 120);
            txtLog.Size = new Size(641, 280);
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
        private RichTextBox txtLog = null!;
    }
}
