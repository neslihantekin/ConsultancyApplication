using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsultancyApplication.Core.Utilities.Converters
{
    public class ArilDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long rawValue))
            {
                string strValue = rawValue.ToString();
                if (DateTime.TryParseExact(strValue, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var dt))
                {
                    return dt;
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(long.Parse(value.Value.ToString("yyyyMMddHHmmss")));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
