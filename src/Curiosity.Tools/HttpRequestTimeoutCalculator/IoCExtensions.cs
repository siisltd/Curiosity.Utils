using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Tools.HttpRequestTimeoutCalculator
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to register <see cref="IHttpRequestParamsCalculator"/>
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds to IoC <see cref="IHttpRequestParamsCalculator"/>
        /// </summary>
        public static IServiceCollection AddHttpRequestTimesCalculator(
            this IServiceCollection services,
            HttpRequestParamsCalculatorOptions options)
        {
            options.AssertValid();
            
            services.TryAddSingleton(options);
            services.TryAddSingleton<IHttpRequestParamsCalculator, HttpRequestParamsCalculator>();
            
            return services;
        }
    }
}