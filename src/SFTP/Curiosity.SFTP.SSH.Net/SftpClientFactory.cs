using System;
using Curiosity.Configuration;
using Curiosity.Tools.TempFiles;
using Microsoft.Extensions.Logging;

namespace Curiosity.SFTP.SSH.Net
{
    /// <inheritdoc />
    public class SftpClientFactory : ISftpClientFactory
    {
        private readonly SftpClientOptions _options;
        private readonly ITempFileStreamFactory _tempFileStreamFactory;
        private readonly ILoggerFactory _loggerFactory;
        
        public SftpClientFactory(
            SftpClientOptions options,
            ITempFileStreamFactory tempFileStreamFactory, 
            ILoggerFactory loggerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _options.AssertValid();
            
            _tempFileStreamFactory = tempFileStreamFactory ?? throw new ArgumentNullException(nameof(tempFileStreamFactory));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        /// <inheritdoc />
        public ISftpClient GetSftpClient()
        {
            return new SftpClient(_options, _tempFileStreamFactory, _loggerFactory);
        }
    }
}
