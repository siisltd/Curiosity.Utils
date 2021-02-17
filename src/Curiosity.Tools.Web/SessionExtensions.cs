using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Curiosity.Tools.Web
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Save object into session as serialized JSON string.
        /// </summary>
        /// <param name="session">Current session.</param>
        /// <param name="key">Key at which serialized object will be stored in session.</param>
        /// <param name="value">Object to store into session.</param>
        public static Task SetObjectAsync(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
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
            return value == null
                ? default
                : JsonConvert.DeserializeObject<T>(value);
        }
    }
}