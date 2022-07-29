using ItemsOrdersGenerator.Helpers;
using ItemsOrdersGenerator.Model;
using ItemsOrdersGenerator.View;
using ItemsOrdersGenerator.Extensions;
using OverpassLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ItemOrderDemonstration;
using ItemOrderDemonstration.Helpers;
using ItemsOrdersGenerator.Exceptions;
using ItemsOrdersGenerator.Generators;

namespace ItemsOrdersGenerator
{
    /// <summary>
    /// Класс консольного ввода
    /// </summary>
    internal static class ConsoleInput
    {
        /// <summary>
        /// Стандартное название выходного файла для товаров в формате XML
        /// </summary>
        private const string DEFAULT_ITEMS_OUTPUT_FILE = "items.xml";

        /// <summary>
        /// Стандартное название выходного файла для заказов в формате XML
        /// </summary>
        private const string DEFAULT_ORDERS_OUTPUT_FILE = "orders.xml";

        /// <summary>
        /// Сообщение, выводящееся пользователю, когда от него ожидается нажатие кнопки для подолжения
        /// работы программы (обычно, для того, чтобы он успел что-то прочитать)
        /// </summary>
        const string INPUT_AWAIT_MSG = "Нажмите любую кнопку, чтобы продолжить...";

        /// <summary>
        /// Создать конфигурационный файл из пути, предоставленный пользователем, либо отказаться 
        /// от конфигурации в пользу ручного ввода
        /// </summary>
        /// <returns>Объект конфигурации или null, если пользователь отказался от использования 
        /// конфигурационного файла</returns>
        public static Config GetConfigFile()
        {
            Config resultConfig = null;
            bool isExitingFunction = false;
            string[] configFilesInAppFolder = FolderHelper.GetConfigFilesInAppFolder();
            string configPath = string.Empty;
            Dictionary<string, string> charToConfigPath = new Dictionary<string, string>();
            string consoleInput;

            while (!isExitingFunction && resultConfig is null)
            {
                Console.Clear();

                StringBuilder allowedInput = new StringBuilder("? C S E Q");
                charToConfigPath.Clear();

                if (configFilesInAppFolder.Length == 0)
                    Console.WriteLine("В папке с приложением не найдено json/conf файлов.");
                else
                {
                    Console.WriteLine("В папке с приложением найдены следующие json/conf файлы:");
                    int configFileCounter = 1;
                    foreach (string jsonFile in configFilesInAppFolder)
                    {
                        charToConfigPath.Add(configFileCounter.ToString(), jsonFile);
                        allowedInput.Append(" " + configFileCounter.ToString());
                        Console.WriteLine("[" + configFileCounter + "]: " + jsonFile);
                        configFileCounter++;
                    }
                }

                Console.WriteLine("Загрузка конфигурационного файла\n" +
                        "[?] для помощи по полям конфигурации,\n" +
                        "[C] для ручного ввода пути к файлу,\n" +
                        "[S] чтобы снова сканировать папку,\n" +
                        "[E] чтобы создать пример конфигурационного файла (user.conf.example),\n" +
                        "[Q] - не загружать новый конфигурационный файл.");
                consoleInput = GetInputFromConsole(allowedInput.ToString());
                switch (consoleInput)
                {
                    case "?":
                        ConfigHelper.ListConfigHelp();
                        WaitForInput();
                        break;
                    case "C":
                        Console.WriteLine(
                        "Укажите относительный или полный путь к json/conf конфигурации\n" +
                        "(например, Папка/user.conf или C:/user.conf)");
                        string customFilePath = Console.ReadLine();
                        if (File.Exists(customFilePath))
                            configPath = customFilePath;
                        else
                        {
                            Console.WriteLine("Файл не найден.");
                            WaitForInput();
                        }
                        break;
                    case "S":
                        configFilesInAppFolder = FolderHelper.GetConfigFilesInAppFolder();
                        break;
                    case "E":
                        Config exampleConfig = new Config()
                        {
                            CityName = "Москва",
                            SearchRectangle = new SearchRectangle(10, 20, 30, 40),
                            PlaceTypes = new string("cinema, kiosk, clinic, fast_food, college, school"),
                            ItemsFilePathInput = "items.xml",
                            OrderDate = new DateTime(2000, 12, 20),
                            PointsCount = 40,
                            MinMaxTimeWindows = new MinMaxTupleJson<int, int>(2, 4),
                            MinMaxItemsPerWindow = new MinMaxTupleJson<int, int>(1, 2),
                            MinMaxItemsCountPerPosition = new MinMaxTupleJson<int, int>(1, 2),
                            TimeRange = new FromToTupleJson<TimeSpan, TimeSpan>(
                                new TimeSpan(6, 0, 0), new TimeSpan(24, 0, 0)),
                            IntervalBetweenTimeRange = new TimeSpan(0, 30, 0),
                            OrdersFilePathOutput = "orders.xml",
                            TxtItemFilePathInput = "tovar.txt",
                            XmlItemFilePathOutput = "items.xml"
                        };
                        exampleConfig.WriteIntoJson("user.conf.example");
                        Console.WriteLine("Пример конфигурационного файла был записан в user.conf.example");
                        WaitForInput();
                        break;
                    case "Q":
                        configPath = null;
                        isExitingFunction = true;
                        break;
                    default:
                        if (!charToConfigPath.TryGetValue(consoleInput, out configPath))
                        {
                            Console.WriteLine("Файл не был найден");
                            WaitForInput();
                        }
                        configFilesInAppFolder = FolderHelper.GetConfigFilesInAppFolder();
                        break;
                }

                if (!string.IsNullOrEmpty(configPath))
                {
                    try
                    {
                        resultConfig = ConfigHelper.ReadFromFile(configPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Во время считывания конфигурационного файла произошла ошибка: " +
                            ex.Message);
                        WaitForInput();
                    }
                }
            }
            return resultConfig;
        }

