using AutoMapper;
using Epplus.Mapper.Extensions;
using Fanex.BADailyWinLoss.Business.Constants;
using Fanex.BADailyWinLoss.Business.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Fanex.BADailyWinLoss.Business.Service
{
    public interface ExcelService
    {
        void Export(
            DateTime winlossDate,
            IEnumerable<BALicWinLossDailyModel> dailyList,
            IEnumerable<BALicWinLossDailyModel> monthToDateList,
            IEnumerable<BALicWinLossDailyModel> yearToDateList,
            out string fileName,
            out string directoryOfFileName);
    }

    public class ExcelServiceImp : ExcelService
    {
        private int currentRow = ExcelConstants.HeaderRowIndex;
        private readonly IMapper mapper;

        public ExcelServiceImp(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public void Export(
            DateTime winlossDate,
            IEnumerable<BALicWinLossDailyModel> dailyList,
            IEnumerable<BALicWinLossDailyModel> monthToDateList,
            IEnumerable<BALicWinLossDailyModel> yearToDateList,
            out string fileName,
            out string directoryOfFileName)
        {
            fileName = string.Format(ExcelConstants.SavedFileName, winlossDate.ToString("yyyy_MM_dd"));
            directoryOfFileName = ConfigService.Config.DataDirectoryWithYearMonth + @"/" + fileName;

            using var stream = new FileStream(ExcelConstants.TemplateFileName, FileMode.Open, FileAccess.Read);
            using var excelPackage = new ExcelPackage(stream);
            using var bodySheet = excelPackage.Workbook.Worksheets.Copy("body", "Result");
            using var headerSheet = excelPackage.Workbook.Worksheets["header"];
            using var itemSheet = excelPackage.Workbook.Worksheets["item"];
            using var subTotalSheet = excelPackage.Workbook.Worksheets["subtotal"];
            using var totalSheet = excelPackage.Workbook.Worksheets["total"];
            using var grandTotalSheet = excelPackage.Workbook.Worksheets["grandtotal"];
            using var headerTitleSheet = excelPackage.Workbook.Worksheets["headertitle"];

            BuildDaily(bodySheet, headerTitleSheet, headerSheet, itemSheet, subTotalSheet, grandTotalSheet, totalSheet, winlossDate, dailyList);
            this.currentRow += 3;

            BuildMonthYearToDate(bodySheet, headerTitleSheet, headerSheet, itemSheet, subTotalSheet, totalSheet, ExcelConstants.MonthToDate, monthToDateList);
            this.currentRow += 3;

            BuildMonthYearToDate(bodySheet, headerTitleSheet, headerSheet, itemSheet, subTotalSheet, totalSheet, ExcelConstants.YearToDate, yearToDateList);
            ClearTemplate(excelPackage);
            SaveExcelFile(directoryOfFileName, excelPackage);
        }

        private static void SaveExcelFile(string directoryOfFileName, ExcelPackage excelPackage)
        {
            if (!Directory.Exists(ConfigService.Config.DataDirectoryWithYearMonth))
            {
                Directory.CreateDirectory(ConfigService.Config.DataDirectoryWithYearMonth);
            }
            excelPackage.SaveAs(new FileInfo(directoryOfFileName), ConfigService.Config.ExcelPasswordProtected);
        }

        private void BuildDaily(ExcelWorksheet bodySheet,
            ExcelWorksheet headerTitleSheet,
            ExcelWorksheet headerSheet,
            ExcelWorksheet itemSheet,
            ExcelWorksheet subTotalSheet,
            ExcelWorksheet grandTotalSheet,
            ExcelWorksheet totalSheet,
            DateTime winlossDate,
            IEnumerable<BALicWinLossDailyModel> data)
        {
            BuildHeaderTitle(bodySheet, headerTitleSheet, winlossDate.ToString(ExcelConstants.HeaderDateFormat));
            BuildHeaderColumns(bodySheet, headerSheet);
            var liveList = data.Where(x => x.LiveIndicator);
            BuildBody(bodySheet, itemSheet, subTotalSheet, totalSheet, liveList, true);
            var nonLiveList = data.Where(x => !x.LiveIndicator);
            BuildBody(bodySheet, itemSheet, subTotalSheet, totalSheet, nonLiveList, false);
            BuildGrandTotal(bodySheet, grandTotalSheet, data);
        }

        private void BuildMonthYearToDate(ExcelWorksheet bodySheet,
            ExcelWorksheet headerTitleSheet,
            ExcelWorksheet headerSheet,
            ExcelWorksheet itemSheet,
            ExcelWorksheet subTotalSheet,
            ExcelWorksheet totalSheet,
            string headerTitle,
            IEnumerable<BALicWinLossDailyModel> data)
        {
            BuildHeaderTitle(bodySheet, headerTitleSheet, headerTitle);
            BuildHeaderColumns(bodySheet, headerSheet);
            BuildBody(bodySheet, itemSheet, subTotalSheet, totalSheet, data, null);
        }

        private void BuildBody(
            ExcelWorksheet bodySheet,
            ExcelWorksheet itemSheet,
            ExcelWorksheet subTotalSheet,
            ExcelWorksheet totalSheet,
            IEnumerable<BALicWinLossDailyModel> list,
            bool? isLive)
        {
            if (list.Any())
            {
                var licenseeList = list.Select(x => x.LicenseeName).Distinct();
                int startRow = this.currentRow;
                foreach (var licensee in licenseeList)
                {
                    BuildBodyForLicensee(bodySheet, itemSheet, subTotalSheet, licensee, list);
                }
                if (isLive.HasValue)
                {
                    UpdateLiveOrNonLiveCell(bodySheet, startRow, this.currentRow - 1, isLive.Value);
                }
                BuildTotal(bodySheet, totalSheet, list);
            }
        }

        private void BuildHeaderTitle(ExcelWorksheet bodySheet, ExcelWorksheet headerTitleSheet, string sectionName)
        {
            var headerModel = new HeaderExcelModel { Header = sectionName };
            bodySheet.ApplyModel(headerTitleSheet, headerModel, this.currentRow);
            this.currentRow += 2;
        }

        private void BuildHeaderColumns(ExcelWorksheet bodySheet, ExcelWorksheet headerSheet)
        {
            bodySheet.ApplyModel(headerSheet, new HeaderColumnsExcelModel(), this.currentRow);
            this.currentRow++;
        }

        private void BuildBodyForLicensee(ExcelWorksheet bodySheet,
            ExcelWorksheet itemSheet,
            ExcelWorksheet subTotalSheet,
            string licenseeName,
            IEnumerable<BALicWinLossDailyModel> list)
        {
            var filteredLicenseeList = list.Where(x => x.LicenseeName == licenseeName);
            if (filteredLicenseeList.Any())
            {
                int startRow = this.currentRow;
                var model = mapper.Map<IEnumerable<BALicWinLossDailyExcelModel>>(filteredLicenseeList);
                bodySheet.ApplyVertical(itemSheet, model, this.currentRow);
                this.currentRow += filteredLicenseeList.Count();
                BuildSubTotal(bodySheet, subTotalSheet, filteredLicenseeList);
                UpdateLicenseeCell(bodySheet, startRow, this.currentRow - 1, licenseeName);
            }
        }

        private void BuildSubTotal(ExcelWorksheet bodySheet, ExcelWorksheet subTotalSheet, IEnumerable<BALicWinLossDailyModel> list)
        {
            var model = CreateTotalExcelModel(list);
            model.CurrencyName = ExcelConstants.SubTotalText;
            bodySheet.ApplyModel(subTotalSheet, model, this.currentRow);
            this.currentRow++;
        }

        private void BuildTotal(ExcelWorksheet bodySheet, ExcelWorksheet totalSheet, IEnumerable<BALicWinLossDailyModel> list)
        {
            var model = CreateTotalExcelModel(list);
            bodySheet.ApplyModel(totalSheet, model, this.currentRow);
            var totalCell = bodySheet.Cells[this.currentRow, 2, this.currentRow, 3];
            totalCell.Merge = true;
            totalCell.Value = ExcelConstants.TotalText;
            this.currentRow++;
        }

        private void BuildGrandTotal(ExcelWorksheet bodySheet, ExcelWorksheet grandTotalSheet, IEnumerable<BALicWinLossDailyModel> list)
        {
            if (list.Any())
            {
                var model = CreateTotalExcelModel(list);
                bodySheet.ApplyModel(grandTotalSheet, model, this.currentRow);
                var totalCell = bodySheet.Cells[this.currentRow, 1, this.currentRow, 3];
                totalCell.Merge = true;
                totalCell.Value = ExcelConstants.GrandTotalText;
                this.currentRow++;
            }
        }

        private BALicWinLossDailyExcelModel CreateTotalExcelModel(IEnumerable<BALicWinLossDailyModel> list)
        {
            var model = new BALicWinLossDailyExcelModel();
            foreach (var item in list)
            {
                model.TurnOverRM += item.TurnOverRM;
                model.BetCount += item.BetCount;
                model.WinlostRM += item.WinlostRM;
                model.CommRM += item.CommRM;
                model.AmtDueToBARM += item.AmtDueToBARM;
            }
            model.Margin = CalculateMargin(model.WinlostRM, model.TurnOverRM);
            return model;
        }

        private void UpdateLicenseeCell(ExcelWorksheet bodySheet, int startRow, int endRow, string licenseeName)
        {
            var cell = bodySheet.Cells[startRow, ExcelConstants.LicenseeCol, endRow, ExcelConstants.LicenseeCol];
            cell.Value = licenseeName;
            cell.Merge = true;
        }

        private void UpdateLiveOrNonLiveCell(ExcelWorksheet bodySheet, int startRow, int endRow, bool isLive)
        {
            var cell = bodySheet.Cells[startRow, ExcelConstants.LiveOrNonLiveCol, endRow, ExcelConstants.LiveOrNonLiveCol];
            cell.Value = isLive ? ExcelConstants.LiveText : ExcelConstants.NonLiveText;
            cell.Merge = true;
            var color = isLive ? Color.FromArgb(221, 217, 196) : Color.FromArgb(118, 147, 60);
            cell.Style.Fill.SetBackground(color);
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
        }

        private double CalculateMargin(double winloss, double turnover)
        {
            return turnover == 0 ? 0 : Convert.ToDouble(-(winloss / turnover));
        }

        private static void ClearTemplate(ExcelPackage excelPackage)
        {
            excelPackage.Workbook.Worksheets.Delete("body");
            excelPackage.Workbook.Worksheets.Delete("header");
            excelPackage.Workbook.Worksheets.Delete("item");
            excelPackage.Workbook.Worksheets.Delete("subtotal");
            excelPackage.Workbook.Worksheets.Delete("total");
            excelPackage.Workbook.Worksheets.Delete("grandtotal");
            excelPackage.Workbook.Worksheets.Delete("headertitle");
        }
    }
}