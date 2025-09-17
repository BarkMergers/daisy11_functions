using Daisy11Functions.Auth;
using Daisy11Functions.Database;
using Daisy11Functions.Database.NewWorld;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();


var logger = builder.Services.BuildServiceProvider()
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Startup");


string? newWorldConnection = Environment.GetEnvironmentVariable("NewSQLConnection");
if (string.IsNullOrWhiteSpace(newWorldConnection))
    logger.LogInformation("Environmental variable 'NewSQLConnection' is not set");

DatabaseMigrator.EnsureDatabase(newWorldConnection);

builder.Services.AddDbContext<NewWorldContext>(options => options.UseSqlServer(newWorldConnection));
builder.Services.AddScoped<INewWorldContext, NewWorldContext>();

string? archiveConnection = Environment.GetEnvironmentVariable("ArchiveDatabase");
if (string.IsNullOrWhiteSpace(archiveConnection))
    logger.LogInformation("Environmental variable 'ArchiveDatabase' is not set");

builder.Services.AddDbContext<ArchiveContext>(options => options.UseSqlServer(archiveConnection));
builder.Services.AddScoped<IArchiveContext, ArchiveContext>();
builder.Services.AddScoped<GetTenantDetail, GetTenantDetail>();




string? maintenanceConnection = Environment.GetEnvironmentVariable("MaintenanceDatabase");
if (string.IsNullOrWhiteSpace(maintenanceConnection))
    logger.LogInformation("Environmental variable 'MaintenanceDatabase' is not set");

builder.Services.AddDbContext<MaintenanceContext>(options => options.UseSqlServer(maintenanceConnection));
builder.Services.AddScoped<IMaintenanceContext, MaintenanceContext>();




builder.Build().Run();