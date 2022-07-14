using OverpassLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ItemOrderDemonstration.Classes
{
    internal static class HelperClass
    {
        public static Random rand = new Random();
        public static string[] GetAllTxtsInAppFolder() =>
            System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.txt");
        public static string[] GetAllConfigFilesInAppFolder() =>
            GetAllFilesWithWildcard("*.conf").Concat(GetAllFilesWithWildcard("*.json")).ToArray();
        public static string[] GetAllFilesWithWildcard(string wildcard) =>
            System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), wildcard);
        public static void ShuffleList<T>(ref List<T> listToShuffle)
        {
            listToShuffle = listToShuffle.OrderBy(x => rand.NextDouble()).ToList();
        }
        public static Dictionary<string, List<Item>> GetAllCorrectXmlItemFilesInAppFolder()
        {
            string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.xml");
            Dictionary<string, List<Item>> result = new Dictionary<string, List<Item>>();
            foreach(string file in files)
            {
                try
                {
                    List<Item> tempList = XmlGenerator.DeserializeItemsFromFileToList(file);
                    result.Add(file, tempList);
                }
                catch(Exception)
                {
                    continue;
                }
            }
            return result;
        }
        public static List<OsmClass> GetAllPoints(OsmClass cityObj, string[] placeTypes)
        {
            return OverpassMethods.GetAllPlacesInCity(cityObj, placeTypes);
        }
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
