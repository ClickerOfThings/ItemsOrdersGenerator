using System;

namespace ItemsOrdersGenerator.Attributes.Validation
{
    /// <summary>
    /// Атрибут проверки поля типа <see cref="TimeSpan"/> на минимальное время
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TimeSpanMinValidationAttribute : Attribute, IPropertyValidation
    {
        private readonly TimeSpan minValueRequired;
        public string ErrorMessage { get => "Указанное время меньше " + minValueRequired; }

        public TimeSpanMinValidationAttribute(int minHours, int minMinutes)
        {
            minValueRequired = new TimeSpan(minHours, minMinutes, 0);
        }

        public bool ValidateProperty(object value)
        {
            if ((TimeSpan)value < minValueRequired)
                return false;
            return true;
        }
    }
}
