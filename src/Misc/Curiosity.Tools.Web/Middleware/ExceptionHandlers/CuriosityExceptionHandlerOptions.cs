using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web.Middleware
{
    /// <summary>
    /// Options for <see cref="CuriosityExceptionHandleMiddleware"/>
    /// </summary>
    public class CuriosityExceptionHandlerOptions
    {
        /// <summary>
        /// Path with action that will handle exception
        /// </summary>
        public PathString ExceptionHandlingPath { get; set; }
    }
}