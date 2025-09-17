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

public class GetAccount
{
    private readonly ILogger<GetAccount> _logger;
    private readonly IMaintenanceContext _maintenanceContext;

    private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        string? redisConnection = Environment.GetEnvironmentVariable("RedisConnection");
        if (string.IsNullOrWhiteSpace(redisConnection))
            throw new Exception("RedisConnection not found/configured");
        return ConnectionMultiplexer.Connect(redisConnection);
    });

    public static ConnectionMultiplexer Connection => lazyConnection.Value;


    public GetAccount(ILogger<GetAccount> logger, IMaintenanceContext maintenanceContext)
    {
        _logger = logger;
        _maintenanceContext = maintenanceContext;
    }

    [Function("GetAccount")]
    public async Task<HttpResponseData> Run_GetAccount([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "GetAccount/{page}/{limit}")] 
        HttpRequestData req, int page, int limit)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        try
        {
            long countValue = 0;

            FilterAndSortValues? filterAndSortConfig = await GetRequestByBody.GetBody<FilterAndSortValues>(req);

            AccountPaginationObject output = new();
            int totalCount = 0;

            IQueryable<Account> unSortedData = _maintenanceContext.Account;
            IOrderedQueryable<Account> sortedData;

            if (filterAndSortConfig == null)
            {
                totalCount = await unSortedData.CountAsync();
                sortedData = unSortedData.OrderBy(x => x.RecordId);
            }
            else
            {
                Dictionary<string, object>? filters = filterAndSortConfig.filterValues;

                if (filters != null)
                {
                    if (filters.ContainsKey("accountName") && !string.IsNullOrWhiteSpace(filters["accountName"].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.AccountName == filters["accountName"].ToString());
                    }

                    if (filters.ContainsKey("vatRegNo") && !string.IsNullOrWhiteSpace(filters["vatRegNo"].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.VATRegNo == filters["vatRegNo"].ToString());
                    }

                    if (filters.ContainsKey("registrationNumber") && !string.IsNullOrWhiteSpace(filters["registrationNumber"].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.RegistrationNumber == filters["registrationNumber"].ToString());
                    }
                }

                totalCount = await unSortedData.CountAsync();

                switch (filterAndSortConfig?.SortValues?.FieldName)
                {
                    case "recordId": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(x => x.RecordId) : unSortedData.OrderByDescending(bson => bson.RecordId); break;
                    case "accountName": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.AccountName) : unSortedData.OrderByDescending(bson => bson.AccountName); break;
                    case "vatRegNo": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.VATRegNo) : unSortedData.OrderByDescending(bson => bson.VATRegNo); break;
                    case "registrationNo": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.RegistrationNumber) : unSortedData.OrderByDescending(bson => bson.RegistrationNumber); break;
                    default: sortedData = unSortedData.OrderBy(bson => bson.RecordId); break;
                }
            }

            output.Data = sortedData.Skip(page).Take(limit).ToList();

            output.Pagination = new PaginationData()
            {
                PageId = page / limit,
                CurrentPage = page,
                TotalItems = limit,
                TotalPages = Convert.ToInt32(decimal.Ceiling(decimal.Divide(totalCount, limit))),
                HasMore = page + limit <= totalCount
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