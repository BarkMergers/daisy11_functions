using Daisy11Functions.Database;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using MongoDB.Driver;
using MongoDB.Bson;
using Daisy11Functions.Database.Tables;
using Daisy11Functions.Helpers;

namespace Daisy11Functions;

public class GetCustomer
{
    private readonly ILogger<GetRole> _logger;
    private readonly IProjectContext _projectContext;

    public GetCustomer(ILogger<GetRole> logger, IProjectContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("GetCustomer")]
    public async Task<HttpResponseData> Run_GetCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "GetCustomer/{page}/{limit}")] 
        HttpRequestData req, int page, int limit)
    {

        _logger.LogInformation("Start at Run_GetCustomer");
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;


        PaginationObject output = new();

        MongoClient dbClient = new MongoClient("mongodb+srv://mymongorabbit:dsad$3fer@mongorabbit.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000");
        //MongoClient dbClient = new MongoClient("mongodb://localhost:27017/local");
        IMongoDatabase database = dbClient.GetDatabase("local");
        IMongoCollection<Customer> collection = database.GetCollection<Customer>("customer");
        long totalCount = collection.Find(new BsonDocument()).CountDocuments();
        output.Data = collection.Find(new BsonDocument()).Skip(page).Limit(limit).ToList();

        //int totalCount = _projectContext.Customer.Count();
        //output.Data = _projectContext.Customer.OrderBy(x => x.id).Skip(page).Take(limit).ToList();

        output.Pagination = new PaginationData()
        {
            CurrentPage = page,
            TotalItems = limit,
            TotalPages = Convert.ToInt32(decimal.Ceiling(decimal.Divide(totalCount, limit))),
            HasMore = page + limit <= totalCount
        };

        return await API.Success(response, output);
    }
}