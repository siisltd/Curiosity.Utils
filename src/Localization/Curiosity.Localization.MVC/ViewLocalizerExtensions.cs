using Microsoft.AspNetCore.Mvc.Localization;

namespace Curiosity.Localization.MVC
{
    /// <summary>
    /// Extension methods for <see cref="IViewLocalizer"/>.
    /// </summary>
    public static class ViewLocalizerExtensions
    {
        public static LocalizedHtmlString GetQuantityString(this IViewLocalizer localizer, string key, int quantity, params object[] args)
        {
            return localizer.GetQuantityString(key, (long) quantity, args);
        }

        public static LocalizedHtmlString GetQuantityString(this IViewLocalizer localizer, string key, long quantity, params object[] args)
        {
            var normalizedQuantity = LocalizerExtensions.GetNormalizedQuantity(quantity);
            return localizer[$"{key}_{normalizedQuantity}", args];
        }
    }
}
