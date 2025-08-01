using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;

namespace Daisy11Functions.Helpers
{
    public static class API
    {
        public static async Task<HttpResponseData> Success (HttpResponseData input, object data)
        {
            input.StatusCode = System.Net.HttpStatusCode.OK;
            await input.WriteStringAsync(JsonSerializer.Serialize(data));
            return input;
        }
    }
}