        /// <summary>
        /// Получить от пользователя путь к входному TXT файлу с товарами. Формат файла не проверяется
        /// </summary>
        /// <returns>Путь к входному TXT файлу с товарами</returns>
        public static string GetItemTxtPathFromUser()
        {
            bool isExitingFunction = false;
            string[] txtFiles = FolderHelper.GetTxtFilesInAppFolder();
            string resultItemPath = string.Empty;
            Dictionary<string, string> charToTxtFilePath = new Dictionary<string, string>();
            string consoleInput;

            while (!isExitingFunction && string.IsNullOrEmpty(resultItemPath))
            {
                Console.Clear();

                StringBuilder allowedInput = new StringBuilder("? C S Q");
                charToTxtFilePath.Clear();

                if (txtFiles.Length == 0)
                    Console.WriteLine("В папке с приложением не найдено txt файлов.\n");
                else
                {
                    Console.WriteLine("В папке с приложением найдены следующие txt файлы:");
                    charToTxtFilePath.Clear();
                    int txtFileCounter = 1;
                    foreach (string txtFile in txtFiles)
                    {
                        charToTxtFilePath.Add(txtFileCounter.ToString(), txtFile);
                        allowedInput.Append(" " + txtFileCounter.ToString());
                        Console.WriteLine("[" + txtFileCounter + "]: " + txtFile);
                        txtFileCounter++;
                    }
                }

                Console.WriteLine("Выбор типа продукции\n" +
                        "[?] для помощи по формату файла,\n" +
                        "[C] для ручного ввода пути к файлу,\n" +
                        "[S] чтобы снова сканировать папку.\n" +
                        "[Q] - назад в меню.");
                consoleInput = GetInputFromConsole(allowedInput.ToString());
                switch (consoleInput)
                {
                    case "?":
                        Console.WriteLine("\n");
                        Console.WriteLine("Формат txt файла для товаров:");
                        Console.WriteLine("На одну строчку - один товар.");
                        Console.WriteLine("Вводятся три переменные, разделяемые точкой с запятой (;): " +
                            "Название товара, количество товара в 1 упаковке, вес в килограммах (допускается " +
                            "ввод десятичного числа, например 0.100)");
                        WaitForInput();
                        break;
                    case "C":
                        Console.WriteLine(
                        "Укажите относительный или полный путь к txt файлу\n" +
                        "(например, Папка/file.txt или C:/file.txt)");
                        string customFilePath = Console.ReadLine();
                        if (File.Exists(customFilePath))
                            resultItemPath = customFilePath;
                        else
                        {
                            Console.WriteLine("Файл не найден.");
                            WaitForInput();
                        }
                        break;
                    case "S":
                        txtFiles = FolderHelper.GetTxtFilesInAppFolder();
                        break;
                    case "Q":
                        isExitingFunction = true;
                        break;
                    default:
                        charToTxtFilePath.TryGetValue(consoleInput, out resultItemPath);
                        txtFiles = FolderHelper.GetTxtFilesInAppFolder();
                        break;
                }
            }
            return resultItemPath;
        }

        /// <summary>
        /// Получить входной и выходной пути файлов для создания файла с товарами
        /// </summary>
        /// <param name="inputItemsFilePath">Входной путь к TXT файлу с товарами 
        /// (формат файла не проверяется)</param>
        /// <param name="outputFilePath">Выходной путь XML файла с товарами</param>
        public static void GetItemFilesPathsFromUser(out string inputItemsFilePath, out string outputFilePath)
        {
            inputItemsFilePath = Program.CurrentConfig?.TxtItemFilePathInput;
            outputFilePath = Program.CurrentConfig?.XmlItemFilePathOutput;

            if (string.IsNullOrEmpty(inputItemsFilePath))
                inputItemsFilePath = GetItemTxtPathFromUser();

            if (string.IsNullOrEmpty(inputItemsFilePath))
                return;

            if (!File.Exists(inputItemsFilePath))
            {
                Console.WriteLine("Указанный файл не найден.");
                WaitForInput();
                return;
            }

            if (string.IsNullOrEmpty(outputFilePath))
            {
                Console.Write($"Введите название выходного файла (Enter для стандартного имени {DEFAULT_ITEMS_OUTPUT_FILE}): ");
                outputFilePath = Console.ReadLine();
                if (outputFilePath == string.Empty)
                    outputFilePath = DEFAULT_ITEMS_OUTPUT_FILE;
            }
        }

