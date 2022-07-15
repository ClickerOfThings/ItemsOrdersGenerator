using ItemsOrdersGenerator.Classes.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.View
{
    /// <summary>
    /// View класс списка заказов
    /// </summary>
    [XmlType("orders")]
    public class OrdersListView
    {
        /// <summary>
        /// Дата списка заказов (не используется при сериализации)
        /// </summary>
        [XmlIgnore]
        public DateTime DateOfOrders;
        /// <summary>
        /// View-свойство объекта <see cref="DateOfOrders"/>, который сериализируется с форматированием
        /// </summary>
        
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
        /// <summary>
        /// Список заказов
        /// </summary>
        [XmlElement("order")]
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
