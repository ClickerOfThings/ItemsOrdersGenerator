using ItemsOrdersGenerator.Classes.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ItemOrderDemonstration.Classes
{
    /// <summary>
    /// Класс генерации товаров из txt файла
    /// </summary>
    internal static class ItemModelGenerator
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
            int errorLineNum = 0; // для вывода строки, в которой произошла ошибка
            while (!reader.EndOfStream)
            {
                errorLineNum++;
                string currentLine = reader.ReadLine().Replace("; ", ";");
                var variables = currentLine.Split(';');
                try
                {
                    if (variables.Length != 3)
                        throw new ArgumentException();
                    Item newItem = StringsToSku(variables);
                    resultList.Add(newItem);
                }
                catch(Exception ex)
                {
                    ConsoleColor defaultForegroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (ex is FormatException || ex is OverflowException)
                    {
                        throw new ArgumentException("Неправильный формат числа, либо слишком большое/маленькое число, " +
                            "строка " + errorLineNum);
                    }
                    if (ex is ArgumentException)
                    {
                        Console.ForegroundColor = defaultForegroundColor;
                        throw new ArgumentException("Количество переменных не равно 3, строка " + errorLineNum);
                    }
                    Console.ForegroundColor = defaultForegroundColor;
                }
            }
            return resultList;
        }

        /// <summary>
        /// Парсинг строк в объект товара
        /// </summary>
        /// <param name="inputStrings">Строки для обработки, см remarks метода ParseTxtFileIntoList</param>
        /// <returns>Обработанный объект товара</returns>
        private static Item StringsToSku(string[] inputStrings)
        {
            // приводим вес к нужному формату
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
