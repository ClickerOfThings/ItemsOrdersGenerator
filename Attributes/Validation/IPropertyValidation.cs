namespace ItemsOrdersGenerator.Attributes.Validation
{
    /// <summary>
    /// Интерфейс проверки поля. Используется для атрибутов полей во время создания конфигурационного файла
    /// </summary>
    public interface IPropertyValidation
    {
        /// <summary>
        /// Провести проверку поля
        /// </summary>
        /// <param name="value">Значение поля</param>
        /// <returns>true если поле имеет корректное значение, false если нет</returns>
        public bool ValidateProperty(object value);

        /// <summary>
        /// Сообщение об ошибке в случае некорректного значения поля во время проверки
        /// </summary>
        public string ErrorMessage { get; }
    }
}
