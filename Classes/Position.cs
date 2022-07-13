using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemOrderDemonstration.Classes
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
