using ItemsOrdersGenerator.Classes.Extensions;
using ItemsOrdersGenerator.Classes.Helpers;
using ItemsOrdersGenerator.Classes.Model;
using ItemsOrdersGenerator.Classes.View;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ItemOrderDemonstration.Classes
{
    /// <summary>
    /// Класс конфигурации
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal partial class Config
    {
        #region ORDERS Configuration Properties
        /// <summary>
        /// Название города, в котором будет вестись поиск 
        /// (используется заместо координат прямоугольника)
        /// </summary>
        [JsonProperty]
        [Description("Название города, в котором будет вестись поиск " +
            "(используется заместо координат прямоугольника)")]
        public string CityName { get; set; }

        /// <summary>
        /// Прямоугольник, в котором будет вестись поиск мест
        /// </summary>
        [JsonProperty]
        [Description("Прямоугольник, в котором будет вестись поиск мест")]
        [CheckSearchRectangle]
        public SearchRectangle SearchRectangle { get; set; }

        /// <summary>
        /// Типы мест (OSM теги), которые будут искаться
        /// </summary>
        [JsonProperty]
        [Description("Типы мест, которые будут искаться. Под типами подразумеваются " +
            "OSM теги, которые определяют, какое предназначение имеет тот или иной объект. Например, " +
            "jewelry - магазин ювелирных изделий, shop - магазин, и т.д.")]
        [Format("Типы объектов, разделяемые через запятую или через запятую и пробел " +
            "(jewelry, shop, park, carpenter)")]
        public string PlaceTypes { get; set; }

        /// <summary>
        /// Путь к файлу с товарами, которые будут представлены в позициях
        /// </summary>
        [JsonProperty]
        [Description("Путь к файлу с товарами, которые будут представлены в позициях")]
        public string ItemsFilePathInput { get; set; }

        /// <summary>
        /// Дата заказов (не сериализуется)
        /// </summary>
        public DateTime? OrderDate;

        /// <summary>
        /// Представление даты заказов с форматированием (сериализуется)
        /// </summary>
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

        /// <summary>
        /// Количество используемых в генерации мест
        /// </summary>
        [JsonProperty]
        [Description("Количество используемых в генерации мест")]
        [CheckIntForMin(1)]
        public int? PointsCount { get; set; }

        /// <summary>
        /// Минимальное и максимальное количество временных окон
        /// </summary>
        [JsonProperty]
        [Description("Минимальное и максимальное количество временных окон")]
        [CheckIntTupleForMin(1)]
        public MinMaxTupleJson<int, int> MinMaxTimeWindows { get; set; }

        /// <summary>
        /// Минимальное и максимальное количество товаров в одном временном окне
        /// </summary>
        [JsonProperty]
        [Description("Минимальное и максимальное количество товаров в одном временном окне")]
        [CheckIntTupleForMin(1)]
        public MinMaxTupleJson<int, int> MinMaxItemsPerWindow { get; set; }

        /// <summary>
        /// Минимальное и максимальное количество товаров в одной позиции
        /// </summary>
        [JsonProperty]
        [Description("Минимальное и максимальное количество товаров в одной позиции")]
        [CheckIntTupleForMin(1)]
        public MinMaxTupleJson<int, int> MinMaxItemsCountPerPosition { get; set; }

        /// <summary>
        /// Промежуток времени от и до, в котором создаются позиции
        /// </summary>
        [JsonProperty]
        [Description("Промежуток времени от и до, в котором создаются позиции")]
        [Format("чч:мм. Секунды не учитываются в работе программы")]
        [JsonConverter(typeof(TimeSpanRangeTupleConverter))]
        [CheckTimeSpanTupleForMin(0, 0)]
        public Tuple<TimeSpan, TimeSpan> TimeRange { get; set; }

        /// <summary>
        /// Интервал между минимальным и максимальным временем (не сериализуется)
        /// </summary>
        [CheckTimeSpanForMin(0, 0)]
        public TimeSpan? IntervalBetweenTimeRange { get; set; }

        /// <summary>
        /// Интервал между минимальным и максимальным временем с форматированием (сериализуется)
        /// </summary>
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

        /// <summary>
        /// Путь выходного файла с заказами
        /// </summary>
        [JsonProperty]
        [Description("Путь выходного файла с заказами")]
        public string OrdersFilePathOutput { get; set; }
        #endregion

        #region ITEMS Configuration Properties
        /// <summary>
        /// Путь TXT-файла с товарами, используемого для создания XML-файла товаров
        /// </summary>
        [JsonProperty]
        [Description("Путь TXT-файла с товарами, используемого для создания XML-файла товаров")]
        [Format("Один товар указывается на одну строку. " +
            "Вводятся три переменные, разделяемые точкой с запятой (;): " +
                            "Название товара, количество товара в 1 упаковке, вес в килограммах (допускается " +
                            "ввод десятичного числа, например 0.100)")]
        public string TxtItemFilePathInput { get; set; }

        /// <summary>
        /// Путь выходного XML-файла с товарами
        /// </summary>
        [JsonProperty]
        [Description("Путь выходного XML-файла с товарами")]
        public string XmlItemFilePathOutput { get; set; }
        #endregion

        /// <summary>
        /// Конструктор пустого объекта конфигурации
        /// </summary>
        public Config() { }

        /// <summary>
        /// Создать объект конфигурации из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу в формате json </param>
        /// <returns>Объект конфигурации с заполненными полями из файла</returns>
        /// <exception cref="BadConfigException">Одно из полей не прошло проверку</exception>
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
                    if (attribute is IPropertyValidation validation)
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

        /// <summary>
        /// Сериализовать объект конфигурации в файл формата json
        /// </summary>
        /// <param name="filePath">Путь к выходному файлу</param>
        public void WriteIntoJson(string filePath)
        {
            JsonSerializer serializer = JsonSerializer.Create();
            using StreamWriter writer = new StreamWriter(File.Create(filePath));
            using JsonTextWriter Jwriter = new JsonTextWriter(writer) { Formatting = Newtonsoft.Json.Formatting.Indented };
            serializer.Serialize(Jwriter, this);
        }
    }

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
                return;

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

    /// <summary>
    /// Атрибут для описания формата поля или файла, который считывается для вставки значения в поле. 
    /// Используется в методе вывода помощи по полям конфигурации
    /// </summary>
    public class FormatAttribute : Attribute
    {
        public string FormatDescription { get; set; }
        public FormatAttribute(string formatDescription)
        {
            FormatDescription = formatDescription;
        }
    }

    /// <summary>
    /// Исключение неправильной конфигурации, либо во время считывания, либо во время непосредственного 
    /// использования
    /// </summary>
    public class BadConfigException : Exception
    {
        public BadConfigException(string message) : base(message) { }
    }

    /// <summary>
    /// Атрибут проверки поля типа <see cref="Int32"/> на минимальное значение
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckIntForMin : Attribute, IPropertyValidation
    {
        private readonly int minRequired;
        public string ErrorMessage { get => "Число меньше " + minRequired; }

        public CheckIntForMin(int minRequired)
        {
            this.minRequired = minRequired;
        }

        public bool ValidateProperty(object value)
        {
            if (Convert.ToInt32(value) < minRequired)
                return false;
            return true;
        }
    }

    /// <summary>
    /// Атрибут проверки поля типа кортежа с двумя <see cref="int"/> на минимальное значение
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckIntTupleForMin : Attribute, IPropertyValidation
    {
        private readonly int minRequired;
        public string ErrorMessage { get => "Одно из чисел меньше " + minRequired; }

        public CheckIntTupleForMin(int minRequired)
        {
            this.minRequired = minRequired;
        }

        public bool ValidateProperty(object value)
        {
            Tuple<int, int> tuple = value as Tuple<int, int>;
            if (tuple is null)
                throw new ArgumentException("Свойство с атрибутом " + nameof(CheckIntTupleForMin)
                    + " не является типом " + typeof(Tuple<int, int>).Name);

            if (tuple.Item1 < minRequired || tuple.Item2 < minRequired)
                return false;
            return true;
        }
    }

    /// <summary>
    /// Атрибут проверки поля типа <see cref="TimeSpan"/> на минимальное время
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckTimeSpanForMin : Attribute, IPropertyValidation
    {
        private readonly TimeSpan minRequired;
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

    /// <summary>
    /// Атрибут проверки поля типа кортежа с двумя <see cref="TimeSpan"/> на минимальное время
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckTimeSpanTupleForMin : Attribute, IPropertyValidation
    {
        private readonly TimeSpan minRequired;
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

    /// <summary>
    /// Атрибут проверки поля типа <see cref="SearchRectangle"/> на корректно введёные данные 
    /// (северо-восточный угол должен быть больше юго-западного угла как в X, так и в Y)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CheckSearchRectangle : Attribute, IPropertyValidation
    {
        string IPropertyValidation.ErrorMessage =>
            "Северо-восточный угол должен быть больше юго-западного угла как в X, так и в Y";

        bool IPropertyValidation.ValidateProperty(object value)
        {
            SearchRectangle rect = value as SearchRectangle;
            if (rect is null)
                throw new ArgumentException("Свойство с атрибутом " + nameof(CheckSearchRectangle)
                    + " не является типом " + typeof(SearchRectangle).Name);

            if (rect.NorthEastCorner.CompareTo(rect.SouthWestCorner) != 1)
                return false;
            return true;
        }
    }

    /// <summary>
    /// Интерфейс проверки поля. Используется для атрибутов полей во время создания конфигурационного файла
    /// </summary>
    public interface IPropertyValidation
    {
        /// <summary>
        /// Провести проверку поля
        /// </summary>
        /// <param name="value">Значение поля</param>
        /// <returns>true если поле имеет корректное значение, false если нет</returns>
        public bool ValidateProperty(object value);

        /// <summary>
        /// Сообщение об ошибке в случае некорректного значения поля во время проверки
        /// </summary>
        public string ErrorMessage { get; }
    }
}
