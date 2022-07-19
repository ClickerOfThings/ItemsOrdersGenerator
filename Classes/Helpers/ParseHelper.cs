using System;

namespace ItemsOrdersGenerator.Classes.Helpers
{
    /// <summary>
    /// Вспомогательный класс для дополнительных реализаций методов Parse и TryParse
    /// </summary>
    internal class ParseHelper
    {
        /// <summary>
        /// Спарсить объект класса <see cref="DateTime"/> с форматированием ОС пользователя
        /// </summary>
        /// <param name="stringToParse">Строка, которую необходимо спарсить</param>
        /// <param name="result">Объект класса <see cref="DateTime"/> при успешном парсинге</param>
        /// <returns>true если удалось спарсить, false если не удалось</returns>
        public static bool TryParseDateTimeFromSystemCulture(string stringToParse, out DateTime result)
        {
            return DateTime.TryParse(stringToParse,
                        System.Globalization.CultureInfo.InstalledUICulture,
                        System.Globalization.DateTimeStyles.None,
                        out result);
        }

        /// <summary>
        /// Спарсить объект класса <see cref="DateTime"/> с форматированием ОС пользователя
        /// </summary>
        /// <param name="stringToParse">Строка, которую необходимо спарсить</param>
        /// <returns>Объект класса <see cref="DateTime"/></returns>
        /// <exception>См. <seealso cref="DateTime"/></exception>
        public static DateTime ParseDateTimeFromSystemCulture(string stringToParse)
        {
            return DateTime.Parse(stringToParse,
                        System.Globalization.CultureInfo.InstalledUICulture,
                        System.Globalization.DateTimeStyles.None);
        }
    }
}
