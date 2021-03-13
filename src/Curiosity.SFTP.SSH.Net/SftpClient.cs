using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Curiosity.TempFiles;
using Microsoft.Extensions.Logging;
using Polly;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Curiosity.SFTP.SSH.Net
{
    /// <inheritdoc />
    /// <remarks>
    ///  Facade over <see cref="N:Renci.SshNet.SftpClient" /> for simplifying 
    ///  work with files over SSH and support many auth methods
    /// </remarks>>
    public class SftpClient : ISftpClient
    {
        private readonly Renci.SshNet.SftpClient _sftpClient;
        private readonly ITempFileStreamFactory _tempFileStreamFactory;
        private readonly ILogger<SftpClient> _logger;


        private readonly AsyncPolicy _retryAsyncPolicy;
        private readonly Policy _retryPolicy;
        
        public SftpClient(
            SftpClientOptions options,
            ITempFileStreamFactory tempFileStreamFactory,
            ILoggerFactory loggerFactory)
        {
            _tempFileStreamFactory = tempFileStreamFactory ?? throw new ArgumentNullException(nameof(tempFileStreamFactory));
            
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<SftpClient>();

            if (options == null)
                throw new ArgumentNullException(nameof(options));
            var timeout = TimeSpan.FromSeconds(options.RetryTimeoutSec);

            var policyBase = Policy
                .Handle<SocketException>()
                .Or<SshConnectionException>()
                .Or<SshException>()
                .Or<ProxyException>();

            _retryAsyncPolicy = policyBase
                .WaitAndRetryAsync(
                    options.RetryCount,
                    i => timeout,
                    (ex, innerTimeout, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            $"Error occured while uploading to CDN. Retry # {retryCount} will started after {innerTimeout.TotalSeconds} sec");
                    });

            _retryPolicy = policyBase
                .WaitAndRetry(
                    options.RetryCount,
                    i => timeout,
                    (ex, innerTimeout, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            $"Error occured while uploading to CDN. Retry # {retryCount} will started after {innerTimeout.TotalSeconds} sec");
                    });
            
            var authMethods = ComposeAuthMethods(options);
            
            var connectionInfo = new ConnectionInfo(
                host: options.SshServer,
                port: options.SshPort,
                username: options.SshLogin,
                authenticationMethods: authMethods);

            _sftpClient = new Renci.SshNet.SftpClient(connectionInfo);
        }

        /// <inheritdoc />
        public void CheckConnection()
        {
            _sftpClient.Connect();
            _sftpClient.Disconnect();
        }

        /// <inheritdoc />
        public byte[]? DownloadFileFromServer(string basePath, string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (String.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            return _retryPolicy.Execute(() =>
            {
                EnsureConnected();

                var filePath = GetFileFullPath(basePath, fileName);

                if (!_sftpClient.Exists(filePath))
                {
                    _logger.LogWarning($"File \"{filePath}\" not found on SFTP server");
                    return null;
                }

                using (var file = _sftpClient.Open(filePath, FileMode.Open))
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            });
        }

        private string GetFileFullPath([NotNull] string basePath, [NotNull] string fileName)
        {
            var tempFileName = fileName.Replace(@"\", "/").Trim('/');
            var filePath = Path.Combine(basePath, tempFileName).Replace(@"\", "/");

            return filePath;
        }

        /// <inheritdoc />
        public Task<TempFileStream?> DownloadFileFromServerAsync(string basePath, string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (String.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            return _retryAsyncPolicy.ExecuteAsync(async () =>
            {
                EnsureConnected();

                var filePath = GetFileFullPath(basePath, fileName);

                if (!_sftpClient.Exists(filePath))
                {
                    _logger.LogWarning($"File \"{filePath}\" not found on SFTP server.");
                    return null;
                }

                using (var file = _sftpClient.Open(filePath, FileMode.Open))
                {
                    var tempFileStream = _tempFileStreamFactory.CreateTempFileStream();
                    await file.CopyToAsync(tempFileStream);
                    await tempFileStream.FlushAsync();

                    // we need to set position to start because temp stream can be used in another places
                    tempFileStream.Position = 0;
                    return tempFileStream;
                }
            });
        }

        /// <inheritdoc />
        public Task UploadFileToServerAsync(byte[] data, string basePath, string fileName, bool overwrite = false)
        {
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (String.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));
            if (data.Length == 0) throw new ArgumentException(LNG.SftpClient_CanNotBeEmpty, nameof(data));
            
            return UploadFileToServerAsync(new MemoryStream(data), basePath, fileName, overwrite);
        }

        /// <inheritdoc />
        public Task UploadFileToServerAsync(Stream stream, string basePath, string fileName, bool overwrite = false)
        {
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (String.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            if (stream == null)
                throw new ArgumentNullException(fileName);
            if (stream.Length == 0)
                throw new ArgumentException(LNG.SftpClient_CanNotBeEmpty, nameof(stream));

            return _retryAsyncPolicy.ExecuteAsync(() =>
            {
                EnsureConnected();

                var filePath = GetFileFullPath(basePath, fileName);

                if (_sftpClient.Exists(filePath))
                {
                    // Check file size.
                    var attributes = _sftpClient.GetAttributes(filePath);
                    if (attributes.Size == stream.Length)
                    {
                        // Size is equal. Assume that files are equal. No need to upload. 
                        _logger.LogWarning(String.Format(LNG.SftpClient_SameFileAlreadyExists, fileName));
                        return Task.CompletedTask;
                    }

                    if (overwrite)
                    {
                        // can overwrite, so delete file
                        _logger.LogWarning(String.Format(
                            LNG.SftpClient_Overwriting,
                            fileName,
                            attributes.Size,
                            stream.Length));
                        _sftpClient.DeleteFile(filePath);
                    }
                    else
                    {
                        // can't overwrite, it's error
                        throw new SshException(
                            String.Format(
                                LNG.SftpClient_DifferentFileAlreadyExists,
                                fileName,
                                attributes.Size,
                                stream.Length));
                    }
                }

                var sftpDirectory = Path.GetDirectoryName(filePath)
                    ?.Replace(@"\", "/") // windows-linux compatibility
                    ?? throw new InvalidOperationException("File path can't be mull");

                if (!_sftpClient.Exists(sftpDirectory))
                {
                    CreateDirectoryRecursively(sftpDirectory);
                }

                //TODO #3 check it, I think we don't need it here
                // we need to set position to start because temp stream can be used in another places
                stream.Position = 0;

                return Task.Factory.FromAsync(
                    _sftpClient.BeginUploadFile(
                        stream,
                        filePath,
                        false,
                        null,
                        null),
                    _sftpClient.EndUploadFile);
            });
        }

        private void CreateDirectoryRecursively(string path)
        {
            var current = "";

            if (path[0] == '/')
            {
                path = path.Substring(1);
            }

            var isFirst = true;
            while (!string.IsNullOrEmpty(path))
            {
                var p = path.IndexOf('/');
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    current += '/';
                }

                if (p >= 0)
                {
                    current += path.Substring(0, p);
                    path = path.Substring(p + 1);
                }
                else
                {
                    current += path;
                    path = "";
                }

                try
                {
                    var attributes = _sftpClient.GetAttributes(current);
                    if (!attributes.IsDirectory)
                    {
                        throw new Exception("not directory");
                    }
                }
                catch (SftpPathNotFoundException)
                {
                    _sftpClient.CreateDirectory(current);
                }
            }
        }

        /// <inheritdoc />
        public void DeleteFileFromServer(string basePath, string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (String.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            _retryPolicy.Execute(() =>
            {
                EnsureConnected();

                var filePath = GetFileFullPath(basePath, fileName);

                if (_sftpClient.Exists(filePath))
                {
                    _sftpClient.DeleteFile(filePath);
                }
                else
                {
                    _logger.LogWarning($"Can't delete \"{filePath}\", because file not found on a server.");
                }    
            });    
        }

        /// <inheritdoc />
        public bool IsExist(string basePath, string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (String.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            return _retryPolicy.Execute(() =>
            {
                EnsureConnected();

                var filePath = GetFileFullPath(basePath, fileName);
                return _sftpClient.Exists(filePath);
            });
        }

        private void EnsureConnected()
        {
            if (!_sftpClient.IsConnected)
            {
                _sftpClient.Connect();
            }
        }
        
        private AuthenticationMethod[] ComposeAuthMethods(SftpClientOptions options)
        {
            if (String.IsNullOrEmpty(options.SshPassword) &&
                String.IsNullOrEmpty(options.SshPrivateKeyPath))
            {
                return new AuthenticationMethod[]
                {
                    new NoneAuthenticationMethod(options.SshLogin)
                };
            }

            if (!String.IsNullOrEmpty(options.SshPassword))
            {
                return new AuthenticationMethod[]
                {
                    new PasswordAuthenticationMethod(options.SshLogin, options.SshPassword)

                };
            }

            if (!String.IsNullOrEmpty(options.SshPrivateKeyPath))
            {
                var keyPhrase = String.IsNullOrWhiteSpace(options.SshPrivateKeyPassphrase)
                    ? null
                    : options.SshPrivateKeyPassphrase;
                return new AuthenticationMethod[]
                {
                    new PrivateKeyAuthenticationMethod(
                        options.SshLogin,
                        new PrivateKeyFile(
                            options.SshPrivateKeyPath,
                            keyPhrase))

                };
            }

            throw new ArgumentException(LNG.SftpClient_AuthMethodsNotSpecified);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _sftpClient.Dispose();
        }
    }
}