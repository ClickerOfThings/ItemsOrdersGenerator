using ItemsOrdersGenerator.Classes.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.View
{
    [XmlType("orders")]
    public class OrdersListView
    {
        [XmlAttribute("date")]
        public string DateOfOrdersView
        {
            get
            {
                return DateOfOrders.ToString("yyyy-MM-dd");
            }
            set
            {
                DateOfOrders = Helpers.ParseHelper.ParseDateTimeFromSystemCulture(value);
            }
        }
        [XmlIgnore]
        public DateTime DateOfOrders;
        [XmlElement("order")]
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
