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
using Grpc.Core;
using System.Text.RegularExpressions;

namespace Daisy11Functions;







public class GetCustomer
{
    private readonly ILogger<GetCustomer> _logger;
    private readonly INewWorldContext _projectContext;


    private class FilterValues
    {
        public string? status { get; set; }
        public string? issuer { get; set; }
        public string? fineoperator { get; set; }
        public string? text { get; set; }
    }


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


            FilterValues? filterConfig = await GetRequestByBody.GetBody<FilterValues>(req);

            PaginationObject output = new();
            MongoClient dbClient = new MongoClient("mongodb+srv://mymongorabbit:dsad$3fer@mongorabbit.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000");
            //MongoClient dbClient = new MongoClient("mongodb://localhost:27017/local");
            IMongoDatabase database = dbClient.GetDatabase("local");
            IMongoCollection<Customer> collection = database.GetCollection<Customer>("customer");

            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Empty;


            if (filterConfig != null)
            {
                if (!string.IsNullOrWhiteSpace(filterConfig.text))
                {
                    var likeFilter = Builders<Customer>.Filter.Regex(
                        c => c.vehicle,
                        new BsonRegularExpression(".*" + Regex.Escape(filterConfig.text) + ".*", "i")
                    );

                    //FilterDefinition<Customer> filterOr = Builders<Customer>.Filter.Empty;
                    //filterOr = Builders<Customer>.Filter.Or(filter, textFilter1);
                    //filterOr = Builders<Customer>.Filter.(filter, textFilter2);
                    filter = Builders<Customer>.Filter.And(filter, likeFilter);
                }

                if (!string.IsNullOrWhiteSpace(filterConfig.status))
                {
                    FilterDefinition<Customer> statusFilter = Builders<Customer>.Filter.Eq(x => x.status, filterConfig.status);
                    filter = Builders<Customer>.Filter.And(filter, statusFilter);
                }

                if (!string.IsNullOrWhiteSpace(filterConfig.issuer))
                {
                    FilterDefinition<Customer> issuerFilter = Builders<Customer>.Filter.Eq(x => x.issuer, filterConfig.issuer);
                    filter = Builders<Customer>.Filter.And(filter, issuerFilter);
                }

                if (!string.IsNullOrWhiteSpace(filterConfig.fineoperator))
                {
                    FilterDefinition<Customer> fineOperatorFilter = Builders<Customer>.Filter.Eq(x => x.fineoperator, filterConfig.fineoperator);
                    filter = Builders<Customer>.Filter.And(filter, fineOperatorFilter);
                }
            }




            long totalCount = collection.Find(filter).CountDocuments();
            output.Data = collection.Find(filter).Skip(page).Limit(limit).ToList();

            //int totalCount = _projectContext.Customer.Count();
            //output.Data = _projectContext.Customer.OrderBy(x => x.id).Skip(page).Take(limit).ToList();

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
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }




    }
}