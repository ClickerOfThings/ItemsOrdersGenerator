#define RETAIL_GUID
//#define RETAIL_REGULAR_ID

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    [XmlType("retailPoint")]
    public class RetailPoint
    {
#if RETAIL_REGULAR_ID
        [XmlAttribute(AttributeName = "id")]
        public long Id { get; set; }
#endif
#if RETAIL_GUID
        [XmlAttribute(AttributeName = "id")]
        public Guid Id { get; set; }
#endif
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("address")]
        public string Address { get; set; }
        [XmlElement("location")]
        public Location Location { get; set; }
    }
    [XmlType("location")]
    public class Location
    {
        [XmlElement("latitude")]
        public double Latitude { get; set; }
        [XmlElement("longitude")]
        public double Longitude { get; set; }
    }
}