        /// <summary>
        /// Получить OSM объект от пользователя (город или прямоугольник)
        /// </summary>
        /// <returns>Объект класса <see cref="OsmClass"/>, в котором гарантированно будет 
        /// указан прямоугольник, либо null, если пользователь отказался выбирать место поиска</returns>
        /// <exception cref="BadConfigException">Указанный город из конфигурации не был найден, либо 
        /// указанный прямоугольник из конфигурации неправильного формата</exception>
        public static OsmClass GetOsmObjectFromUser()
        {
            bool isExitingFunction = false;
            OsmClass foundCity = null;
            string consoleInput;

            string cityNameFromConf = Program.CurrentConfig?.CityName;
            if (!string.IsNullOrEmpty(cityNameFromConf))
            {
                Console.WriteLine("Ищем город, подождите...");
                foundCity = GetOneCityFromList(OverpassMethods.GetCityInfo(cityNameFromConf));
                if (foundCity is null)
                    throw new BadConfigException($"Город {cityNameFromConf} из конфигурации не был найден");
            }

            SearchRectangle rectFromConfig = Program.CurrentConfig?.SearchRectangle;
            if (foundCity is null && rectFromConfig != null)
            {
                if (rectFromConfig.NorthEastCorner.CompareTo(rectFromConfig.SouthWestCorner) != 1)
                    throw new BadConfigException("В прямоугольнике северо-восточный угол должен быть " +
                        "больше юго-восточного угла как в X, так и в Y");
                else
                    foundCity = new OsmClass
                    {
                        CityNorthEast = rectFromConfig.NorthEastCorner,
                        CitySouthWest = rectFromConfig.SouthWestCorner
                    };
            }

            while (!isExitingFunction && foundCity is null)
            {
                Console.Clear();
                Console.WriteLine("Укажите место выборки данных:\n" +
                    "[1] - по названию города,\n" +
                    "[2] - по прямоугольнику (координаты).\n" +
                    "[Q] - назад в меню.");
                consoleInput = GetInputFromConsole("1 2 Q");
                switch (consoleInput)
                {
                    case "1":
                        Console.Write("Укажите название города: ");
                        string cityNameFromConsole = Console.ReadLine();
                        Console.WriteLine("Ищем город, подождите...");
                        foundCity = GetOneCityFromList(OverpassMethods.GetCityInfo(cityNameFromConsole));
                        if (foundCity is null)
                        {
                            Console.WriteLine("Города не существует");
                            WaitForInput();
                        }
                        break;
                    case "2":
                        PointF northEastPoint, southWestPoint;
                        Console.Clear();
                        if (GetCoordinatesFromConsole(out northEastPoint, out southWestPoint))
                        {
                            isExitingFunction = true;
                            foundCity = new OsmClass
                            {
                                CityNorthEast = northEastPoint,
                                CitySouthWest = southWestPoint
                            };
                        }
                        else
                        {
                            Console.WriteLine("Неправильно введены координаты");
                            WaitForInput();
                        }
                        break;
                    case "Q":
                        isExitingFunction = true;
                        break;
                }
            }
            return foundCity;
        }

        /// <summary>
        /// Получить один город из списка городов от пользователя
        /// </summary>
        /// <param name="listToGetFrom">Список с городами, из которого один город 
        /// должен выбрать пользователь</param>
        /// <returns>Объект города</returns>
        /// <remarks>Cписок с городами может быть пустым или содержать только один город. 
        /// В таких случаях, пользователю не будет даваться выбор (демократия)</remarks>
        public static OsmClass GetOneCityFromList(List<OsmClass> listToGetFrom)
        {
            if (listToGetFrom is null || listToGetFrom.Count == 0)
                return null;
            if (listToGetFrom.Count == 1)
                return listToGetFrom.First();

            StringBuilder allowedInput = new StringBuilder();
            Dictionary<string, OsmClass> charToCity = new Dictionary<string, OsmClass>();

            int cityIndex = 1;
            Console.WriteLine("Выберите один город из нескольких приведённых:");
            foreach (OsmClass city in listToGetFrom)
            {
                allowedInput.Append(" " + cityIndex);
                charToCity.Add(cityIndex.ToString(), city);
                Console.WriteLine("[" + cityIndex + "] - " + city.State + ", " + city.City);
                cityIndex++;
            }

            return charToCity[GetInputFromConsole(allowedInput.ToString())];
        }

