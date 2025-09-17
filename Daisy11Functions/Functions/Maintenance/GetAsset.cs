using Daisy11Functions.Auth;
using Daisy11Functions.Database;
using Daisy11Functions.Database.Maintenance.Tables;
using Daisy11Functions.Database.Pagination;
using Daisy11Functions.Helpers;
using Daisy11Functions.Models.FilterAndSort;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NewWorldFunctions.Helpers;
using StackExchange.Redis;

namespace Daisy11Functions;


public class GetAsset
{
    private readonly ILogger<GetAsset> _logger;
    private readonly IMaintenanceContext _maintenanceContext;




    private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        string? redisConnection = Environment.GetEnvironmentVariable("RedisConnection");
        if (string.IsNullOrWhiteSpace(redisConnection))
            throw new Exception("RedisConnection not found/configured");
        return ConnectionMultiplexer.Connect(redisConnection);
    });

    public static ConnectionMultiplexer Connection => lazyConnection.Value;






    public GetAsset(ILogger<GetAsset> logger, IMaintenanceContext maintenanceContext)
    {
        _logger = logger;
        _maintenanceContext = maintenanceContext;
    }


    [Function("GetAsset")]
    public async Task<HttpResponseData> Run_GetAsset([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "GetAsset/{firstRecord}/{limit}")] 
        HttpRequestData req, int firstRecord, int limit)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;


        try
        {
            long countValue = 0;

            FilterAndSortValues? filterAndSortConfig = await GetRequestByBody.GetBody<FilterAndSortValues>(req);

            AssetPaginationObject output = new();
            int totalCount = -1;

            IQueryable<Asset> unSortedData = _maintenanceContext.Asset;

            IOrderedQueryable<Asset> sortedData;

            if (filterAndSortConfig == null)
            {
                //totalCount = await unSortedData.CountAsync();
                sortedData = unSortedData.OrderBy(x => x.RecordId);
            }
            else
            {
                Dictionary<string, object>? filters = filterAndSortConfig.filterValues;

                if (filters != null)
                {
                    if (filters.ContainsKey("assetName") && !string.IsNullOrWhiteSpace(filters["assetName"].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.AssetName != null && x.AssetName.Contains(filters["assetName"].ToString()));
                    }

                    if (filters.ContainsKey("assetTypeId") && !string.IsNullOrWhiteSpace(filters["assetTypeId"].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.AssetTypeId == Convert.ToInt32(filters["assetTypeId"]));
                    }

                    if (filters.ContainsKey("registrationNumber") && !string.IsNullOrWhiteSpace(filters["registrationNumber"].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.RegistrationNumber != null && x.RegistrationNumber.Contains(filters["registrationNumber"].ToString()));
                    }
                }

                // totalCount = await unSortedData.CountAsync();

                switch (filterAndSortConfig?.SortValues?.FieldName)
                {
                    case "recordId": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(x => x.RecordId) : unSortedData.OrderByDescending(bson => bson.RecordId); break;
                    case "assetName": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.AssetName) : unSortedData.OrderByDescending(bson => bson.AssetName); break;
                    case "registrationNumber": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.RegistrationNumber) : unSortedData.OrderByDescending(bson => bson.RegistrationNumber); break;
                    case "assetTypeId": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.AssetTypeId) : unSortedData.OrderByDescending(bson => bson.AssetTypeId); break;
                    default: sortedData = unSortedData.OrderBy(bson => bson.RecordId); break;
                }
            }

            output.Data = sortedData.Skip(firstRecord).Take(limit).ToList();


            output.Pagination = new PaginationData()
            {
                PageId = firstRecord / limit,
                CurrentPage = firstRecord,
                TotalItems = limit,
                TotalPages = -1, // Convert.ToInt32(decimal.Ceiling(decimal.Divide(totalCount, limit))),
                HasMore = true //page + limit <= totalCount
            };


            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Runtime error:");
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}