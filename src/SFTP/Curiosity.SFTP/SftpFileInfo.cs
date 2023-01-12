using System;
using System.ComponentModel;

namespace Curiosity.SFTP
{
    /// <summary>
    /// Information about file stored at SFTP.
    /// </summary>
    public readonly struct SftpFileInfo
    {
        /// <summary>
        /// File's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// File's full name.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// File's type.
        /// </summary>
        public SftpFileType Type { get; }

        /// <summary>
        /// File's size in bytes.
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// Timestamp of last access in UTC. 
        /// </summary>
        public DateTime LastAccessTimeUtc { get; }

        /// <summary>
        /// Timestamp of last write in UTC. 
        /// </summary>
        public DateTime LastWriteTimeUtc { get; }

        /// <inheritdoc cref="SftpFileInfo"/>
        public SftpFileInfo(
            string name,
            string fullName,
            SftpFileType type,
            long length,
            DateTime lastAccessTimeUtc,
            DateTime lastWriteTimeUtc) : this()
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (String.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(fullName));
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (!Enum.IsDefined(typeof(SftpFileType), type)) throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(SftpFileType));

            Name = name;
            FullName = fullName;
            Type = type;
            Length = length;
            LastAccessTimeUtc = lastAccessTimeUtc;
            LastWriteTimeUtc = lastWriteTimeUtc;
        }
    }
}
