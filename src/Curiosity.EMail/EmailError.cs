namespace Curiosity.EMail
{
    /// <summary>
    /// Errors of sending email.
    /// </summary>
    public enum EmailError
    {
        /// <summary>
        /// None errors.
        /// </summary>
        None = 0,

        /// <summary>
        /// Auth error (incorrect auth credentials, etc).
        /// </summary>
        Auth = 1,

        /// <summary>
        /// Network error or remote service in unavailable.
        /// </summary>
        Communication = 2,

        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 3,

        /// <summary>
        /// Sending was interrupted by sender because we have rich the limits.
        /// </summary>
        RateLimit = 4
    }
}
