using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

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
            string? authHeader = req.Headers.TryGetValues("Authorization", out var values) ? values.FirstOrDefault() : null;

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                HttpResponseData unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorized.WriteStringAsync("Missing or invalid Authorization header.");
                return unauthorized;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            string tenantId = "3fdf479e-e456-4ae5-9431-657da2d108ec";
            string backendClientID = "6be0af57-8832-4ef2-adc4-060e2067bcf6";

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

                // Token is valid — you can access claims here
                string? userId = principal.FindFirst("sub")?.Value;

            }
            catch (SecurityTokenException ex)
            {
                HttpResponseData unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorized.WriteStringAsync("Invalid token: " + ex.Message);
                return unauthorized;
            }

            return null;
        }
    }
}
