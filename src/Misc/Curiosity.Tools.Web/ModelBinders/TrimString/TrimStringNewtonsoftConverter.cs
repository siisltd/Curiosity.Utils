using System;
using Newtonsoft.Json;

namespace Curiosity.Tools.Web.ModelBinders
{
    /// <summary>
    /// Calls Trim() for string while writing JSON.
    /// It works only for models binding [FromBody].
    /// </summary>
    /// <remarks>
    /// Works only with Newtonsoft serialization.
    /// Add attribute [JsonConverter(typeof(TrimStringNewtonsoftConverter))] for string property.
    /// </remarks>
    public class TrimStringNewtonsoftConverter : JsonConverter<string?>
    {
        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override string? ReadJson(
            JsonReader reader,
            Type objectType,
            string? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return (reader.Value as string)?.Trim();
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
