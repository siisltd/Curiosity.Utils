using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.DAL.Dapper
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to register Curiosity dapper
    /// </summary>
    public static class DapperServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="DapperSqlExecutor"/> to services as implementation of <see cref="ISqlExecutor"/>
        /// </summary>
        public static IServiceCollection AddCuriosityDapper(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<ISqlExecutor, DapperSqlExecutor>();
            
            return services;
        }
    }
}