using System.Collections.Generic;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Model
{
    /// <summary>
    /// Класс заказа
    /// </summary>
    [XmlType("order")]
    public class Order
    {
        /// <summary>
        /// Точка, в которой заказ будет принят
        /// </summary>
        [XmlElement("retailPoint")]
        public RetailPoint RetailPoint { get; set; }

        /// <summary>
        /// Запросы в заказе
        /// </summary>
        [XmlArray("demands")]
        public List<Demand> Demands { get; set; }
    }
}
