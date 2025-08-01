using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;
using Daisy11Functions.Helpers;
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
    public async Task<HttpResponseData> Run_GetRole([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "GetRole/{agent}")] HttpRequestData req, string? agent)
    {
        _logger.LogInformation("Start at Run_GetRole");
        if (await TokenValidation.Validate(req) is { } validation) return validation;
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;

        Role? agentRecord = _projectContext.Role.FirstOrDefault(x => x.agent == agent && x.active);

        return await API.Success(response, new { Role = agentRecord == null ? "none" : agentRecord.role });
    }
}