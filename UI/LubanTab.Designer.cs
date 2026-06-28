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
            toolTip = new ToolTip(components);
            pnlTop = new Panel();
            lblBatPath = new Label();
            txtBatPath = new TextBox();
            btnBrowseBat = new Button();
            btnOpenBat = new Button();
            pnlConfigActions = new Panel();
            btnRefresh = new Button();
            btnReset = new Button();
            btnWriteShared = new Button();
            tabsCommands = new TabControl();
            pnlBottom = new Panel();
            btnRun = new Button();
            btnCancel = new Button();
            pbRun = new ProgressBar();
            txtLog = new RichTextBox();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            pnlTop.SuspendLayout();
            pnlConfigActions.SuspendLayout();
            pnlBottom.SuspendLayout();
            ctxLog.SuspendLayout();
            SuspendLayout();
            //
            // toolTip
            //
            toolTip.AutoPopDelay = 8000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 200;
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(lblBatPath);
            pnlTop.Controls.Add(txtBatPath);
            pnlTop.Controls.Add(btnBrowseBat);
            pnlTop.Controls.Add(btnOpenBat);
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
            toolTip.SetToolTip(lblBatPath, "Luban 导表脚本（gen.bat）的路径，工具会解析其中的命令，供下方编辑参数后执行。");
            // 
            // txtBatPath
            // 
            txtBatPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBatPath.Location = new Point(90, 9);
            txtBatPath.Name = "txtBatPath";
            txtBatPath.ReadOnly = true;
            txtBatPath.Size = new Size(454, 23);
            txtBatPath.TabIndex = 1;
            // 
            // btnBrowseBat
            // 
            btnBrowseBat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseBat.Location = new Point(548, 8);
            btnBrowseBat.Name = "btnBrowseBat";
            btnBrowseBat.Size = new Size(75, 28);
            btnBrowseBat.TabIndex = 2;
            btnBrowseBat.Text = "浏览...";
            btnBrowseBat.Click += btnBrowseBat_Click;
            //
            // btnOpenBat
            //
            btnOpenBat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenBat.Font = new Font("Segoe UI Emoji", 9F);
            btnOpenBat.Location = new Point(625, 8);
            btnOpenBat.Name = "btnOpenBat";
            btnOpenBat.Size = new Size(28, 28);
            btnOpenBat.TabIndex = 3;
            btnOpenBat.Text = "📂";
            btnOpenBat.Click += btnOpenBat_Click;
            // 
            // pnlConfigActions
            // 
            pnlConfigActions.Controls.Add(btnRefresh);
            pnlConfigActions.Controls.Add(btnReset);
            pnlConfigActions.Controls.Add(btnWriteShared);
            pnlConfigActions.Dock = DockStyle.Top;
            pnlConfigActions.Location = new Point(0, 43);
            pnlConfigActions.Name = "pnlConfigActions";
            pnlConfigActions.Size = new Size(665, 40);
            pnlConfigActions.TabIndex = 4;
            //
            // btnRefresh
            //
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnRefresh.Font = new Font("Segoe UI Emoji", 9F);
            btnRefresh.Location = new Point(12, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(90, 28);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "🔄 刷新";
            toolTip.SetToolTip(btnRefresh, "按当前磁盘状态重新校验路径的红/绿显示（新建对应文件夹后点此即可更新，不重读 gen.bat、保留当前编辑）。");
            btnRefresh.Click += btnRefresh_Click;
            //
            // btnReset
            //
            btnReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReset.Enabled = false;
            btnReset.Location = new Point(405, 5);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(80, 28);
            btnReset.TabIndex = 1;
            btnReset.Text = "重置";
            toolTip.SetToolTip(btnReset, "清除本机的所有路径覆盖与未写入改动，还原为 gen.bat 的共享配置（点击后需确认）。");
            btnReset.Click += btnReset_Click;
            //
            // btnWriteShared
            //
            btnWriteShared.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnWriteShared.Enabled = false;
            btnWriteShared.Location = new Point(493, 5);
            btnWriteShared.Name = "btnWriteShared";
            btnWriteShared.Size = new Size(160, 28);
            btnWriteShared.TabIndex = 2;
            btnWriteShared.Text = "写入 gen.bat（共享）";
            toolTip.SetToolTip(btnWriteShared, "将当前本地配置（含路径覆盖）写入共享的 gen.bat（进版本控制，影响团队，点击后需确认）。");
            btnWriteShared.Click += btnWriteShared_Click;
            // 
            // tabsCommands
            // 
            tabsCommands.Dock = DockStyle.Fill;
            tabsCommands.Location = new Point(0, 83);
            tabsCommands.Name = "tabsCommands";
            tabsCommands.SelectedIndex = 0;
            tabsCommands.Size = new Size(665, 177);
            tabsCommands.TabIndex = 1;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnRun);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Controls.Add(pbRun);
            pnlBottom.Controls.Add(txtLog);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 260);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(665, 210);
            pnlBottom.TabIndex = 2;
            // 
            // btnRun
            // 
            btnRun.Location = new Point(12, 7);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(110, 32);
            btnRun.TabIndex = 1;
            btnRun.Text = "▶ 执行导表";
            toolTip.SetToolTip(btnRun, "以子进程运行 gen.bat 执行 Luban 导表，并在下方实时输出日志。");
            btnRun.Click += btnRun_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(130, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "■ 取消";
            btnCancel.Click += btnCancel_Click;
            // 
            // pbRun
            // 
            pbRun.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbRun.Location = new Point(218, 11);
            pbRun.Name = "pbRun";
            pbRun.Size = new Size(435, 23);
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
            Controls.Add(pnlConfigActions);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Name = "LubanTab";
            Size = new Size(665, 470);
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlConfigActions.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            ctxLog.ResumeLayout(false);
            ResumeLayout(false);
        }

        private ToolTip toolTip = null!;
        private Panel pnlTop = null!;
        private Label lblBatPath = null!;
        private TextBox txtBatPath = null!;
        private Button btnBrowseBat = null!;
        private Button btnOpenBat = null!;
        private Panel pnlConfigActions = null!;
        private Button btnRefresh = null!;
        private Button btnReset = null!;
        private Button btnWriteShared = null!;
        private TabControl tabsCommands = null!;
        private Panel pnlBottom = null!;
        private Button btnRun = null!;
        private Button btnCancel = null!;
        private ProgressBar pbRun = null!;
        private ContextMenuStrip ctxLog = null!;
        private ToolStripMenuItem ctxMenuItemClearLog = null!;
        private ToolStripMenuItem ctxMenuItemCopyLog = null!;
        private RichTextBox txtLog = null!;
    }
}
