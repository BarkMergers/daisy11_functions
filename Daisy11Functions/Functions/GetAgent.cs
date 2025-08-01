using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using Daisy11Functions.Helpers;

namespace Daisy11Functions;

public class GetAgent
{
    private readonly ILogger<GetAgent> _logger;
    private readonly IProjectContext _projectContext;

    public GetAgent(ILogger<GetAgent> logger, IProjectContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("GetAgent")]
    public async Task<HttpResponseData> Run_GetAgent([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "GetAgent/{agent}")] 
            HttpRequestData req, string? agent)
    {
        _logger.LogInformation("Start at Run_GetAgent");
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req, _logger) is { } validation) return validation;

        //Role? agentRecord = _projectContext.Role.FirstOrDefault(x => x.agent == agent);

        Role agentRecord = new Role()
        {
            firstname = "Mark",
            lastname = "Burgess",
            role = "Comms",
            age = 22,
            active = true
        };

        return await API.Success(response, agentRecord = agentRecord == null ? new Role() : agentRecord);
    }
}