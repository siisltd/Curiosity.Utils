using Newtonsoft.Json;

namespace Curiosity.SMS.Smsc
{
    internal class SmscResponseData
    {
        [JsonProperty("Error")]
        public string? Error { get; set; }

        [JsonProperty("error_code")]
        public int? error_code { get; set; }

        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("cnt")]
        public int? Count { get; set; }

        [JsonProperty("Cost")]
        public decimal? Cost { get; set; }

        [JsonProperty("Balance")]
        public decimal? Balance { get; set; }
    }
}
