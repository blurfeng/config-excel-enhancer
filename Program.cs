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
            // .NET 5+ 默认不含代码页编码，注册后才能使用 GBK(936) 等代码页
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}