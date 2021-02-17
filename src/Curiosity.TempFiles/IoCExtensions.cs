using System;
using Curiosity.AppInitializer;
using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.TempFiles
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds all services for working with temp file streams to services.
        /// </summary>
        public static IServiceCollection AddTempFileServices(
            this IServiceCollection services,
            TempFileOptions tempFileOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (tempFileOptions == null) throw new ArgumentNullException(nameof(tempFileOptions));
            
            tempFileOptions.AssertValid();

            services.TryAddSingleton(tempFileOptions);
            services.TryAddSingleton<ITempFileStreamFactory, TempFileStreamFactory>();
            services.AddAppInitializer<TempFilesInitService>();
            
            return services;
        }
    }
}