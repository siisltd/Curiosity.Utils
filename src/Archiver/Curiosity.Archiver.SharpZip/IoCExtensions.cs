using System;
using Curiosity.Tools.TempFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Archiver.SharpZip
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> for registering SharpZip archiver. 
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds SharpZip archiver to services. 
        /// </summary>
        public static IServiceCollection AddSharpZipArchiver(this IServiceCollection services, TempFileOptions tempFileOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (tempFileOptions == null) throw new ArgumentNullException(nameof(tempFileOptions));

            services.TryAddSingleton<IArchiver, SharpZipArchiver>();
            
            // requires temp file subsystem
            services.AddTempFileServices(tempFileOptions);
            
            return services;
        }
    }
}