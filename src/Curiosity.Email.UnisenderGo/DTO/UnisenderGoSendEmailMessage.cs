using System.Collections.Generic;
using Newtonsoft.Json;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Email message data.
    /// </summary>
    internal class UnisenderGoSendEmailMessage
    {
        /// <summary>
        /// Recipients of this email.
        /// </summary>
        [JsonProperty("recipients")]
        public IReadOnlyList<UnisenderGoRecipient> Recipients { get; set; } = null!;

        /// <summary>
        /// The unique identifier of the template created earlier by the template/set method. 
        /// </summary>
        /// <remarks>
        /// If specified, the template fields are substituted instead of the missing email/send fields.
        /// For example, if body is not specified in email/send, the body of the letter is taken from the template,
        /// and if subject is not specified, the subject is taken from the template.
        /// </remarks>
        [JsonProperty("template_id")]
        public string? TemplateId { get; set; }

        /// <summary>
        /// Skip or not skip adding a standard block with an unsubscribe link to the HTML part of the letter.
        /// 1=skip, 0=add, by default 0. To use 1, you need to ask tech support to enable this feature.
        /// </summary>
        [JsonProperty("skip_unsubscribe")]
        public int? SkipUnsubscribe { get; set; }

        /// <summary>
        /// Title for selecting the link language and unsubscribe page.
        /// Acceptable values are “be”, “de”, “en", “es", “fr", “it", “pl”, “pt", “ru", “ua".
        /// </summary>
        [JsonProperty("global_language")]
        public string? GlobalLanguage { get; set; }

        /// <summary>
        /// The parameter for selecting the template engine is either “simple” or “velocity". The default is “simple".
        /// </summary>
        [JsonProperty("template_engine")]
        public string? TemplateEngine { get; set; }

        /// <summary>
        /// An object that contains html, plain text, and amp parts of the letter.
        /// </summary>
        /// <remarks>
        /// Either <see cref="UnisenderGoSendEmailMessageBody.Html"/> or <see cref="UnisenderGoSendEmailMessageBody.PlainText"/> part must be present.
        /// </remarks>
        [JsonProperty("body")]
        public UnisenderGoSendEmailMessageBody Body { get; set; } = null!;

        /// <summary>
        /// Subject of the email.
        /// </summary>
        [JsonProperty("subject")]
        public string? Subject { get; set; }

        /// <summary>
        /// The sender's email address. Required only if <see cref="TemplateId"/> is empty.
        /// </summary>
        [JsonProperty("from_email")]
        public string? FromEmail { get; set; }

        /// <summary>
        /// The sender's name.
        /// </summary>
        [JsonProperty("from_name")]
        public string? FromName { get; set; }

        /// <summary>
        /// Optional email address for replies (in case it differs from the sender's address).
        /// </summary>
        [JsonProperty("reply_to")]
        public string? ReplyTo { get; set; }

        /// <summary>
        /// 1=link tracking enabled (default value), 0=disabled.
        /// </summary>
        [JsonProperty("track_links")]
        public int? TrackLinks { get; set; }

        /// <summary>
        /// 1=read tracking enabled (default value), 0=disabled.
        /// </summary>
        [JsonProperty("track_read")]
        public int? TrackRead { get; set; }

        /// <summary>
        /// Extra options.
        /// </summary>
        [JsonProperty("options")]
        public UnisenderGoSendEmailMessageOptions? Options { get; set; }
    }
}
