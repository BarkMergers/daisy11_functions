using Daisy11Functions.Auth;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Daisy11Functions;








public class Search
{


    //private class AISolution
    //{
    //    public string? Table { get; set; }
    //    public string[]? Columns { get; set; }
    //    public Dictionary<string, string>? Filters { get; set; }
    //    public string[]? Groupby { get; set; }
    //    public Dictionary<string, string>? Sort { get; set; }
    //    public string? Limit { get; set; }
    //}

    public class AISolution
    {
        public string? table { get; set; }
        public List<string>? columns { get; set; }
        public string? filter { get; set; }
        public Dictionary<string, JsonElement>? paramsList { get; set; }
        public List<string>? groupby { get; set; }
        public List<string>? sort { get; set; }
        public int? limit { get; set; }
    }


    /*
    {
    "table": "Asset",
    "columns": ["ID", "AssetTypeID", "Weight", "Colour"],
    "filters": { },
    "groupby": [],
    "sort": { },
    "limit": null
    }
    */













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


        // OpenAI API Key (use Azure Key Vault or configuration, don’t hardcode)
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        string intro2 = @"

            I have the following tables which have the fields listed:

            'tables': {
                'Customer': [
                        {'name': 'ID', 'type': 'bigint'},
                        {'name': 'vehicle', 'type': 'nvarchar'},
                        {'name': 'fineoperator', 'type': 'nvarchar'},
                        {'name': 'fineamount', 'type': 'float'},
                        {'name': 'age', 'type': 'int'},
                        {'name': 'power', 'type': 'float'},
                        {'name': 'issuer', 'type': 'string'},
                        {'name': 'status', 'type': 'string'},
                        {'name': 'increasedate', 'type': 'datetime'}],
                'Asset': [
                        {'name':'ID', 'type': 'bigint'},
                        {'name':'AssetTypeID', 'type': 'int'},
                        {'name':'Weight', 'type': 'int'},
                        {'name':'Colour', 'type': 'string'},
                        {'name':'LocationID', 'type': 'int'}],
                'Location': [
                        {'name':'ID', 'type': 'bigint'},
                        {'name':'Town', 'type': 'nvarchar'},
                        {'name':'Name', 'type': 'nvarchar'},
                        {'name':'Phonenumber', 'type': 'nvarchar'},
                        {'name':'Secure', 'type': 'boolean'},
                        {'name':'Active', 'type': 'boolean'}]
            }

            customer.status = [""To load"", ""Complete"", ""Processing""]
            customer.issuer = [""External"", ""Internal""]

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
              ""filter"": ""fieldname = @param1 AND fieldname != @param2"",
              ""paramsList"": {  
                ""param1"": ""filter value"",
                ""param2"": ""filter value""
              },
              ""groupby"": [fieldlist],
              ""sort"": [
                ""column name asc"",
                ""direction desc""
              ],
              ""limit"": numeric value
            }

        " + prompt;

        JsonElement aiResponse;
        try
        {
            aiResponse = await runAI(intro2);
        }
        catch (Exception ex)
        {
            return await API.Fail( response, System.Net.HttpStatusCode.TooManyRequests, ex.Message);
        }

        //string textPlan = output2.ToString();
        AISolution? plan = JsonSerializer.Deserialize<AISolution>(aiResponse.ToString());

        if (plan?.table == null)
        {
            string? msg = aiResponse.GetString();
            return await API.Fail(response, System.Net.HttpStatusCode.TooManyRequests, msg);
        }

        string sql = "select";

        if (plan.limit != null)
            sql += " top " + plan.limit;

        sql += " " +  (plan.columns == null || plan.columns.Count == 0 ? "*" : string.Join(", ", plan.columns)) + " from " + plan.table;

        SqlParameter[] parameterList = new SqlParameter[0];

        if (!string.IsNullOrWhiteSpace(plan.filter) && plan.paramsList != null)
        {
            sql += " where " + plan.filter;

            parameterList = new SqlParameter[plan.paramsList.Count];
            int ii = 0;
            foreach (KeyValuePair<string, JsonElement> i in plan.paramsList)
            {
                switch (i.Value.ValueKind)
                {
                    case JsonValueKind.True: parameterList[ii] = new SqlParameter(i.Key, i.Value.GetBoolean()); break;
                    case JsonValueKind.String: parameterList[ii] = new SqlParameter(i.Key, i.Value.GetString()); break;
                    case JsonValueKind.Number: parameterList[ii] = new SqlParameter(i.Key, i.Value.GetInt32()); break;
                }
                ii++;
            }
        }

        if (plan.sort != null && plan.sort.Count > 0)
        {
            sql += " order by " + plan.sort[0];
        }

        List<Dictionary<string, object>> dataset = new List<Dictionary<string, object>>();

        using (var command = _projectContext.GetDbConnection().CreateCommand())
        {
            command.CommandText = sql;

            foreach (var param in parameterList)
            {
                command.Parameters.Add(param);
            }

            _projectContext.GetDbConnection().Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string fieldName = reader.GetName(i);
                        object? value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row.Add(fieldName, value);
                    }

                    dataset.Add(row);
                }
            }
        }

        return await API.Success(response,  JsonSerializer.Serialize( dataset));
    }


    private async Task<JsonElement> runAI(string userPrompt)
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

        try
        {
            var assistantMessage = result.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content");

            return assistantMessage;
        }
        catch (Exception)
        {
            throw new Exception(result.RootElement.GetProperty("error").GetProperty("message").GetString());
        }
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
