using System.Runtime.InteropServices;

namespace ConfigExcelEnhancer.Core
{
    public static class FunctionLibrary
    {
        /// <summary>
        /// 用 Excel COM 批量刷新公式缓存值。必须在 STA 线程上调用。
        /// </summary>
        /// <returns>true = 刷新已执行；false = 本机未安装 Excel，已跳过。</returns>
        public static bool RefreshFormulasViaExcel(IReadOnlyList<string> filePaths)
        {
            var excelType = Type.GetTypeFromProgID("Excel.Application");
            if (excelType == null) return false;

            dynamic? excel = null;
            try
            {
                excel = Activator.CreateInstance(excelType)!;
                excel.Visible = false;
                excel.DisplayAlerts = false;
                excel.ScreenUpdating = false;

                foreach (var path in filePaths)
                {
                    dynamic workbook = excel.Workbooks.Open(path);
                    workbook.Save();
                    workbook.Close(SaveChanges: false);
                    Marshal.ReleaseComObject(workbook);
                }
            }
            finally
            {
                try { excel?.Quit(); } catch { }
                if (excel != null) Marshal.ReleaseComObject(excel);
            }
            return true;
        }

        /// <summary>
        /// 在任意线程调用：内部自动创建 STA 线程执行 COM 刷新。
        /// </summary>
        public static bool RefreshFormulasViaSTA(IReadOnlyList<string> filePaths)
        {
            bool result = false;
            var sta = new Thread(() => result = RefreshFormulasViaExcel(filePaths));
            sta.SetApartmentState(ApartmentState.STA);
            sta.Start();
            sta.Join();
            return result;
        }
    }
}
