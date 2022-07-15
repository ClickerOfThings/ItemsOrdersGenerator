using System;
using System.Collections.Generic;
using System.Text;

namespace ItemsOrdersGenerator.Classes.Helpers
{
    internal class ParseHelper
    {
        public static bool TryParseDateTimeFromSystemCulture(string parseString, out DateTime result)
        {
            return DateTime.TryParse(parseString,
                        System.Globalization.CultureInfo.InstalledUICulture,
                        System.Globalization.DateTimeStyles.None,
                        out result);
        }
        public static DateTime ParseDateTimeFromSystemCulture(string parseString)
        {
            return DateTime.Parse(parseString,
                        System.Globalization.CultureInfo.InstalledUICulture,
                        System.Globalization.DateTimeStyles.None);
        }
    }
}
