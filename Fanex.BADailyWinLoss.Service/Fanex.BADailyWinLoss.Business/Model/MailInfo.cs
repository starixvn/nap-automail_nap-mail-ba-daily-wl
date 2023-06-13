using Fanex.BADailyWinLoss.Business.Service;
using System.Collections.Generic;

namespace Fanex.BADailyWinLoss.Business.Model
{
    public class MailInfo
    {
        public MailInfo()
        {
            FileName = string.Empty;
            NapReportURL = ConfigService.Config.NapReportURL;
            FileNamesPath = new List<string>();
            Subject = ConfigService.Config.MailInfoSubject;
        }

        public string FileName { get; set; }

        public string NapReportURL { get; set; }

        public List<string> FileNamesPath { get; set; }

        public string Subject { get; set; }
    }
}
