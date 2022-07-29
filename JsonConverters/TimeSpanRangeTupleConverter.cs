using ItemOrderDemonstration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ItemsOrdersGenerator.JsonConverters
{
    /// <summary>
    /// Класс для конвертации промежутка времени от и до с возможностью 
    /// указать "24:00" в качестве конечного времени
    /// </summary>
    internal class TimeSpanRangeTupleConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Tuple<TimeSpan, TimeSpan>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken tupleToken = JToken.Load(reader);
            if (tupleToken.Type == JTokenType.Null)
                return null;

            JObject tupleObj = (JObject)tupleToken;
            string fromStr = tupleObj.Value<string>("From");
            string toStr = tupleObj.Value<string>("To");
            if (toStr == "24:00")
                toStr = "01:00:00:00";

            return new Tuple<TimeSpan, TimeSpan>(
                TimeSpan.Parse(fromStr),
                TimeSpan.Parse(toStr));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Tuple<TimeSpan, TimeSpan> tuple = value as Tuple<TimeSpan, TimeSpan>;
            if (tuple is null)
                writer.WriteNull();

            JObject resultTupleJObj = new JObject
            {
                { "From", tuple.Item1.ToString(Program.TIME_FORMAT) },
                { "To", tuple.Item2.Days == 1 ? "24:00" : tuple.Item2.ToString(Program.TIME_FORMAT) }
            };

            resultTupleJObj.WriteTo(writer);
            return;
        }
    }
}
