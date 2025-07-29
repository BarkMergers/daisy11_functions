using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;

namespace Daisy11Functions;

public class Role
{
    public long id { get; set; }
    public string? agent { get; set; }
    public string? role { get; set; }
    public bool active { get; set; }
}

public interface IProjectContext: IDisposable
{
    DbSet<Role> Role { get; set; }
    int SaveChanges();
}

public class ProjectContext: DbContext, IProjectContext
{
    public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }
    public DbSet<Role> Role { get; set; }
}

public class Output
{
    public string? Role { get; set; }
}

public class GetRole
{
    private readonly ILogger<GetRole> _logger;
    private readonly IProjectContext _projectContext;

    public GetRole(ILogger<GetRole> logger, IProjectContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("GetRole")]
    public IActionResult Run_GetRole([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetRole/{agent}")] HttpRequestData req, string? agent)
    {
        _logger.LogInformation("Start at GetRole");

        if (CORS.IsPreFlight(req)) return CORS.PreFlightData();

        Role? agentRecord = _projectContext.Role.FirstOrDefault(x => x.agent == agent && x.active);

        return new OkObjectResult(new Output() { Role = agentRecord == null ? "none" : agentRecord.role });
    }
}