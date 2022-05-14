using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools.TempFiles;

namespace Curiosity.Archiver
{
    /// <summary>
    /// Curiosity archiver.
    /// </summary>
    public interface IArchiver
    {
        /// <summary>
        /// Archives directory recursively into ZIP.
        /// </summary>
        /// <param name="dirToCompressPath">Full path to directory to compress</param>
        /// <param name="useZip64">Whether use Zip64 for large files support or not. Enabling it may lead to
        /// incompatibility with old software such as Windows XP and Android prior to 6.0</param>
        /// <param name="cts">Cancellation token</param>
        /// <returns>File stream with temp file</returns>
        Task<TempFileStream> ZipDirAsync(
            string dirToCompressPath,
            bool useZip64 = true, 
            CancellationToken cts = default);

        [Obsolete("Use method with file names collection")]
        Task<TempFileStream> ZipFilesToStreamAsync(
            IList<string> sourceFiles,
            bool useZip64 = true, 
            string? zipFileName = null,
            IList<string>? zipFileNames = null,
            CancellationToken cts = default);
        
        /// <summary>
        /// Archives specified files in async manner.
        /// </summary>
        /// <param name="sourceFiles">Collection of the full path to files that will be archived and users file names</param>
        /// <param name="useZip64">Whether use Zip64 for large files support or not. Enabling it may lead to
        /// incompatibility with old software such as Windows XP and Android prior to 6.0</param>
        /// <param name="zipFileName">Target zip file name. If <see langword="null"/>, unique name will be generated</param>
        /// <param name="cts">Cancellation token</param>
        /// <returns>File stream with zip file in temp directory</returns>
        Task<TempFileStream> ZipFilesToStreamAsync(
            IReadOnlyList<FileNames> sourceFiles,
            bool useZip64 = true, 
            string? zipFileName = null,
            CancellationToken cts = default);
        
        [Obsolete("Use method with file names collection")]
        Task<string> ZipFilesToFileAsync(
            IList<string> sourceFiles,
            bool useZip64 = true,
            string? zipFileName = null,
            IList<string>? zipFileNames = null,
            CancellationToken cts = default);
        
        /// <summary>
        /// Archives specified files in async manner.
        /// </summary>
        /// <param name="sourceFiles">Collection of the full path to files that will be archived and users file names</param>
        /// <param name="useZip64">Whether use Zip64 for large files support or not. Enabling it may lead to
        /// incompatibility with old software such as Windows XP and Android prior to 6.0</param>
        /// <param name="zipFileName">Target zip file name. If <see langword="null"/>, unique name will be generated</param>
        /// <param name="cts">Cancellation token</param>
        /// <returns>Full path to zip archive</returns>
        Task<string> ZipFilesToFileAsync(
            IReadOnlyList<FileNames> sourceFiles,
            bool useZip64 = true,
            string? zipFileName = null,
            CancellationToken cts = default);

        /// <summary>
        /// Unzip specified archive.
        /// </summary>
        /// <param name="file">Stream with zip archive.</param>
        /// <param name="unzipDirectoryPath">Path to folder to store unzipped files. Folder will be created in a temp directory in no value specified.</param>
        /// <returns>Path to directory with unzipped files.</returns>
        string UnzipFileAsync(FileStream file, string? unzipDirectoryPath = null);
    }
}
