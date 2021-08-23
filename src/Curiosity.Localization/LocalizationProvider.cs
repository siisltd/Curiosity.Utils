using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Curiosity.Localization
{
    [Obsolete("Need to delete after all localization will be migrated to new LNG way.")]
    public static class LocalizationProvider
    {
        private static IServiceProvider _serviceProvider = null!;

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        public static IStringLocalizer<T> GetStringLocalizer<T>()
        {
            if (_serviceProvider == null) 
                throw new InvalidOperationException($"ServiceProvider not specified. Specify it by method {nameof(SetServiceProvider)}");

            return _serviceProvider.GetRequiredService<IStringLocalizer<T>>();
        }
    }
}
