using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.AppInitializer;
using Curiosity.Configuration;
using Microsoft.Extensions.Logging;

namespace Curiosity.SFTP.SSH.Net
{
    /// <summary>
    /// Service for initialization SFTP connections
    /// </summary>
    public class SftpInitializer : IAppInitializer
    {
        private readonly ILogger _logger;
        private readonly ISftpClientFactory _sftpClientFactory;
        private readonly SftpClientOptions _sftpClientOptions;
        
        public SftpInitializer(
            ILogger<SftpInitializer> logger, 
            ISftpClientFactory sftpClientFactory, 
            SftpClientOptions sftpClientOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sftpClientFactory = sftpClientFactory ?? throw new ArgumentNullException(nameof(sftpClientFactory));
            _sftpClientOptions = sftpClientOptions ?? throw new ArgumentNullException(nameof(sftpClientOptions));
            _sftpClientOptions.AssertValid();
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_sftpClientOptions.CheckOnStart)
            {
                _logger.LogInformation("Check connection to ssh storage...");

                using (var sftpClient = _sftpClientFactory.GetSftpClient())
                {
                    sftpClient.CheckConnection();
                }

                _logger.LogInformation("Check connection to ssh storage completed");
            }
            else
            {
                _logger.LogInformation("Check connection to ssh storage disabled");
            }
            return Task.CompletedTask;
        }
    }
}