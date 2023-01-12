using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.Tools.AppInitializer;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.TempFiles
{
    /// <summary>
    /// Service for init temp files service on application start.
    /// </summary>
    /// <remarks>
    /// Creates temp directory if it not exist.
    /// </remarks>
    public class TempFilesInitService : IAppInitializer
    {
        private readonly TempFileOptions _tempFileOptions;
        private readonly ILogger _logger;

        public TempFilesInitService(
            TempFileOptions tempFileOptions, 
            ILogger<TempFilesInitService> logger)
        {
            _tempFileOptions = tempFileOptions ?? throw new ArgumentNullException(nameof(tempFileOptions));
            tempFileOptions.AssertValid();
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Initialization of temp files started...");
            
            _logger.LogInformation($"Check existence of temp directory \"{_tempFileOptions.TempPath}\"...");
            if (Directory.Exists(_tempFileOptions.TempPath))
            {
                _logger.LogInformation($"Temp directory \"{_tempFileOptions.TempPath}\" have been already created.");
            }
            else
            {
                Directory.CreateDirectory(_tempFileOptions.TempPath);
                _logger.LogInformation($"Temp directory \"{_tempFileOptions.TempPath}\" was created.");
            }
            
            _logger.LogInformation("Initialization of temp files completed");
            
            return Task.CompletedTask;
        }
    }
}
