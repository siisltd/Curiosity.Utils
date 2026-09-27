using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web
{
    public static class SessionExtensions
    {
        // Case-insensitive reading and fields keep values stored by the previous Newtonsoft.Json-based version readable.
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };

        /// <summary>
        /// Save object into session as serialized JSON string.
        /// </summary>
        /// <param name="session">Current session.</param>
        /// <param name="key">Key at which serialized object will be stored in session.</param>
        /// <param name="value">Object to store into session.</param>
        public static Task SetObjectAsync(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value, SerializerOptions));
            return session.CommitAsync();
        }

        /// <summary>
        /// Get previously stored into session object.
        /// </summary>
        /// <param name="session">Current session.</param>
        /// <param name="key">Key at which serialized object was stored in session.</param>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <returns>Previously stored object or default value for T.</returns>
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return (value == null
                ? default
                : JsonSerializer.Deserialize<T>(value, SerializerOptions))!;
        }
    }
}
