namespace ConfigExcelEnhancer.UI
{
    partial class SettingsTab
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
            groupBoxFiles = new GroupBox();
            btnClearSettings = new Button();
            btnOpenSettingsFolder = new Button();
            lblSettingsDesc = new Label();
            btnClearLocalState = new Button();
            btnOpenLocalStateFolder = new Button();
            lblLocalStateDesc = new Label();
            groupBoxAbout = new GroupBox();
            lblVersion = new Label();
            linkLuban = new LinkLabel();
            linkProject = new LinkLabel();
            groupBoxDiagnostics = new GroupBox();
            txtDiagnostics = new TextBox();
            btnCopyDiagnostics = new Button();
            groupBoxFiles.SuspendLayout();
            groupBoxAbout.SuspendLayout();
            groupBoxDiagnostics.SuspendLayout();
            SuspendLayout();
            //
            // toolTip
            //
            toolTip.AutoPopDelay = 8000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 200;
            //
            // groupBoxFiles
            //
            groupBoxFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxFiles.Controls.Add(btnClearSettings);
            groupBoxFiles.Controls.Add(btnOpenSettingsFolder);
            groupBoxFiles.Controls.Add(lblSettingsDesc);
            groupBoxFiles.Controls.Add(btnClearLocalState);
            groupBoxFiles.Controls.Add(btnOpenLocalStateFolder);
            groupBoxFiles.Controls.Add(lblLocalStateDesc);
            groupBoxFiles.Location = new Point(12, 14);
            groupBoxFiles.Name = "groupBoxFiles";
            groupBoxFiles.Size = new Size(641, 130);
            groupBoxFiles.TabIndex = 0;
            groupBoxFiles.TabStop = false;
            groupBoxFiles.Text = "配置文件管理";
            //
            // btnClearSettings
            //
            btnClearSettings.Location = new Point(6, 26);
            btnClearSettings.Name = "btnClearSettings";
            btnClearSettings.Size = new Size(150, 32);
            btnClearSettings.TabIndex = 0;
            btnClearSettings.Text = "清空 settings.json";
            toolTip.SetToolTip(btnClearSettings, "清空 settings.json，将所有工具设置恢复为默认值（不影响 local_state.json 等机器本地状态）。");
            btnClearSettings.Click += BtnClearSettings_Click;
            //
            // btnOpenSettingsFolder
            //
            btnOpenSettingsFolder.Location = new Point(162, 26);
            btnOpenSettingsFolder.Name = "btnOpenSettingsFolder";
            btnOpenSettingsFolder.Size = new Size(120, 32);
            btnOpenSettingsFolder.TabIndex = 1;
            btnOpenSettingsFolder.Text = "打开所在文件夹";
            toolTip.SetToolTip(btnOpenSettingsFolder, "在资源管理器中定位 settings.json。");
            btnOpenSettingsFolder.Click += BtnOpenSettingsFolder_Click;
            //
            // lblSettingsDesc
            //
            lblSettingsDesc.Location = new Point(288, 33);
            lblSettingsDesc.Name = "lblSettingsDesc";
            lblSettingsDesc.Size = new Size(347, 20);
            lblSettingsDesc.TabIndex = 2;
            lblSettingsDesc.Text = "共享配置（随项目提交），清空后恢复默认值。";
            //
            // btnClearLocalState
            //
            btnClearLocalState.Location = new Point(6, 78);
            btnClearLocalState.Name = "btnClearLocalState";
            btnClearLocalState.Size = new Size(150, 32);
            btnClearLocalState.TabIndex = 3;
            btnClearLocalState.Text = "清空 local_state.json";
            toolTip.SetToolTip(btnClearLocalState, "清空机器本地状态：项目根目录、窗口尺寸、导出缓存等，恢复为默认值。");
            btnClearLocalState.Click += BtnClearLocalState_Click;
            //
            // btnOpenLocalStateFolder
            //
            btnOpenLocalStateFolder.Location = new Point(162, 78);
            btnOpenLocalStateFolder.Name = "btnOpenLocalStateFolder";
            btnOpenLocalStateFolder.Size = new Size(120, 32);
            btnOpenLocalStateFolder.TabIndex = 4;
            btnOpenLocalStateFolder.Text = "打开所在文件夹";
            toolTip.SetToolTip(btnOpenLocalStateFolder, "在资源管理器中定位 local_state.json。");
            btnOpenLocalStateFolder.Click += BtnOpenLocalStateFolder_Click;
            //
            // lblLocalStateDesc
            //
            lblLocalStateDesc.Location = new Point(288, 85);
            lblLocalStateDesc.Name = "lblLocalStateDesc";
            lblLocalStateDesc.Size = new Size(347, 20);
            lblLocalStateDesc.TabIndex = 5;
            lblLocalStateDesc.Text = "机器本地状态（不提交），清空后需重设项目根目录。";
            //
            // groupBoxAbout
            //
            groupBoxAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxAbout.Controls.Add(lblVersion);
            groupBoxAbout.Controls.Add(linkLuban);
            groupBoxAbout.Controls.Add(linkProject);
            groupBoxAbout.Location = new Point(12, 156);
            groupBoxAbout.Name = "groupBoxAbout";
            groupBoxAbout.Size = new Size(641, 90);
            groupBoxAbout.TabIndex = 1;
            groupBoxAbout.TabStop = false;
            groupBoxAbout.Text = "关于";
            //
            // lblVersion
            //
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(12, 28);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(0, 17);
            lblVersion.TabIndex = 0;
            //
            // linkLuban
            //
            linkLuban.AutoSize = true;
            linkLuban.Location = new Point(12, 56);
            linkLuban.Name = "linkLuban";
            linkLuban.Size = new Size(0, 17);
            linkLuban.TabIndex = 1;
            linkLuban.TabStop = true;
            linkLuban.Text = "Luban 官方仓库";
            linkLuban.LinkClicked += LinkLuban_LinkClicked;
            //
            // linkProject
            //
            linkProject.AutoSize = true;
            linkProject.Location = new Point(140, 56);
            linkProject.Name = "linkProject";
            linkProject.Size = new Size(0, 17);
            linkProject.TabIndex = 2;
            linkProject.TabStop = true;
            linkProject.Text = "本项目仓库";
            linkProject.LinkClicked += LinkProject_LinkClicked;
            //
            // groupBoxDiagnostics
            //
            groupBoxDiagnostics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxDiagnostics.Controls.Add(txtDiagnostics);
            groupBoxDiagnostics.Controls.Add(btnCopyDiagnostics);
            groupBoxDiagnostics.Location = new Point(12, 258);
            groupBoxDiagnostics.Name = "groupBoxDiagnostics";
            groupBoxDiagnostics.Size = new Size(641, 160);
            groupBoxDiagnostics.TabIndex = 2;
            groupBoxDiagnostics.TabStop = false;
            groupBoxDiagnostics.Text = "诊断信息";
            //
            // txtDiagnostics
            //
            txtDiagnostics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDiagnostics.Location = new Point(6, 26);
            txtDiagnostics.Multiline = true;
            txtDiagnostics.Name = "txtDiagnostics";
            txtDiagnostics.ReadOnly = true;
            txtDiagnostics.ScrollBars = ScrollBars.Vertical;
            txtDiagnostics.Size = new Size(629, 88);
            txtDiagnostics.TabIndex = 0;
            //
            // btnCopyDiagnostics
            //
            btnCopyDiagnostics.Location = new Point(6, 120);
            btnCopyDiagnostics.Name = "btnCopyDiagnostics";
            btnCopyDiagnostics.Size = new Size(150, 32);
            btnCopyDiagnostics.TabIndex = 1;
            btnCopyDiagnostics.Text = "复制到剪贴板";
            toolTip.SetToolTip(btnCopyDiagnostics, "复制诊断信息，便于排查问题时贴给他人。");
            btnCopyDiagnostics.Click += BtnCopyDiagnostics_Click;
            //
            // SettingsTab
            //
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxFiles);
            Controls.Add(groupBoxAbout);
            Controls.Add(groupBoxDiagnostics);
            Name = "SettingsTab";
            Size = new Size(665, 470);
            groupBoxFiles.ResumeLayout(false);
            groupBoxAbout.ResumeLayout(false);
            groupBoxAbout.PerformLayout();
            groupBoxDiagnostics.ResumeLayout(false);
            groupBoxDiagnostics.PerformLayout();
            ResumeLayout(false);
        }

        private ToolTip toolTip = null!;
        private GroupBox groupBoxFiles = null!;
        private Button btnClearSettings = null!;
        private Button btnOpenSettingsFolder = null!;
        private Label lblSettingsDesc = null!;
        private Button btnClearLocalState = null!;
        private Button btnOpenLocalStateFolder = null!;
        private Label lblLocalStateDesc = null!;
        private GroupBox groupBoxAbout = null!;
        private Label lblVersion = null!;
        private LinkLabel linkLuban = null!;
        private LinkLabel linkProject = null!;
        private GroupBox groupBoxDiagnostics = null!;
        private TextBox txtDiagnostics = null!;
        private Button btnCopyDiagnostics = null!;
    }
}
