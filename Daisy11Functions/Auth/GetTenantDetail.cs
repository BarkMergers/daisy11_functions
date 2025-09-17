using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;

namespace Daisy11Functions.Auth;

public class GetTenantDetail
{
    private readonly ILogger<GetAgentDetails> _logger;
    private readonly INewWorldContext _projectContext;

    public GetTenantDetail(ILogger<GetAgentDetails> logger, INewWorldContext projectContext)
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