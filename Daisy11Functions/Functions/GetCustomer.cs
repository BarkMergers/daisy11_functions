using Daisy11Functions.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;

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
    public IActionResult Run_GetCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetCustomer/{page}/{limit}")] 
        HttpRequestData req, int page, int limit)
    {
        _logger.LogInformation("Start at GetCustomer");

        CORS.IsPreFlight(req); //return CORS.PreFlightData();

        PaginationObject output = new();
        int totalCount = _projectContext.Customer.Count();

        output.Data = _projectContext.Customer.OrderBy(x => x.id).Skip(page).Take(limit).ToList();

        output.Pagination = new PaginationData()
        {
            CurrentPage = page,
            TotalItems = limit,
            TotalPages = Convert.ToInt32(decimal.Ceiling(decimal.Divide(totalCount, limit))),
            HasMore = page + limit <= totalCount
        };

        return new OkObjectResult(output);
    }
}