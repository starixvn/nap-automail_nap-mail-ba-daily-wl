using AutoMapper;
using Fanex.BADailyWinLoss.Business.Model;

namespace Fanex.BADailyWinLoss.Service
{
    public class BADailyWinLossProfile : Profile
    {
        public BADailyWinLossProfile()
        {
            CreateMap<BALicWinLossDailyModel, BALicWinLossDailyExcelModel>();
        }
    }
}
