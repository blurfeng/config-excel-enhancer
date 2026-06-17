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
            groupBoxSettings = new GroupBox();
            lblClearDesc = new Label();
            btnClearSettings = new Button();
            groupBoxSettings.SuspendLayout();
            SuspendLayout();
            //
            // toolTip
            //
            toolTip.AutoPopDelay = 8000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 200;
            // 
            // groupBoxSettings
            // 
            groupBoxSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxSettings.Controls.Add(lblClearDesc);
            groupBoxSettings.Controls.Add(btnClearSettings);
            groupBoxSettings.Location = new Point(12, 14);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Size = new Size(641, 100);
            groupBoxSettings.TabIndex = 0;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "settings.json";
            // 
            // lblClearDesc
            // 
            lblClearDesc.Location = new Point(162, 30);
            lblClearDesc.Name = "lblClearDesc";
            lblClearDesc.Size = new Size(236, 20);
            lblClearDesc.TabIndex = 0;
            lblClearDesc.Text = "清空后，所有工具设置将恢复为默认值。";
            // 
            // btnClearSettings
            // 
            btnClearSettings.Location = new Point(6, 22);
            btnClearSettings.Name = "btnClearSettings";
            btnClearSettings.Size = new Size(150, 32);
            btnClearSettings.TabIndex = 1;
            btnClearSettings.Text = "清空 settings.json";
            toolTip.SetToolTip(btnClearSettings, "清空 settings.json，将所有工具设置恢复为默认值（不影响 local_state.json 等机器本地状态）。");
            btnClearSettings.Click += BtnClearSettings_Click;
            // 
            // SettingsTab
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxSettings);
            Name = "SettingsTab";
            Size = new Size(665, 470);
            groupBoxSettings.ResumeLayout(false);
            ResumeLayout(false);
        }

        private ToolTip toolTip = null!;
        private GroupBox groupBoxSettings = null!;
        private Button btnClearSettings = null!;
        private Label lblClearDesc = null!;
    }
}
