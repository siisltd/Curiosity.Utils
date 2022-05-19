using Newtonsoft.Json;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// An object that contains html, plain text, and amp parts of the letter.
    /// </summary>
    /// <remarks>
    /// Either <see cref="UnisenderGoSendEmailMessageBody.Html"/> or <see cref="UnisenderGoSendEmailMessageBody.PlainText"/> part must be present.
    /// </remarks>
    internal class UnisenderGoSendEmailMessageBody
    {
        /// <summary>
        /// HTML part of the letter.
        /// </summary>
        [JsonProperty("html")]
        public string? Html { get; set; }

        /// <summary>
        /// Plaintext part of the letter.
        /// </summary>
        [JsonProperty("plaintext")]
        public string? PlainText { get; set; }

        /// <summary>
        /// Amp part of the letter.
        /// </summary>
        [JsonProperty("amp")]
        public string? Amp { get; set; }
    }
}
