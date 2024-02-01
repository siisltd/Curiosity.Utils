using System.Globalization;
using SIISLtd.Utils.Localization;

namespace SIISLtd.Utils.FileDataReaderWriters.Resources
{
    internal static class LNG
    {
        private static readonly LocalizerCore Localizer;
        private static readonly LocalizationOptions Options;

        static LNG()
        {
            Options = new LocalizationOptions();
            Localizer = new LocalizerCore(typeof(LNG).Assembly, Options);
        }

        public static string Get(string source, params object[] arguments)
        {
            return Localizer.Get(Options.Prefix, source, false, arguments);
        }
        
        public static string GetForCulture(CultureInfo culture, string source, params object[] arguments)
        {
            return Localizer.GetForCulture(culture, Options.Prefix, source, false, arguments);
        }
    }
}
