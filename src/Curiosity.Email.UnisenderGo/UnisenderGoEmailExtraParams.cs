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
        public string? ApiKey { get; set; }

        /// <summary>
        /// Region of API.
        /// </summary>
        public UnisenderGoRegion? Region { get; set; }

        /// <summary>
        /// Sender's EMail
        /// </summary>
        public string? EmailFrom { get; set; }

        /// <summary>
        /// Sender's name.
        /// </summary>
        public string? FromName { get; set; }

        /// <summary>
        /// Reply address for EMail.
        /// </summary>
        public string? ReplyTo { get; set; }

        /// <summary>
        /// Link for user's unsubscribe page.
        /// </summary>
        public string? UnsubscribeUrl { get; set; }

        /// <summary>
        /// Is tracking opening links enabled?
        /// </summary>
        public bool? TrackLinks { get; set; }
        
        /// <summary>
        /// Is tracking email reading enabled?
        /// </summary>
        public bool? TrackReads { get; set; }
    }
}
