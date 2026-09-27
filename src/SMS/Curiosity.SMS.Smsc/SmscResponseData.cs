using System.Text.Json.Serialization;

namespace Curiosity.SMS.Smsc
{
    internal class SmscResponseData
    {
        [JsonPropertyName("Error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_code")]
        public int? error_code { get; set; }

        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("cnt")]
        public int? Count { get; set; }

        [JsonPropertyName("Cost")]
        public decimal? Cost { get; set; }

        [JsonPropertyName("Balance")]
        public decimal? Balance { get; set; }
    }
}
