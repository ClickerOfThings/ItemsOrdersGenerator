//#define DISABLE_CATCH_ALL_EXCEPTIONS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using ItemOrderDemonstration.Classes;

using OverpassLibrary;

using System.Linq;
using Newtonsoft.Json;
using ItemsOrdersGenerator.Classes.Model;

namespace ItemOrderDemonstration
{
    internal class Program
    {
        public const string TIME_FORMAT = @"hh\:mm";
        public static Config CurrentConfig { get; set; }
        static void Main(string[] args)
        {
            // для разделителей decimal и float чисел в едином формате
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            CurrentConfig = ConsoleInput.GetConfigFile();
            if (CurrentConfig is null)
            {
                Console.WriteLine("Конфигурационный файл не загружен. При работе с программой " +
                    "данные будут вводиться вручную.");
                ConsoleInput.WaitForInput();
            }

            string consoleInput;
            do
            {
                Console.Clear();
                Console.WriteLine("Выбор - нажатием нужной клавиши в квадратных скобках, затем нажатием Enter.");
                Console.WriteLine("\n");
                Console.WriteLine("Сгенерировать: [1] - файл товаров, [2] - файл заказов, [3] - оба файла.");
                Console.WriteLine("[C] для загрузки нового конфигурационного файла.");
                Console.WriteLine("[Q] для выхода.");
                consoleInput = ConsoleInput.GetInputFromConsole("1 2 3 C Q");
                try
                {
                    switch (consoleInput)
                    {
                        case "1":
                            CreateItemsFile();
                            break;
                        case "2":
                            CreateOrdersFile();
                            break;
                        case "3":
                            CreateItemsFile();
                            CreateOrdersFile();
                            break;
                        case "C":
                            Config newLoadedConfig;
                            try
                            {
                                newLoadedConfig = ConsoleInput.GetConfigFile();
                            }
                            catch (Exception ex)
                            {
                                throw new BadConfigException(ex.Message);
                            }
                            if (newLoadedConfig is null)
                            {
                                Console.WriteLine("Конфигурационный файл не был загружен. Вы хотите " +
                                    "работать без загруженной ранее конфигурации [Y], или оставить " +
                                    "ранее загруженную конфигурацию? [N]");
                                string confInput = ConsoleInput.GetInputFromConsole("Y N");
                                switch (confInput)
                                {
                                    case "Y":
                                        CurrentConfig = null;
                                        Console.WriteLine("Текущая конфигурация была сброшена. При работе с программой " +
                                            "данные будут вводиться вручную.");
                                        break;
                                }
                                ConsoleInput.WaitForInput();
                            }
                            else
                                CurrentConfig = newLoadedConfig;
                            break;
                    }
                }
                catch (BadConfigException ex)
                {
                    Console.WriteLine("Во время обработки запроса произошла ошибка: " + ex.Message);
                    Console.WriteLine("Если у вас загружен конфигурационный файл, проверьте его содержимое " +
                        "на синтаксические и/или логические ошибки.");
                    ConsoleInput.WaitForInput();
                }
#if !DISABLE_CATCH_ALL_EXCEPTIONS
                catch (Exception ex)
                {
                    Console.WriteLine("Во время обработки запроса произошла ошибка: " + ex.Message);
                    ConsoleInput.WaitForInput();
                }
#endif
            } while (consoleInput != "Q");

            return;
        }

        private static void CreateItemsFile()
        {
            ConsoleInput.GetItemFilesPathsFromUser(out string inputItemsFilePath, out string outputFilePath);

            try
            {
                Console.WriteLine("Файл создаётся, подождите...");
                XmlGenerator.SerializeItemsListToFile(outputFilePath, ModelGenerator.ParseTxtItemsIntoList(inputItemsFilePath));
                Console.WriteLine("Файл был успешно создан.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Во время создания файла товаров произошла ошибка: " + ex.ToString());
            }

            ConsoleInput.WaitForInput();
        }

        private static void CreateOrdersFile()
        {
            OsmClass foundObj = ConsoleInput.GetOsmObjectFromUser();
            if (foundObj is null)
                return;

            string[] foundTypes = ConsoleInput.GetPlaceTypesFromUser();
            if (foundTypes is null)
                return;

            Console.WriteLine("Ищем места, подождите...");
            List<OsmClass> foundPointsList = new List<OsmClass>();
            try
            {
                foundPointsList = OverpassMethods.GetAllPlacesInBox(
                    foundObj.CityNorthEast, foundObj.CitySouthWest, foundTypes);
                if (foundPointsList.Count == 0)
                {
                    throw new Exception("Места не были найдены");
                }
                Console.WriteLine("Найдено " + foundPointsList.Count + " мест");
                ConsoleInput.WaitForInput();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Во время поиска мест произошла ошибка: " + ex.Message);
                ConsoleInput.WaitForInput();
                return;
            }

            List<Item> foundItemsList = ConsoleInput.GetItemsFromUser();
            if (foundItemsList is null)
                return;
            Console.Clear();

            DateTime ordersDateTime = ConsoleInput.GetDateFromUser();
            Console.Clear();

            ConsoleInput.GetMinsAndMaxsFromUser(foundPointsList.Count, foundItemsList.Count,
                out int pointsCount,
                out Tuple<int, int> windows,
                out Tuple<int, int> itemsPerWindow,
                out Tuple<int, int> itemsCountPerPosition,
                out Tuple<TimeSpan, TimeSpan> fromToRange,
                out TimeSpan intervalBetweenFromTo);
            Console.Clear();

            ConsoleInput.GetOrderFilesPathsFromUser(out string outputOrdersFileName);

            try
            {
                Console.WriteLine("Создаётся файл заказов, пожалуйста, подождите...");
                XmlGenerator.GenerateOrdersFile(foundItemsList, foundPointsList,
                    ordersDateTime, outputOrdersFileName, pointsCount,
                    windows, itemsPerWindow, itemsCountPerPosition,
                    fromToRange, intervalBetweenFromTo);
                Console.WriteLine("Файл " + outputOrdersFileName + " был успешно создан");
            }
            catch (BadIntervalException ex)
            {
                var previousConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("При создании файла возникла ошибка интервала: " + ex.Message);
                Console.ForegroundColor = previousConsoleColor;
            }
            catch (Exception ex)
            {
                var previousConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("При создании файла возникла ошибка: " + ex.Message);
                Console.ForegroundColor = previousConsoleColor;
            }
            ConsoleInput.WaitForInput();
        }
    }
}
