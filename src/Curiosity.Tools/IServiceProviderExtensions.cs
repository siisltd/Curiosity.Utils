using System;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Tools
{
    /// <summary>
    /// Extensions methods for <see cref="IServiceProvider"/>
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Creates a new instance of the class using <see cref= "IServiceProvider" /> and <see cref= "ActivatorUtilities"/>
        /// </summary>
        public static T CreateInstance<T>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            return ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }
    }
}