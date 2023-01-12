namespace Curiosity.SFTP
{
    /// <summary>
    /// Factory for <see cref="ISftpClient"/> creation.
    /// </summary>
    public interface ISftpClientFactory
    {
        /// <summary>
        /// Creates new instance of <see cref="ISftpClient"/>.
        /// </summary>
        /// <returns>New instance of <see cref="ISftpClient"/></returns>
        ISftpClient GetSftpClient();
    }
}