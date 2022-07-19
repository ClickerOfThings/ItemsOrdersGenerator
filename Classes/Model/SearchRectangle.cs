using ItemOrderDemonstration.Classes;
using Newtonsoft.Json;
using System.Drawing;

namespace ItemsOrdersGenerator.Classes.Model
{
    /// <summary>
    /// Класс прямоугольника для поиска
    /// </summary>
    internal class SearchRectangle
    {
        /// <summary>
        /// Создать объект класса из двух точек <see cref="PointF"/>
        /// </summary>
        /// <param name="northEastCorner">Северо-восточная точка</param>
        /// <param name="southWestCorner">Юго-западная точка</param>
        [JsonConstructor()]
        public SearchRectangle(PointF northEastCorner, PointF southWestCorner)
        {
            NorthEastCorner = northEastCorner;
            SouthWestCorner = southWestCorner;
        }

        /// <summary>
        /// Создать объект класса из четырёх float значений
        /// </summary>
        /// <param name="NEX">Горизонтальная позиция (X) северо-восточной точки</param>
        /// <param name="NEY">Вертикальная позиция (Y) северо-восточной точки</param>
        /// <param name="SWX">Горизонтальная позиция (X) юго-западной точки</param>
        /// <param name="SWY">Вертикальная позиция (Y) юго-западной точки</param>
        public SearchRectangle(float NEX, float NEY, float SWX, float SWY)
        {
            NorthEastCorner = new PointF(NEX, NEY);
            SouthWestCorner = new PointF(SWX, SWY);
        }

        /// <summary>
        /// Северо-восточная точка
        /// </summary>
        [JsonRequired]
        [JsonProperty]
        [JsonConverter(typeof(PointFStringJsonConverter))]
        public PointF NorthEastCorner { get; set; }

        /// <summary>
        /// Юго-западная точка
        /// </summary>
        [JsonRequired]
        [JsonProperty]
        [JsonConverter(typeof(PointFStringJsonConverter))]
        public PointF SouthWestCorner { get; set; }
    }
}
