using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="parseString">Строка, которую необходимо спарсить</param>
        /// <param name="result">Объект класса <see cref="DateTime"/> при успешном парсинге</param>
        /// <returns>true если удалось спарсить, false если не удалось</returns>
        public static bool TryParseDateTimeFromSystemCulture(string parseString, out DateTime result)
        {
            return DateTime.TryParse(parseString,
                        System.Globalization.CultureInfo.InstalledUICulture,
                        System.Globalization.DateTimeStyles.None,
                        out result);
        }
        /// <summary>
        /// Спарсить объект класса <see cref="DateTime"/> с форматированием ОС пользователя
        /// </summary>
        /// <param name="parseString">Строка, которую необходимо спарсить</param>
        /// <returns>Объект класса <see cref="DateTime"/></returns>
        /// <exception>См. <seealso cref="DateTime"/></exception>
        public static DateTime ParseDateTimeFromSystemCulture(string parseString)
        {
            return DateTime.Parse(parseString,
                        System.Globalization.CultureInfo.InstalledUICulture,
                        System.Globalization.DateTimeStyles.None);
        }
    }
}
