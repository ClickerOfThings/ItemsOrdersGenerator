using ItemsOrdersGenerator.Attributes;
using ItemsOrdersGenerator.Attributes.Validation;
using ItemsOrdersGenerator.Exceptions;
using ItemsOrdersGenerator.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace ItemOrderDemonstration.Helpers
{
    internal static class ConfigHelper
    {
        /// <summary>
        /// Десериализовать объект конфигурации из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу в формате json </param>
        /// <returns>Объект конфигурации с заполненными полями из файла</returns>
        /// <exception cref="BadConfigException">Одно из полей не прошло проверку</exception>
        public static Config ReadFromFile(string filePath)
        {
            Config resultConfig;

            JsonSerializer serializer = JsonSerializer.Create();
            using (StreamReader reader = new StreamReader(File.Open(filePath, FileMode.Open)))
            {
                resultConfig = serializer.Deserialize(reader, typeof(Config)) as Config;
            }

            foreach (PropertyInfo property in resultConfig.GetType().GetProperties())
            {
                if (property.GetValue(resultConfig) is null)
                    continue;
                foreach (Attribute attribute in property.GetCustomAttributes())
                {
                    if (attribute is IPropertyValidation validation)
                    {
                        if (!validation.ValidateProperty(property.GetValue(resultConfig)))
                        {
                            throw new BadConfigException("Поле " + property.Name + " не прошло проверку: "
                                + validation.ErrorMessage);
                        }
                    }
                }
            }

            return resultConfig;
        }

        /// <summary>
        /// Вывести помощь по переменным конфигурационного файла в консоль
        /// </summary>
        public static void ListConfigHelp()
        {
            Console.Clear();
            Console.WriteLine("Справка по полям конфигурационного файла:");

            var defaultConsoleColour = Console.ForegroundColor;
            var configProperties = typeof(Config).GetProperties();
            foreach(PropertyInfo property in configProperties)
            {
                var jsonPropertyAttribute = property.GetCustomAttribute(typeof(Newtonsoft.Json.JsonPropertyAttribute))
                    as Newtonsoft.Json.JsonPropertyAttribute;
                if (jsonPropertyAttribute is null)
                    continue;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Поле " + (jsonPropertyAttribute.PropertyName ?? property.Name));

                var descriptionAttribute = property.GetCustomAttribute(typeof(System.ComponentModel.DescriptionAttribute));
                if (descriptionAttribute != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Описание поля: ");
                    Console.WriteLine((descriptionAttribute as System.ComponentModel.DescriptionAttribute).Description);
                }

                var formatAttribute = property.GetCustomAttribute(typeof(FormatAttribute));
                if (formatAttribute != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Формат поля: ");
                    Console.WriteLine((formatAttribute as FormatAttribute).FormatDescription);
                }

                Console.WriteLine();
            }
            Console.ForegroundColor = defaultConsoleColour;
        }
    }
}
