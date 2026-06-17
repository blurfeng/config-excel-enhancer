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
            lblProjectNameCaption = new Label();
            txtProjectName = new TextBox();
            chkFuzzyFindProjectRoot = new CheckBox();
            btnClearProjectRoot = new Button();
            btnFindProjectRoot = new Button();
            btnBrowseProjectRoot = new Button();
            ctxLog = new ContextMenuStrip(components);
            ctxMenuItemClearLog = new ToolStripMenuItem();
            ctxMenuItemCopyLog = new ToolStripMenuItem();
            pbOverall = new ProgressBar();
            pnlProjectName = new Panel();
            pnlActions = new Panel();
            grpStatus = new GroupBox();
            tlpStatus = new TableLayoutPanel();
            pnlSecRoot = new Panel();
            lblProjectRootDot = new Label();
            lblProjectRoot = new Label();
            pnlSecCheck = new Panel();
            lblCheckDot = new Label();
            lblCheckSummary = new Label();
            lblCheckIssues = new Label();
            pnlSecLuban = new Panel();
            lblGenBatDot = new Label();
            lblLubanTitle = new Label();
            lblGenBat = new Label();
            pnlSecTemplate = new Panel();
            lblTemplateJobsDot = new Label();
            lblTemplateTitle = new Label();
            lblTemplateJobs = new Label();
            lblTablesCs = new Label();
            pnlSecLastExport = new Panel();
            lblLastExportDot = new Label();
            lblLastExport = new Label();
            grpLog = new GroupBox();
            txtLog = new RichTextBox();
            ctxLog.SuspendLayout();
            pnlProjectName.SuspendLayout();
            pnlActions.SuspendLayout();
            grpStatus.SuspendLayout();
            tlpStatus.SuspendLayout();
            pnlSecRoot.SuspendLayout();
            pnlSecCheck.SuspendLayout();
            pnlSecLuban.SuspendLayout();
            pnlSecTemplate.SuspendLayout();
            pnlSecLastExport.SuspendLayout();
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
            chkIncludeEnum.Text = "执行 Enum 验证";
            toolTip.SetToolTip(chkIncludeEnum, "执行前先运行 Enum 验证，确保枚举值合法。运行快，建议保持勾选");
            chkIncludeEnum.UseVisualStyleBackColor = true;
            // 
            // btnGoLuban
            // 
            btnGoLuban.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGoLuban.Location = new Point(695, 3);
            btnGoLuban.Name = "btnGoLuban";
            btnGoLuban.Size = new Size(108, 23);
            btnGoLuban.TabIndex = 5;
            btnGoLuban.Text = "→ Luban";
            toolTip.SetToolTip(btnGoLuban, "跳转到 Luban 选项卡");
            btnGoLuban.UseVisualStyleBackColor = true;
            btnGoLuban.Click += btnGoLuban_Click;
            // 
            // btnGoTemplate1
            // 
            btnGoTemplate1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGoTemplate1.Location = new Point(695, 3);
            btnGoTemplate1.Name = "btnGoTemplate1";
            btnGoTemplate1.Size = new Size(108, 23);
            btnGoTemplate1.TabIndex = 6;
            btnGoTemplate1.Text = "→ 导出模板类";
            toolTip.SetToolTip(btnGoTemplate1, "跳转到导出模板类选项卡");
            btnGoTemplate1.UseVisualStyleBackColor = true;
            btnGoTemplate1.Click += btnGoTemplate_Click;
            // 
            // lblProjectNameCaption
            // 
            lblProjectNameCaption.AutoSize = true;
            lblProjectNameCaption.Location = new Point(8, 8);
            lblProjectNameCaption.Name = "lblProjectNameCaption";
            lblProjectNameCaption.Size = new Size(68, 17);
            lblProjectNameCaption.TabIndex = 0;
            lblProjectNameCaption.Text = "项目名称：";
            toolTip.SetToolTip(lblProjectNameCaption, "通用参数，用于自动设置本地项目根目录等。");
            // 
            // txtProjectName
            // 
            txtProjectName.Location = new Point(72, 5);
            txtProjectName.Name = "txtProjectName";
            txtProjectName.Size = new Size(200, 23);
            txtProjectName.TabIndex = 1;
            toolTip.SetToolTip(txtProjectName, "通用参数，用于自动设置本地项目根目录等。");
            // 
            // chkFuzzyFindProjectRoot
            // 
            chkFuzzyFindProjectRoot.AutoSize = true;
            chkFuzzyFindProjectRoot.Checked = true;
            chkFuzzyFindProjectRoot.CheckState = CheckState.Checked;
            chkFuzzyFindProjectRoot.Location = new Point(280, 7);
            chkFuzzyFindProjectRoot.Name = "chkFuzzyFindProjectRoot";
            chkFuzzyFindProjectRoot.Size = new Size(111, 21);
            chkFuzzyFindProjectRoot.TabIndex = 2;
            chkFuzzyFindProjectRoot.Text = "模糊查找根目录";
            toolTip.SetToolTip(chkFuzzyFindProjectRoot, "勾选后忽略大小写及连字符/下划线差异（如 GodsClash 可匹配 gods-clash）");
            chkFuzzyFindProjectRoot.UseVisualStyleBackColor = true;
            // 
            // btnClearProjectRoot
            // 
            btnClearProjectRoot.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearProjectRoot.Location = new Point(517, 6);
            btnClearProjectRoot.Name = "btnClearProjectRoot";
            btnClearProjectRoot.Size = new Size(56, 23);
            btnClearProjectRoot.TabIndex = 32;
            btnClearProjectRoot.Text = "清空";
            toolTip.SetToolTip(btnClearProjectRoot, "清空本机的项目根目录（仅清除本机记录，不影响 settings.json）");
            btnClearProjectRoot.UseVisualStyleBackColor = true;
            btnClearProjectRoot.Click += btnClearProjectRoot_Click;
            // 
            // btnFindProjectRoot
            // 
            btnFindProjectRoot.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFindProjectRoot.Location = new Point(579, 6);
            btnFindProjectRoot.Name = "btnFindProjectRoot";
            btnFindProjectRoot.Size = new Size(110, 23);
            btnFindProjectRoot.TabIndex = 33;
            btnFindProjectRoot.Text = "自动查找根目录";
            toolTip.SetToolTip(btnFindProjectRoot, "自动定位项目根目录：优先用已配置路径验证，其次按项目名称查找同名文件夹");
            btnFindProjectRoot.UseVisualStyleBackColor = true;
            btnFindProjectRoot.Click += btnFindProjectRoot_Click;
            // 
            // btnBrowseProjectRoot
            // 
            btnBrowseProjectRoot.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseProjectRoot.Location = new Point(695, 6);
            btnBrowseProjectRoot.Name = "btnBrowseProjectRoot";
            btnBrowseProjectRoot.Size = new Size(108, 23);
            btnBrowseProjectRoot.TabIndex = 34;
            btnBrowseProjectRoot.Text = "设置根目录...";
            toolTip.SetToolTip(btnBrowseProjectRoot, "设置本机的项目根目录（仅保存到本机，不会提交到版本控制）");
            btnBrowseProjectRoot.UseVisualStyleBackColor = true;
            btnBrowseProjectRoot.Click += btnBrowseProjectRoot_Click;
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
            // pnlProjectName
            // 
            pnlProjectName.Controls.Add(lblProjectNameCaption);
            pnlProjectName.Controls.Add(txtProjectName);
            pnlProjectName.Controls.Add(chkFuzzyFindProjectRoot);
            pnlProjectName.Dock = DockStyle.Top;
            pnlProjectName.Location = new Point(0, 80);
            pnlProjectName.Name = "pnlProjectName";
            pnlProjectName.Size = new Size(819, 32);
            pnlProjectName.TabIndex = 3;
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
            grpStatus.AutoSize = true;
            grpStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            grpStatus.Controls.Add(tlpStatus);
            grpStatus.Dock = DockStyle.Top;
            grpStatus.Location = new Point(0, 112);
            grpStatus.Name = "grpStatus";
            grpStatus.Padding = new Padding(8);
            grpStatus.Size = new Size(819, 197);
            grpStatus.TabIndex = 1;
            grpStatus.TabStop = false;
            grpStatus.Text = "当前配置状态";
            // 
            // tlpStatus
            // 
            tlpStatus.AutoSize = true;
            tlpStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlpStatus.ColumnCount = 1;
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpStatus.Controls.Add(pnlSecRoot, 0, 0);
            tlpStatus.Controls.Add(pnlSecCheck, 0, 1);
            tlpStatus.Controls.Add(pnlSecLuban, 0, 2);
            tlpStatus.Controls.Add(pnlSecTemplate, 0, 3);
            tlpStatus.Controls.Add(pnlSecLastExport, 0, 4);
            tlpStatus.Dock = DockStyle.Top;
            tlpStatus.Location = new Point(8, 24);
            tlpStatus.Name = "tlpStatus";
            tlpStatus.RowCount = 5;
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpStatus.RowStyles.Add(new RowStyle());
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpStatus.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tlpStatus.Size = new Size(803, 165);
            tlpStatus.TabIndex = 0;
            // 
            // pnlSecRoot
            // 
            pnlSecRoot.Controls.Add(lblProjectRootDot);
            pnlSecRoot.Controls.Add(lblProjectRoot);
            pnlSecRoot.Controls.Add(btnClearProjectRoot);
            pnlSecRoot.Controls.Add(btnFindProjectRoot);
            pnlSecRoot.Controls.Add(btnBrowseProjectRoot);
            pnlSecRoot.Dock = DockStyle.Fill;
            pnlSecRoot.Location = new Point(0, 0);
            pnlSecRoot.Margin = new Padding(0);
            pnlSecRoot.Name = "pnlSecRoot";
            pnlSecRoot.Size = new Size(803, 34);
            pnlSecRoot.TabIndex = 0;
            // 
            // lblProjectRootDot
            // 
            lblProjectRootDot.AutoSize = true;
            lblProjectRootDot.Font = new Font("Segoe UI", 10F);
            lblProjectRootDot.ForeColor = Color.Gray;
            lblProjectRootDot.Location = new Point(10, 7);
            lblProjectRootDot.Name = "lblProjectRootDot";
            lblProjectRootDot.Size = new Size(17, 19);
            lblProjectRootDot.TabIndex = 30;
            lblProjectRootDot.Text = "●";
            // 
            // lblProjectRoot
            // 
            lblProjectRoot.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblProjectRoot.AutoEllipsis = true;
            lblProjectRoot.AutoSize = false;
            lblProjectRoot.Location = new Point(28, 9);
            lblProjectRoot.Name = "lblProjectRoot";
            lblProjectRoot.Size = new Size(481, 17);
            lblProjectRoot.TabIndex = 31;
            lblProjectRoot.Text = "本地项目根目录：未配置";
            lblProjectRoot.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlSecCheck
            // 
            pnlSecCheck.AutoSize = true;
            pnlSecCheck.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlSecCheck.Controls.Add(lblCheckDot);
            pnlSecCheck.Controls.Add(lblCheckSummary);
            pnlSecCheck.Controls.Add(lblCheckIssues);
            pnlSecCheck.Dock = DockStyle.Top;
            pnlSecCheck.Location = new Point(0, 34);
            pnlSecCheck.Margin = new Padding(0);
            pnlSecCheck.Name = "pnlSecCheck";
            pnlSecCheck.Padding = new Padding(0, 0, 0, 4);
            pnlSecCheck.Size = new Size(803, 43);
            pnlSecCheck.TabIndex = 1;
            // 
            // lblCheckDot
            // 
            lblCheckDot.AutoSize = true;
            lblCheckDot.Font = new Font("Segoe UI", 10F);
            lblCheckDot.ForeColor = Color.Gray;
            lblCheckDot.Location = new Point(10, 2);
            lblCheckDot.Name = "lblCheckDot";
            lblCheckDot.Size = new Size(17, 19);
            lblCheckDot.TabIndex = 20;
            lblCheckDot.Text = "●";
            // 
            // lblCheckSummary
            // 
            lblCheckSummary.AutoSize = true;
            lblCheckSummary.Location = new Point(28, 4);
            lblCheckSummary.Name = "lblCheckSummary";
            lblCheckSummary.Size = new Size(77, 17);
            lblCheckSummary.TabIndex = 21;
            lblCheckSummary.Text = "配置检查中...";
            // 
            // lblCheckIssues
            // 
            lblCheckIssues.AutoSize = true;
            lblCheckIssues.ForeColor = Color.Orange;
            lblCheckIssues.Location = new Point(28, 22);
            lblCheckIssues.Name = "lblCheckIssues";
            lblCheckIssues.Size = new Size(0, 17);
            lblCheckIssues.TabIndex = 22;
            lblCheckIssues.Visible = false;
            // 
            // pnlSecLuban
            // 
            pnlSecLuban.Controls.Add(lblGenBatDot);
            pnlSecLuban.Controls.Add(lblLubanTitle);
            pnlSecLuban.Controls.Add(lblGenBat);
            pnlSecLuban.Controls.Add(btnGoLuban);
            pnlSecLuban.Dock = DockStyle.Fill;
            pnlSecLuban.Location = new Point(0, 77);
            pnlSecLuban.Margin = new Padding(0);
            pnlSecLuban.Name = "pnlSecLuban";
            pnlSecLuban.Size = new Size(803, 30);
            pnlSecLuban.TabIndex = 2;
            // 
            // lblGenBatDot
            // 
            lblGenBatDot.AutoSize = true;
            lblGenBatDot.Font = new Font("Segoe UI", 10F);
            lblGenBatDot.ForeColor = Color.Gray;
            lblGenBatDot.Location = new Point(10, 6);
            lblGenBatDot.Name = "lblGenBatDot";
            lblGenBatDot.Size = new Size(17, 19);
            lblGenBatDot.TabIndex = 0;
            lblGenBatDot.Text = "●";
            // 
            // lblLubanTitle
            // 
            lblLubanTitle.AutoSize = true;
            lblLubanTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLubanTitle.Location = new Point(28, 8);
            lblLubanTitle.Name = "lblLubanTitle";
            lblLubanTitle.Size = new Size(40, 15);
            lblLubanTitle.TabIndex = 12;
            lblLubanTitle.Text = "Luban";
            // 
            // lblGenBat
            // 
            lblGenBat.AutoSize = true;
            lblGenBat.Location = new Point(125, 8);
            lblGenBat.Name = "lblGenBat";
            lblGenBat.Size = new Size(64, 17);
            lblGenBat.TabIndex = 1;
            lblGenBat.Text = "gen.bat：";
            // 
            // pnlSecTemplate
            // 
            pnlSecTemplate.Controls.Add(lblTemplateJobsDot);
            pnlSecTemplate.Controls.Add(lblTemplateTitle);
            pnlSecTemplate.Controls.Add(lblTemplateJobs);
            pnlSecTemplate.Controls.Add(lblTablesCs);
            pnlSecTemplate.Controls.Add(btnGoTemplate1);
            pnlSecTemplate.Dock = DockStyle.Fill;
            pnlSecTemplate.Location = new Point(0, 107);
            pnlSecTemplate.Margin = new Padding(0);
            pnlSecTemplate.Name = "pnlSecTemplate";
            pnlSecTemplate.Size = new Size(803, 30);
            pnlSecTemplate.TabIndex = 3;
            // 
            // lblTemplateJobsDot
            // 
            lblTemplateJobsDot.AutoSize = true;
            lblTemplateJobsDot.Font = new Font("Segoe UI", 10F);
            lblTemplateJobsDot.ForeColor = Color.Gray;
            lblTemplateJobsDot.Location = new Point(10, 6);
            lblTemplateJobsDot.Name = "lblTemplateJobsDot";
            lblTemplateJobsDot.Size = new Size(17, 19);
            lblTemplateJobsDot.TabIndex = 6;
            lblTemplateJobsDot.Text = "●";
            // 
            // lblTemplateTitle
            // 
            lblTemplateTitle.AutoSize = true;
            lblTemplateTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTemplateTitle.Location = new Point(28, 8);
            lblTemplateTitle.Name = "lblTemplateTitle";
            lblTemplateTitle.Size = new Size(77, 15);
            lblTemplateTitle.TabIndex = 13;
            lblTemplateTitle.Text = "导出模板类";
            // 
            // lblTemplateJobs
            // 
            lblTemplateJobs.AutoSize = true;
            lblTemplateJobs.Location = new Point(125, 8);
            lblTemplateJobs.Name = "lblTemplateJobs";
            lblTemplateJobs.Size = new Size(68, 17);
            lblTemplateJobs.TabIndex = 7;
            lblTemplateJobs.Text = "模板任务：";
            // 
            // lblTablesCs
            // 
            lblTablesCs.AutoSize = true;
            lblTablesCs.Location = new Point(284, 8);
            lblTablesCs.Name = "lblTablesCs";
            lblTablesCs.Size = new Size(73, 17);
            lblTablesCs.TabIndex = 9;
            lblTablesCs.Text = "Tables.cs：";
            // 
            // pnlSecLastExport
            // 
            pnlSecLastExport.Controls.Add(lblLastExportDot);
            pnlSecLastExport.Controls.Add(lblLastExport);
            pnlSecLastExport.Dock = DockStyle.Fill;
            pnlSecLastExport.Location = new Point(0, 137);
            pnlSecLastExport.Margin = new Padding(0);
            pnlSecLastExport.Name = "pnlSecLastExport";
            pnlSecLastExport.Size = new Size(803, 28);
            pnlSecLastExport.TabIndex = 4;
            // 
            // lblLastExportDot
            // 
            lblLastExportDot.AutoSize = true;
            lblLastExportDot.Font = new Font("Segoe UI", 10F);
            lblLastExportDot.ForeColor = Color.Gray;
            lblLastExportDot.Location = new Point(10, 5);
            lblLastExportDot.Name = "lblLastExportDot";
            lblLastExportDot.Size = new Size(17, 19);
            lblLastExportDot.TabIndex = 10;
            lblLastExportDot.Text = "●";
            // 
            // lblLastExport
            // 
            lblLastExport.AutoSize = true;
            lblLastExport.Location = new Point(28, 5);
            lblLastExport.Name = "lblLastExport";
            lblLastExport.Size = new Size(68, 17);
            lblLastExport.TabIndex = 11;
            lblLastExport.Text = "上次导出：";
            // 
            // grpLog
            // 
            grpLog.Controls.Add(txtLog);
            grpLog.Dock = DockStyle.Fill;
            grpLog.Location = new Point(0, 309);
            grpLog.Name = "grpLog";
            grpLog.Padding = new Padding(8);
            grpLog.Size = new Size(819, 258);
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
            txtLog.Size = new Size(803, 226);
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
            Controls.Add(pnlProjectName);
            Controls.Add(pnlActions);
            Name = "HomeTab";
            Size = new Size(819, 567);
            ctxLog.ResumeLayout(false);
            pnlProjectName.ResumeLayout(false);
            pnlProjectName.PerformLayout();
            pnlActions.ResumeLayout(false);
            pnlActions.PerformLayout();
            grpStatus.ResumeLayout(false);
            grpStatus.PerformLayout();
            tlpStatus.ResumeLayout(false);
            tlpStatus.PerformLayout();
            pnlSecRoot.ResumeLayout(false);
            pnlSecRoot.PerformLayout();
            pnlSecCheck.ResumeLayout(false);
            pnlSecCheck.PerformLayout();
            pnlSecLuban.ResumeLayout(false);
            pnlSecLuban.PerformLayout();
            pnlSecTemplate.ResumeLayout(false);
            pnlSecTemplate.PerformLayout();
            pnlSecLastExport.ResumeLayout(false);
            pnlSecLastExport.PerformLayout();
            grpLog.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private ToolTip toolTip;
        private ContextMenuStrip ctxLog;
        private ToolStripMenuItem ctxMenuItemClearLog;
        private ToolStripMenuItem ctxMenuItemCopyLog;

        private Panel pnlProjectName;
        private Label lblProjectNameCaption;
        private TextBox txtProjectName;
        private CheckBox chkFuzzyFindProjectRoot;

        private Panel pnlActions;
        private Button btnOneClick;
        private Button btnLubanOnly;
        private Button btnCancel;
        private ProgressBar pbOverall;
        private CheckBox chkIncludeEnum;

        private GroupBox grpStatus;
        private TableLayoutPanel tlpStatus;
        private Panel pnlSecRoot;
        private Panel pnlSecCheck;
        private Panel pnlSecLuban;
        private Panel pnlSecTemplate;
        private Panel pnlSecLastExport;
        private Label lblProjectRootDot;
        private Label lblProjectRoot;
        private Button btnClearProjectRoot;
        private Button btnFindProjectRoot;
        private Button btnBrowseProjectRoot;
        private Label lblGenBatDot;
        private Label lblGenBat;
        private Button btnGoLuban;
        private Label lblTemplateJobsDot;
        private Label lblTemplateJobs;
        private Button btnGoTemplate1;
        private Label lblTablesCs;
        private Label lblLastExportDot;
        private Label lblLastExport;
        private Label lblLubanTitle;
        private Label lblTemplateTitle;
        private Label lblCheckDot;
        private Label lblCheckSummary;
        private Label lblCheckIssues;

        private GroupBox grpLog;
        private RichTextBox txtLog;
    }
}
