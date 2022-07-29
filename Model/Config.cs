using ItemsOrdersGenerator.Attributes;
using ItemsOrdersGenerator.Attributes.Validation;
using ItemsOrdersGenerator.JsonConverters;
using ItemsOrdersGenerator.View;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;

namespace ItemsOrdersGenerator.Model
{
    /// <summary>
    /// Класс конфигурации
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class Config
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
        [SearchRectangleValidation]
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
        /// Дата заказов
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime? OrderDate;

        /// <summary>
        /// Количество используемых в генерации мест
        /// </summary>
        [JsonProperty]
        [Description("Количество используемых в генерации мест")]
        [IntMinValidation(1)]
        public int? PointsCount { get; set; }

        /// <summary>
        /// Минимальное и максимальное количество временных окон
        /// </summary>
        [JsonProperty]
        [Description("Минимальное и максимальное количество временных окон")]
        [IntTupleMinValidation(1)]
        public MinMaxTupleJson<int, int> MinMaxTimeWindows { get; set; }

        /// <summary>
        /// Минимальное и максимальное количество товаров в одном временном окне
        /// </summary>
        [JsonProperty]
        [Description("Минимальное и максимальное количество товаров в одном временном окне")]
        [IntTupleMinValidation(1)]
        public MinMaxTupleJson<int, int> MinMaxItemsPerWindow { get; set; }

        /// <summary>
        /// Минимальное и максимальное количество товаров в одной позиции
        /// </summary>
        [JsonProperty]
        [Description("Минимальное и максимальное количество товаров в одной позиции")]
        [IntTupleMinValidation(1)]
        public MinMaxTupleJson<int, int> MinMaxItemsCountPerPosition { get; set; }

        /// <summary>
        /// Промежуток времени от и до, в котором создаются позиции
        /// </summary>
        [JsonProperty]
        [Description("Промежуток времени от и до, в котором создаются позиции")]
        [Format("чч:мм. Секунды не учитываются в работе программы. Возможен ввод '24:00' для обозначения конца дня")]
        [JsonConverter(typeof(TimeSpanRangeTupleConverter))]
        [TimeSpanTupleMinValidation(0, 0)]
        public Tuple<TimeSpan, TimeSpan> TimeRange { get; set; }

        /// <summary>
        /// Интервал между минимальным и максимальным временем
        /// </summary>
        [JsonProperty]
        [TimeSpanMinValidation(0, 0)]
        [Description("Интервал между минимальным и максимальным временем")]
        [Format("чч:мм. Секунды не учитываются в работе программы")]
        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public TimeSpan? IntervalBetweenTimeRange { get; set; }

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
        /// Сериализовать объект конфигурации в файл формата json
        /// </summary>
        /// <param name="filePath">Путь к выходному файлу</param>
        public void WriteIntoJson(string filePath)
        {
            JsonSerializer serializer = JsonSerializer.Create();
            using StreamWriter writer = new StreamWriter(File.Create(filePath));
            using JsonTextWriter Jwriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented };
            serializer.Serialize(Jwriter, this);
        }
    }
}
