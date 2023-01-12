using System;
using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web
{
    /// <summary>
    /// Extension methods for <see cref= "HttpRequest"/>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Checks whether the `Content-Type` header contains the `multipart/`value
        /// </summary>
        /// <param name= "request" >Request</param>
        /// <returns>The validation result</returns>
        /// <exception cref= "ArgumentNullException" >If the argument <see langword= "null" /></exception>
        public static bool IsMultipartContentType(this HttpRequest request)
        {
            if (request == null) 
                throw new ArgumentNullException(nameof(request));
            
            return !string.IsNullOrEmpty(request.ContentType)
                   && request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}