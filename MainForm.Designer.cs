using ConfigExcelEnhancer.UI;

namespace ConfigExcelEnhancer
{
    partial class MainForm
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
            tabControl = new TabControl();
            tabHome = new TabPage();
            homeTab = new HomeTab();
            tabEnum = new TabPage();
            enumTab = new EnumTab();
            tabLuban = new TabPage();
            lubanTab = new LubanTab();
            tabTableDesign = new TabPage();
            tableDesignTab = new TableDesignTab();
            tabTemplate = new TabPage();
            templateTab = new TemplateTab();
            tabSettings = new TabPage();
            settingsTab = new SettingsTab();
            tabControl.SuspendLayout();
            tabHome.SuspendLayout();
            tabEnum.SuspendLayout();
            tabLuban.SuspendLayout();
            tabTableDesign.SuspendLayout();
            tabTemplate.SuspendLayout();
            tabSettings.SuspendLayout();
            SuspendLayout();

            // tabControl
            tabControl.Dock = DockStyle.Fill;
            tabControl.Controls.Add(tabHome);
            tabControl.Controls.Add(tabTemplate);
            tabControl.Controls.Add(tabLuban);
            tabControl.Controls.Add(tabEnum);
            tabControl.Controls.Add(tabTableDesign);
            tabControl.Controls.Add(tabSettings);

            // tabHome
            tabHome.Text = "主页";
            tabHome.Padding = new Padding(4);
            tabHome.Controls.Add(homeTab);

            // homeTab
            homeTab.Dock = DockStyle.Fill;

            // tabEnum
            tabEnum.Text = "Enum 验证";
            tabEnum.Padding = new Padding(4);
            tabEnum.Controls.Add(enumTab);

            // enumTab
            enumTab.Dock = DockStyle.Fill;

            // tabLuban
            tabLuban.Text = "Luban";
            tabLuban.Padding = new Padding(4);
            tabLuban.Controls.Add(lubanTab);

            // lubanTab
            lubanTab.Dock = DockStyle.Fill;

            // tabTableDesign
            tabTableDesign.Text = "表设计";
            tabTableDesign.Padding = new Padding(4);
            tabTableDesign.Controls.Add(tableDesignTab);

            // tableDesignTab
            tableDesignTab.Dock = DockStyle.Fill;

            // tabTemplate
            tabTemplate.Text = "导出模板类";
            tabTemplate.Padding = new Padding(4);
            tabTemplate.Controls.Add(templateTab);

            // templateTab
            templateTab.Dock = DockStyle.Fill;

            // tabSettings
            tabSettings.Text = "设置";
            tabSettings.Padding = new Padding(4);
            tabSettings.Controls.Add(settingsTab);

            // settingsTab
            settingsTab.Dock = DockStyle.Fill;

            // MainForm
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 600);
            MinimumSize = new Size(800, 560);
            Controls.Add(tabControl);
            Text = "Config Excel Enhancer";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
            tabControl.ResumeLayout(false);
            tabHome.ResumeLayout(false);
            tabEnum.ResumeLayout(false);
            tabLuban.ResumeLayout(false);
            tabTableDesign.ResumeLayout(false);
            tabTemplate.ResumeLayout(false);
            tabSettings.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TabControl tabControl = null!;
        private TabPage tabHome = null!;
        private HomeTab homeTab = null!;
        private TabPage tabEnum = null!;
        private EnumTab enumTab = null!;
        private TabPage tabLuban = null!;
        private LubanTab lubanTab = null!;
        private TabPage tabTableDesign = null!;
        private TableDesignTab tableDesignTab = null!;
        private TabPage tabTemplate = null!;
        private TemplateTab templateTab = null!;
        private TabPage tabSettings = null!;
        private SettingsTab settingsTab = null!;
    }
}
