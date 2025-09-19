using Newtonsoft.Json;

namespace Daisy11Functions.Models.FilterAndSort.StructuredData
{
    public class GetCustomerDateTimeFilter
    {
        /// <summary>
        /// Type of filter modes
        /// Currently supported: Before/After/Between
        /// </summary>
        [JsonProperty("mode")]
        public string Mode { get; set; }


        /// <summary>
        /// Used for single use dates
        /// </summary>
        [JsonProperty("date")]
        public string? Date { get; set; }


        /// <summary>
        /// Indicates before this date
        /// </summary>
        [JsonProperty("to")]
        public string? To { get; set; }


        /// <summary>
        /// Indicates after this date
        /// </summary>
        [JsonProperty("from")]
        public string? From { get; set; }
    }
}
