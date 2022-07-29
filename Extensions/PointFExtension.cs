using System.Drawing;

namespace ItemsOrdersGenerator.Extensions
{
    /// <summary>
    /// Класс расширения <see cref="PointF"/>
    /// </summary>
    internal static class PointFExtension
    {
        /// <summary>
        /// Сравнить два объекта класса <see cref="PointF"/>
        /// </summary>
        /// <param name="first">Первая точка</param>
        /// <param name="second">Вторая точка</param>
        /// <returns>1 если X и Y первой точки больше второй, 
        /// -1 если X и Y первой точки меньше второй, 
        /// 0 если точки равны, или в случае, 
        /// если одно из переменных (X или Y) первого объекта меньше второго, 
        /// а другая переменная (X или Y) первого объекта больше второго</returns>
        public static int CompareTo(this PointF first, PointF second)
        {
            if (first.X > second.X &&
                first.Y > second.Y)
                return 1;
            else if (first.X < second.X &&
                first.Y < second.Y)
                return -1;
            else
                return 0;
        }
    }
}
