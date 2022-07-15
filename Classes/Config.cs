using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Reflection;
using ItemsOrdersGenerator.Classes.Model;
using ItemsOrdersGenerator.Classes.Helpers;
using ItemsOrdersGenerator.Classes.View;

namespace ItemOrderDemonstration.Classes
{
    /// <summary>
    /// Класс конфигурации
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal partial class Config
    {
        /// <summary>
        /// Создать пустой объект конфигурации
        /// </summary>
        public Config()
        {

        }
        public static Config ReadFromFile(string filePath)
        {
            Config resultConfig;
            JsonSerializer serializer = JsonSerializer.Create();
            using (StreamReader reader = new StreamReader(File.Open(filePath, FileMode.Open)))
            {
                resultConfig = serializer.Deserialize(reader, typeof(Config)) as Config;
            }
            foreach (PropertyInfo property in resultConfig.GetType().GetProperties())
            {
                if (property.GetValue(resultConfig) is null)
                    continue;
                foreach (Attribute attribute in property.GetCustomAttributes())
                {
                    IPropertyValidation validation = attribute as IPropertyValidation;
                    if (validation != null)
                    {
                        if (!validation.ValidateProperty(property.GetValue(resultConfig)))
                        {
                            throw new BadConfigException("Поле " + property.Name + " не прошло проверку: "
                                + validation.ErrorMessage);
                        }
                    }
                }
            }
            return resultConfig;
        }
        public void WriteIntoJson(string filePath)
        {
            JsonSerializer serializer = JsonSerializer.Create();
            using (StreamWriter writer = new StreamWriter(File.Create(filePath)))
            using (JsonTextWriter Jwriter = new JsonTextWriter(writer) { Formatting = Newtonsoft.Json.Formatting.Indented })
            {
                serializer.Serialize(Jwriter, this);
            }
        }

        #region ORDERS Configuration Properties
        [JsonProperty]
        [Description("Название города, в котором будет вестись поиск " +
            "(используется заместо координат прямоугольника)")]
        public string CityName { get; set; }
        [JsonProperty]
        [Description("Прямоугольник, в котором будет вестись поиск точек")]
        public SearchRectangle SearchRectangle { get; set; }
        [JsonProperty]
        [Description("Типы точек, которые будут искаться. Под типами подразумеваются " +
            "OSM теги, которые определяют, какое предназначение имеет тот или иной объект. Например, " +
            "jewelry - магазин ювелирных изделий, shop - магазин, и т.д.")]
        [Format("Типы объектов, разделяемые через запятую или через запятую и пробел " +
            "(jewelry, shop, park, carpenter)")]
        public string PlaceTypes { get; set; }
        [JsonProperty]
        [Description("Путь к файлу с товарами, которые будут представлены в позициях")]
        public string ItemsFilePathInput { get; set; }
        public DateTime? OrderDate;
        [JsonProperty(PropertyName = "OrderDate")]
        [Description("Дата заказов")]
        public string OrderDateView
        {
            get
            {
                if (OrderDate.HasValue)
                    return OrderDate.Value.ToString(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat.ShortDatePattern);
                else
                    return string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    OrderDate = null;
                else
                    OrderDate = ParseHelper.ParseDateTimeFromSystemCulture(value).Date;
            }
        }
        //private int _pointsCount;
        [JsonProperty]
        [Description("Количество используемых в генерации точек")]
        [CheckNumberForMin(1)]
        public int? PointsCount { get; set; }
        [JsonProperty]
        [Description("Минимальное и максимальное количество временных окон")]
        [CheckNumberTupleForMin(1)]
        public MinMaxTupleJson<int, int> MinMaxTimeWindows { get; set; }
        [JsonProperty]
        [Description("Минимальное и максимальное количество товаров в одном временном окне, " +
            "используемое в случайной генерации")]
        [CheckNumberTupleForMin(1)]
        public MinMaxTupleJson<int, int> MinMaxItemsPerWindow { get; set; }
        [JsonProperty]
        [Description("Минимальное и максимальное количество товаров в одной позиции, " +
            "используемое в случайной генерации")]
        [CheckNumberTupleForMin(1)]
        public MinMaxTupleJson<int, int> MinMaxItemsCountPerPosition { get; set; }
        [JsonProperty]
        [Description("Промежуток времени от и до, в котором создаются заказы")]
        [Format("чч:мм. Секунды не учитываются в работе программы")]
        [JsonConverter(typeof(TimeSpanTupleConverter))]
        [CheckTimeSpanTupleForMin(0, 0)]
        public Tuple<TimeSpan, TimeSpan> TimeRange { get; set; }
        [CheckTimeSpanForMin(0, 0)]
        public TimeSpan? IntervalBetweenTimeRange { get; set; }
        [JsonProperty(PropertyName = "IntervalBetweenTimeRange")]
        [Description("Интервал между минимальным и максимальным временем")]
        public string IntervalBetweenTimeRangeView
        {
            get => IntervalBetweenTimeRange?.ToString(Program.TIME_FORMAT) ?? "";
            set
            {
                TimeSpan temp = TimeSpan.Parse(value);
                IntervalBetweenTimeRange = new TimeSpan(temp.Hours, temp.Minutes, 0);
            }
        }
        [JsonProperty]
        [Description("Путь выходного файла с заказами")]
        public string OrdersFilePathOutput { get; set; }
        #endregion

        #region ITEMS Configuration Properties
        [JsonProperty]
        [Description("Путь TXT-файла с товарами, используемые для создания XML-файла товаров")]
        [Format("Один товар указывается на одну строку. " +
            "Вводятся три переменные, разделяемые точкой с запятой (;): " +
                            "Название товара, количество товара в 1 упаковке, вес в килограммах (допускается " +
                            "ввод десятичного числа, например 0.100)")]
        public string TxtItemFilePathInput { get; set; }
        [JsonProperty]
        [Description("Путь выходного XML-файла с товарами")]
        public string XmlItemFilePathOutput { get; set; }
        #endregion
    }

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
                return;
            PointF point = (value as PointF?).Value;
            JObject obj = new JObject();
            obj.Add("X", point.X);
            obj.Add("Y", point.Y);
            obj.WriteTo(writer);
            return;
        }
    }
    internal class TimeSpanTupleConverter : JsonConverter
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
            JObject obj = new JObject();
            obj.Add("From", tuple.Item1.ToString(Program.TIME_FORMAT));
            obj.Add("To", tuple.Item2.Days == 1 ? "24:00" : tuple.Item2.ToString(Program.TIME_FORMAT));
            obj.WriteTo(writer);
            return;
        }
    }

    public class FormatAttribute : Attribute
    {
        public string FormatDescription { get; set; }
        public FormatAttribute(string formatDescription)
        {
            FormatDescription = formatDescription;
        }
    }

    public class BadConfigException : Exception
    {
        public BadConfigException(string message) : base(message)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckNumberForMin : Attribute, IPropertyValidation
    {
        private int minRequired;
        public string ErrorMessage { get => "Число меньше " + minRequired; }
        public CheckNumberForMin(int minRequired)
        {
            this.minRequired = minRequired;
        }

        public bool ValidateProperty(object value)
        {
            if (Convert.ToInt64(value) < minRequired)
                return false;
            return true;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckNumberTupleForMin : Attribute, IPropertyValidation
    {
        private int minRequired;
        public string ErrorMessage { get => "Одно из чисел меньше " + minRequired; }
        public CheckNumberTupleForMin(int minRequired)
        {
            this.minRequired = minRequired;
        }
        public bool ValidateProperty(object value)
        {
            Tuple<int, int> tuple = value as Tuple<int, int>;
            if (tuple is null)
                throw new ArgumentException("Свойство с атрибутом " + nameof(CheckNumberTupleForMin)
                    + " не является типом " + typeof(Tuple<int, int>).Name);
            if (tuple.Item1 < minRequired || tuple.Item2 < minRequired)
                return false;
            return true;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckTimeSpanForMin : Attribute, IPropertyValidation
    {
        private TimeSpan minRequired;
        public string ErrorMessage { get => "Указанное время меньше " + minRequired; }
        public CheckTimeSpanForMin(int minHours, int minMinutes)
        {
            this.minRequired = new TimeSpan(minHours, minMinutes, 0);
        }

        public bool ValidateProperty(object value)
        {
            if (((TimeSpan)value) < minRequired)
                return false;
            return true;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckTimeSpanTupleForMin : Attribute, IPropertyValidation
    {
        private TimeSpan minRequired;
        public string ErrorMessage { get => "Одно из указанных времён меньше " + minRequired; }
        public CheckTimeSpanTupleForMin(int minHours, int minMinutes)
        {
            this.minRequired = new TimeSpan(minHours, minMinutes, 0);
        }

        public bool ValidateProperty(object value)
        {
            Tuple<TimeSpan, TimeSpan> tuple = value as Tuple<TimeSpan, TimeSpan>;
            if (value is null)
                throw new ArgumentException("Свойство с атрибутом " + nameof(CheckTimeSpanTupleForMin)
                    + " не является типом " + typeof(Tuple<TimeSpan, TimeSpan>).Name);
            if (tuple.Item1 < minRequired || tuple.Item2 < minRequired)
                return false;
            return true;
        }
    }

    public interface IPropertyValidation
    {
        public bool ValidateProperty(object value);
        public string ErrorMessage { get; }
    }
}
