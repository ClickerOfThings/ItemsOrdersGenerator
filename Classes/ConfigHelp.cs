﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace ItemOrderDemonstration.Classes
{
    internal partial class Config
    {
        public static void ConfigHelp()
        {
            Console.Clear();
            Console.WriteLine("Справка по полям конфигурационного файла:");

            var configProperties = typeof(Config).GetProperties();
            var standardColour = Console.ForegroundColor;
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
                var formatAttribute = property.GetCustomAttribute(typeof(Classes.FormatAttribute));
                if (formatAttribute != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Формат поля: ");
                    Console.WriteLine((formatAttribute as Classes.FormatAttribute).FormatDescription);
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = standardColour;

            //throw new NotImplementedException();
            return;
        }
    }
}
