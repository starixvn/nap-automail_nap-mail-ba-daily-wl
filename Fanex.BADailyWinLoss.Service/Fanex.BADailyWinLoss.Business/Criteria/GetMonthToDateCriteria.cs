using Fanex.Data.Repository;

namespace Fanex.BADailyWinLoss.Business.Criteria
{
    public class GetMonthToDateCriteria : CriteriaBase
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_BALicWinloss";

        public override bool IsValid() => true;
    }
}
