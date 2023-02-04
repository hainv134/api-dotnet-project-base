using Domain.Global;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common.Untils
{
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _formater = Config.DateTimeFormat.DEFAULT;

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString() ?? "", _formater, DateTimeFormatInfo.InvariantInfo);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}