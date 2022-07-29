using System;

namespace ItemsOrdersGenerator.Attributes.Validation
{
    /// <summary>
    /// Атрибут проверки поля типа кортежа с двумя <see cref="TimeSpan"/> на минимальное время
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TimeSpanTupleMinValidationAttribute : Attribute, IPropertyValidation
    {
        private readonly TimeSpan minValueRequired;
        public string ErrorMessage { get => "Одно из указанных времён меньше " + minValueRequired; }

        public TimeSpanTupleMinValidationAttribute(int minHours, int minMinutes)
        {
            minValueRequired = new TimeSpan(minHours, minMinutes, 0);
        }

        public bool ValidateProperty(object value)
        {
            Tuple<TimeSpan, TimeSpan> tuple = value as Tuple<TimeSpan, TimeSpan>;
            if (value is null)
                throw new ArgumentException("Свойство с атрибутом " + nameof(TimeSpanTupleMinValidationAttribute)
                    + " не является типом " + typeof(Tuple<TimeSpan, TimeSpan>).Name);

            if (tuple.Item1 < minValueRequired || tuple.Item2 < minValueRequired)
                return false;
            return true;
        }
    }
}
