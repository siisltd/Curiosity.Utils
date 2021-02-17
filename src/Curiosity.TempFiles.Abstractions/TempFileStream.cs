using System.IO;

namespace Curiosity.TempFiles
{
    /// <summary>
    /// File stream for temporary file.
    /// Associated file will be automatically deleted when that stream will be disposed. 
    /// </summary>
    public class TempFileStream : FileStream
    {
        #region Default values from FileStream
        
        private const FileShare DefaultShare = FileShare.Read;
        private const int DefaultBufferSize = 4096;
        
        #endregion

        private const FileOptions DefaultFileOptions = FileOptions.DeleteOnClose;
        
        /// <inheritdoc />
        public TempFileStream(string path, FileMode mode) 
            : base(
                path,
                mode,
                mode == FileMode.Append
                    ? FileAccess.Write
                    : FileAccess.ReadWrite,
                DefaultShare,
                DefaultBufferSize,
                DefaultFileOptions)
        {
            FileName = Path.GetFileName(path);
            FullPath = Path.GetFullPath(path);
        }
        
        /// <summary>
        /// File name with extensions
        /// </summary>
        /// <remarks>
        /// <see cref="FileStream.Name"/> is raw file name passed to constructor.
        /// </remarks>
        public string FileName { get; }
        
        /// <summary>
        /// Full path to the file
        /// </summary>
        /// <remarks>
        /// <see cref="FileStream.Name"/> is raw file name passed to constructor.
        /// </remarks>
        public string FullPath { get; }
    }
}