using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemOrderDemonstration.Classes
{
    [XmlType("order")]
    public class Order
    {
        [XmlElement("retailPoint")]
        public RetailPoint RetailPoint { get; set; }
        [XmlArray("demands")]
        public List<Demand> Demands { get; set; }
    }
}
