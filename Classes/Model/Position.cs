using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    [XmlType("position")]
    public class Position
    {
        [XmlElement("sku")]
        public Guid ItemId { get; set; }
        [XmlElement("amount")]
        public int Amount { get; set; }
    }
}
