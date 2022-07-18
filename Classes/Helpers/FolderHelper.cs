using ItemOrderDemonstration.Classes;
using ItemsOrdersGenerator.Classes.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemsOrdersGenerator.Classes.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с файлами/папками
    /// </summary>
    internal class FolderHelper
    {
        /// <summary>
        /// Извлечь пути всех файлов с расширением .txt в директории приложения
        /// </summary>
        /// <returns>Пути всех файлов с расширением .txt</returns>
        public static string[] GetTxtFilesInAppFolder() =>
            System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.txt");
        /// <summary>
        /// Извлечь пути всех файлов с расширениями .conf и .json в директории приложения
        /// </summary>
        /// <returns>Пути всех файлов с расширениями .conf и .json</returns>
        public static string[] GetConfigFilesInAppFolder() =>
            GetFilesWithWildcard("*.conf").Concat(GetFilesWithWildcard("*.json")).ToArray();
        /// <summary>
        /// Излечь пути всех файлов с определённым wildcard-ом в директории приложения
        /// </summary>
        /// <param name="wildcard">wildcard, который используется при поиске файлов</param>
        /// <returns>Пути всех файлов по wildcard-у</returns>
        public static string[] GetFilesWithWildcard(string wildcard) =>
            System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), wildcard);
        /// <summary>
        /// Излечь пути всех файлов с расширением .xml, 
        /// которые успешно десериализируются в объект класса <see cref="Item"/>
        /// </summary>
        /// <returns>Словарь, ключ - путь к файлу, значение - список с десериализированными товарами</returns>
        public static Dictionary<string, List<Item>> GetItemXmlFilesInAppFolder()
        {
            string[] itemsFiles = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.xml");
            Dictionary<string, List<Item>> resultDictionary = new Dictionary<string, List<Item>>();
            foreach (string itemFile in itemsFiles)
            {
                try
                {
                    List<Item> tempList = XmlGenerator.DeserializeItemsFromFileToList(itemFile);
                    resultDictionary.Add(itemFile, tempList);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return resultDictionary;
        }
    }
}
