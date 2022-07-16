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

            string input;
            do
            {
                Console.Clear();
                Console.WriteLine("Выбор - нажатием нужной клавиши в квадратных скобках, затем нажатием Enter.");
                Console.WriteLine("\n");
                Console.WriteLine("Сгенерировать: [1] - файл товаров, [2] - файл заказов, [3] - оба файла.");
                Console.WriteLine("[C] для загрузки нового конфигурационного файла.");
                Console.WriteLine("[Q] для выхода.");
                input = ConsoleInput.GetInputFromConsole("1 2 3 C Q");
                try
                {
                    switch (input)
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
                            Config newConf;
                            try
                            {
                                newConf = ConsoleInput.GetConfigFile();
                            }
                            catch (Exception ex)
                            {
                                throw new BadConfigException(ex.Message);
                            }
                            if (newConf is null)
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
                                CurrentConfig = newConf;
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
            } while (input != "Q");

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
            OsmClass searchObj = ConsoleInput.GetOsmObjectFromUser();
            if (searchObj is null)
                return;
            string[] types = ConsoleInput.GetPlaceTypesFromUser();
            if (types is null)
                return;
            Console.WriteLine("Ищем места, подождите...");
            List<OsmClass> pointsList = new List<OsmClass>();
            try
            {
                pointsList = OverpassMethods.GetAllPlacesInBox(searchObj.CityNorthEast, searchObj.CitySouthWest, types);
                if (pointsList.Count == 0)
                {
                    throw new Exception("Места не были найдены");
                }
                Console.WriteLine("Найдено " + pointsList.Count + " мест");
                ConsoleInput.WaitForInput();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Во время поиска мест произошла ошибка: " + ex.Message);
                ConsoleInput.WaitForInput();
                return;
            }
            List<Item> itemsList = ConsoleInput.GetItemsFromUser();
            if (itemsList is null)
                return;
            Console.Clear();

            DateTime ordersDateTime = ConsoleInput.GetDateFromUser();
            Console.Clear();

            ConsoleInput.GetMinsAndMaxsFromUser(pointsList.Count, itemsList.Count,
                out int pointsCount,
                out Tuple<int, int> windowsMinMax,
                out Tuple<int, int> itemsPerWindow,
                out Tuple<int, int> itemsCountPerPosition,
                out Tuple<TimeSpan, TimeSpan> fromTo,
                out TimeSpan intervalBetweenFromTo);
            Console.Clear();

            ConsoleInput.GetOrderFilesPathsFromUser(out string fileName);

            try
            {
                Console.WriteLine("Создаётся файл заказов, пожалуйста, подождите...");
                XmlGenerator.GenerateOrdersFile(itemsList, pointsList,
                    ordersDateTime, fileName, pointsCount,
                    windowsMinMax, itemsPerWindow, itemsCountPerPosition,
                    fromTo, intervalBetweenFromTo);
                Console.WriteLine("Файл " + fileName + " был успешно создан");
            }
            catch (BadIntervalException ex)
            {
                var prevColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("При создании файла возникла ошибка интервала: " + ex.Message);
                Console.ForegroundColor = prevColour;
            }
            catch (Exception ex)
            {
                var prevColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("При создании файла возникла ошибка: " + ex.Message);
                Console.ForegroundColor = prevColour;
            }
            ConsoleInput.WaitForInput();
        }
    }
}
