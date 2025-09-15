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
using Microsoft.EntityFrameworkCore;
using Daisy11Functions.Auth;

namespace Daisy11Functions;

public class CustomerFilterData
{
    public List<string>? StatusList { get; set; }
    public List<string>? FineOperatorList { get; set; }
    public List<string>? IssuerList { get; set; }
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
            //    StatusList = await GetDistinctList(dbClient, "status"),
            //    FineOperatorList = await GetDistinctList(dbClient, "fineoperator"),
            //    IssuerList = await GetDistinctList(dbClient, "issuer")
            //};


            CustomerFilterData output = new()
            {
                StatusList = await _projectContext.Customer.Select(x => x.status).Distinct().ToListAsync<string>(),
                FineOperatorList = await _projectContext.Customer.Select(x => x.fineoperator).Distinct().ToListAsync<string>(),
                IssuerList = await _projectContext.Customer.Select(x => x.issuer).Distinct().ToListAsync<string>()
            };




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