using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Model
{
    /// <summary>
    /// Класс запроса в заказе
    /// </summary>
    [XmlType("demand")]
    public class Demand
    {
        /// <summary>
        /// Начало время запроса (не сериализируется в xml файле)
        /// </summary>
        [XmlIgnore]
        public TimeSpan From { get; set; }

        /// <summary>
        /// View-свойство начала времени запроса, который сериализируется 
        /// в xml файл с форматированием из метода <see cref="FormatTimeSpanValues(TimeSpan)"/>
        /// </summary>
        /// <remarks>Зачем это свойство нужно:
        /// Сериализатор XML не умеет сериализовать сложные типы данных (свои написанные классы), 
        /// только те, которые он сам поддерживает 
        /// (https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlattributeattribute.datatype?view=net-6.0#remarks). 
        /// Если попытаться реализовать интерфейс <see cref="IXmlSerializable"/> в классе и присвоить свойству класса 
        /// атрибут <see cref="XmlAttributeAttribute"/>, будет генерироваться исключение о том, что сериализатор 
        /// не может этого сделать 
        /// (https://stackoverflow.com/a/17848048)</remarks>
        [XmlAttribute("from")]
        public string FromView
        {
            get
            {
                return FormatTimeSpanValues(From);
            }
            set
            {
                From = TimeSpan.Parse(value);
            }
        }

        /// <summary>
        /// Окончание время запроса (не сериализируется в xml файле)
        /// </summary>
        [XmlIgnore]
        public TimeSpan To { get; set; }

        /// <summary>
        /// View-свойство конца времени запроса, который сериализируется 
        /// в xml файл с форматированием из метода <see cref="FormatTimeSpanValues(TimeSpan)"/>
        /// </summary>
        /// <remarks>Зачем это свойство нужно:
        /// Сериализатор XML не умеет сериализовать сложные типы данных (свои написанные классы), 
        /// только те, которые он сам поддерживает 
        /// (https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlattributeattribute.datatype?view=net-6.0#remarks). 
        /// Если попытаться реализовать интерфейс <see cref="IXmlSerializable"/> в классе и присвоить свойству класса 
        /// атрибут <see cref="XmlAttributeAttribute"/>, будет генерироваться исключение о том, что сериализатор 
        /// не может этого сделать 
        /// (https://stackoverflow.com/a/17848048)</remarks>
        [XmlAttribute("to")]
        public string ToView
        {
            get
            {
                return FormatTimeSpanValues(To);
            }
            set
            {
                To = TimeSpan.Parse(value);
            }
        }

        /// <summary>
        /// Позиция товара
        /// </summary>
        [XmlElement("position")]
        public Position Position { get; set; }

        /// <summary>
        /// Форматирование значений из TimeSpan для сериализации в xml файл заказов
        /// </summary>
        /// <param name="tsToProcess">Объект TimeSpan, который необходимо отформатировать</param>
        /// <returns>Отформатированная строка из объекта TimeSpan</returns>
        /// <remarks>
        /// <para>
        /// Форматирование времени: если <paramref name="tsToProcess"/> пустой, то пустая строка;
        /// </para>
        /// <para>
        /// если <paramref name="tsToProcess"/> имеет только час, то час;
        /// </para>
        /// <para>
        /// если <paramref name="tsToProcess"/> имеет и час и минуты, то в формате чч:мм (если час меньше десяти, то 
        /// выводится одна цифра часа).
        /// </para>
        /// </remarks>
        private string FormatTimeSpanValues(TimeSpan tsToProcess)
        {
            if (tsToProcess.Hours == 0 && tsToProcess.Minutes == 0)
                return string.Empty;
            if (tsToProcess.Minutes == 0)
                return tsToProcess.Hours.ToString();
            else
            {
                StringBuilder resultTimeFormatted = new StringBuilder(string.Format("{0:00}", tsToProcess.Hours));
                resultTimeFormatted.Append(":");
                resultTimeFormatted.Append(string.Format("{0:00}", tsToProcess.Minutes));
                return resultTimeFormatted.ToString();
            }
        }
    }
}
