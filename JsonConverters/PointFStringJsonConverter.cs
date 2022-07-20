using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;

namespace ItemsOrdersGenerator.JsonConverters
{
    /// <summary>
    /// Класс конвертации класса <see cref="PointF"/> в формат json и обратно
    /// </summary>
    internal class PointFStringJsonConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PointF);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken pointToken = JToken.Load(reader);
            if (pointToken.Type == JTokenType.Null)
                return null;

            JObject pointObj = (JObject)pointToken;
            return new PointF(pointObj.Value<float>("X"), pointObj.Value<float>("Y"));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value as PointF?).HasValue)
                writer.WriteNull();

            PointF point = (value as PointF?).Value;
            JObject resultPointJObj = new JObject
            {
                { "X", point.X },
                { "Y", point.Y }
            };

            resultPointJObj.WriteTo(writer);
            return;
        }
    }
}
