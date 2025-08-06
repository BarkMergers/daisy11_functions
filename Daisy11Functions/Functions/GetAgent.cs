using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using Daisy11Functions.Helpers;
using Daisy11Functions.Auth;

namespace Daisy11Functions;

public class GetAgent
{
    private readonly ILogger<GetAgent> _logger;
    private readonly IProjectContext _projectContext;
    private readonly GetTenantDetail _getTenantDetail;

    private class ReturnData
    {
        public long id { get; set; }
        public string? agent { get; set; }
        public string? role { get; set; }
        public string tenant { get; set; }
        public bool active { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int? age { get; set; }
    }




    public GetAgent(ILogger<GetAgent> logger, IProjectContext projectContext, GetTenantDetail getTenantDetail)
    {
        _logger = logger;
        _projectContext = projectContext;
        _getTenantDetail = getTenantDetail;
    }

    [Function("GetAgent")]
    public async Task<HttpResponseData> Run_GetAgent([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "GetAgent/{agent}")] 
            HttpRequestData req, string? agent)
    {
        _logger.LogInformation("Start at Run_GetAgent");
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req, _logger) is { } validation) return validation;

        Tenant? tenant = _getTenantDetail.Data(req);

        ReturnData? agentRecord = _projectContext.Agent.Where(x => x.agent == agent && x.tenantid == tenant.id)
                            .Select(x => new ReturnData
                            {
                                id = x.id,
                                active = x.active,
                                age = x.age,
                                agent = x.agent,
                                firstname = x.firstname,
                                lastname = x.lastname,
                                role = x.role,
                                tenant = tenant.tenantname
                            }).FirstOrDefault();

        return await API.Success(response, agentRecord = agentRecord == null ? new ReturnData() : agentRecord);
    }
}