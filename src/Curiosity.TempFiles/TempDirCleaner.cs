using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.TempFiles
{
    /// <summary>
    /// Periodically checks all files in the directory.
    /// Deletes those files, the last access to which was more than the maximum allowed.
    /// </summary>
    public class TempDirCleaner : BackgroundService
    {
        private readonly TimeSpan _frequency;
        private readonly TimeSpan _ttl;
        private readonly string _tempPath;
        private readonly ILogger<TempDirCleaner> _logger;

        public TempDirCleaner(ILogger<TempDirCleaner> logger, TempFileOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            _frequency = TimeSpan.FromHours(options.CleaningFrequencyHours);
            _ttl = TimeSpan.FromHours(options.FileTtlHours);
            _tempPath = options.TempPath;
        }
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting {nameof(TempDirCleaner)}...");

            // to avoid failure while executing
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
                _logger.LogInformation($"Directory \"{_tempPath}\" was successfully created");
            }
            
            await base.StartAsync(cancellationToken);
            _logger.LogInformation($"Starting {nameof(TempDirCleaner)} completed.");
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var checkedDate = DateTime.Now - _ttl;
                    var files = Directory.GetFiles(_tempPath);
            
                    _logger.LogDebug($"Cleaning started: \"{_tempPath}\" contains {files.Length} files");
            
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.LastAccessTime < checkedDate)
                        {
                            fileInfo.Delete();
                            _logger.LogInformation($"File (name = \"{fileInfo.Name}\", LastAccessTime = {fileInfo.LastAccessTime}) successfully deleted");
                        }
                    }
            
                    _logger.LogDebug($"Cleaning finished: {Directory.GetFiles(_tempPath).Length} files left");
                    await Task.Delay(_frequency, stoppingToken);
                }
                catch (Exception e) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(e, $"Stopping {nameof(TempDirCleaner)}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Critical error in {nameof(TempDirCleaner)}");
                    
                    _logger.LogInformation($"{nameof(TempDirCleaner)} is waiting for 10 minutes and then is going to try again.");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                    _logger.LogInformation($"{nameof(TempDirCleaner)} try to execute again.");
                }
            }
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping {nameof(TempDirCleaner)}...");
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"Stopping {nameof(TempDirCleaner)} completed.");
        }
    }
}