using Daisy11Functions.Auth;
using Daisy11Functions.Database;
using Daisy11Functions.Helpers;
using Daisy11Functions.Models.FilterAndSort;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NewWorldFunctions.Helpers;

namespace Daisy11Functions;

public class AssetFilterData
{
    public FilterDescription AssetName { get; set; } = new();
    public FilterDescription AssetTypeId { get; set; } = new();
    public FilterDescription RegistrationNumber { get; set; } = new();
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
            AssetFilterData output = new();

            output.AssetName.Type = "list";
            output.AssetTypeId.Type = "numberlist";
            output.RegistrationNumber.Type = "list";
            output.AssetName.Data = await _maintenanceContext.Asset.Select(x => x.AssetName).Distinct().ToListAsync();
            output.AssetTypeId.IntData = await _maintenanceContext.Asset.Select(x => x.AssetTypeId).Distinct().ToListAsync();
            output.RegistrationNumber.Data = await _maintenanceContext.Asset.Select(x => x.RegistrationNumber).Distinct().ToListAsync();



            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}