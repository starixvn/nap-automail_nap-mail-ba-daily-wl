using Epplus.Mapper.Annotations;

namespace Fanex.BADailyWinLoss.Business.Model
{
    public class BALicWinLossDailyExcelModel
    {
        [Cell("A6")]
        public string ColumnForTemplate { get; set; }

        [Cell("B6")]
        public string LicenseeName { get; set; }

        [Cell("C6")]
        public string CurrencyName { get; set; }

        [Cell("D6")]
        public double TurnOverRM { get; set; }

        [Cell("E6")]
        public double BetCount { get; set; }

        [Cell("F6")]
        public double WinlostRM { get; set; }

        [Cell("G6")]
        public double CommRM { get; set; }

        [Cell("H6")]
        public double AmtDueToBARM { get; set; }

        [Cell("I6")]
        public double Margin { get; set; }
    }

    public class LiveOrNonLiveExcelModel
    {
        [Cell("A6")]
        public string Text { get; set; }
    }

    public class HeaderExcelModel
    {
        [Cell("A3:D3")]
        public string Header { get; set; }
    }

    public class HeaderColumnsExcelModel
    {
        [Cell("B5")]
        public string LicenseeName { get; set; } = "Licensee";

        [Cell("C5")]
        public string CurrencyName { get; set; } = "Currency";

        [Cell("D5")]
        public string TurnOverRM { get; set; } = "Turnover";

        [Cell("E5")]
        public string BetCount { get; set; } = "Total Trans";

        [Cell("F5")]
        public string WinlostRM { get; set; } = "Customer W/L";

        [Cell("G5")]
        public string CommRM { get; set; } = "Licensee Comm";

        [Cell("H5")]
        public string AmtDueToBARM { get; set; } = "Amount Due to BA";

        [Cell("I5")]
        public string Margin { get; set; } = "Margin";
    }
}
