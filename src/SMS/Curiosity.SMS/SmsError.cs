namespace Curiosity.SMS
{
    /// <summary>
    /// Errors of sending SMS.
    /// </summary>
    public enum SmsError
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
        /// Unknown error.
        /// </summary>
        Unknown = 3,
        
        /// <summary>
        /// Reached the limit on attempts.
        /// </summary>
        RateLimit = 4,
        
        /// <summary>
        /// Account has no money.
        /// </summary>
        NoMoney = 5,
        
        /// <summary>
        /// Can't to deliver
        /// </summary>
        DeliveryError = 6,
    }
}
