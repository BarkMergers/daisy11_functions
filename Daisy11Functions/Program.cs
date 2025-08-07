using Daisy11Functions;
using Daisy11Functions.Auth;
using Daisy11Functions.Database;
using Daisy11Functions.Database.NewWorld;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

string? newWorldConnection = Environment.GetEnvironmentVariable("NewWorldDatabase");
builder.Services.AddDbContext<NewWorldContext>(options => options.UseSqlServer(newWorldConnection));
builder.Services.AddScoped<INewWorldContext, NewWorldContext>();


string? archiveConnection = Environment.GetEnvironmentVariable("ArchiveDatabase");
builder.Services.AddDbContext<ArchiveContext>(options => options.UseSqlServer(archiveConnection));
builder.Services.AddScoped<IArchiveContext, ArchiveContext>();


builder.Services.AddScoped<GetTenantDetail, GetTenantDetail>();
builder.Build().Run();