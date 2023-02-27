using Fanex.BADailyWinLoss.Business.Criteria;
using Fanex.BADailyWinLoss.Business.Model;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;

namespace Fanex.BADailyWinLoss.Business.Service
{
    public interface BetAggregatorService
    {
        IEnumerable<BALicWinLossDailyModel> GetWinlossDaily(DateTime date);

        IEnumerable<BALicWinLossDailyModel> GetWinlossMonthToDate(DateTime date);

        IEnumerable<BALicWinLossDailyModel> GetWinlossYearToDate(DateTime date);
    }

    public class BetAggregatorServiceImp : BetAggregatorService
    {
        private readonly IDynamicRepository repository;

        public BetAggregatorServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<BALicWinLossDailyModel> GetWinlossDaily(DateTime date)
        {
            var criteria = new GetByDateCriteria { Date = date.ToShortDateString() };
            return repository.Fetch<BALicWinLossDailyModel>(criteria);
        }

        public IEnumerable<BALicWinLossDailyModel> GetWinlossMonthToDate(DateTime date)
        {
            var criteria = new GetMonthToDateCriteria
            {
                FromDate = date.Month.ToString() + "/1/" + date.Year.ToString(),
                ToDate = date.ToShortDateString()
            };
            return repository.Fetch<BALicWinLossDailyModel>(criteria);
        }

        public IEnumerable<BALicWinLossDailyModel> GetWinlossYearToDate(DateTime date)
        {
            var criteria = new GetYearToDateCriteria
            {
                FromDate = "1/1/" + date.Year.ToString(),
                ToDate = date.ToShortDateString()
            };
            return repository.Fetch<BALicWinLossDailyModel>(criteria);
        }
    }
}
