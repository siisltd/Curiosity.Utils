namespace Curiosity.EMail.Smtp
{
    /// <summary>
    /// Extra params for sending EMail via SMTP.
    /// </summary>
    public class SmtpEMailExtraParams : IEMailExtraParams
    {
        /// <summary>
        /// Sender's EMail.
        /// </summary>
        public string? EMailFrom { get; }
        /// <summary>
        /// Sender's name.
        /// </summary>
        public string? SenderName { get; }
        /// <summary>
        /// Reply address for EMail.
        /// </summary>
        public string? ReplyTo { get; }

        public SmtpEMailExtraParams(string? eMailFrom, string? senderName, string? replyTo)
        {
            EMailFrom = eMailFrom;
            SenderName = senderName;
            ReplyTo = replyTo;
        }
    }
}