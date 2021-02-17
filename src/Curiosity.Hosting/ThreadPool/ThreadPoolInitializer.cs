using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.AppInitializer;
using Curiosity.Configuration;
using Microsoft.Extensions.Logging;

namespace Curiosity.Hosting.ThreadPool
{
    /// <summary>
    /// Service for initialization of <see cref="ThreadPool"/>.
    /// </summary>
    public class ThreadPoolInitializer : IAppInitializer
    {
        private readonly ThreadPoolOptions _options;
        private readonly ILogger _logger;

        public ThreadPoolInitializer(
            ThreadPoolOptions options,
            ILogger<ThreadPoolInitializer> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            options.AssertValid();
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Thread pool initialization started...");
            
            var newWorkerThreads = Math.Max(_options.MinWorkerThreads, 0);
            var newCompletionPortThreads = Math.Max(_options.MinCompletionPortThreads, 0);

            _logger.LogInformation($"Set minimum threads: worker threads = {newWorkerThreads}, I/O threads = {newCompletionPortThreads} (processor count = {Environment.ProcessorCount})");
            System.Threading.ThreadPool.SetMinThreads(newWorkerThreads, newCompletionPortThreads);
            
            _logger.LogInformation("Thread pool initialization completed.");
            
            return Task.CompletedTask;
        }
    }
}