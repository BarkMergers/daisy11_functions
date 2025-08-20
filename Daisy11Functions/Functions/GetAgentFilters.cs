using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using MongoDB.Driver;
using MongoDB.Bson;
using Daisy11Functions.Helpers;
using System.Net;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Daisy11Functions.Database.Pagination;
using System.Net.Sockets;

namespace Daisy11Functions;

public class AgentFilterData
{
    public List<string>? Name { get; set; }
    public List<string>? Job { get; set; }
    public List<string>? Color { get; set; }
}


public class GetAgentFilter
{
    private readonly ILogger<GetCustomer> _logger;
    private readonly INewWorldContext _projectContext;

    public GetAgentFilter(ILogger<GetCustomer> logger, INewWorldContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("GetAgentFilter")]
    public async Task<HttpResponseData> Run_GetAgentFilter([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "GetAgentFilter")]
        HttpRequestData req)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        try
        {
            AgentFilterData output = new()
            {
                Name = new List<string>() { "Hart Hagerty", "Brice Swyre", "Marjy Frencz", "Yancy Tear" },
                Color = new List<string>() { "Red", "Purple" },
                Job = new List<string>() { "Zemlak, Daniel and Leannon", "Caroll Group", "Rowe-Schoen", "Wyman-Ledner" }
            };

            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}