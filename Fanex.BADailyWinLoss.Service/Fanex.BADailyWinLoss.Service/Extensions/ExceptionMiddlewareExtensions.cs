using Fanex.BADailyWinLoss.Service.Model;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Fanex.BADailyWinLoss.Service.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var logMessage = new LogMessage(contextFeature.Error.Message)
                            .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(contextFeature.Error))
                            .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(contextFeature.Error))
                            .ToString();
                        LogService.Error(logMessage);
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}
