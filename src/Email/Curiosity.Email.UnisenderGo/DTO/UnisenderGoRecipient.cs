using System.Text.Json.Serialization;

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
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
    }
}
