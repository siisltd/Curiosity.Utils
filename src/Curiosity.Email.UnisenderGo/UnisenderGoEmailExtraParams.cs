using Curiosity.EMail;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Extra params for sending email via UnisenderGo.
    /// </summary>
    public class UnisenderGoEmailExtraParams : IEMailExtraParams
    {
        /// <summary>
        /// API key.
        /// </summary>
        public string? ApiKey { get; }

        /// <summary>
        /// Region of API.
        /// </summary>
        public UnisenderGoRegion? Region { get; }

        /// <summary>
        /// Sender's EMail
        /// </summary>
        public string? EmailFrom { get; }

        /// <summary>
        /// Sender's name.
        /// </summary>
        public string? FromName { get; }

        /// <summary>
        /// Reply address for EMail.
        /// </summary>
        public string? ReplyTo { get; }

        /// <summary>
        /// Link for user's unsubscribe page.
        /// </summary>
        public string? UnsubscribeUrl { get; }

        /// <summary>
        /// Is tracking opening links enabled?
        /// </summary>
        public bool? TrackLinks { get; }
        
        /// <summary>
        /// Is tracking email reading enabled?
        /// </summary>
        public bool? TrackReads { get; }

        /// <summary>
        /// Should Unisender add their footer with unsubscribed url.
        /// </summary>
        /// <remarks>
        /// This option can not affect on anything if Unisender doesn't activate disabling footer for client.
        /// </remarks>
        public bool? SkipUnisenderUnsubscribeFooter { get; }

        /// <inheritdoc cref="UnisenderGoEmailExtraParams"/>
        public UnisenderGoEmailExtraParams(
            string? apiKey = null,
            UnisenderGoRegion? region = null,
            string? emailFrom = null,
            string? fromName = null,
            string? replyTo = null,
            string? unsubscribeUrl = null,
            bool? trackLinks = null,
            bool? trackReads = null,
            bool? skipUnisenderUnsubscribeFooter = null)
        {
            if (apiKey != null)
                UnisenderGoGuard.AssertApiKey(apiKey);
            if (region != null)
                UnisenderGoGuard.AssertRegion(region.Value);
            if (emailFrom != null)
                UnisenderGoGuard.AssertApiKey(emailFrom);
            if (fromName != null)
                UnisenderGoGuard.AssertApiKey(fromName);

            ApiKey = apiKey;
            Region = region;
            EmailFrom = emailFrom;
            FromName = fromName;
            ReplyTo = replyTo;
            UnsubscribeUrl = unsubscribeUrl;
            TrackLinks = trackLinks;
            TrackReads = trackReads;
            SkipUnisenderUnsubscribeFooter = skipUnisenderUnsubscribeFooter;
        }
    }
}
