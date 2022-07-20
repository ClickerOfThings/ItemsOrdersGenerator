using System;

namespace ItemsOrdersGenerator.Attributes.Validation
{
    /// <summary>
    /// Атрибут проверки поля типа кортежа с двумя <see cref="int"/> на минимальное значение
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IntTupleMinValidationAttribute : Attribute, IPropertyValidation
    {
        private readonly int minValueRequired;
        public string ErrorMessage { get => "Одно из чисел меньше " + minValueRequired; }

        public IntTupleMinValidationAttribute(int minValueRequired)
        {
            this.minValueRequired = minValueRequired;
        }

        public bool ValidateProperty(object value)
        {
            Tuple<int, int> tuple = value as Tuple<int, int>;
            if (tuple is null)
                throw new ArgumentException("Свойство с атрибутом " + nameof(IntTupleMinValidationAttribute)
                    + " не является типом " + typeof(Tuple<int, int>).Name);

            if (tuple.Item1 < minValueRequired || tuple.Item2 < minValueRequired)
                return false;
            return true;
        }
    }
}
