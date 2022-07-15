using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemsOrdersGenerator.Classes.View
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class MinMaxTupleJson<T1, T2> : Tuple<T1, T2>
    {
        [JsonProperty(PropertyName = "Min")]
        [JsonRequired]
        public new T1 Item1 { get; }
        [JsonProperty(PropertyName = "Max")]
        [JsonRequired]
        public new T2 Item2 { get; }
        public MinMaxTupleJson(T1 item1, T2 item2) : base(item1, item2)
        {
            Item1 = base.Item1;
            Item2 = base.Item2;
        }
    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class FromToTupleJson<T1, T2> : Tuple<T1, T2>
    {
        [JsonProperty(PropertyName = "From")]
        [JsonRequired]
        public new T1 Item1 { get; }
        [JsonProperty(PropertyName = "To")]
        [JsonRequired]
        public new T2 Item2 { get; }
        public FromToTupleJson(T1 item1, T2 item2) : base(item1, item2)
        {
            Item1 = base.Item1;
            Item2 = base.Item2;
        }
    }
}
