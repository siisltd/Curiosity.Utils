using Newtonsoft.Json;

namespace Curiosity.SMS.Smsc
{
    internal class SmscResponseData
    {
        [JsonProperty("Error")]
        public string? Error { get; set; }

        [JsonProperty("ErrorCode")]
        public int? ErrorCode { get; set; }

        [JsonProperty("Id")]
        public string? Id { get; set; }

        [JsonProperty("cnt")]
        public int? Count { get; set; }

        [JsonProperty("Cost")]
        public decimal? Cost { get; set; }

        [JsonProperty("Balance")]
        public decimal? Balance { get; set; }
    }
}
