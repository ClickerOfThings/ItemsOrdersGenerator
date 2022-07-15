using ItemOrderDemonstration.Classes;
using ItemsOrdersGenerator.Classes.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemsOrdersGenerator.Classes.Helpers
{
    internal class FolderHelper
    {
        public static string[] GetAllTxtsInAppFolder() =>
            System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.txt");
        public static string[] GetAllConfigFilesInAppFolder() =>
            GetAllFilesWithWildcard("*.conf").Concat(GetAllFilesWithWildcard("*.json")).ToArray();
        public static string[] GetAllFilesWithWildcard(string wildcard) =>
            System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), wildcard);
        public static Dictionary<string, List<Item>> GetCorrectXmlItemFilesInAppFolder()
        {
            string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.xml");
            Dictionary<string, List<Item>> result = new Dictionary<string, List<Item>>();
            foreach (string file in files)
            {
                try
                {
                    List<Item> tempList = XmlGenerator.DeserializeItemsFromFileToList(file);
                    result.Add(file, tempList);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return result;
        }
    }
}
