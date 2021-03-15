using Curiosity.Hosting.ThreadPool;

namespace Curiosity.Hosting.Web
{
    /// <summary>
    /// Basic web app configuration. 
    /// </summary>
    public interface ICuriosityWebAppConfiguration : ICuriosityAppConfiguration
    {
        /// <summary>
        /// Urls to listen for web app.
        /// </summary>
        public string? Urls { get; }
        
        /// <summary>
        /// Some Kestrel options.
        /// </summary>
        KestrelOptions? Kestrel { get; }
        
        /// <summary>
        /// Thread pool options.
        /// </summary>
        ThreadPoolOptions ThreadPool { get; }
        
        /// <summary>
        /// Name collection of the fields the data in which we want to hide
        /// </summary>
        public string[]? SensitiveDataFieldNames { get; }
    }
}