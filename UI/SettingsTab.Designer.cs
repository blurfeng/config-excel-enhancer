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
            groupBoxSettings = new GroupBox();
            btnClearSettings = new Button();
            lblClearDesc = new Label();
            groupBoxSettings.SuspendLayout();
            SuspendLayout();

            // groupBoxSettings
            groupBoxSettings.Text = "settings.json";
            groupBoxSettings.Location = new Point(12, 12);
            groupBoxSettings.Size = new Size(460, 100);
            groupBoxSettings.Controls.Add(lblClearDesc);
            groupBoxSettings.Controls.Add(btnClearSettings);

            // lblClearDesc
            lblClearDesc.Text = "清空后，所有工具设置将恢复为默认值。";
            lblClearDesc.Location = new Point(10, 24);
            lblClearDesc.Size = new Size(380, 36);
            lblClearDesc.AutoSize = false;

            // btnClearSettings
            btnClearSettings.Text = "清空 settings.json";
            btnClearSettings.Location = new Point(10, 60);
            btnClearSettings.Size = new Size(150, 28);
            btnClearSettings.Click += BtnClearSettings_Click;

            // SettingsTab
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxSettings);
            groupBoxSettings.ResumeLayout(false);
            ResumeLayout(false);
        }

        private GroupBox groupBoxSettings = null!;
        private Button btnClearSettings = null!;
        private Label lblClearDesc = null!;
    }
}
