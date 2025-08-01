﻿using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NewWorldFunctions.Helpers
{
    public static class GetRequestByBody
    {
        public static async Task<T> GetBody<T>(HttpRequestData req)
        {
            using (StreamReader stream = new StreamReader(req.Body))
            {
                string body = await stream.ReadToEndAsync();
                string format = "yyyy-MM-ddTHH:mm";
                IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                if (string.IsNullOrWhiteSpace(body))
                    throw new Exception("Request Body was blank");
                else
                    return JsonConvert.DeserializeObject<T>(body, dateTimeConverter);
            }
        }
    }
}