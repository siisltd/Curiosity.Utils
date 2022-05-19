using Newtonsoft.Json;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Extra options for <see cref="UnisenderGoSendEmailMessage"/>.
    /// </summary>
    internal class UnisenderGoSendEmailMessageOptions
    {
        /// <summary>
        /// Date and time in the format “YYYY-MM-DD hh:mm:ss” in the UTC time zone.
        /// Allows you to schedule the time of sending for the future, within 24 hours of the current time.
        /// </summary>
        [JsonProperty("send_at")]
        public string? SendAt { get; set; }

        /// <summary>
        /// Custom unsubscribe link.
        /// </summary>
        [JsonProperty("unsubscribe_url")]
        public string? UnsubscribeUrl { get; set; }
    }
}