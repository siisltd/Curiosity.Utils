using Newtonsoft.Json;

namespace SIISLtd.SSNG.ISACR.Core
{
    public static class SmsConstants
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.None
            };
    }
}
