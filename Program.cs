namespace ConfigExcelEnhancer
{
    /// <summary>
    /// 应用程序入口点。
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// 应用程序主入口。初始化 WinForms 配置并启动主窗体。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}