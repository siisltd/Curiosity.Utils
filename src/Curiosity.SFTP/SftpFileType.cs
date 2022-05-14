namespace Curiosity.SFTP
{
    /// <summary>
    /// Type of <see cref="SftpFileInfo"/>.
    /// </summary>
    public enum SftpFileType
    {
        /// <summary>
        /// Unknown file type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Regular file.
        /// </summary>
        Regular = 1,

        /// <summary>
        /// Directory.
        /// </summary>
        Directory = 2,

        /// <summary>
        /// Socket.
        /// </summary>
        Socket = 3,

        /// <summary>
        /// Block device.
        /// </summary>
        BlockDevice = 4,

        /// <summary>
        /// Character device.
        /// </summary>
        CharacterDevice = 5,

        /// <summary>
        /// Named pipe.
        /// </summary>
        NamedPipe = 6,

        /// <summary>
        /// Symbolic link. 
        /// </summary>
        SymbolicLink = 7
    }
}
