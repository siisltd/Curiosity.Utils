using System.Text.Json.Serialization;

namespace Curiosity.Tools.Web.ReCaptcha
{
    internal class ReCaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool success { get; set; }

        public long challange_ts  { get; set; }

        [JsonPropertyName("hostname")]
        public string hostname  { get; set; } = null!;
    }
}