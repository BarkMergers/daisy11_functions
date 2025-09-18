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
using Daisy11Functions.Models.FilterAndSort;

namespace Daisy11Functions;

public class AccountFilterData
{
    public FilterDescription AccountName { get; set; } = new();
    public FilterDescription RegistrationNumber { get; set; } = new();
    public FilterDescription VATRegNo { get; set; } = new();
}

public class GetAccountFilter
{
    private readonly ILogger<GetAccountFilter> _logger;
    private readonly IMaintenanceContext _maintenanceContext;

    public GetAccountFilter(ILogger<GetAccountFilter> logger, IMaintenanceContext maintenanceContext)
    {
        _logger = logger;
        _maintenanceContext = maintenanceContext;
    }

    [Function("GetAccountFilter")]
    public async Task<HttpResponseData> Run_GetAccountFilter([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "GetAccountFilter")]
        HttpRequestData req)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        try
        {
            AccountFilterData output = new();

            output.AccountName.Type = "list";
            output.RegistrationNumber.Type = "list";
            output.VATRegNo.Type = "list";
            output.AccountName.Data = await _maintenanceContext.Account.Select(x => x.AccountName).Distinct().ToListAsync();
            output.RegistrationNumber.Data = await _maintenanceContext.Account.Select(x => x.RegistrationNumber).Distinct().ToListAsync();
            output.VATRegNo.Data = await _maintenanceContext.Account.Select(x => x.VATRegNo).Distinct().ToListAsync();






            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}