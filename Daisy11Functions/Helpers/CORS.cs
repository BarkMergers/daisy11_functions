using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace NewWorldFunctions.Helpers
{
    public static class CORS
    {
        private static readonly string[] AllowedOrigins = new[]
        {
            "https://nice-beach-0b426541e.1.azurestaticapps.net",
            "http://localhost:59414"
        };

        public static bool IsPreFlight(HttpRequestData req, out HttpResponseData response)
        {
            // Has no effect when deployed as Azure manages these headers. This is only for running in Visual Studio
            // Preflight check must be before Token validation - token wont be passed within the Preflight and would fail before
            // the Preflight message is returned

            string? origin = req.Headers.TryGetValues("Origin", out IEnumerable<string>? values) ? values.FirstOrDefault() : null;

            response = req.CreateResponse(HttpStatusCode.NoContent);

            if (!string.IsNullOrWhiteSpace(origin) && AllowedOrigins.Contains(origin))
            {
                response.Headers.Add("Access-Control-Allow-Origin", origin);
                response.Headers.Add("Access-Control-Allow-Credentials", "true");
                response.Headers.Add("Access-Control-Allow-Headers", req.Method == HttpMethod.Options.Method ? "Authorization": "Content-Type");
                response.Headers.Add("Access-Control-Allow-Methods", $"{req.Method}, OPTIONS");
            }

            return (req.Method == HttpMethod.Options.Method);
        }
    }
}