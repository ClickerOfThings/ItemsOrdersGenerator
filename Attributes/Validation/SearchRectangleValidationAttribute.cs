using ItemsOrdersGenerator.Extensions;
using ItemsOrdersGenerator.Model;
using System;

namespace ItemsOrdersGenerator.Attributes.Validation
{
    /// <summary>
    /// Атрибут проверки поля типа <see cref="SearchRectangle"/> на корректно введёные данные 
    /// (северо-восточный угол должен быть больше юго-западного угла как в X, так и в Y)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SearchRectangleValidationAttribute : Attribute, IPropertyValidation
    {
        string IPropertyValidation.ErrorMessage =>
            "Северо-восточный угол должен быть больше юго-западного угла как в X, так и в Y";

        bool IPropertyValidation.ValidateProperty(object value)
        {
            SearchRectangle rect = value as SearchRectangle;
            if (rect is null)
                throw new ArgumentException("Свойство с атрибутом " + nameof(SearchRectangleValidationAttribute)
                    + " не является типом " + typeof(SearchRectangle).Name);

            if (rect.NorthEastCorner.CompareTo(rect.SouthWestCorner) != 1)
                return false;
            return true;
        }
    }
}
