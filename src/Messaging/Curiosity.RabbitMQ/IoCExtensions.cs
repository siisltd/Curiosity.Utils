using System;
using Curiosity.Configuration;
using Curiosity.RabbitMQ.RPC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.RabbitMQ;

/// <summary>
/// Extension methods to IoC to add RabbitMQ RPC services.
/// </summary>
public static class IoCExtensions
{
    /// <summary>
    /// Add RabbitMQ RPC services to IoC.
    /// </summary>
    public static IServiceCollection AddRabbitMQRPC(
        this IServiceCollection services,
        RabbitMQOptions options)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (options == null) throw new ArgumentNullException(nameof(options));

        options.AssertValid();

        services.TryAddSingleton(options);
        services.TryAddSingleton<RabbitMqRpcClientFactory>();

        return services;
    }
}
