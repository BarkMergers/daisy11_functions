using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;

namespace Daisy11Functions.Auth;

public class GetTenantDetail
{
    private readonly ILogger<GetAgent> _logger;
    private readonly IProjectContext _projectContext;


    public GetTenantDetail(ILogger<GetAgent> logger, IProjectContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }


    public Tenant? Data(HttpRequestData req)
    {
        string subdomain = GetSubdomain.Value(req);
        return _projectContext.Tenant.FirstOrDefault(x => x.subdomain == subdomain);
    }
}