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
using Daisy11Functions.Auth;
using Daisy11Functions.Database;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Daisy11Functions;

public class AssetFilterData
{
    public List<string?>? AssetName { get; set; }
    public List<int>? AssetTypeId { get; set; }
    public List<string?>? RegistrationNumber { get; set; }
}

public class GetAssetFilter
{
    private readonly ILogger<GetAccountFilter> _logger;
    private readonly IMaintenanceContext _maintenanceContext;

    public GetAssetFilter(ILogger<GetAccountFilter> logger, IMaintenanceContext maintenanceContext)
    {
        _logger = logger;
        _maintenanceContext = maintenanceContext;
    }

    [Function("GetAssetFilter")]
    public async Task<HttpResponseData> Run_GetAssetFilter([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "GetAssetFilter")]
        HttpRequestData req)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        try
        {
            AssetFilterData output = new()
            {
                AssetName = await _maintenanceContext.Asset.Select(x => x.AssetName).Distinct().ToListAsync(),
                AssetTypeId = await _maintenanceContext.Asset.Select(x => x.AssetTypeId).Distinct().ToListAsync(),
                RegistrationNumber = await _maintenanceContext.Asset.Select(x => x.RegistrationNumber).Distinct().ToListAsync()
            };

            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}