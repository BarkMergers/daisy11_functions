using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace NewWorldFunctions.Helpers
{
    public static class CORS
    {
        public static bool IsPreFlight(HttpRequestData req)
        {

            string? requestSource = req.Headers.Contains("origin") ? req.Headers.GetValues("origin").FirstOrDefault() : null;
            if (string.IsNullOrEmpty(requestSource))
                return false;

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            switch (requestSource)
            {
                case "http://localhost:59414":
                    response.Headers.Add("Access-Control-Allow-Origin", requestSource);
                    break;
            }
            
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            if (req.Method == HttpMethod.Options.Method)
                return true;

            return false;
        }

        public static IActionResult PreFlightData()
        {
            return new OkObjectResult(null);
        }
    }
}