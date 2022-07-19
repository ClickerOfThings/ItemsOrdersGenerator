#define RETAIL_GUID
//#define RETAIL_REGULAR_ID

using System;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    /// <summary>
    /// Класс точки
    /// </summary>
    [XmlType("retailPoint")]
    public class RetailPoint
    {
#if RETAIL_REGULAR_ID
        /// <summary>
        /// ID точки из базы данных OSM
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public long Id { get; set; }
#endif
#if RETAIL_GUID
        /// <summary>
        /// GUID точки
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public Guid Id { get; set; }
#endif

        /// <summary>
        /// Наименование точки
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Полный адрес точки
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// Позиция точки на карте в виде координат
        /// </summary>
        [XmlElement("location")]
        public Location Location { get; set; }
    }
}
