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
            toolTip = new ToolTip(components);
            pnlConfig = new Panel();
            lblXmlDir = new Label();
            txtXmlDir = new TextBox();
            btnBrowseXml = new Button();
            btnOpenXml = new Button();
            excelPicker = new ExcelPickerControl();
            grpOptions = new GroupBox();
            chkHideEnumDataSheet = new CheckBox();
            chkForceRewrite = new CheckBox();
            chkBoolValidation = new CheckBox();
            btnUpdate = new Button();
            btnCancel = new Button();
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
            // toolTip
            //
            toolTip.AutoPopDelay = 8000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 200;
            // 
            // pnlConfig
            // 
            pnlConfig.Controls.Add(lblXmlDir);
            pnlConfig.Controls.Add(txtXmlDir);
            pnlConfig.Controls.Add(btnBrowseXml);
            pnlConfig.Controls.Add(btnOpenXml);
            pnlConfig.Controls.Add(excelPicker);
            pnlConfig.Controls.Add(grpOptions);
            pnlConfig.Controls.Add(btnUpdate);
            pnlConfig.Controls.Add(btnCancel);
            pnlConfig.Controls.Add(pbUpdate);
            pnlConfig.Dock = DockStyle.Top;
            pnlConfig.Location = new Point(0, 0);
            pnlConfig.Name = "pnlConfig";
            pnlConfig.Size = new Size(665, 316);
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
            toolTip.SetToolTip(lblXmlDir, "Luban 的 .xml 数据定义（schema）所在目录，工具会从中扫描 enum 枚举与 bool 定义。");
            // 
            // txtXmlDir
            // 
            txtXmlDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtXmlDir.Location = new Point(141, 12);
            txtXmlDir.Name = "txtXmlDir";
            txtXmlDir.Size = new Size(403, 23);
            txtXmlDir.TabIndex = 2;
            txtXmlDir.TextChanged += txtXmlDir_TextChanged;
            // 
            // btnBrowseXml
            // 
            btnBrowseXml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseXml.Location = new Point(548, 9);
            btnBrowseXml.Name = "btnBrowseXml";
            btnBrowseXml.Size = new Size(75, 28);
            btnBrowseXml.TabIndex = 3;
            btnBrowseXml.Text = "浏览...";
            btnBrowseXml.Click += btnBrowseXml_Click;
            //
            // btnOpenXml
            //
            btnOpenXml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenXml.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenXml.Location = new Point(625, 9);
            btnOpenXml.Name = "btnOpenXml";
            btnOpenXml.Size = new Size(28, 28);
            btnOpenXml.TabIndex = 4;
            btnOpenXml.Text = "📂";
            btnOpenXml.Click += btnOpenXml_Click;
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
            grpOptions.Controls.Add(chkHideEnumDataSheet);
            grpOptions.Controls.Add(chkForceRewrite);
            grpOptions.Controls.Add(chkBoolValidation);
            grpOptions.Location = new Point(12, 190);
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
            toolTip.SetToolTip(chkHideEnumDataSheet, "将写入 Excel 的隐藏数据表 __enum_data 设为隐藏，避免干扰正常编辑（验证下拉仍可用）。");
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
            toolTip.SetToolTip(chkForceRewrite, "忽略 schema 哈希比对，强制重写所有 Excel。默认仅在 enum 定义变化时才写入，以减少不必要的文件改动（git 噪音）。");
            chkForceRewrite.CheckedChanged += chkForceRewrite_CheckedChanged;
            // 
            // chkBoolValidation
            // 
            chkBoolValidation.AutoSize = true;
            chkBoolValidation.Checked = true;
            chkBoolValidation.CheckState = CheckState.Checked;
            chkBoolValidation.Location = new Point(148, 22);
            chkBoolValidation.Name = "chkBoolValidation";
            chkBoolValidation.Size = new Size(111, 21);
            chkBoolValidation.TabIndex = 14;
            chkBoolValidation.Text = "布尔值数据验证";
            toolTip.SetToolTip(chkBoolValidation, "为 bool 类型的列额外添加 TRUE/FALSE 下拉数据验证。");
            chkBoolValidation.CheckedChanged += chkBoolValidation_CheckedChanged;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(12, 282);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(140, 32);
            btnUpdate.TabIndex = 8;
            btnUpdate.Text = "▶ 更新 Enum 验证";
            toolTip.SetToolTip(btnUpdate, "扫描 XML 定义，并向所选 Excel 中匹配的列注入 enum/bool 数据验证下拉。");
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(160, 282);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "■ 取消";
            btnCancel.Click += btnCancel_Click;
            // 
            // pbUpdate
            // 
            pbUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbUpdate.Location = new Point(250, 286);
            pbUpdate.Name = "pbUpdate";
            pbUpdate.Size = new Size(403, 23);
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
            txtLog.Location = new Point(0, 316);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(665, 154);
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

        private ToolTip toolTip = null!;
        private Panel pnlConfig = null!;
        private Label lblXmlDir = null!;
        private TextBox txtXmlDir = null!;
        private Button btnBrowseXml = null!;
        private Button btnOpenXml = null!;
        private ExcelPickerControl excelPicker = null!;
        private GroupBox grpOptions = null!;
        private CheckBox chkHideEnumDataSheet = null!;
        private CheckBox chkForceRewrite = null!;
        private CheckBox chkBoolValidation = null!;
        private Button btnUpdate = null!;
        private Button btnCancel = null!;
        private ProgressBar pbUpdate = null!;
        private ContextMenuStrip ctxLog = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog = null!;
        private RichTextBox txtLog = null!;
        private SaveFileDialog saveFileDialog1;
    }
}
