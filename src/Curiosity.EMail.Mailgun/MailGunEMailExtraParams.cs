namespace Curiosity.EMail.Mailgun
{
    /// <summary>
    /// Extra params for sending EMail message via MailGun.
    /// </summary>
    public class MailGunEMailExtraParams : IEMailExtraParams
    {
        /// <summary>
        /// API key for MailGun that will be used instead of default.
        /// </summary>
        public string? MailGunApiKey { get; }

        /// <summary>
        /// Sender's EMail that will be used instead of default.
        /// </summary>
        public string? EMailFrom { get; }

        public MailGunEMailExtraParams(string? mailGunApiKey, string? eMailFrom)
        {
            MailGunApiKey = mailGunApiKey;
            EMailFrom = eMailFrom;
        }
    }
}