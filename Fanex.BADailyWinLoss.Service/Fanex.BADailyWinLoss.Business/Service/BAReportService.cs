using System;

namespace Fanex.BADailyWinLoss.Business.Service
{
    public interface BAReportService
    {
        void GenerateReport(DateTime winLossDate, out string fileName, out string directoryOfFileName);
    }

    public class BAReportServiceImp : BAReportService
    {
        private readonly BetAggregatorService betAggregatorService;
        private readonly ExcelService excelService;

        public BAReportServiceImp(BetAggregatorService betAggregatorService, ExcelService excelService)
        {
            this.betAggregatorService = betAggregatorService;
            this.excelService = excelService;
        }

        public void GenerateReport(DateTime winLossDate, out string fileName, out string directoryOfFileName)
        {
            var dailyList = betAggregatorService.GetWinlossDaily(winLossDate);
            var monthToDateList = betAggregatorService.GetWinlossMonthToDate(winLossDate);
            var yearToDateList = betAggregatorService.GetWinlossYearToDate(winLossDate);

            excelService.Export(winLossDate, dailyList, monthToDateList, yearToDateList, out fileName, out directoryOfFileName);
        }
    }
}