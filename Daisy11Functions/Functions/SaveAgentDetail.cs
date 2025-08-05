using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;
using Daisy11Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;

namespace Daisy11Functions;

public class UpdateAgentData
{
    public string? agent { get; set; }
    public string? firstname { get; set; }
    public string? lastname { get; set; }
    public string? role { get; set; }
    public bool active { get; set; }
    public int age { get; set; }
}

public class SaveAgentDetail
{
    private readonly ILogger<SaveAgentDetail> _logger;
    private readonly IProjectContext _projectContext;

    public SaveAgentDetail(ILogger<SaveAgentDetail> logger, IProjectContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("SaveAgent")]
    public async Task<HttpResponseData> Run_SaveAgent([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "SaveAgent/")]
            HttpRequestData req)
    {

        _logger.LogInformation("Start at Run_GetAgent");
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        UpdateAgentData bodyData = await GetRequestByBody.GetBody<UpdateAgentData>(req);
        string tenant = GetTenant.Value(req);

        Role? roleRecord = _projectContext.Role.FirstOrDefault(x => x.agent == bodyData.agent && x.tenant == tenant);
        if (roleRecord != null)
        {
            roleRecord.firstname = bodyData.firstname;
            roleRecord.lastname = bodyData.lastname;
            roleRecord.age = bodyData.age;
            roleRecord.role = bodyData.role;
            roleRecord.active = bodyData.active;
        }
        _projectContext.SaveChanges();

        return await API.Success(response, new { Result = "Success" });
    }
}