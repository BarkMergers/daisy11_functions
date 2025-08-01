using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace NewWorldFunctions.Helpers
{
    public static class CORS
    {
        public static bool IsPreFlight(HttpRequestData req, out HttpResponseData response)
        {
            string? requestSource = req.Headers.Contains("origin") ? req.Headers.GetValues("origin").FirstOrDefault() : null;

            response = req.CreateResponse(HttpStatusCode.NoContent);

            response.Headers.Add("Access-Control-Allow-Headers", req.Method == HttpMethod.Options.Method ? "Authorization": "Content-Type");
            response.Headers.Add("Access-Control-Allow-Methods", req.Method);   // Needed?
            response.Headers.Add("Content-Type", "application/json");           // Needed?

            switch (requestSource)
                {
                    case "http://localhost:59414":
                        response.Headers.Add("Access-Control-Allow-Origin", requestSource);
                        break;
                }
            
            if (req.Method == HttpMethod.Options.Method)
                return true;

            return false;
        }
    }
}