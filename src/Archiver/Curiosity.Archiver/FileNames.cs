using System;
using System.IO;

namespace Curiosity.Archiver
{
    public class FileNames
    {
        /// <summary>
        /// Full path of the file.
        /// </summary>
        public string StorageFileName { get; }
        
        /// <summary>
        /// A filename that a user sees in the archive
        /// </summary>
        public string UserFileName { get; }

        public FileNames(string storageFileName, string userFileName)
        {
            if (String.IsNullOrWhiteSpace(storageFileName)) throw new ArgumentNullException(nameof(storageFileName));
            if (String.IsNullOrWhiteSpace(userFileName)) throw new ArgumentNullException(nameof(userFileName));
            
            StorageFileName = storageFileName;
            UserFileName = userFileName;
        }
        
        public FileNames(string storageFileName)
        {
            if (String.IsNullOrWhiteSpace(storageFileName)) throw new ArgumentNullException(nameof(storageFileName));
            
            StorageFileName = storageFileName;
            UserFileName = Path.GetFileName(storageFileName);
        }
    }
}