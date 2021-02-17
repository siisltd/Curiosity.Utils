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
    }
}