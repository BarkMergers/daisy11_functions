using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using Daisy11Functions.Helpers;

namespace Daisy11Functions;



public class StoreToken
{
    private class GetToken
    {
        public string? Token { get; set; }
    }

    private readonly ILogger<StoreToken> _logger;

    public StoreToken(ILogger<StoreToken> logger)
    {
        _logger = logger;
    }

    [Function("StoreToken")]
    public async Task<HttpResponseData> Run_StoreToken([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post")] 
            HttpRequestData req)
    {
        _logger.LogInformation("Start at Run_StoreToken");
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;

        GetToken bodyData = await GetRequestByBody.GetBody<GetToken>(req);

        response.Headers.Add("Set-Cookie",
            "access_token=" + bodyData.Token +
            "; HttpOnly; Secure; SameSite=None; Path=/; Max-Age=3600");

        return await API.Success(response, new { });
    }

    [Function("RemoveToken")]
    public async Task<HttpResponseData> Run_RemoveToken([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post")]
            HttpRequestData req)
    {
        _logger.LogInformation("Start at Run_RemoveToken");
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;

        response.Headers.Add("Set-Cookie",
            "access_token=; HttpOnly; Secure; SameSite=None; Path=/; Expires=Thu, 01 Jan 1970 00:00:00 GMT");

        return await API.Success(response, new { });
    }




}