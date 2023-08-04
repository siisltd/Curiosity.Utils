using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.SMS.Iqsms;

public static class IoCExtensions
{
    /// <summary>
    /// Adds SMTP EMail sender to services.
    /// </summary>
    public static IServiceCollection AddIqsmsSender(
        this IServiceCollection services,
        IqsmsOptions options,
        bool useAsDefaultSender = true)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (options  == null) throw new ArgumentNullException(nameof(options));

        options.AssertValid();

        services.TryAddSingleton(options);
        services.TryAddSingleton<IIqsmsSender, IqsmsSender>();

        if (useAsDefaultSender)
        {
            services.TryAddSingleton<ISmsSender>(s => s.GetRequiredService<IIqsmsSender>());
        }

        return services;
    }
}