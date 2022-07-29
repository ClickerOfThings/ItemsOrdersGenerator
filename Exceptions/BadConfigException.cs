using System;

namespace ItemsOrdersGenerator.Exceptions
{
    /// <summary>
    /// Исключение неправильной конфигурации, либо во время считывания, либо во время непосредственного 
    /// использования
    /// </summary>
    public class BadConfigException : Exception
    {
        public BadConfigException(string message) : base(message) { }
    }
}
