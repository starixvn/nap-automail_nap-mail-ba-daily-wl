using System;

namespace Fanex.BADailyWinLoss.Business.Model
{
    public class ConfigModel
    {
        public string DataDirectory { get; set; }

        public string ExcelPasswordProtected { get; set; }

        public bool IsTest { get; set; }

        public DateTime TestedDate { get; set; }

        public int FunctionId { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }

        public string NapReportURL { get; set; }

        public string MailInfoSubject { get; set; }

        public string DataDirectoryWithYearMonth
        {
            get
            {
                string month = DateTime.Today.ToString("MM");
                string dataDirectory = DataDirectory + @"/" + DateTime.Today.Year + @"/" + month;
                return dataDirectory;
            }
        }
    }
}