using System;
using Curiosity.Configuration;
using Curiosity.Tools.AppInitializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.SFTP.SSH.Net
{
    /// <summary>
    /// Extensions methods for <see cref="IServiceCollection"/> for SSH.NET SFTP services registration
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds to SSH.NET SFTP services.
        /// </summary>
        public static IServiceCollection AddSftpServices(
            this IServiceCollection services,
            SftpClientOptions sftpClientOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (sftpClientOptions == null) throw new ArgumentNullException(nameof(sftpClientOptions));
            
            sftpClientOptions.AssertValid();
            
            services.TryAddSingleton(sftpClientOptions);
            services.TryAddSingleton<ISftpClientFactory, SftpClientFactory>();
            services.AddAppInitializer<SftpInitializer>();
            
            return services;
        }
    }
}
