using Daisy11Functions.Database;
using Daisy11Functions.Database.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;

using MongoDB.Driver;
using MongoDB.Bson;
using System.Net;
using Azure;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;


namespace Daisy11Functions;

public class GetAgent
{
    private readonly ILogger<GetRole> _logger;
    private readonly IProjectContext _projectContext;

    public GetAgent(ILogger<GetRole> logger, IProjectContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }

    [Function("GetAgent")]
    public async Task<HttpResponseData> Run_GetAgent([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetAgent/{agent}")] Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, string? agent)
    {


        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;



        /*
         * dotnet add package Microsoft.IdentityModel.Tokens
           dotnet add package System.IdentityModel.Tokens.Jwt
        */



        string? authHeader = req.Headers.TryGetValues("Authorization", out var values)
    ? values.FirstOrDefault()
    : null;

        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            // Unauthorized
            var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorized.WriteStringAsync("Missing or invalid Authorization header.");
            return unauthorized;
        }

        string token = authHeader.Substring("Bearer ".Length).Trim();



        var tenantId = "6d52e229-7b09-4772-885e-edd85950a304";
        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever());
        var openIdConfig = await configManager.GetConfigurationAsync();




        Console.WriteLine("Signing keys loaded:");
        foreach (var key in openIdConfig.SigningKeys)
        {
            Console.WriteLine($"Key ID: {key.KeyId}");
        }




        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://sts.windows.net/6d52e229-7b09-4772-885e-edd85950a304/", // e.g., your auth server or Azure AD
            ValidateAudience = true,
            ValidAudience = "6be0af57-8832-4ef2-adc4-060e2067bcf6", //"00000003-0000-0000-c000-000000000000", //,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = openIdConfig.SigningKeys, //new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-signing-key")),
        };



        /*
         * , 
                "https://login.microsoftonline.com/6d52e229-7b09-4772-885e-edd85950a304/v2.0"]
         */


        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Token is valid — you can access claims here
            var userId = principal.FindFirst("sub")?.Value;

            // Continue handling the request
        }
        catch (SecurityTokenException ex)
        {
            var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorized.WriteStringAsync("Invalid token: " + ex.Message);
            return unauthorized;
        }






        //Role? agentRecord = _projectContext.Role.FirstOrDefault(x => x.agent == agent);

        Role agentRecord = new Role()
        {
            firstname = "Mark",
            lastname = "Burgess",
            role = "Comms",
            age = 22,
            active = true
        };

        await response.WriteStringAsync(JsonSerializer.Serialize(
            agentRecord = agentRecord == null ? new Role() : agentRecord
        ));

        return response;
    }
}