using System;
using Microsoft.Extensions.Localization;

namespace Curiosity.Localization
{
    /// <summary>
    /// Extension methods for <see cref="IStringLocalizer"/>.
    /// </summary>
    public static class LocalizerExtensions
    {
        public static string GetQuantityString(this IStringLocalizer localizer, string key, int quantity, params object[] args)
        {
            if (localizer == null) throw new ArgumentNullException(nameof(localizer));

            return localizer.GetQuantityString(key, (long) quantity, args);
        }

        public static string GetQuantityString(this IStringLocalizer localizer, string key, long quantity, params object[] args)
        {
            if (localizer == null) throw new ArgumentNullException(nameof(localizer));

            var normalizedQuantity = GetNormalizedQuantity(quantity);
            return localizer[$"{key}_{normalizedQuantity}", args];
        }

        public static long GetNormalizedQuantity(long quantity)
        {
            quantity = Math.Abs(quantity) % 100;

            // If ends with 10 .. 19 - use the form for the number 5.
            if (quantity > 10 && quantity < 20) 
                return 5;
            
            quantity = quantity % 10;
            // If ends with 1 - use the form for the number 1.
            if (quantity == 1) 
                return 1;

            // If ends with 2 .. 4 - use the form for the number 2.
            if (quantity > 1 && quantity < 5) 
                return 2;

            // If ends with 0, 5 .. 9 - use the form for the number 5.
            return 5;
        }
    }
}
