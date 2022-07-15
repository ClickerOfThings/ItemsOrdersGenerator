using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    [XmlType(TypeName = "sku")]
    public class Item
    {
        [XmlAttribute(AttributeName = "id")]
        public Guid Id { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "amountPerTray")]
        public int AmountPerTray { get; set; }
        [XmlElement(ElementName = "weight")]
        public float Weight { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
