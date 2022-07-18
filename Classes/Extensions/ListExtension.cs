using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemsOrdersGenerator.Classes.Extensions
{
    /// <summary>
    /// Класс расширения списка
    /// </summary>
    internal static class ListExtension
    {
        /// <summary>
        /// Перемешать все элементы в списке
        /// </summary>
        /// <param name="listToShuffle">Список, элементы в котором будут перемешаны</param>
        public static void ShuffleList<T>(this List<T> listToShuffle)
        {
            listToShuffle = listToShuffle.OrderBy(x => Helpers.GeneralHelper.rand.NextDouble()).ToList();
        }
    }
}
