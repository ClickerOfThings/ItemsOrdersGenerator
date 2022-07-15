using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
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
