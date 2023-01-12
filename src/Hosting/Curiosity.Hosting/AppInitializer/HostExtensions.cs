using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Curiosity.Hosting.AppInitializer
{
    /// <summary>
    /// Provides extension methods to perform async initialization of an application.
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Initializes the application, by calling all registered async initializers.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the initialization completion.</returns>
        public static async Task InitAsync(this IHost host, CancellationToken cancellationToken = default)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            using (var scope = host.Services.CreateScope())
            {
                var rootInitializer = scope.ServiceProvider.GetService<Hosting.AppInitializer.AppInitializer?>();
                if (rootInitializer == null)
                {
                    throw new InvalidOperationException("The async initialization service isn't registered, register it by calling AddAsyncInitialization() on the service collection or by adding an async initializer.");
                }

                await rootInitializer.InitializeAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Initializes the application, by calling all registered async initializers.
        /// </summary>
        /// <param name="scope">DI scope.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the initialization completion.</returns>
        public static Task InitAsync(this IServiceScope scope, CancellationToken cancellationToken = default)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            var rootInitializer = scope.ServiceProvider.GetService<Hosting.AppInitializer.AppInitializer?>();
            if (rootInitializer == null)
            {
                throw new InvalidOperationException("The async initialization service isn't registered, register it by calling AddAsyncInitialization() on the service collection or by adding an async initializer.");
            }

            return rootInitializer.InitializeAsync(cancellationToken);
        }
    }
}
