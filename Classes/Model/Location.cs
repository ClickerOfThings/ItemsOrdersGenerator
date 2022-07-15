using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    /// <summary>
    /// Класс позиции на карте в виде координат
    /// </summary>
    [XmlType("location")]
    public class Location
    {
        /// <summary>
        /// Широта позиции
        /// </summary>
        [XmlElement("latitude")]
        public double Latitude { get; set; }
        /// <summary>
        /// Долгота позиции
        /// </summary>
        [XmlElement("longitude")]
        public double Longitude { get; set; }
    }
}
