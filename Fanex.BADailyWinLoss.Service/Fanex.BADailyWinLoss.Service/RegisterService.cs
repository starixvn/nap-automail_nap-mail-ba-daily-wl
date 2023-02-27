using AutoMapper;
using Fanex.BADailyWinLoss.Business.Model;
using Fanex.BADailyWinLoss.Business.Service;
using Fanex.BADailyWinLoss.Service.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.BADailyWinLoss.Service
{
    public static class RegisterService
    {
        public static void RegisterServices(IConfiguration Configuration, IServiceCollection services)
        {
            LoadConfigModel();
            ConfigureFanexData(Configuration);
            SentryRegister(Configuration);
            DependencyInjection(services);
        }

        private static void ConfigureFanexData(IConfiguration Configuration)
        {
            var connections = Configuration.GetSection("ConnectionStrings")
                                   .GetChildren()
                                   .AsEnumerable()
                                   .ToDictionary(
                                            x => x.GetValue<string>("Name"),
                                            y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
                                    );

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                .WithSqlServer()
                .UseDefaultDbSettingProvider(Configuration.GetSection("StoredProcedurePath").Value)
                .Run();
        }

        private static void SentryRegister(IConfiguration Configuration)
        {
            LogManager
            .SetDefaultLogCategory(Configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("SentryEngineOptions:Dsn").Value)
                })
            });
        }

        private static void LoadConfigModel()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("appsettings.json");
            ConfigService.Config = JsonConvert.DeserializeObject<ConfigModel>(text);
        }

        private static void DependencyInjection(IServiceCollection services)
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
