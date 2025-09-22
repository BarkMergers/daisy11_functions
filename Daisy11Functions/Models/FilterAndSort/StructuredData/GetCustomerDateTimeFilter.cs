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


        /* Automatic convertors for the string date time passed in via JSON */
        [JsonIgnore]
        internal DateTime? ToDateTime => ConvertDateTimeFromString(this.To!);
        [JsonIgnore]
        internal DateTime? FromDateTime => ConvertDateTimeFromString(this.From!);
        [JsonIgnore]
        internal DateTime? DateDateTime => ConvertDateTimeFromString(this.Date!);

        
        /// <summary>
        /// Swaps To/From if the incorrect way round
        /// </summary>
        internal void FixDates()
        {
            if (!this.FromDateTime.HasValue || !this.ToDateTime.HasValue)
                return; // Nothing to swap as both values haven't been provided

            
            // Swaps JSON strings around if necessary (as they control DateTime properties)
            if (this.FromDateTime!.Value > this.ToDateTime!.Value)
            {
                var temp = this.FromDateTime!.Value.ToString();
                this.From = this.To;
                this.To = temp;
            }
        }

        private DateTime? ConvertDateTimeFromString(string nValue)
        {
            if(string.IsNullOrEmpty(nValue) || !DateTime.TryParse(nValue, out DateTime result))
                return null;
            return result;
        }
    }
}
