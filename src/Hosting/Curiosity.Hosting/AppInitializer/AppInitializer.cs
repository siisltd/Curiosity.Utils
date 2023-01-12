using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools;
using Curiosity.Tools.AppInitializer;
using Microsoft.Extensions.Logging;

namespace Curiosity.Hosting.AppInitializer
{
    internal class AppInitializer
    {
        private readonly ILogger<AppInitializer> _logger;
        private readonly IEnumerable<IAppInitializer> _initializers;

        public AppInitializer(ILogger<AppInitializer> logger, IEnumerable<IAppInitializer> initializers)
        {
            _logger = logger;
            _initializers = initializers;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting async initialization");

            try
            {
                foreach (var initializer in _initializers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    _logger.LogInformation("Starting async initialization for {InitializerType}", initializer.GetType());
                    try
                    {
                        await initializer.InitializeAsync(cancellationToken);
                        _logger.LogInformation("Async initialization for {InitializerType} completed", initializer.GetType());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Async initialization for {InitializerType} failed", initializer.GetType());
                        throw;
                    }
                }

                _logger.LogInformation("Async initialization completed");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Async initialization failed");
                throw;
            }
        }
    }
}
