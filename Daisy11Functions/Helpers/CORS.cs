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
            // Has no effect when deployed as Azure manages these headers. This is only for running in Visual Studio

            string? requestSource = req.Headers.Contains("origin") ? req.Headers.GetValues("origin").FirstOrDefault() : null;

            response = req.CreateResponse(HttpStatusCode.NoContent);

            response.Headers.Add("Access-Control-Allow-Headers", req.Method == HttpMethod.Options.Method ? "Authorization": "Content-Type");
            response.Headers.Add("Access-Control-Allow-Methods", req.Method);   // Needed?
            response.Headers.Add("Content-Type", "application/json");           // Needed?

            switch (requestSource)
                {
                    // case "https://nice-beach-0b426541e.1.azurestaticapps.net/":
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