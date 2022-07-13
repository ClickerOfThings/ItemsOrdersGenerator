using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ItemOrderDemonstration.Classes
{
    internal static class DataGenerator
    {
        /// <summary>
        /// Парсинг txt файла для извлечения данных о товаре
        /// </summary>
        /// <param name="txtFilePath">Путь к txt файлу с товарами</param>
        /// <returns>Список объектов товаров</returns>
        /// <remarks>
        /// Формат txt файла для товара:
        /// %название товара%; %количество в 1 шт., тип int%; %вес в кг., тип float%
        /// допускается разделение переменных как с точкой-запятой И пробелом, так и без
        /// </remarks>
        public static List<Item> ParseTxtItemsIntoList(string txtFilePath)
        {
            List<Item> resultList = new List<Item>();
            using StreamReader reader = new StreamReader(txtFilePath);
            int lineNum = 0; // для вывода строки, в которой произошла ошибка
            while (!reader.EndOfStream)
            {
                lineNum++;
                string currentLine = reader.ReadLine().Replace("; ", ";");
                var attrs = currentLine.Split(';');
                try
                {
                    if (attrs.Length != 3)
                        throw new ArgumentException();
                    Item newItem = strings_to_sku(attrs);
                    resultList.Add(newItem);
                }
                catch(Exception ex)
                {
                    ConsoleColor prevForeColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (ex is FormatException || ex is OverflowException)
                    {
                        throw new ArgumentException("Неправильный формат числа, либо слишком большое/маленькое число, " +
                            "строка " + lineNum);
                    }
                    if (ex is ArgumentException)
                    {
                        Console.ForegroundColor = prevForeColor;
                        throw new ArgumentException("Количество переменных не равно 3, строка " + lineNum);
                    }
                    Console.ForegroundColor = prevForeColor;
                }
            }
            return resultList;
        }

        /// <summary>
        /// Парсинг строк в объект товара
        /// </summary>
        /// <param name="inputStrings">Строки для обработки, см remarks метода ParseTxtFileIntoList</param>
        /// <returns>Обработанный объект товара</returns>
        private static Item strings_to_sku(string[] inputStrings)
        {
            inputStrings[2] = inputStrings[2].Replace(",", ".");
            Item return_item = new Item()
            {
                Id = Guid.NewGuid(),
                Name = inputStrings[0],
                AmountPerTray = int.Parse(inputStrings[1]),
                Weight = float.Parse(inputStrings[2], style: System.Globalization.NumberStyles.Any)
            };
            return return_item;
        }
    }
}
