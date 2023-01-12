namespace Curiosity.Tools.TempFiles
{
    /// <summary>
    /// Factory for <see cref="TempFileStream"/>.
    /// </summary>
    /// <remarks>
    /// Simplifies creation of <see cref="TempFileStream"/> by generating unique file name and placing file into
    /// temp folder.
    /// </remarks>
    public interface ITempFileStreamFactory
    {
        /// <summary>
        /// Creates new stream for temp file.
        /// </summary>
        /// <param name="fileName">Temporary file name without full path. If <see langword="null"/>, unique name will be generated.</param>
        /// <returns>Stream for temp file</returns>
        TempFileStream CreateTempFileStream(string? fileName = null);
    }
}
