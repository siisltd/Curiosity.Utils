using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Curiosity.Tools.TempFiles;

namespace Curiosity.SFTP
{
    /// <summary>
    /// Client for working with SFTP.
    /// </summary>
    public interface ISftpClient : IDisposable
    {
        /// <summary>
        /// Tries to open connection to check it.
        /// Throws exception if something goes wrong.
        /// </summary>
        void CheckConnection();

        /// <summary>
        /// Downloads file by his name.
        /// </summary>
        /// <param name="basePath">Base directory for combining path for file download.</param>
        /// <param name="fileName">Relative (from base directory) file name.</param>
        /// <returns>File content in bytes array.</returns>
        byte[]? DownloadFileFromServer(string basePath, string fileName);
        
        /// <summary>
        /// Downloads file from SFTP in async manner.
        /// </summary>
        /// <param name="basePath">Base directory for combining path for file download.</param>
        /// <param name="fileName">Relative (from base directory) file name.</param>
        /// <returns>File content in temp file stream.</returns>
        Task<TempFileStream?> DownloadFileFromServerAsync(string basePath, string fileName);

        /// <summary>
        /// Gets file stream from SFTP.
        /// </summary>
        /// <param name="basePath">Base directory for combining path for file download.</param>
        /// <param name="fileName">Relative (from base directory) file name.</param>
        /// <returns>File content in stream.</returns>
        Stream? GetDownloadStream(string basePath, string fileName);

        /// <summary>
        /// Uploads file to SFTP in async manner.
        /// </summary>
        /// <param name="data">File content in byte array.</param>
        /// <param name="basePath">Base directory for combining path for file download.</param>
        /// <param name="fileName">Relative (from base directory) file name.</param>
        /// <param name="overwrite">Upload behaviour when file with same name and different size exists on the server.
        /// When true - file will be overwritten with warning in log. When false - exception will be thrown.</param>
        Task UploadFileToServerAsync(byte[] data, string basePath, string fileName, bool overwrite = false);
        
        /// <summary>
        /// Uploads file to SFTP in async manner.
        /// </summary>
        /// <param name="stream">File content in stream.</param>
        /// <param name="basePath">Base directory for combining path for file download.</param>
        /// <param name="fileName">Relative (from base directory) file name.</param>
        /// <param name="overwrite">Upload behaviour when file with same name and different size exists on the server.
        /// When true - file will be overwritten with warning in log. When false - exception will be thrown.</param>
        Task UploadFileToServerAsync(Stream stream, string basePath, string fileName, bool overwrite = false);

        /// <summary>
        /// Deletes file from SFTP.
        /// </summary>
        /// <param name="basePath">Base directory for combining path for file download.</param>
        /// <param name="fileName">Relative (from base directory) file name.</param>
        void DeleteFileFromServer(string basePath, string fileName);
        
        /// <summary>
        /// Checks file existence.
        /// </summary>
        /// <param name="basePath">Base directory for combining path for file.</param>
        /// <param name="fileName">Relative (from base directory) file name.</param>
        bool IsExist(string basePath, string fileName);

        /// <summary>
        /// Returns info about directories files.
        /// </summary>
        /// <param name="basePath">Relative (from base directory) path to the directory.</param>
        public IReadOnlyList<SftpFileInfo> ListDirectoryContents(string basePath);
    }
}
