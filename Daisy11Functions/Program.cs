using Daisy11Functions;
using Daisy11Functions.Auth;
using Daisy11Functions.Database;
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

string? connection = Environment.GetEnvironmentVariable("Daisy11Database");


builder.Services.AddDbContext<ProjectContext>(options => options.UseSqlServer(connection));
builder.Services.AddScoped<IProjectContext, ProjectContext>();
builder.Services.AddScoped<GetTenantDetail, GetTenantDetail>();


builder.Build().Run();
