using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;

namespace Daisy11Functions;



public class UpdateAgentData
{
    public string agent { get; set; }
    public string? firstname { get; set; }
    public string? lastname { get; set; }
    public string? role { get; set; }
    public bool active { get; set; }
    public int age { get; set; }
}



public class SaveAgentDetail
{
    private readonly ILogger<GetRole> _logger;
    private readonly IProjectContext _projectContext;

    public SaveAgentDetail(ILogger<GetRole> logger, IProjectContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("SaveAgentDetail")]
    public async Task<IActionResult> Run_SaveAgentDetail([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "SaveAgentDetail")] 
            HttpRequestData req)
    {
        _logger.LogInformation("Start at GetRole");

        if (CORS.IsPreFlight(req)) return CORS.PreFlightData();

        UpdateAgentData bodyData = await GetRequestByBody.GetBody<UpdateAgentData>(req);

        Role? roleRecord = _projectContext.Role.FirstOrDefault(x => x.agent == bodyData.agent);

        if (roleRecord != null)
        {
            roleRecord.firstname = bodyData.firstname;
            roleRecord.lastname = bodyData.lastname;
            roleRecord.age = bodyData.age;
            roleRecord.role = bodyData.role;
            roleRecord.active = bodyData.active;
        }

        return new OkObjectResult(new { Result = "Success" });
    }
}