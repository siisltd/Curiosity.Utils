using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools;
using Curiosity.Tools.AppInitializer;
using Microsoft.Extensions.Logging;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Initializer of <see cref="FireAndForget"/>.
    /// </summary>
    /// <remarks>
    /// Sets default logger.
    /// </remarks>
    public class FireAndForgetInitializer : IAppInitializer
    {
        private readonly ILoggerFactory _loggerFactory;

        public FireAndForgetInitializer(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var logger = _loggerFactory.CreateLogger(typeof(FireAndForget));
            FireAndForget.SetLogger(logger);
            
            return Task.CompletedTask;
        }
    }
}