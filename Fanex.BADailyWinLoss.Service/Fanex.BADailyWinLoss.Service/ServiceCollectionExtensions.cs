using AutoMapper;
using Fanex.BADailyWinLoss.Business.Model;
using Fanex.BADailyWinLoss.Business.Service;
using Fanex.BADailyWinLoss.Service.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Fanex.BADailyWinLoss.Service
{
    public static class ServiceCollectionExtensions
    {
        public static void AddConfig(this IServiceCollection services)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("appsettings.json");
            ConfigService.Config = JsonConvert.DeserializeObject<ConfigModel>(text);
        }

        public static void AddInternalServices(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<BetAggregatorService, BetAggregatorServiceImp>();
            services.AddSingleton<MailingService, MailingServiceImp>();
            services.AddSingleton<ExcelService, ExcelServiceImp>();
            services.AddSingleton<BAReportService, BAReportServiceImp>();
            services.AddSingleton<Log, LogImp>();
            services.AddSingleton(CreateMapper);
        }

        private static IMapper CreateMapper
        {
            get
            {
                var mapperConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new BADailyWinLossProfile());
                });

                return mapperConfig.CreateMapper();
            }
        }
    }
}
