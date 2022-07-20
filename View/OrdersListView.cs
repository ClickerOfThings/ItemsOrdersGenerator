using ItemsOrdersGenerator.Model;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.View
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
        /// <remarks>Зачем это свойство нужно:
        /// Сериализатор XML не умеет сериализовать сложные типы данных (свои написанные классы), 
        /// только те, которые он сам поддерживает 
        /// (https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlattributeattribute.datatype?view=net-6.0#remarks). 
        /// Если попытаться реализовать интерфейс <see cref="IXmlSerializable"/> в классе и присвоить свойству класса 
        /// атрибут <see cref="XmlAttributeAttribute"/>, будет генерироваться исключение о том, что сериализатор 
        /// не может этого сделать 
        /// (https://stackoverflow.com/a/17848048)</remarks>
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
