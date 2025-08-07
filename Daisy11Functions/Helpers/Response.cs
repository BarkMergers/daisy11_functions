using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Daisy11Functions.Helpers
{
    public static class API
    {
        public static async Task<HttpResponseData> Success (HttpResponseData input, object data)
        {
            input.StatusCode = System.Net.HttpStatusCode.OK;

            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await input.WriteStringAsync(JsonSerializer.Serialize(data, options));
            return input;
        }



        public static async Task<HttpResponseData> Fail(HttpResponseData input, System.Net.HttpStatusCode errorCode, string errorMessage)
        {
            input.StatusCode = errorCode;
            await input.WriteStringAsync(errorMessage);
            return input;
        }



    }
}
