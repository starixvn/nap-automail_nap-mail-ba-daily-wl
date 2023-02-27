using Fanex.Data.Repository;

namespace Fanex.BADailyWinLoss.Business.Criteria
{
    public class GetByDateCriteria : CriteriaBase
    {
        public string Date { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_BALicWinlossDaily";

        public override bool IsValid() => true;
    }
}
