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

        public MailGunEMailExtraParams(string? mailgunUser, string? mailGunApiKey, string? mailGunDomain, string? eMailFrom)
        {
            MailGunApiKey = mailGunApiKey;
            EMailFrom = eMailFrom;
            MailGunDomain = mailGunDomain;
            MailgunUser = mailgunUser;
        }
    }
}
