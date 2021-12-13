namespace Curiosity.EMail.Mailgun
{
    /// <summary>
    /// Extra params for sending Email message via Mailgun.
    /// </summary>
    public class MailgunEmailExtraParams : IEMailExtraParams
    {
        /// <summary>
        /// Mailgun user.
        /// </summary>
        public string? MailgunUser { get; }

        /// <summary>
        /// API key for MailGun that will be used instead of default.
        /// </summary>
        public string? MailgunApiKey { get; }

        /// <summary>
        /// Domain of MailGun
        /// </summary>
        public string? MailgunDomain { get; }

        /// <summary>
        /// Sender's EMail that will be used instead of default.
        /// </summary>
        public string? EmailFrom { get; }

        /// <summary>
        /// Region where Mailgun API is located.
        /// </summary>
        public MailgunRegion? MailgunRegion { get; }

        public MailgunEmailExtraParams(
            string? mailgunUser,
            string? mailgunApiKey,
            string? mailgunDomain,
            string? emailFrom,
            MailgunRegion? mailgunRegion = null)
        {
            MailgunApiKey = mailgunApiKey;
            EmailFrom = emailFrom;
            MailgunRegion = mailgunRegion;
            MailgunDomain = mailgunDomain;
            MailgunUser = mailgunUser;
        }
    }
}
