using Newtonsoft.Json;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Recipient of email.
    /// </summary>
    internal class UnisenderGoRecipient
    {
        /// <summary>
        /// Email address of a recipient.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; } = null!;
    }
}
