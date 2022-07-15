using ItemOrderDemonstration.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ItemsOrdersGenerator.Classes.Model
{
    internal class SearchRectangle
    {
        [JsonConstructor()]
        public SearchRectangle(PointF northEastCorner, PointF southWestCorner)
        {
            NorthEastCorner = northEastCorner;
            SouthWestCorner = southWestCorner;
        }
        public SearchRectangle(float NEX, float NEY, float SWX, float SWY)
        {
            NorthEastCorner = new PointF(NEX, NEY);
            SouthWestCorner = new PointF(SWX, SWY);
        }
        [JsonRequired]
        [JsonProperty]
        [JsonConverter(typeof(PointFStringJsonConverter))]
        public PointF NorthEastCorner { get; set; }
        [JsonRequired]
        [JsonProperty]
        [JsonConverter(typeof(PointFStringJsonConverter))]
        public PointF SouthWestCorner { get; set; }
    }
}
