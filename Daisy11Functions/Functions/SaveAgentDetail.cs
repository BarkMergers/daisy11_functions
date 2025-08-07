using Daisy11Functions.Auth;
using Daisy11Functions.Database;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Daisy11Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
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
    public int? age { get; set; }
}

public class SaveAgentDetail
{
    private readonly ILogger<SaveAgentDetail> _logger;
    private readonly GetTenantDetail _getTenantDetail;
    private readonly INewWorldContext _newWorldContext;
    private readonly IArchiveContext _archiveContext;

    public SaveAgentDetail(ILogger<SaveAgentDetail> logger, INewWorldContext newWorldContext, IArchiveContext archiveContext, GetTenantDetail getTenantDetail)
    {
        _logger = logger;
        _getTenantDetail = getTenantDetail;
        _newWorldContext = newWorldContext;
        _archiveContext = archiveContext;
    }

    [Function("SaveAgent")]
    public async Task<HttpResponseData> Run_SaveAgent([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "SaveAgent/")]
            HttpRequestData req)
    {

        _logger.LogInformation("Start at Run_SaveAgent");
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        UpdateAgentData bodyData = await GetRequestByBody.GetBody<UpdateAgentData>(req);
        Tenant? tenant = _getTenantDetail.Data(req);

        if (tenant == null)
            throw new Exception("Unknown tenant");

        Agent? roleRecord = _newWorldContext.Agent.FirstOrDefault(x => x.agent == bodyData.agent && x.tenantid == tenant.id);
        if (roleRecord == null)
        {
            roleRecord = new Agent();
            _newWorldContext.Agent.Add(roleRecord);
            roleRecord.agent = bodyData.agent;
            roleRecord.tenantid = tenant.id;
        }

        _archiveContext.Log.Add(new Database.Archive.Tables.Log()
        {
            message = $"The new message is as follows:{bodyData.firstname} {bodyData.lastname} age: {bodyData.age}",
            timestamp = DateTime.Now
        });

        roleRecord.firstname = bodyData.firstname;
        roleRecord.lastname = bodyData.lastname;
        roleRecord.age = bodyData.age;
        roleRecord.role = bodyData.role;
        roleRecord.active = bodyData.active;



        IDbContextTransaction newWorldTX = await _newWorldContext.BeginTransaction();
        IDbContextTransaction archiveTX = await _archiveContext.BeginTransaction();
        try
        {
            _newWorldContext.SaveChanges();
            _archiveContext.SaveChanges();
            await newWorldTX.CommitAsync();
            await archiveTX.CommitAsync();
            return await API.Success(response, new { Result = "Success" });
        }

        catch(Exception ex)
        {
            await newWorldTX.RollbackAsync();
            await archiveTX.RollbackAsync();
            return await API.Fail(response, System.Net.HttpStatusCode.InsufficientStorage, ex.InnerException == null ? "" : ex.InnerException.Message);
        }

    }
}