        /// <summary>
        /// Получить список типов мест от пользователя
        /// </summary>
        /// <returns>Массив строк с типами мест, либо null, если пользователь отказался от выбора типов</returns>
        public static string[] GetPlaceTypesFromUser()
        {
            bool isExitingFunction = false;
            string[] resultTypes = null;
            string consoleInput;

            string placeTypesFromConf = Program.CurrentConfig?.PlaceTypes;
            if (!string.IsNullOrEmpty(placeTypesFromConf))
                resultTypes = placeTypesFromConf.Replace(", ", ",").Split(",");

            while (!isExitingFunction && resultTypes is null)
            {
                Console.Clear();
                Console.WriteLine("Выбор типов мест\n" +
                    "[1] - выбор стандартного набора (администрация и муниципалитет, " +
                    "магазины, кафе и рестораны и проч.)\n" +
                    "[2] - все типы мест (НЕ РЕКОМЕНДУЕТСЯ)\n\n" +
                    "[?] - помощь по формату файла с типами.\n" +
                    "[C] - ввод файла с типами,\n" +
                    "[Q] - выйти в меню.");
                consoleInput = GetInputFromConsole("1 2 ? C Q");
                switch (consoleInput)
                {
                    case "1":
                        resultTypes = OverpassConsts.MAIN_PLACES;
                        break;
                    case "2":
                        resultTypes = OverpassConsts.ALL_PLACES;
                        break;
                    case "?":
                        Console.WriteLine("Формат файла с типами мест:");
                        Console.WriteLine("Типы мест (на английском, из базы данных OSM) вводятся в одну линию, " +
                            "каждый тип разделяется запятой (допускается пробел после запятой).\n" +
                            "Например: sewing,curtain,electronics,jewelry\n" +
                            "Будут выбраны пошивочные, магазины по продаже занавесок, электроники и ювелирных изделий.");
                        WaitForInput();
                        break;
                    case "C":
                        Console.WriteLine("Укажите относительный или полный путь к файлу с типами\n" +
                        "(например, Папка/file.txt или C:/file.txt)");
                        string fileName = Console.ReadLine();
                        if (!File.Exists(fileName))
                        {
                            Console.WriteLine("Файл не найден.");
                            WaitForInput();
                            break;
                        }
                        using (StreamReader reader = new StreamReader(File.Open(fileName, FileMode.Open)))
                        {
                            resultTypes = reader.ReadToEnd().Replace(", ", ",").Split(",");
                        }
                        Console.WriteLine("Были извлечены следующие типы (разделённые точкой с запятой):");
                        foreach (string s in resultTypes)
                        {
                            Console.Write(s + "; ");
                        }
                        Console.Write("\nЭто правильные типы? [y/n] ");
                        if (GetInputFromConsole("Y N") == "N")
                            resultTypes = null;
                        break;
                    case "Q":
                        isExitingFunction = true;
                        break;
                }
            }
            return resultTypes;
        }

