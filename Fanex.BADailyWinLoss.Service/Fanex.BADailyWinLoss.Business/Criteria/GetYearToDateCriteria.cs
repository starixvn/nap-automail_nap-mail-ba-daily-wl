using Fanex.Data.Repository;

namespace Fanex.BADailyWinLoss.Business.Criteria
{
    class GetYearToDateCriteria : CriteriaBase
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_BALicWinloss";

        public override bool IsValid() => true;
    }
}
