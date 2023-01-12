namespace Curiosity.Tools.Humanization
{
    public static class FileSizeExtensions
    {
        /// <summary>
        /// Converts file's size from bytes to human readable view.
        /// </summary>
        /// <param name="fileSizeBytes"></param>
        /// <returns></returns>
        public static string ToHumanReadableSizeString(this long fileSizeBytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            var order = 0;

            var fileSize = (double) fileSizeBytes;
            while (fileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSize = fileSize / 1024;
            }
            
            return $"{fileSize:0.00} {sizes[order]}";
        }
    }
}