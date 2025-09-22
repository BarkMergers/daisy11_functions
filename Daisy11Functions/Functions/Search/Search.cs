using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using Daisy11Functions.Helpers;
using Daisy11Functions.Auth;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http;


namespace Daisy11Functions;

public class Search
{
    private readonly ILogger<GetAgentDetails> _logger;
    private readonly INewWorldContext _projectContext;
    private readonly GetTenantDetail _getTenantDetail;

    private class ReturnData
    {
        public string text { get; set; }
    }

    private readonly HttpClient httpClient = new HttpClient();


    public Search(ILogger<GetAgentDetails> logger, INewWorldContext projectContext, GetTenantDetail getTenantDetail)
    {
        _logger = logger;
        _projectContext = projectContext;
        _getTenantDetail = getTenantDetail;
    }




    [Function("Search")]
    public async Task<HttpResponseData> Run_Search([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "Search/{prompt}")] 
            HttpRequestData req, string prompt)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;
        //if (await TokenValidation.Validate(req, _logger) is { } validation) return validation;


        // Read input text
        ///string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//var input = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
       // string userPrompt = input["prompt"];

        // OpenAI API Key (use Azure Key Vault or configuration, don’t hardcode)
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);


        //string intro1 = @"

        //    I have three database tables. 

        //        Customer: The people who have signed up to my website to buy and use my services
        //        Asset: The physical objects which my company owns and manages
        //        Location: The locations at which assets are held

        //    I want to create a data query based on a natural language prompt, to query one of these tables.
        //    Which table should the query focus on? Answer in the format { ""table"": ""<tablename>"" }

        //    Here is the prompt:

        //" + prompt;


        //var output1 = await runAI(intro1);




        string intro2 = @"

            I have the following tables which have the fields listed:

            ""tables"": {
                ""Customer"": [""ID"", ""Age""(int)(years old), ""Name""(string), ""Height""(int)(centimeters)],
                ""Asset"": [""ID"", ""AssetTypeID""(int)(lookup_1), ""Weight""(int)(kilograms), ""Colour""(string)],
                ""Location"": [""ID"",""Town""(string), ""Secured""(boolean)],
                ""LocationAsset"": [""id"", ""LocationID""(int)(foregin key to location.id), ""AssetID""(int)(foreign key to asset.id)]
            }

            lookup_1 = {
                1=Car
                2=LCV
                3=Van
            }

            I will provide a natural language statment, return a sql query to meet the statement.
            If a query cannot be made, return a generic error message.

            IF the prompt makes refernce to general knowledge, please look it up from reliable sources


            Return results as a single JSON object with this structure
            {
              ""table"": ""tablename"",
              ""columns"": [""fieldname"", ""fieldname""],
              ""filters"": {
                ""fieldname"": ""filter value""
              },
              ""sort"": {
                ""column"": ""column name"",
                ""direction"": ""sort direction""
              },
              ""limit"": numeric value
            }

        " + prompt;


        


        var output2 = await runAI(intro2);


        return await API.Success(response, output2);


    }


    private async Task<object> runAI(string userPrompt)
    {

        // Create request
        var requestData = new
        {
            model = "gpt-4.1-mini", // or the model you want to use
            messages = new object[]
            {
                new { role = "system", content = "You are a helpful assistant. Always respond in strict JSON format." },
                new { role = "user", content = userPrompt }
            },
            response_format = new { type = "json_object" } // ensures valid JSON
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        // Call OpenAI
        var airesponse = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        string responseString = await airesponse.Content.ReadAsStringAsync();

        // Deserialize
        var result = JsonDocument.Parse(responseString);
        var assistantMessage = result.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content");

        return assistantMessage;

    }


}




//public static class ChatGptFunction
//{
//    private static readonly HttpClient httpClient = new HttpClient();

//    [FunctionName("ChatGptFunction")]
//    public static async Task<IActionResult> Run(
//        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
//        ILogger log)
//    {
//        // Read input text
//        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//        var input = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
//        string userPrompt = input["prompt"];

//        // OpenAI API Key (use Azure Key Vault or configuration, don’t hardcode)
//        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

//        httpClient.DefaultRequestHeaders.Authorization =
//            new AuthenticationHeaderValue("Bearer", apiKey);

//        // Create request
//        var requestData = new
//        {
//            model = "gpt-4.1-mini", // or the model you want to use
//            messages = new object[]
//            {
//                new { role = "system", content = "You are a helpful assistant. Always respond in strict JSON format." },
//                new { role = "user", content = userPrompt }
//            },
//            response_format = new { type = "json_object" } // ensures valid JSON
//        };

//        var content = new StringContent(
//            JsonSerializer.Serialize(requestData),
//            Encoding.UTF8,
//            "application/json"
//        );

//        // Call OpenAI
//        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
//        string responseString = await response.Content.ReadAsStringAsync();

//        // Deserialize
//        var result = JsonDocument.Parse(responseString);
//        string assistantMessage = result.RootElement
//            .GetProperty("choices")[0]
//            .GetProperty("message")
//            .GetProperty("content")
//            .GetString();

//        return new OkObjectResult(assistantMessage);
//    }
//}
