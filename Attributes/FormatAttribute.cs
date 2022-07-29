using System;

namespace ItemsOrdersGenerator.Attributes
{
    /// <summary>
    /// Атрибут для описания формата поля или файла, который считывается для вставки значения в поле. 
    /// Используется в методе вывода помощи по полям конфигурации
    /// </summary>
    public class FormatAttribute : Attribute
    {
        public string FormatDescription { get; set; }
        public FormatAttribute(string formatDescription)
        {
            FormatDescription = formatDescription;
        }
    }
}
