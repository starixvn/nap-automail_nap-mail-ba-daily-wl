using Fanex.BADailyWinLoss.Service;
using Fanex.BADailyWinLoss.Service.Extensions;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
services.AddConfig();
services.AddFanexSql(configuration);
services.AddFanexLogSentry(configuration);
services.AddInternalServices();

// Configure services
builder.Services.AddControllers();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigureExceptionHandler();
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();

await app.RunAsync();