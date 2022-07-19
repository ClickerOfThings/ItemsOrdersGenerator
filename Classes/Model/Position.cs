using System;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    /// <summary>
    /// Класс позиции в запросе
    /// </summary>
    [XmlType("position")]
    public class Position
    {
        /// <summary>
        /// GUID товара в позиции
        /// </summary>
        [XmlElement("sku")]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Количество товара <see cref="ItemId"/>
        /// </summary>
        [XmlElement("amount")]
        public int Amount { get; set; }
    }
}
