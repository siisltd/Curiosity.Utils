using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Tools.Web.SiteMap
{
    public static class IoCExtensions
    {
        public static IServiceCollection AddSiteMap<TExtraPageProvider>(this IServiceCollection services, IMvcBuilder mvcBuilder) where TExtraPageProvider: SiteMapExtraPageProvider
        {
            services.TryAddSingleton<TExtraPageProvider>();
            services.TryAddSingleton<SiteMapExtraPageProvider>(c => c.GetRequiredService<TExtraPageProvider>());
            
            // adds controller
            mvcBuilder.AddApplicationPart(typeof(SiteMapController).Assembly);

            return services;
        }

        public static IServiceCollection AddSiteMap(this IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            return services.AddSiteMap<SiteMapExtraPageProvider>(mvcBuilder);
        }
    }
}