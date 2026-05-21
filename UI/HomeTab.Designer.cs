namespace ConfigExcelEnhancer.UI
{
    partial class HomeTab
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
            btnOneClick = new Button();
            btnLubanOnly = new Button();
            btnCancel = new Button();
            chkIncludeEnum = new CheckBox();
            btnGoLuban = new Button();
            btnGoTemplate1 = new Button();
            btnGoTemplate2 = new Button();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            pbOverall = new ProgressBar();
            pnlActions = new Panel();
            grpStatus = new GroupBox();
            lblGenBatDot = new Label();
            lblGenBat = new Label();
            lblTemplateJobsDot = new Label();
            lblTemplateJobs = new Label();
            lblTablesCsDot = new Label();
            lblTablesCs = new Label();
            lblLastExportDot = new Label();
            lblLastExport = new Label();
            grpLog = new GroupBox();
            txtLog = new RichTextBox();
            ctxLog.SuspendLayout();
            pnlActions.SuspendLayout();
            grpStatus.SuspendLayout();
            grpLog.SuspendLayout();
            SuspendLayout();
            // 
            // btnOneClick
            // 
            btnOneClick.Location = new Point(8, 10);
            btnOneClick.Name = "btnOneClick";
            btnOneClick.Size = new Size(120, 32);
            btnOneClick.TabIndex = 0;
            btnOneClick.Text = "▶ 一键导出";
            toolTip.SetToolTip(btnOneClick, "按顺序执行：Enum 验证（可选）→ Luban 导表 → 导出模板类");
            btnOneClick.UseVisualStyleBackColor = true;
            btnOneClick.Click += btnOneClick_Click;
            // 
            // btnLubanOnly
            // 
            btnLubanOnly.Location = new Point(136, 10);
            btnLubanOnly.Name = "btnLubanOnly";
            btnLubanOnly.Size = new Size(100, 32);
            btnLubanOnly.TabIndex = 1;
            btnLubanOnly.Text = "▶ 仅导表";
            toolTip.SetToolTip(btnLubanOnly, "仅执行 Luban 导表为 JSON 数据（根据配置），跳过模板类导出");
            btnLubanOnly.UseVisualStyleBackColor = true;
            btnLubanOnly.Click += btnLubanOnly_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(244, 10);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "■ 取消";
            toolTip.SetToolTip(btnCancel, "取消当前正在执行的任务");
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // chkIncludeEnum
            // 
            chkIncludeEnum.AutoSize = true;
            chkIncludeEnum.Checked = true;
            chkIncludeEnum.CheckState = CheckState.Checked;
            chkIncludeEnum.Location = new Point(13, 48);
            chkIncludeEnum.Name = "chkIncludeEnum";
            chkIncludeEnum.Size = new Size(115, 21);
            chkIncludeEnum.TabIndex = 4;
            chkIncludeEnum.Text = "包含 Enum 验证";
            toolTip.SetToolTip(chkIncludeEnum, "执行前先运行 Enum 验证，确保枚举值合法。运行快，建议保持勾选");
            chkIncludeEnum.UseVisualStyleBackColor = true;
            // 
            // btnGoLuban
            // 
            btnGoLuban.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGoLuban.Location = new Point(730, 22);
            btnGoLuban.Name = "btnGoLuban";
            btnGoLuban.Size = new Size(80, 23);
            btnGoLuban.TabIndex = 5;
            btnGoLuban.Text = "→ Luban";
            toolTip.SetToolTip(btnGoLuban, "跳转到 Luban 选项卡");
            btnGoLuban.UseVisualStyleBackColor = true;
            btnGoLuban.Click += btnGoLuban_Click;
            // 
            // btnGoTemplate1
            // 
            btnGoTemplate1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGoTemplate1.Location = new Point(700, 50);
            btnGoTemplate1.Name = "btnGoTemplate1";
            btnGoTemplate1.Size = new Size(110, 23);
            btnGoTemplate1.TabIndex = 6;
            btnGoTemplate1.Text = "→ 导出模板类";
            toolTip.SetToolTip(btnGoTemplate1, "跳转到导出模板类选项卡");
            btnGoTemplate1.UseVisualStyleBackColor = true;
            btnGoTemplate1.Click += btnGoTemplate_Click;
            // 
            // btnGoTemplate2
            // 
            btnGoTemplate2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGoTemplate2.Location = new Point(700, 78);
            btnGoTemplate2.Name = "btnGoTemplate2";
            btnGoTemplate2.Size = new Size(110, 23);
            btnGoTemplate2.TabIndex = 7;
            btnGoTemplate2.Text = "→ 导出模板类";
            toolTip.SetToolTip(btnGoTemplate2, "跳转到导出模板类选项卡");
            btnGoTemplate2.UseVisualStyleBackColor = true;
            btnGoTemplate2.Click += btnGoTemplate_Click;
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
            ctxMenuItemClearLog.Click += ctxMenuItemClearLog_Click;
            // 
            // ctxMenuItemCopyLog
            // 
            ctxMenuItemCopyLog.Name = "ctxMenuItemCopyLog";
            ctxMenuItemCopyLog.Size = new Size(100, 22);
            ctxMenuItemCopyLog.Text = "复制";
            ctxMenuItemCopyLog.Click += ctxMenuItemCopyLog_Click;
            // 
            // pbOverall
            // 
            pbOverall.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbOverall.Location = new Point(332, 16);
            pbOverall.Name = "pbOverall";
            pbOverall.Size = new Size(479, 20);
            pbOverall.TabIndex = 3;
            pbOverall.Visible = false;
            // 
            // pnlActions
            // 
            pnlActions.Controls.Add(btnOneClick);
            pnlActions.Controls.Add(btnLubanOnly);
            pnlActions.Controls.Add(btnCancel);
            pnlActions.Controls.Add(pbOverall);
            pnlActions.Controls.Add(chkIncludeEnum);
            pnlActions.Dock = DockStyle.Top;
            pnlActions.Location = new Point(0, 0);
            pnlActions.Name = "pnlActions";
            pnlActions.Size = new Size(819, 80);
            pnlActions.TabIndex = 2;
            // 
            // grpStatus
            // 
            grpStatus.Controls.Add(lblGenBatDot);
            grpStatus.Controls.Add(lblGenBat);
            grpStatus.Controls.Add(btnGoLuban);
            grpStatus.Controls.Add(lblTemplateJobsDot);
            grpStatus.Controls.Add(lblTemplateJobs);
            grpStatus.Controls.Add(btnGoTemplate1);
            grpStatus.Controls.Add(lblTablesCsDot);
            grpStatus.Controls.Add(lblTablesCs);
            grpStatus.Controls.Add(btnGoTemplate2);
            grpStatus.Controls.Add(lblLastExportDot);
            grpStatus.Controls.Add(lblLastExport);
            grpStatus.Dock = DockStyle.Top;
            grpStatus.Location = new Point(0, 80);
            grpStatus.Name = "grpStatus";
            grpStatus.Padding = new Padding(8);
            grpStatus.Size = new Size(819, 138);
            grpStatus.TabIndex = 1;
            grpStatus.TabStop = false;
            grpStatus.Text = "当前配置状态";
            // 
            // lblGenBatDot
            // 
            lblGenBatDot.AutoSize = true;
            lblGenBatDot.Font = new Font("Segoe UI", 10F);
            lblGenBatDot.ForeColor = Color.Gray;
            lblGenBatDot.Location = new Point(10, 25);
            lblGenBatDot.Name = "lblGenBatDot";
            lblGenBatDot.Size = new Size(17, 19);
            lblGenBatDot.TabIndex = 0;
            lblGenBatDot.Text = "●";
            // 
            // lblGenBat
            // 
            lblGenBat.AutoSize = true;
            lblGenBat.Location = new Point(28, 25);
            lblGenBat.Name = "lblGenBat";
            lblGenBat.Size = new Size(64, 17);
            lblGenBat.TabIndex = 1;
            lblGenBat.Text = "gen.bat：";
            // 
            // lblTemplateJobsDot
            // 
            lblTemplateJobsDot.AutoSize = true;
            lblTemplateJobsDot.Font = new Font("Segoe UI", 10F);
            lblTemplateJobsDot.ForeColor = Color.Gray;
            lblTemplateJobsDot.Location = new Point(10, 53);
            lblTemplateJobsDot.Name = "lblTemplateJobsDot";
            lblTemplateJobsDot.Size = new Size(17, 19);
            lblTemplateJobsDot.TabIndex = 6;
            lblTemplateJobsDot.Text = "●";
            // 
            // lblTemplateJobs
            // 
            lblTemplateJobs.AutoSize = true;
            lblTemplateJobs.Location = new Point(28, 53);
            lblTemplateJobs.Name = "lblTemplateJobs";
            lblTemplateJobs.Size = new Size(68, 17);
            lblTemplateJobs.TabIndex = 7;
            lblTemplateJobs.Text = "模板任务：";
            // 
            // lblTablesCsDot
            // 
            lblTablesCsDot.AutoSize = true;
            lblTablesCsDot.Font = new Font("Segoe UI", 10F);
            lblTablesCsDot.ForeColor = Color.Gray;
            lblTablesCsDot.Location = new Point(10, 81);
            lblTablesCsDot.Name = "lblTablesCsDot";
            lblTablesCsDot.Size = new Size(17, 19);
            lblTablesCsDot.TabIndex = 8;
            lblTablesCsDot.Text = "●";
            // 
            // lblTablesCs
            // 
            lblTablesCs.AutoSize = true;
            lblTablesCs.Location = new Point(28, 81);
            lblTablesCs.Name = "lblTablesCs";
            lblTablesCs.Size = new Size(73, 17);
            lblTablesCs.TabIndex = 9;
            lblTablesCs.Text = "Tables.cs：";
            // 
            // lblLastExportDot
            // 
            lblLastExportDot.AutoSize = true;
            lblLastExportDot.Font = new Font("Segoe UI", 10F);
            lblLastExportDot.ForeColor = Color.Gray;
            lblLastExportDot.Location = new Point(10, 109);
            lblLastExportDot.Name = "lblLastExportDot";
            lblLastExportDot.Size = new Size(17, 19);
            lblLastExportDot.TabIndex = 10;
            lblLastExportDot.Text = "●";
            // 
            // lblLastExport
            // 
            lblLastExport.AutoSize = true;
            lblLastExport.Location = new Point(28, 109);
            lblLastExport.Name = "lblLastExport";
            lblLastExport.Size = new Size(68, 17);
            lblLastExport.TabIndex = 11;
            lblLastExport.Text = "上次导出：";
            // 
            // grpLog
            // 
            grpLog.Controls.Add(txtLog);
            grpLog.Dock = DockStyle.Fill;
            grpLog.Location = new Point(0, 218);
            grpLog.Name = "grpLog";
            grpLog.Padding = new Padding(8);
            grpLog.Size = new Size(819, 349);
            grpLog.TabIndex = 0;
            grpLog.TabStop = false;
            grpLog.Text = "运行日志（右键清空/复制）";
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.BorderStyle = BorderStyle.None;
            txtLog.ContextMenuStrip = ctxLog;
            txtLog.Dock = DockStyle.Fill;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.ForeColor = Color.White;
            txtLog.Location = new Point(8, 24);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(803, 317);
            txtLog.TabIndex = 0;
            txtLog.Text = "";
            txtLog.WordWrap = false;
            // 
            // HomeTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpLog);
            Controls.Add(grpStatus);
            Controls.Add(pnlActions);
            Name = "HomeTab";
            Size = new Size(819, 567);
            ctxLog.ResumeLayout(false);
            pnlActions.ResumeLayout(false);
            pnlActions.PerformLayout();
            grpStatus.ResumeLayout(false);
            grpStatus.PerformLayout();
            grpLog.ResumeLayout(false);
            ResumeLayout(false);
        }

        private ToolTip toolTip;
        private ContextMenuStrip ctxLog;
        private ToolStripMenuItem ctxMenuItemClearLog;
        private ToolStripMenuItem ctxMenuItemCopyLog;

        private Panel pnlActions;
        private Button btnOneClick;
        private Button btnLubanOnly;
        private Button btnCancel;
        private ProgressBar pbOverall;
        private CheckBox chkIncludeEnum;

        private GroupBox grpStatus;
        private Label lblGenBatDot;
        private Label lblGenBat;
        private Button btnGoLuban;
        private Label lblTemplateJobsDot;
        private Label lblTemplateJobs;
        private Button btnGoTemplate1;
        private Label lblTablesCsDot;
        private Label lblTablesCs;
        private Button btnGoTemplate2;
        private Label lblLastExportDot;
        private Label lblLastExport;

        private GroupBox grpLog;
        private RichTextBox txtLog;
    }
}
