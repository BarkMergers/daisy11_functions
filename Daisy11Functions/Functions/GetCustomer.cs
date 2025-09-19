using Daisy11Functions.Auth;
using Daisy11Functions.Database.Archive.Tables;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Daisy11Functions.Database.Pagination;
using Daisy11Functions.Helpers;
using Daisy11Functions.Models.FilterAndSort;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using NewWorldFunctions.Helpers;
using StackExchange.Redis;
using System.Drawing.Text;
using System.Linq.Expressions;

namespace Daisy11Functions;







public class GetCustomer
{
    private readonly ILogger<GetCustomer> _logger;
    private readonly INewWorldContext _projectContext;




    private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        string? redisConnection = Environment.GetEnvironmentVariable("RedisConnection");
        if (string.IsNullOrWhiteSpace(redisConnection))
            throw new Exception("RedisConnection not found/configured");
        return ConnectionMultiplexer.Connect(redisConnection);
    });

    public static ConnectionMultiplexer Connection => lazyConnection.Value;






    public GetCustomer(ILogger<GetCustomer> logger, INewWorldContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }


    [Function("GetCustomer")]
    public async Task<HttpResponseData> Run_GetCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "GetCustomer/{page}/{limit}")] 
        HttpRequestData req, int page, int limit)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;






        try
        {

            /*
            IDatabase db = Connection.GetDatabase();
            if (!db.KeyExists("useCounter"))
            {
                db.StringSet("useCounter", 0);
            }
            long.TryParse(db.StringGet("useCounter"), out long countValue);
            db.StringSet("useCounter", countValue + 1);
            */

            long countValue = 0;





            FilterAndSortValues? filterAndSortConfig = await GetRequestByBody.GetBody<FilterAndSortValues>(req);

            /*

            PaginationObject output = new();
            MongoClient dbClient = new MongoClient("mongodb+srv://mymongorabbit:dsad$3fer@mongorabbit.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000");
            //MongoClient dbClient = new MongoClient("mongodb://localhost:27017/local");
            IMongoDatabase database = dbClient.GetDatabase("local");
            IMongoCollection<Customer> collection = database.GetCollection<Customer>("customer");

            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Empty;

            if (filterAndSortConfig != null)
            {
                Dictionary<string, object>? filters = filterAndSortConfig.filterValues;

                //if (!string.IsNullOrWhiteSpace(filterAndSortConfig?.filterValues?["Text"].ToString()))
                //{

                //    string? filterValueText = filterAndSortConfig?.filterValues?["Text"].ToString();

                //    var likeFilter = Builders<Customer>.Filter.Regex(
                //        c => c.vehicle,
                //        new BsonRegularExpression(".*" + Regex.Escape(string.IsNullOrEmpty(filterValueText) ? "" : filterValueText) + ".*", "i")
                //    );

                //    //FilterDefinition<Customer> filterOr = Builders<Customer>.Filter.Empty;
                //    //filterOr = Builders<Customer>.Filter.Or(filter, textFilter1);
                //    //filterOr = Builders<Customer>.Filter.(filter, textFilter2);
                //    filter = Builders<Customer>.Filter.And(filter, likeFilter);
                //}

                if (filters != null)
                {
                    if (filters.ContainsKey("status") && !string.IsNullOrWhiteSpace(filters["status"].ToString()))
                    {
                        FilterDefinition<Customer> statusFilter = Builders<Customer>.Filter.Eq(x => x.status, filters["status"].ToString());
                        filter = Builders<Customer>.Filter.And(filter, statusFilter);
                    }

                    if (filters.ContainsKey("issuer") && !string.IsNullOrWhiteSpace(filters["issuer"].ToString()))
                    {
                        FilterDefinition<Customer> issuerFilter = Builders<Customer>.Filter.Eq(x => x.issuer, filters["issuer"].ToString());
                        filter = Builders<Customer>.Filter.And(filter, issuerFilter);
                    }

                    if (filters.ContainsKey("fineOperator") && !string.IsNullOrWhiteSpace(filters["fineOperator"].ToString()))
                    {
                        FilterDefinition<Customer> fineOperatorFilter = Builders<Customer>.Filter.Eq(x => x.fineoperator, filters["fineOperator"].ToString());
                        filter = Builders<Customer>.Filter.And(filter, fineOperatorFilter);
                    }
                }
            }

            long totalCount = collection.Find(filter).CountDocuments();

            IFindFluent<Customer, Customer> unSortedData = collection.Find(filter);
            IOrderedFindFluent<Customer, Customer> sortedData;

            switch (filterAndSortConfig?.SortValues?.FieldName)
            {
                case "id": sortedData =  filterAndSortConfig.SortValues.SortOrder == "ascending" ?  unSortedData.SortBy(bson => bson.id) : unSortedData.SortByDescending(bson => bson.id); break;
                case "vehicle": sortedData =  filterAndSortConfig.SortValues.SortOrder == "ascending" ?  unSortedData.SortBy(bson => bson.vehicle) : unSortedData.SortByDescending(bson => bson.vehicle); break;
                case "increasedate": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.SortBy(bson => bson.increasedate) : unSortedData.SortByDescending(bson => bson.increasedate); break;
                case "fineoperator": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.SortBy(bson => bson.fineoperator) : unSortedData.SortByDescending(bson => bson.fineoperator); break;
                default: sortedData = unSortedData.SortBy(bson => bson.id); break;
            }

            output.Data = sortedData.Skip(page).Limit(limit).ToList();


            */



            PaginationObject output = new();
            int totalCount = 0; // _projectContext.Customer.Count();

            IQueryable<Customer> unSortedData = _projectContext.Customer;



            IOrderedQueryable<Customer> sortedData;

            if (filterAndSortConfig == null)
            {
                totalCount = await unSortedData.CountAsync();
                sortedData = unSortedData.OrderBy(x => x.id);
            }
            else
            {
                Dictionary<string, object>? filters = filterAndSortConfig.filterValues;

                const string statusKeyName = "status";
                const string issuerKeyName = "issuer";
                const string fineOpKeyName = "fineOperator";
                const string dateTimeKeyName = "dateTime";


                if (filters != null)
                {
                    if (filters.ContainsKey(statusKeyName) && !string.IsNullOrWhiteSpace(filters[statusKeyName].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.status == filters[statusKeyName].ToString());
                    }

                    if (filters.ContainsKey(issuerKeyName) && !string.IsNullOrWhiteSpace(filters[issuerKeyName].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.issuer == filters[issuerKeyName].ToString());
                    }

                    if (filters.ContainsKey(fineOpKeyName) && !string.IsNullOrWhiteSpace(filters[fineOpKeyName].ToString()))
                    {
                        unSortedData = unSortedData.Where(x => x.fineoperator == filters[fineOpKeyName].ToString());
                    }

                    if (filters.ContainsKey(dateTimeKeyName) && !string.IsNullOrEmpty(filters[dateTimeKeyName].ToString()))
                    {
                        const string modeKey = "mode";


                        JObject obj = JObject.Parse(filters[dateTimeKeyName].ToString()!);

                        string mode = obj[modeKey]!.ToString();
                        switch (mode)
                        {
                            case "before":
                            case "after":
                                unSortedData = unSortedData.Where(GetDateTimeFilter(mode, obj["date"]!.ToString()));
                                break;
                            case "between":
                                unSortedData = unSortedData.Where(GetDateTimeFilter(mode, obj["from"]!.ToString(), obj["to"]!.ToString()));
                                break;
                            default:
                                break; // Log that something isn't configured correctly
                        }
                    }
                }

                totalCount = await unSortedData.CountAsync();

                switch (filterAndSortConfig?.SortValues?.FieldName)
                {
                    case "id": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(x => x.id) : unSortedData.OrderByDescending(bson => bson.id); break;
                    case "vehicle": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.vehicle) : unSortedData.OrderByDescending(bson => bson.vehicle); break;
                    case "increasedate": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.increasedate) : unSortedData.OrderByDescending(bson => bson.increasedate); break;
                    case "fineoperator": sortedData = filterAndSortConfig.SortValues.SortOrder == "ascending" ? unSortedData.OrderBy(bson => bson.fineoperator) : unSortedData.OrderByDescending(bson => bson.fineoperator); break;
                    default: sortedData = unSortedData.OrderBy(bson => bson.id); break;
                }
            }

            output.Data = sortedData.Skip(page * limit).Take(limit).ToList();


            output.Pagination = new PaginationData()
            {
                PageId = page / limit,
                CurrentPage = page,
                TotalItems = limit,
                TotalPages = Convert.ToInt32(decimal.Ceiling(decimal.Divide(totalCount, limit))),
                HasMore = page + limit <= totalCount
            };

            foreach (var item in output.Data)
            {
                item.vehicle +=  $" - #{countValue}";
            }

            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Runtime error:");
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }

    private Expression<Func<Customer, bool>> GetDateTimeFilter(string mode, string beforeDate, string? afterDate = null)
    {
        // Check to make sure required values are not empty
        if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(beforeDate))
            return null!;

        // Check to make sure date is valid
        if (!DateTime.TryParse(beforeDate, out DateTime beforeResult))
            return null!;


        DateTime afterResult = default(DateTime);
        // This checks to make sure an input is valid if provided
        if (!string.IsNullOrEmpty(afterDate) && !DateTime.TryParse(afterDate, out afterResult)) 
            return null!;

        if (afterResult.Ticks != default(DateTime).Ticks) // Only happens if an "after" date is provided
        {
            DateTime tempBefore, tempAfter;
            if (beforeResult > afterResult) // If wrong way around, correct them by swapping the values
            {
                tempBefore = afterResult;
                tempAfter = beforeResult;

                afterResult = tempAfter;
                beforeResult = tempBefore;
            }
        }

        switch (mode.ToLower())
        {
            case "before":
                return x => x.increasedate.HasValue
                            && x.increasedate.Value.Date <= beforeResult.Date;

            case "after":
                return x => x.increasedate.HasValue 
                            && x.increasedate.Value.Date >= beforeResult.Date;

            case "between":
                return x => x.increasedate.HasValue
                            && afterResult.Ticks != default(DateTime).Ticks // Checks to make sure a value has been parsed
                            && x.increasedate.Value.Date >= beforeResult.Date
                            && x.increasedate.Value.Date <= afterResult.Date;

            default:
                return x => false;
        }
    }
}