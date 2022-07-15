using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemsOrdersGenerator.Classes.Extensions
{
    internal static class ListExtension
    {
        public static void ShuffleList<T>(this List<T> listToShuffle)
        {
            listToShuffle = listToShuffle.OrderBy(x => Helpers.GeneralHelper.rand.NextDouble()).ToList();
        }
    }
}
