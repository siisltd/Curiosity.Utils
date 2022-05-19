using Newtonsoft.Json;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Request for sending email via UnisenderGo.
    /// </summary>
    internal class UnisenderGoSendEmailRequest
    {
        /// <summary>
        /// Data with all params of send message.
        /// </summary>
        [JsonProperty("message")]
        public UnisenderGoSendEmailMessage Message { get; set; } = null!;
    }
}
