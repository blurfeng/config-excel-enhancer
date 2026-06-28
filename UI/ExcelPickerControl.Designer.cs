namespace ConfigExcelEnhancer.UI
{
    partial class ExcelPickerControl
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
            lblTitle = new Label();
            pnlRadioGroup = new Panel();
            rdoDirectory = new RadioButton();
            rdoList = new RadioButton();
            pnlDirMode = new Panel();
            txtDir = new TextBox();
            btnOpenDir = new Button();
            btnBrowse = new Button();
            pnlListMode = new Panel();
            lstFiles = new ListBox();
            btnAdd = new Button();
            btnRemove = new Button();
            btnClear = new Button();
            pnlRadioGroup.SuspendLayout();
            pnlDirMode.SuspendLayout();
            pnlListMode.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(0, 8);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(77, 17);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "目标 Excel：";
            // 
            // pnlRadioGroup
            // 
            pnlRadioGroup.Controls.Add(rdoDirectory);
            pnlRadioGroup.Controls.Add(rdoList);
            pnlRadioGroup.Location = new Point(83, 4);
            pnlRadioGroup.Name = "pnlRadioGroup";
            pnlRadioGroup.Size = new Size(160, 26);
            pnlRadioGroup.TabIndex = 1;
            // 
            // rdoDirectory
            // 
            rdoDirectory.AutoSize = true;
            rdoDirectory.Checked = true;
            rdoDirectory.Location = new Point(4, 3);
            rdoDirectory.Name = "rdoDirectory";
            rdoDirectory.Size = new Size(50, 21);
            rdoDirectory.TabIndex = 0;
            rdoDirectory.TabStop = true;
            rdoDirectory.Text = "目录";
            rdoDirectory.CheckedChanged += rdoDirectory_CheckedChanged;
            // 
            // rdoList
            // 
            rdoList.AutoSize = true;
            rdoList.Location = new Point(70, 3);
            rdoList.Name = "rdoList";
            rdoList.Size = new Size(50, 21);
            rdoList.TabIndex = 1;
            rdoList.Text = "列表";
            rdoList.CheckedChanged += rdoList_CheckedChanged;
            // 
            // pnlDirMode
            // 
            pnlDirMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDirMode.Controls.Add(txtDir);
            pnlDirMode.Controls.Add(btnOpenDir);
            pnlDirMode.Controls.Add(btnBrowse);
            pnlDirMode.Location = new Point(0, 34);
            pnlDirMode.Name = "pnlDirMode";
            pnlDirMode.Size = new Size(641, 102);
            pnlDirMode.TabIndex = 2;
            // 
            // txtDir
            // 
            txtDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDir.Location = new Point(0, 4);
            txtDir.Name = "txtDir";
            txtDir.Size = new Size(532, 23);
            txtDir.TabIndex = 0;
            txtDir.TextChanged += txtDir_TextChanged;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(536, 2);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 28);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "浏览...";
            btnBrowse.Click += btnBrowse_Click;
            //
            // btnOpenDir
            //
            btnOpenDir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenDir.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenDir.Location = new Point(613, 2);
            btnOpenDir.Name = "btnOpenDir";
            btnOpenDir.Size = new Size(28, 28);
            btnOpenDir.TabIndex = 2;
            btnOpenDir.Text = "📂";
            btnOpenDir.Click += btnOpenDir_Click;
            // 
            // pnlListMode
            // 
            pnlListMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlListMode.Controls.Add(lstFiles);
            pnlListMode.Controls.Add(btnAdd);
            pnlListMode.Controls.Add(btnRemove);
            pnlListMode.Controls.Add(btnClear);
            pnlListMode.Location = new Point(0, 34);
            pnlListMode.Name = "pnlListMode";
            pnlListMode.Size = new Size(641, 102);
            pnlListMode.TabIndex = 3;
            pnlListMode.Visible = false;
            // 
            // lstFiles
            // 
            lstFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFiles.HorizontalScrollbar = true;
            lstFiles.Location = new Point(0, 3);
            lstFiles.Name = "lstFiles";
            lstFiles.SelectionMode = SelectionMode.MultiSimple;
            lstFiles.Size = new Size(553, 89);
            lstFiles.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Location = new Point(566, 2);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 28);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "添加...";
            btnAdd.Click += btnAdd_Click;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemove.Location = new Point(566, 33);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 28);
            btnRemove.TabIndex = 2;
            btnRemove.Text = "移除选中";
            btnRemove.Click += btnRemove_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClear.Location = new Point(566, 64);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 28);
            btnClear.TabIndex = 3;
            btnClear.Text = "清空";
            btnClear.Click += btnClear_Click;
            // 
            // ExcelPickerControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTitle);
            Controls.Add(pnlRadioGroup);
            Controls.Add(pnlDirMode);
            Controls.Add(pnlListMode);
            Name = "ExcelPickerControl";
            Size = new Size(641, 136);
            pnlRadioGroup.ResumeLayout(false);
            pnlRadioGroup.PerformLayout();
            pnlDirMode.ResumeLayout(false);
            pnlDirMode.PerformLayout();
            pnlListMode.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle = null!;
        private Panel pnlRadioGroup = null!;
        private RadioButton rdoDirectory = null!;
        private RadioButton rdoList = null!;
        private Panel pnlDirMode = null!;
        private TextBox txtDir = null!;
        private Button btnOpenDir = null!;
        private Button btnBrowse = null!;
        private Panel pnlListMode = null!;
        private ListBox lstFiles = null!;
        private Button btnAdd = null!;
        private Button btnRemove = null!;
        private Button btnClear = null!;
    }
}