        /// <summary>
        /// Получить список товаров от пользователя
        /// </summary>
        /// <returns>Список с товарами, либо null, если пользователь отказался от выбора</returns>
        /// <exception cref="BadConfigException">Файла с товарами не существует, либо ошибка 
        /// во время парсинга файла с товарами</exception>
        public static List<Item> GetItemsFromUser()
        {
            bool isExitingFunction = false;
            List<Item> resultItemsList = null;
            string consoleInput;
            Dictionary<string, List<Item>> formattedXmlFiles = FolderHelper.GetItemXmlFilesInAppFolder();
            Dictionary<string, string> charToXmlFilePath = new Dictionary<string, string>();

            Console.WriteLine("Выбор товаров");

            string filePathFromConfig = Program.CurrentConfig?.ItemsFilePathInput;
            if (!string.IsNullOrEmpty(filePathFromConfig))
            {
                if (!File.Exists(filePathFromConfig))
                    throw new BadConfigException("Файла с товарами по пути " + filePathFromConfig + " не существует");
                try
                {
                    resultItemsList = XmlGenerator.DeserializeItemsFromFileToList(filePathFromConfig);
                }
                catch (InvalidOperationException ex)
                {
                    throw new BadConfigException(ex.InnerException.Message ?? ex.Message);
                }
            }

            while (!isExitingFunction && resultItemsList is null)
            {
                Console.Clear();

                StringBuilder allowedInput = new StringBuilder("C S Q");
                if (formattedXmlFiles.Count == 0)
                {
                    Console.WriteLine("В папке с приложением не найдено xml файлов товаров с правильным форматом.\n");
                    charToXmlFilePath.Clear();
                }
                else
                {
                    Console.WriteLine("В папке с приложением найдены следующие xml файлы:");
                    charToXmlFilePath.Clear();
                    int fileCounter = 1;
                    foreach (string xmlFile in formattedXmlFiles.Keys)
                    {
                        charToXmlFilePath.Add(fileCounter.ToString(), xmlFile);
                        allowedInput.Append(" " + fileCounter.ToString());
                        Console.WriteLine("[" + fileCounter + "]: " + xmlFile);
                        fileCounter++;
                    }
                }

                Console.WriteLine("[C] для ручного ввода пути к файлу,");
                Console.WriteLine("[S] чтобы снова сканировать папку.");
                Console.WriteLine("[Q] - выйти в меню.");
                consoleInput = GetInputFromConsole(allowedInput.ToString());
                switch (consoleInput)
                {
                    case "C":
                        Console.WriteLine(
                        "Укажите относительный или полный путь к txt файлу\n" +
                        "(например, Папка/file.txt или C:/file.txt)");
                        string customFilePath = Console.ReadLine();
                        if (File.Exists(customFilePath))
                        {
                            try
                            {
                                resultItemsList = XmlGenerator.DeserializeItemsFromFileToList(customFilePath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("При обработке xml файла произошла ошибка: " + ex.Message);
                                WaitForInput();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Файл не найден.");
                            WaitForInput();
                        }
                        break;
                    case "S":
                        formattedXmlFiles = FolderHelper.GetItemXmlFilesInAppFolder();
                        break;
                    case "Q":
                        isExitingFunction = true;
                        break;
                    default:
                        charToXmlFilePath.TryGetValue(consoleInput, out string filePath);
                        if (!string.IsNullOrEmpty(filePath))
                            resultItemsList = formattedXmlFiles[filePath];
                        break;
                }
            }
            return resultItemsList;
        }

        /// <summary>
        /// Получить поля для случайной генерации от пользователя
        /// </summary>
        /// <param name="maxPoints">Максимальное количество мест, которое может выбрать пользователь</param>
        /// <param name="maxItems">Максимальное количество товаров на одно место, 
        /// которое может выбрать пользователь</param>
        /// <param name="points">Выбранное количество точек</param>
        /// <param name="minMaxWindows">Выбранное минимальное и максимальное количество временных окон</param>
        /// <param name="itemsPerWindow">Выбранное минимальное и максимальное количество товаров на одно временное окно</param>
        /// <param name="itemsCountPerPosition">Выбранное минимальное и максимальное количество единиц товара на одну позицию</param>
        /// <param name="timespanFromTo">Выбранный промежуток от и до на временные окна</param>
        /// <param name="intervalBetweenFromTo">Выбранный интервал между временами от и до</param>
        /// <exception cref="BadConfigException">Одно из полей из конфигурации указано неверно</exception>
        /// <exception cref="BadIntervalException">Неправильный интервал из конфигурации</exception>
        public static void GetVariablesForRandomFromUser(int maxPoints, int maxItems,
            out int points, out Tuple<int, int> minMaxWindows,
            out Tuple<int, int> itemsPerWindow, out Tuple<int, int> itemsCountPerPosition,
            out Tuple<TimeSpan, TimeSpan> timespanFromTo, out TimeSpan intervalBetweenFromTo)
        {
        MinsMaxStart:
            #region Config Values;
            int? configPoints = Program.CurrentConfig?.PointsCount;
            if (configPoints != null)
            {
                if (configPoints.Value > maxPoints)
                    throw new BadConfigException("Количество указанных точек (" + configPoints.Value + ") " +
                    "больше найденных точек (" + maxPoints + ")");
                else
                    points = configPoints.Value;
            }
            else
                points = -1;

            minMaxWindows = Program.CurrentConfig?.MinMaxTimeWindows;
            if (minMaxWindows?.Item1 > minMaxWindows?.Item2)
                throw new BadConfigException("Минимальное количество окон (" + minMaxWindows.Item1 + ") " +
                    "больше максимального количества (" + minMaxWindows.Item2 + ")");

            itemsPerWindow = Program.CurrentConfig?.MinMaxItemsPerWindow;
            if (itemsPerWindow != null)
                if (itemsPerWindow.Item1 > itemsPerWindow.Item2)
                    throw new BadConfigException("Минимальное количество товаров на окно (" + itemsPerWindow.Item1 + ") " +
                        "больше максимального количества (" + itemsPerWindow.Item2 + ")");
                else if (itemsPerWindow.Item2 > maxItems)
                    throw new BadConfigException("Максимальное количество товаров на окно (" + itemsPerWindow.Item2 + ") " +
                        "больше максимально возможных товаров (" + maxItems + ")");

            itemsCountPerPosition = Program.CurrentConfig?.MinMaxItemsCountPerPosition;
            if (itemsCountPerPosition?.Item1 > itemsCountPerPosition?.Item2)
                throw new BadConfigException("Минимальное количество товаров на позицию (" + itemsCountPerPosition.Item1 + ") " +
                    "больше максимального количества (" + itemsCountPerPosition.Item2 + ")");

            timespanFromTo = Program.CurrentConfig?.TimeRange;
            if (timespanFromTo?.Item1 > timespanFromTo?.Item2)
                throw new BadConfigException("Минимальное время в промежутке " +
                    "(" + timespanFromTo.Item1.ToString(Program.TIME_FORMAT) + ") " +
                    "больше максимального времени " +
                    "(" + timespanFromTo.Item2.ToString(Program.TIME_FORMAT) + ")");

            TimeSpan? configIntervalBetweenFromTo = Program.CurrentConfig?.IntervalBetweenTimeRange;
            if (configIntervalBetweenFromTo != null)
            {
                if (!TimeSpanHelper.CheckIfIntervalMeetsRange(timespanFromTo.Item1, timespanFromTo.Item2,
                            configIntervalBetweenFromTo.Value, itemsPerWindow.Item2))
                    throw new BadIntervalException(timespanFromTo.Item1, timespanFromTo.Item2,
                    configIntervalBetweenFromTo.Value, itemsPerWindow.Item2);
                else
                    intervalBetweenFromTo = configIntervalBetweenFromTo.Value;
            }
            else
                intervalBetweenFromTo = TimeSpan.MinValue;
            #endregion

            if (configPoints is null)
            {
                Console.Clear();
                Console.WriteLine("Введите количество точек (максимум " + maxPoints + " точек включительно)");
                points = GetIntInRange(1, maxPoints);
            }
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Ввод данных для случайной генерации");
                try
                {
                    if (minMaxWindows is null)
                    {
                        Console.WriteLine("Минимальное и максимальное количество временных окон");
                        minMaxWindows = GetIntMinMax(min: 1);
                    }

                    if (timespanFromTo is null)
                    {
                        Console.Clear();
                        Console.WriteLine("Минимальное и максимальное время (от и до) во временном окне");
                        timespanFromTo = GetMinMaxHMTime();
                    }

                    if (configIntervalBetweenFromTo is null)
                    {
                        Console.Clear();
                        Console.WriteLine("Интервал между минимальным и максимальным временем (от и до)");
                        intervalBetweenFromTo = GetHMTimeFromConsole();
                    }

                    if (!TimeSpanHelper.CheckIfIntervalMeetsRange(timespanFromTo.Item1, timespanFromTo.Item2,
                        intervalBetweenFromTo, minMaxWindows.Item2))
                        throw new BadIntervalException(timespanFromTo.Item1, timespanFromTo.Item2,
                            intervalBetweenFromTo, minMaxWindows.Item2);

                    break;
                }
                catch (BadIntervalException ex)
                {
                    if (Program.CurrentConfig != null)
                        throw new BadConfigException(ex.Message);

                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Введите данные заново.");
                    WaitForInput();

                    goto MinsMaxStart; // считываем значения снова, заместо выставления переменных в null как ниже
                    //minMaxWindows = null; timespanFromTo = null; intervalBetweenFromTo = TimeSpan.MinValue;
                }
            }

            if (itemsPerWindow is null)
            {
                Console.Clear();
                Console.WriteLine("Минимальное и максимальное количество товаров на временное окно");
                itemsPerWindow = GetIntMinMax(1, maxItems);
            }

            if (itemsCountPerPosition is null)
            {
                Console.Clear();
                Console.WriteLine("Минимальное и максимальное количество товаров на одну позицию");
                itemsCountPerPosition = GetIntMinMax(1);
            }
        }

        /// <summary>
        /// Получить дату от пользователя (день, месяц, год) с форматированием операционной системы
        /// </summary>
        /// <returns>Объект даты</returns>
        public static DateTime GetDateFromUser()
        {
            DateTime? returnDateFromConfig = Program.CurrentConfig?.OrderDate;
            if (returnDateFromConfig != null)
                return returnDateFromConfig.Value;

            DateTime returnDate;
            StringBuilder dateFormat =
                new StringBuilder(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat.ShortDatePattern);
            dateFormat = dateFormat.Replace("d", "д").Replace("M", "м").Replace("y", "г");

            Console.WriteLine("Дата заказов\n" +
                "Введите дату в формате " + dateFormat.ToString());
            do
            {
                Console.Write(">");
            } while (!ParseHelper.TryParseDateTimeFromSystemCulture(Console.ReadLine(), out returnDate) ||
                    returnDate.TimeOfDay != TimeSpan.Zero);

            return returnDate;
        }

        /// <summary>
        /// Получить путь выходного файла для заказов от пользователя
        /// </summary>
        /// <param name="outputOrdersFile">Путь выходного файла</param>
        public static void GetOrderFilesPathsFromUser(out string outputOrdersFile)
        {
            outputOrdersFile = Program.CurrentConfig?.OrdersFilePathOutput;
            if (string.IsNullOrEmpty(outputOrdersFile))
            {
                Console.WriteLine($"Введите название выходного файла (Enter для стандартного имени {DEFAULT_ORDERS_OUTPUT_FILE}):");
                outputOrdersFile = Console.ReadLine();
                if (outputOrdersFile == string.Empty)
                    outputOrdersFile = DEFAULT_ORDERS_OUTPUT_FILE;
            }
        }

        /// <summary>
        /// Получить от пользователя координаты двух точек - северо-восточную и юго-западную
        /// </summary>
        /// <param name="northEast">Северо-восточная тока</param>
        /// <param name="southWest">Юго-западная</param>
        /// <returns>true если удалось ввести координаты, false если координаты введены неправильно, или 
        /// северо-восточный угол не больше юго-западного</returns>
        public static bool GetCoordinatesFromConsole(out PointF northEast, out PointF southWest)
        {
            Console.WriteLine("Введите координаты. Принимаются следующие форматы (без скобочек):\n" +
                            "(45.1234) - одна точка, последовательно принимаются все четыре точки координат;\n" +
                            "(45.1234, 45.4321) - точка северо-восточного угла, в последующем ожидается " +
                            "точка юго-западного угла;\n" +
                            "(45.1234, 45.4321, 40.1234, 40.4321) - точка северо-восточного и юго-западного угла.");

            string consoleInput = Console.ReadLine();
            string[] coordinatesStringArr = consoleInput.Replace(", ", ",").Split(",");
            float nX = -1, nY = -1, sX = -1, sY = -1;
            bool success = true;

            if (coordinatesStringArr.Length == 4)
            {
                success = float.TryParse(coordinatesStringArr[0], out nX) &&
                          float.TryParse(coordinatesStringArr[1], out nY) &&
                          float.TryParse(coordinatesStringArr[2], out sX) &&
                          float.TryParse(coordinatesStringArr[3], out sY);
            }
            else if (coordinatesStringArr.Length == 2)
            {
                success = float.TryParse(coordinatesStringArr[0], out nX);
                success = success is true && float.TryParse(coordinatesStringArr[1], out nY);

                if (success is false)
                    goto coordinatesEnd;

                do
                {
                    Console.Write("Введите координаты юго-западного угла: ");
                    consoleInput = Console.ReadLine();
                    coordinatesStringArr = consoleInput.Replace(", ", ",").Split(",");
                } while (coordinatesStringArr.Length != 2);

                success = float.TryParse(coordinatesStringArr[0], out sX);
                success = success is true && float.TryParse(coordinatesStringArr[1], out sY);
            }
            else
            {
                if (GetOneCoordinate(true, true, out nX) ||
                    GetOneCoordinate(false, true, out nY) ||
                    GetOneCoordinate(true, false, out sX) ||
                    GetOneCoordinate(false, false, out sY))
                    goto coordinatesEnd;
            }
        coordinatesEnd:
            northEast = new PointF(nX, nY);
            southWest = new PointF(sX, sY);

            if (northEast.CompareTo(southWest) == -1)
                return false;
            return success;
        }

        /// <summary>
        /// Получить одно значение координаты от пользователя
        /// </summary>
        /// <param name="isX">Является ли координата x-координатой</param>
        /// <param name="isNorthEast">Является ли координата северно-восточным углом</param>
        /// <param name="parsedResult">Выходная координата, если удалось спарсить</param>
        /// <returns>true если удалось спарсить, false если не удалось</returns>
        private static bool GetOneCoordinate(bool isX, bool isNorthEast, out float parsedResult)
        {
            Console.Write($"Введите {(isX ? "X" : "Y")} - координату " +
                $"{(isNorthEast ? "северо-восточного" : "юго-западного")} угла: ");
            return float.TryParse(Console.ReadLine(), out parsedResult);
        }

        /// <summary>
        /// Получить кортеж минимального и максимального значения от пользователя (доступен ввод в диапазоне)
        /// </summary>
        /// <param name="min">Минимальное значение включительно, либо null, чтобы не учитывать</param>
        /// <param name="max">Максимальное значение вкючительно, либо null, чтобы не учитывать</param>
        /// <returns>Кортеж минимального и максимального значений, введённых пользователем 
        /// (если указан диапазон - числа будут в этом диапазоне)</returns>
        public static Tuple<int, int> GetIntMinMax(int? min = null, int? max = null)
        {
            int newMin, newMax;

            Console.WriteLine("Введите МИНИМУМ" +
                (min.HasValue ? " от " + min.Value : "") +
                (max.HasValue ? " до " + max.Value : ""));
            newMin = GetIntInRange(min, max);

            min = min.HasValue ? newMin > min.Value ? newMin : min.Value : newMin;
            Console.WriteLine("Введите МАКСИМУМ" +
                " от " + min +
                (max.HasValue ? " до " + max.Value : ""));
            newMax = GetIntInRange(min, max);

            return new Tuple<int, int>(newMin, newMax);
        }

        /// <summary>
        /// Получить час и минуту от пользователя (с возможностью указать 24:00)
        /// </summary>
        /// <returns>Объект класса <see cref="TimeSpan"/> с часами и минутами, введённые пользователем</returns>
        public static TimeSpan GetHMTimeFromConsole()
        {
            TimeSpan result;
            string timeString;

            Console.WriteLine("Введите время в формате чч:мм");
            do
            {
                Console.Write(">");
                timeString = Console.ReadLine();
                if (timeString == "24:00")
                    timeString = "01:00:00:00";
            } while (!timeString.Contains(":")
            || !TimeSpan.TryParse(timeString, out result)
            || result < TimeSpan.Zero);

            return result;
        }

        /// <summary>
        /// Получить минимум и ммксимум времени от пользователя
        /// </summary>
        /// <returns>Кортеж с минимальным и максимальным временами</returns>
        public static Tuple<TimeSpan, TimeSpan> GetMinMaxHMTime()
        {
            Console.WriteLine("Минимум времени");
            TimeSpan from = GetHMTimeFromConsole();

            TimeSpan to;
            do
            {
                Console.WriteLine("Максимум времени (не должно быть меньше минимума)");
                to = GetHMTimeFromConsole();
            } while (to < from);

            return new Tuple<TimeSpan, TimeSpan>(from, to);
        }

        #region Tech Methods
        /// <summary>
        /// Ввод с консоли только разрешённых строк
        /// </summary>
        /// <param name="allowedInputs">Разрешённый ввод сплошной строкой, разделяется пробелом. 
        /// См. <see cref="GetInputFromConsole(string[])"/></param>
        /// <returns>Строка, введённая пользователем</returns>
        public static string GetInputFromConsole(string allowedInputs)
        {
            return GetInputFromConsole(allowedInputs.Trim().Split(" "));
        }

        /// <summary>
        /// Ввод с консоли только разрешённых строк
        /// </summary>
        /// <param name="allowedInputs">Массив строк, которые может ввести пользователь</param>
        /// <returns>Строка, введёная пользователем, которая гарантированно будет в массиве <paramref name="allowedInputs"/></returns>
        /// <remarks>Данный метод позволяет указать только тот ввод, который может ввести пользователь. Этот метод 
        /// полезен при выборе пунктов в меню, чтобы пользователь не мог вводить что угодно, а только то, 
        /// что необходимо программе</remarks>
        /// <example>
        /// <code>
        /// Перегрузка метода со сплошной строкой:
        /// GetInputFromConsole("W Q D S PP");
        /// Пользователь сможет ввести только символы W, Q, D, S, либо строку PP
        /// </code>
        /// </example>
        public static string GetInputFromConsole(string[] allowedInputs)
        {
            for (int i = 0; i < allowedInputs.Length; i++)
                allowedInputs[i] = allowedInputs[i].ToUpper();
            string consoleInput;
            do
            {
                Console.Write(">");
                consoleInput = Console.ReadLine().ToUpper();
            } while (!allowedInputs.Contains(consoleInput));
            return consoleInput;
        }

        /// <summary>
        /// Остановить вывод консоли, пока пользователь не нажмёт любую кнопку
        /// </summary>
        public static void WaitForInput()
        {
            Console.WriteLine(INPUT_AWAIT_MSG);
            Console.ReadKey();
        }

        /// <summary>
        /// Получить переменную типа <see cref="int"/> от пользователя
        /// </summary>
        /// <returns>Целое число, введённое пользователем</returns>
        public static int GetIntFromConsole()
        {
            int result;
            string consoleInput;

            do
            {
                Console.Write(">");
                consoleInput = Console.ReadLine();
            } while (!int.TryParse(consoleInput, out result));
            return result;
        }

        /// <summary>
        /// Получить переменную типа <see cref="double"/> от пользователя
        /// </summary>
        /// <returns>Число двойной точности, введённое пользователем</returns>
        public static double GetDoubleFromConsole()
        {
            double result;
            string consoleInput;

            do
            {
                Console.Write(">");
                consoleInput = Console.ReadLine();
            } while (!double.TryParse(consoleInput, out result));
            return result;
        }

        /// <summary>
        /// Получить переменную типа <see cref="float"/> от пользователя
        /// </summary>
        /// <returns>Число с плавающей точкой, введённое пользователем</returns>
        public static float GetFloatFromConsole()
        {
            float result;
            string consoleInput;

            do
            {
                Console.Write(">");
                consoleInput = Console.ReadLine();
            } while (!float.TryParse(consoleInput, out result));
            return result;
        }

        /// <summary>
        /// Получить переменную типа <see cref="int"/> от пользователя в указанном диапазоне
        /// </summary>
        /// <param name="rangeFrom">Минимальное значение включительно, либо null, чтобы не учитывать</param>
        /// <param name="rangeTo">Максимальное значение вкючительно, либо null, чтобы не учитывать</param>
        /// <returns>Число, введённое пользователем, в указанном диапазоне</returns>
        public static int GetIntInRange(int? rangeFrom = null, int? rangeTo = null)
        {
            int result;

            do
            {
                Console.Write(">");
            } while (!int.TryParse(Console.ReadLine(), out result) ||
                rangeFrom.HasValue && result < rangeFrom.Value ||
                rangeTo.HasValue && result > rangeTo.Value);
            return result;
        }
        #endregion
    }
}
