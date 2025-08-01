using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;

namespace Daisy11Functions;

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

        //if (CORS.IsPreFlight(req)) return CORS.PreFlightData(req);

        Role? agentRecord = _projectContext.Role.FirstOrDefault(x => x.agent == agent && x.active);

        return new OkObjectResult(new { Role = agentRecord == null ? "none" : agentRecord.role });
    }
}