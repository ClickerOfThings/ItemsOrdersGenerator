using System;

namespace ItemsOrdersGenerator.Attributes.Validation
{
    /// <summary>
    /// Атрибут проверки поля типа <see cref="int"/> на минимальное значение
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IntMinValidationAttribute : Attribute, IPropertyValidation
    {
        private readonly int minValueRequired;
        public string ErrorMessage { get => "Число меньше " + minValueRequired; }

        public IntMinValidationAttribute(int minValueRequired)
        {
            this.minValueRequired = minValueRequired;
        }

        public bool ValidateProperty(object value)
        {
            if (Convert.ToInt32(value) < minValueRequired)
                return false;
            return true;
        }
    }
}
