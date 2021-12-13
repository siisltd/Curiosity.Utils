namespace Curiosity.EMail.Mailgun
{
    /// <summary>
    /// Extra params for sending EMail message via MailGun.
    /// </summary>
    public class MailGunEMailExtraParams : IEMailExtraParams
    {

        /// <summary>
        /// Mailgun user.
        /// </summary>
        public string? MailgunUser { get; }

        /// <summary>
        /// API key for MailGun that will be used instead of default.
        /// </summary>
        public string? MailGunApiKey { get; }

        /// <summary>
        /// Domain of MailGun
        /// </summary>
        public string? MailGunDomain { get; }

        /// <summary>
        /// Sender's EMail that will be used instead of default.
        /// </summary>
        public string? EMailFrom { get; }

        /// <summary>
        /// Region where Mailgun API is located.
        /// </summary>
        public MailgunRegion? MailgunRegion { get; }

        public MailGunEMailExtraParams(
            string? mailgunUser,
            string? mailGunApiKey,
            string? mailGunDomain,
            string? eMailFrom,
            MailgunRegion? mailgunRegion = null)
        {
            MailGunApiKey = mailGunApiKey;
            EMailFrom = eMailFrom;
            MailgunRegion = mailgunRegion;
            MailGunDomain = mailGunDomain;
            MailgunUser = mailgunUser;
        }
    }
}
