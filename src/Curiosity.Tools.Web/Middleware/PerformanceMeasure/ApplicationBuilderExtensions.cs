using System;
using Microsoft.AspNetCore.Builder;

namespace Curiosity.Tools.Web.Middleware
{
    public static partial class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds middleware to measure Web performance.
        /// </summary>
        public static IApplicationBuilder UsePerformanceMeasurer(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<PerformanceMeasureMiddleware>();
        }
    }
}