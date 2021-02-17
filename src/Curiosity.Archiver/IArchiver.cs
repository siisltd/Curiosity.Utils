using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.TempFiles;

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

        /// <summary>
        /// Archives specified files in async manner.
        /// </summary>
        /// <param name="sourceFiles">Full path to files that will be archive</param>
        /// <param name="useZip64">Whether use Zip64 for large files support or not. Enabling it may lead to
        /// incompatibility with old software such as Windows XP and Android prior to 6.0</param>
        /// <param name="zipFileName">Target zip file name. If <see langword="null"/>, unique name will be generated</param>
        /// <param name="zipFileNames">Names of files in zip archive. If not <see langword="null"/>, item's count must be equal to <paramref name="sourceFiles"/></param>
        /// <param name="cts">Cancellation token</param>
        /// <returns>File stream with zip file in temp directory</returns>
        Task<TempFileStream> ZipFilesToStreamAsync(
            IList<string> sourceFiles,
            bool useZip64 = true, 
            string? zipFileName = null,
            IList<string>? zipFileNames = null,
            CancellationToken cts = default);
        
        /// <summary>
        /// Archives specified files in async manner.
        /// </summary>
        /// <param name="sourceFiles">Full path to files that will be archive</param>
        /// <param name="useZip64">Whether use Zip64 for large files support or not. Enabling it may lead to
        /// incompatibility with old software such as Windows XP and Android prior to 6.0</param>
        /// <param name="zipFileName">Target zip file name. If <see langword="null"/>, unique name will be generated</param>
        /// <param name="zipFileNames">Names of files in zip archive. If not <see langword="null"/>, item's count must be equal to <paramref name="sourceFiles"/></param>
        /// <param name="cts">Cancellation token</param>
        /// <returns>Full path to zip archive</returns>
        Task<string> ZipFilesToFileAsync(
            IList<string> sourceFiles,
            bool useZip64 = true,
            string? zipFileName = null,
            IList<string>? zipFileNames = null,
            CancellationToken cts = default);
    }
}