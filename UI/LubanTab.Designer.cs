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
            components = new System.ComponentModel.Container();
            pnlTop = new Panel();
            lblBatPath = new Label();
            txtBatPath = new TextBox();
            btnBrowseBat = new Button();
            tabsCommands = new TabControl();
            pnlBottom = new Panel();
            btnSave = new Button();
            btnRun = new Button();
            btnCancel = new Button();
            btnReset = new Button();
            pbRun = new ProgressBar();
            txtLog = new RichTextBox();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            pnlTop.SuspendLayout();
            pnlBottom.SuspendLayout();
            ctxLog.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(lblBatPath);
            pnlTop.Controls.Add(txtBatPath);
            pnlTop.Controls.Add(btnBrowseBat);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(665, 43);
            pnlTop.TabIndex = 3;
            // 
            // lblBatPath
            // 
            lblBatPath.AutoSize = true;
            lblBatPath.Location = new Point(12, 12);
            lblBatPath.Name = "lblBatPath";
            lblBatPath.Size = new Size(64, 17);
            lblBatPath.TabIndex = 0;
            lblBatPath.Text = "gen.bat：";
            // 
            // txtBatPath
            // 
            txtBatPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBatPath.Location = new Point(90, 9);
            txtBatPath.Name = "txtBatPath";
            txtBatPath.ReadOnly = true;
            txtBatPath.Size = new Size(480, 23);
            txtBatPath.TabIndex = 1;
            // 
            // btnBrowseBat
            // 
            btnBrowseBat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseBat.Location = new Point(578, 8);
            btnBrowseBat.Name = "btnBrowseBat";
            btnBrowseBat.Size = new Size(75, 28);
            btnBrowseBat.TabIndex = 2;
            btnBrowseBat.Text = "浏览...";
            btnBrowseBat.Click += btnBrowseBat_Click;
            // 
            // tabsCommands
            // 
            tabsCommands.Dock = DockStyle.Fill;
            tabsCommands.Location = new Point(0, 43);
            tabsCommands.Name = "tabsCommands";
            tabsCommands.SelectedIndex = 0;
            tabsCommands.Size = new Size(665, 217);
            tabsCommands.TabIndex = 1;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnSave);
            pnlBottom.Controls.Add(btnRun);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Controls.Add(btnReset);
            pnlBottom.Controls.Add(pbRun);
            pnlBottom.Controls.Add(txtLog);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 260);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(665, 210);
            pnlBottom.TabIndex = 2;
            // 
            // btnSave
            // 
            btnSave.Enabled = false;
            btnSave.Location = new Point(12, 7);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(110, 32);
            btnSave.TabIndex = 0;
            btnSave.Text = "保存配置";
            btnSave.Click += btnSave_Click;
            // 
            // btnRun
            // 
            btnRun.Location = new Point(130, 7);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(110, 32);
            btnRun.TabIndex = 1;
            btnRun.Text = "▶ 执行导表";
            btnRun.Click += btnRun_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(248, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "■ 取消";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnReset
            // 
            btnReset.Enabled = false;
            btnReset.Location = new Point(336, 7);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(80, 32);
            btnReset.TabIndex = 4;
            btnReset.Text = "重置";
            btnReset.Click += btnReset_Click;
            // 
            // pbRun
            // 
            pbRun.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbRun.Location = new Point(424, 11);
            pbRun.Name = "pbRun";
            pbRun.Size = new Size(229, 23);
            pbRun.TabIndex = 3;
            pbRun.Visible = false;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ContextMenuStrip = ctxLog;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Location = new Point(0, 50);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(665, 160);
            txtLog.TabIndex = 5;
            txtLog.Text = "";
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
            // LubanTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabsCommands);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Name = "LubanTab";
            Size = new Size(665, 470);
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlBottom.ResumeLayout(false);
            ctxLog.ResumeLayout(false);
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
        private ProgressBar pbRun = null!;
        private Button btnReset = null!;
        private ContextMenuStrip ctxLog = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog = null!;
        private RichTextBox txtLog = null!;
    }
}
