using Daisy11Functions.Auth;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Daisy11Functions.Helpers;
using Daisy11Functions.Models.FilterAndSort;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NewWorldFunctions.Helpers;

namespace Daisy11Functions;



public class CustomerFilterData
{
    public FilterDescription Status { get; set; } = new();
    public FilterDescription FineOperator { get; set; } = new();
    public FilterDescription Issuer { get; set; } = new();
}






public class GetCustomerFilter
{
    private readonly ILogger<GetCustomer> _logger;
    private readonly INewWorldContext _projectContext;

    public GetCustomerFilter(ILogger<GetCustomer> logger, INewWorldContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("GetCustomerFilter")]
    public async Task<HttpResponseData> Run_GetCustomerFilter([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "GetCustomerFilter")]
        HttpRequestData req)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        if (await TokenValidation.Validate(req) is { } validation) return validation;

        try
        {

            //MongoClient dbClient = new MongoClient("mongodb+srv://mymongorabbit:dsad$3fer@mongorabbit.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000");
            //CustomerFilterData output = new()
            //{
            //    Status. = await GetDistinctList(dbClient, "status"),
            //    FineOperatorList = await GetDistinctList(dbClient, "fineoperator"),
            //    IssuerList = await GetDistinctList(dbClient, "issuer")
            //};


            CustomerFilterData output = new();

            output.Status.Type = "list";
            output.FineOperator.Type = "list";
            output.Issuer.Type = "list";
            output.Status.Data = await _projectContext.Customer.Select(x => x.status).Distinct().ToListAsync<string?>();
            output.FineOperator.Data = await _projectContext.Customer.Select(x => x.fineoperator).Distinct().ToListAsync<string?>();
            output.Issuer.Data = await _projectContext.Customer.Select(x => x.issuer).Distinct().ToListAsync<string?>();




            return await API.Success(response, output);
        }
        catch (Exception ex)
        {
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }
    }

    private async Task<List<string>> GetDistinctList(MongoClient dbClient, string fieldName) {

        IMongoDatabase database = dbClient.GetDatabase("local");
        IMongoCollection<Customer> collection = database.GetCollection<Customer>("customer");
        return await collection
            .Distinct<string>(fieldName, filter: Builders<Customer>.Filter.Empty)
            .ToListAsync();
    }
}