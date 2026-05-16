namespace ConfigExcelEnhancer.UI
{
    partial class LubanTab
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
            pnlTop = new Panel();
            lblBatPath = new Label();
            txtBatPath = new TextBox();
            btnBrowseBat = new Button();
            btnReset = new Button();
            tabsCommands = new TabControl();
            pnlBottom = new Panel();
            btnSave = new Button();
            btnRun = new Button();
            btnCancel = new Button();
            txtLog = new RichTextBox();
            pnlTop.SuspendLayout();
            pnlBottom.SuspendLayout();
            SuspendLayout();

            // pnlTop
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 38;
            pnlTop.Controls.Add(lblBatPath);
            pnlTop.Controls.Add(txtBatPath);
            pnlTop.Controls.Add(btnBrowseBat);

            // lblBatPath
            lblBatPath.AutoSize = true;
            lblBatPath.Location = new Point(8, 11);
            lblBatPath.Text = "gen.bat：";

            // txtBatPath
            txtBatPath.Location = new Point(72, 8);
            txtBatPath.Size = new Size(480, 23);
            txtBatPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBatPath.ReadOnly = true;

            // btnBrowseBat
            btnBrowseBat.Text = "浏览...";
            btnBrowseBat.Location = new Point(560, 7);
            btnBrowseBat.Size = new Size(75, 25);
            btnBrowseBat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseBat.Click += btnBrowseBat_Click;

            // tabsCommands
            tabsCommands.Dock = DockStyle.Fill;

            // pnlBottom
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 188;
            pnlBottom.Controls.Add(btnSave);
            pnlBottom.Controls.Add(btnRun);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Controls.Add(btnReset);
            pnlBottom.Controls.Add(txtLog);

            // btnSave
            btnSave.Text = "保存配置";
            btnSave.Location = new Point(8, 8);
            btnSave.Size = new Size(100, 28);
            btnSave.Enabled = false;
            btnSave.Click += btnSave_Click;

            // btnRun
            btnRun.Text = "执行导表";
            btnRun.Location = new Point(116, 8);
            btnRun.Size = new Size(100, 28);
            btnRun.Click += btnRun_Click;

            // btnCancel
            btnCancel.Text = "取消";
            btnCancel.Location = new Point(224, 8);
            btnCancel.Size = new Size(75, 28);
            btnCancel.Enabled = false;
            btnCancel.Click += btnCancel_Click;

            // btnReset
            btnReset.Text = "重置";
            btnReset.Size = new Size(75, 28);
            btnReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReset.Location = new Point(572, 8);
            btnReset.Enabled = false;
            btnReset.Click += btnReset_Click;

            // txtLog
            txtLog.Location = new Point(8, 44);
            txtLog.Size = new Size(631, 136);
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.ReadOnly = true;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Font = new Font("Consolas", 9f);
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;

            // LubanTab
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabsCommands);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Size = new Size(655, 415);
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel pnlTop = null!;
        private Label lblBatPath = null!;
        private TextBox txtBatPath = null!;
        private Button btnBrowseBat = null!;
        private TabControl tabsCommands = null!;
        private Panel pnlBottom = null!;
        private Button btnSave = null!;
        private Button btnRun = null!;
        private Button btnCancel = null!;
        private Button btnReset = null!;
        private RichTextBox txtLog = null!;
    }
}
