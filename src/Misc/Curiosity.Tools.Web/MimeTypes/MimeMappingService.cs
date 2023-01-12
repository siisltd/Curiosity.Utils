using System;
using Microsoft.AspNetCore.StaticFiles;

namespace Curiosity.Tools.Web.MimeTypes
{
    /// <summary>
    /// Service for getting Mime type by file name.
    /// </summary>
    /// <remarks>
    /// A more convenient wrapper over the standard <see cref= "FileExtensionContentTypeProvider"/>.
    /// </remarks>
    public class MimeMappingService
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public MimeMappingService(FileExtensionContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }
        
        /// <summary>
        /// Returns the Mime type by file name
        /// </summary>
        /// <param name= "fileName" >a file Name that also includes the file extension</param>
        /// <returns>MIME file type or application/octet-stream if the mime type could not be obtained correctly</returns>
        /// <exception cref= "ArgumentNullException" >If <see cref= "fileName"/> <see langword= "null" /></exception>
        public string GetMimeType(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            
            return contentType;
        }
    }
}