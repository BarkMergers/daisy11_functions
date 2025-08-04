using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;

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
    }
}
