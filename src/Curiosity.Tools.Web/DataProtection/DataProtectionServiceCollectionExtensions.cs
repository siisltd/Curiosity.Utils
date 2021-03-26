using System;
using System.IO;
using System.Linq;
using Curiosity.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Tools.Web.DataProtection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to add data protection to web applications
    /// </summary>
    public static class DataProtectionServiceCollectionExtensions
    {
        public static IServiceCollection DisableDataProtection(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return services.AddCuriosityDataProtection(new DataProtectionOptions
            {
                IsEnabled = false
            });
        }
        
        /// <summary>
        /// Adds data protection to web application's form
        /// </summary>
        /// <param name="services">IoC</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCuriosityDataProtection(
            this IServiceCollection services,
            DataProtectionOptions? options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) return services;
            
            if (options.IsEnabled)
            {
                options.AssertValid();
                
               services.AddDataProtection()
                    .SetApplicationName(options.ApplicationName)
                    .DisableAutomaticKeyGeneration()
                    // ReSharper disable once AssignNullToNotNullAttribute
                    // already checked
                    .PersistKeysToFileSystem(new DirectoryInfo(options.KeyRingPath));
            }
            else
            {
                // if we do not need protection, then insert the stub
                var dataProtectionProviderDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDataProtectionProvider));
                if (dataProtectionProviderDescriptor != null)
                {
                    services.Replace(
                        ServiceDescriptor.Describe(
                            dataProtectionProviderDescriptor.ServiceType,
                            sp =>
                            {
                                var innerDataProtectionProvider = (IDataProtectionProvider) dataProtectionProviderDescriptor.ImplementationFactory(sp);

                                return new SecureLessDataProtectionProvider(innerDataProtectionProvider, options);
                            },
                            dataProtectionProviderDescriptor.Lifetime));
                }
            }
            
            services.TryAddSingleton(options);

            return services;
        }
    }
}