using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Curiosity.Tools.Web.ModelBinders
{
    /// <summary>
    /// Calls Trim() for string while writing JSON.
    /// It works only for models binding [FromBody].
    /// </summary>
    /// <remarks>
    /// Works only with System.Json serialization.
    /// Add attribute [JsonConverter(typeof(TrimStringSystemJsonConverter))] for string property.
    /// </remarks>
    public class TrimStringSystemJsonConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString()?.Trim();
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.Trim());
        }
    }
}
