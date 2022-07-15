using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    /// <summary>
    /// Класс товара
    /// </summary>
    [XmlType(TypeName = "sku")]
    public class Item
    {
        /// <summary>
        /// GUID товара
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Наименование товара
        /// </summary>
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        /// <summary>
        /// Количество товара в одной штуке
        /// </summary>
        [XmlElement(ElementName = "amountPerTray")]
        public int AmountPerTray { get; set; }
        /// <summary>
        /// Вес товара в килограммах
        /// </summary>
        [XmlElement(ElementName = "weight")]
        public float Weight { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
