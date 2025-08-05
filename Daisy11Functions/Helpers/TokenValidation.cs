using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Daisy11Functions.Helpers
{
    /*
     *  dotnet add package Microsoft.IdentityModel.Tokens
     *  dotnet add package System.IdentityModel.Tokens.Jwt
     */

    public static class TokenValidation
    {
        public static async Task<HttpResponseData?> Validate(Microsoft.Azure.Functions.Worker.Http.HttpRequestData req)
        {
            return await Validate(req, null);
        }

        public static async Task<HttpResponseData?> Validate(Microsoft.Azure.Functions.Worker.Http.HttpRequestData req,
            ILogger? _logger)
        {
            IHttpCookie? cookieToken = req.Cookies.FirstOrDefault(x => x.Name == "access_token");

            if (cookieToken == null || cookieToken.Value == null)
            {
                HttpResponseData unauthorized = req.CreateResponse(HttpStatusCode.NotAcceptable);
                await unauthorized.WriteStringAsync("Missing or invalid Authorization header.");
                return unauthorized;
            }

            string authHeader = cookieToken.Value;
            string? tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
            string? backendClientID = Environment.GetEnvironmentVariable("BACKEND_CLIENT_ID");
            string token = authHeader.Trim();

            ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = await configManager.GetConfigurationAsync();

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://sts.windows.net/{tenantId}/",
                ValidateAudience = true,
                ValidAudience = $"api://{backendClientID}",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = openIdConfig.SigningKeys,
            };

            try
            {
                System.Security.Claims.ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Token is valid — Access claims here
                string? userId = principal.FindFirst("sub")?.Value;

            }
            catch (SecurityTokenException ex)
            {
                HttpResponseData unauthorized = req.CreateResponse(HttpStatusCode.Ambiguous);
                await unauthorized.WriteStringAsync("Invalid token: " + ex.Message);
                return unauthorized;
            }

            return null;
        }
    }
}
