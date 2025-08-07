using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using Daisy11Functions.Helpers;
using Daisy11Functions.Auth;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;

namespace Daisy11Functions;

public class GetAgent
{
    private readonly ILogger<GetAgent> _logger;
    private readonly INewWorldContext _projectContext;
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




    public GetAgent(ILogger<GetAgent> logger, INewWorldContext projectContext, GetTenantDetail getTenantDetail)
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

        try
        {




            Tenant? tenant = _getTenantDetail.Data(req);

            if (tenant == null)
                throw new Exception("Unknown tenant");



            string tenantName = (tenant == null || tenant.tenantname == null) ? "" : tenant.tenantname;

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
                tenant = tenantName
            }).FirstOrDefault();

            return await API.Success(response, agentRecord = agentRecord == null ? new ReturnData() { agent = agent, tenant = tenantName } : agentRecord);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}