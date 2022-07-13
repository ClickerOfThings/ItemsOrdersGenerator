using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemOrderDemonstration.Classes
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
                DateOfOrders = HelperClass.ParseDateTimeFromSystemCulture(value);
            }
        }
        [XmlIgnore]
        public DateTime DateOfOrders;
        [XmlElement("order")]
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
