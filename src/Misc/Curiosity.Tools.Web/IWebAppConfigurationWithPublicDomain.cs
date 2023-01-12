using Curiosity.Configuration;

namespace Curiosity.Tools.Web
{
    /// <summary>
    /// Interface for web app configuration that has public domain which differs from hosting url.
    /// </summary>
    public interface IWebAppConfigurationWithPublicDomain : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// Public address to access web app.
        /// </summary>
        public string PublicDomain { get; }
    }
}