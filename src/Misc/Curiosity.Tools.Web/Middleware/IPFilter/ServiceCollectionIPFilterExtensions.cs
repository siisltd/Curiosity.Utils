// using System;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Curiosity.Tools.Web.Middleware.IPFilter
// {
//     public static class ServiceCollectionIPFilterExtensions
//     {
//         public static void AddIPFilter(this IServiceCollection services, IPFilterOptions options)
//         {
//             if (services == null) throw new ArgumentNullException(nameof(services));
//             if (options == null) throw new ArgumentNullException(nameof(options));
//             
//             services.AddSingleton(options);
//         }
//     }
// }