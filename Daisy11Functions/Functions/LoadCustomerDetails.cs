using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using MongoDB.Driver;
using Daisy11Functions.Helpers;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Microsoft.EntityFrameworkCore;

namespace Daisy11Functions;

public class LoadCustomerDetails
{
    private readonly ILogger<LoadCustomerDetails> _logger;
    private readonly INewWorldContext _projectContext;

    public LoadCustomerDetails(ILogger<LoadCustomerDetails> logger, INewWorldContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("LoadCustomerDetails")]
    public async Task<HttpResponseData> Run_LoadCustomerDetails([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "LoadCustomerDetails/{id}")] 
        HttpRequestData req, int id)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;





        try
        {



            Customer? output = await _projectContext.Customer.FirstOrDefaultAsync(x => x.id == id);



            //MongoClient dbClient = new MongoClient("mongodb+srv://mymongorabbit:dsad$3fer@mongorabbit.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000");
            ////MongoClient dbClient = new MongoClient("mongodb://localhost:27017/local");
            //IMongoDatabase database = dbClient.GetDatabase("local");
            //IMongoCollection<Customer> collection = database.GetCollection<Customer>("customer");

            //FilterDefinition<Customer> filter = Builders<Customer>.Filter.Empty;

            //FilterDefinition<Customer> statusFilter = Builders<Customer>.Filter.Eq(x => x.id, id);
            //filter = Builders<Customer>.Filter.And(filter, statusFilter);

            //Customer output = collection.Find(filter).FirstOrDefault();

            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}