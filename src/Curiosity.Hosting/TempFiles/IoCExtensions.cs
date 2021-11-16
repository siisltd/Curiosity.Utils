using System;
using Curiosity.Configuration;
using Curiosity.Tools.TempFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Hosting.TempFiles
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds temp directory cleaner as hosted service.
        /// </summary>
        public static IServiceCollection AddTempDirCleaner(this IServiceCollection services, TempFileOptions tempFileOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (tempFileOptions == null) throw new ArgumentNullException(nameof(tempFileOptions));

            tempFileOptions.AssertValid();

            services.TryAddSingleton(tempFileOptions);
            services.AddHostedService<TempDirCleaner>();

            return services;
        }
    }
}
