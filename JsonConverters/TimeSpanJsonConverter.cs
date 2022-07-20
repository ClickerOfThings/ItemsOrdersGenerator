using ItemOrderDemonstration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace ItemsOrdersGenerator.JsonConverters
{
    /// <summary>
    /// Класс конвертации класса <see cref="TimeSpan?"/> в формат json и обратно
    /// </summary>
    internal class TimeSpanJsonConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken timespanToken = JToken.Load(reader);
            if (timespanToken.Type == JTokenType.Null)
                return null;

            JValue timespanValue = (JValue)timespanToken;

            return TimeSpan.ParseExact(timespanValue.Value<string>(),
                Program.TIME_FORMAT,
                CultureInfo.CurrentUICulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TimeSpan? nullableTimeSpanValue = value as TimeSpan?;
            if (!nullableTimeSpanValue.HasValue)
                writer.WriteNull();

            TimeSpan timeSpanValue = nullableTimeSpanValue.Value;

            writer.WriteValue(timeSpanValue.ToString(
                Program.TIME_FORMAT,
                CultureInfo.CurrentUICulture));
        }
    }
}
