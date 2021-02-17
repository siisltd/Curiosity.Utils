// using System;
// using Microsoft.AspNetCore.Builder;
//
// namespace Curiosity.Tools.Web.Middleware.IPFilter
// {
//     public static class ApplicationBuilderIPFilterExtensions
//     {
//         /// <summary>
//         /// Adds to pipeline middleware for filtering by IP (adds if there are settings)
//         /// </summary>
//         public static IApplicationBuilder UseIPFilter(this IApplicationBuilder builder, IPFilterOptions options)
//         {
//             if (builder == null) throw new ArgumentNullException(nameof(builder));
//             if (options == null) throw new ArgumentNullException(nameof(options));
//
//             if (options.AllowedIP != null && options.AllowedIP.Length > 0)
//             {
//                 return builder.UseMiddleware<IPFilter>();
//             }
//
//             return builder;
//         }
//     }
// }