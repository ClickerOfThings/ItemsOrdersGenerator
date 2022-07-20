using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace ItemsOrdersGenerator.JsonConverters
{
    /// <summary>
    /// Класс конвертации класса <see cref="DateTime?"/> в формат json и обратно
    /// </summary>
    internal class DateTimeJsonConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken datetimeToken = JToken.Load(reader);
            if (datetimeToken.Type == JTokenType.Null)
                return null;

            JValue datetimeObject = (JValue)datetimeToken;

            return DateTime.ParseExact(datetimeObject.Value<string>(), 
                CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern, 
                CultureInfo.CurrentUICulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime? nullableDateTimeValue = value as DateTime?;
            if (!nullableDateTimeValue.HasValue)
                writer.WriteNull();

            DateTime dateTimeValue = nullableDateTimeValue.Value;

            writer.WriteValue(dateTimeValue.ToString(
                CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern,
                CultureInfo.CurrentUICulture));
        }
    }
}
