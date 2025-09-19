using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using MongoDB.Driver;
using Daisy11Functions.Helpers;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Microsoft.EntityFrameworkCore;
using Daisy11Functions.Auth;

namespace Daisy11Functions;

public class SaveCustomerDetails
{
    private readonly ILogger<SaveCustomerDetails> _logger;
    private readonly INewWorldContext _projectContext;

    public SaveCustomerDetails(ILogger<SaveCustomerDetails> logger, INewWorldContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("SaveCustomerDetails")]
    public async Task<HttpResponseData> Run_SaveCustomerDetails([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "SaveCustomerDetails")] 
        HttpRequestData req, int id)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        try
        {

            CustomerIn? newCustomerData = await GetRequestByBody.GetBody<CustomerIn>(req);




            //MongoClient dbClient = new MongoClient("mongodb+srv://mymongorabbit:dsad$3fer@mongorabbit.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000");
            ////MongoClient dbClient = new MongoClient("mongodb://localhost:27017/local");
            //IMongoDatabase database = dbClient.GetDatabase("local");
            //IMongoCollection<Customer> collection = database.GetCollection<Customer>("customer");
            //FilterDefinition<Customer> filter = Builders<Customer>.Filter.Empty;
            //FilterDefinition<Customer> statusFilter = Builders<Customer>.Filter.Eq(x => x.id, id);
            //filter = Builders<Customer>.Filter.And(filter, statusFilter);
            //Customer customerRecord = collection.Find(filter).FirstOrDefault();


            Customer? customerRecord = await _projectContext.Customer.FirstOrDefaultAsync(x => x.id == id);
            if (customerRecord != null)
            {
                DateTime? increaseDateTime = null;
                if (!string.IsNullOrEmpty(newCustomerData.increasedate) && DateTime.TryParse(newCustomerData.increasedate, out DateTime parsedDT))
                    increaseDateTime = parsedDT;

                customerRecord.vehicle = newCustomerData.vehicle;
                customerRecord.power = newCustomerData.power;
                customerRecord.issuer = newCustomerData.issuer;
                customerRecord.age = newCustomerData.age;
                customerRecord.fineoperator = newCustomerData.fineoperator;
                customerRecord.fineamount = newCustomerData.fineamount;
                customerRecord.status = newCustomerData.status;
                customerRecord.increasedate = increaseDateTime;
                //var updateResult = await collection.ReplaceOneAsync(filter, customerRecord);
                _projectContext.SaveChanges();
            }


            return await API.Success(response);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }
}