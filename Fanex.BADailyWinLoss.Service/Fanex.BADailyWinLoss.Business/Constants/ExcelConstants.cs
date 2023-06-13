namespace Fanex.BADailyWinLoss.Business.Constants
{
    public static class ExcelConstants
    {
        public const string TemplateFileName = @"Templates/Excel/BADailyWinLoss.xlsx";

        public const int HeaderRowIndex = 3;

        public const string HeaderDateFormat = "MM/dd/yyyy";

        public const string SubTotalText = "Sub Total";

        public const string TotalText = "Total";

        public const string GrandTotalText = "Grand Total";

        public const int LicenseeCol = 2;

        public const int LiveOrNonLiveCol = 1;

        public const string LiveText = "Live";

        public const string NonLiveText = "Non-Live";

        public const string MonthToDate = "MONTH-TO-DATE";

        public const string YearToDate = "YEAR-TO-DATE";

        public const string SavedFileName = "Bet_Aggregator_Daily_Win_Loss_Report_For_{0}.xlsx";
    }
}
