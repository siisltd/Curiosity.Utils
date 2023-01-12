using System;
using System.IO;
using Curiosity.Configuration;

namespace Curiosity.Tools.TempFiles
{
    /// <inheritdoc />
    public class TempFileStreamFactory : ITempFileStreamFactory
    {
        /// <summary>
        /// Option for working with temporary files.
        /// </summary>
        private readonly TempFileOptions _tempFileOptions;

        public TempFileStreamFactory(TempFileOptions tempFileOptions)
        {
            if (String.IsNullOrWhiteSpace(tempFileOptions.TempPath))
                throw new ArgumentNullException(nameof(tempFileOptions.TempPath));
            
            _tempFileOptions = tempFileOptions ?? throw new ArgumentNullException(nameof(tempFileOptions));
            
            tempFileOptions.AssertValid();
        }

        /// <inheritdoc />
        public TempFileStream CreateTempFileStream(string? fileName = null)
        {
            var tempFileName = String.IsNullOrWhiteSpace(fileName)
                ? $"{Guid.NewGuid()}.tmp"
                : fileName;
            var tempFilePath = Path.Combine(_tempFileOptions.TempPath, tempFileName);
            
            return new TempFileStream(tempFilePath, FileMode.Create);
        }
    }
}