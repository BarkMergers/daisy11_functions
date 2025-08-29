using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using static System.Net.WebRequestMethods;

namespace NewWorldFunctions.Helpers
{
    public static class CORS
    {
        private static readonly string[] AllowedOrigins = new[]
        {
            "http://localhost:59414",
            "http://localhost:56172",
            "http://localhost:5176"
        };

        public static bool IsPreFlight(HttpRequestData req, out HttpResponseData response)
        {
            string? origin = req.Headers.TryGetValues("Origin", out IEnumerable<string>? values) ? values.FirstOrDefault() : null;

            response = req.CreateResponse(HttpStatusCode.NoContent);

            if (!string.IsNullOrWhiteSpace(origin) && (AllowedOrigins.Contains(origin) || origin.StartsWith("http://localhost:")))
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