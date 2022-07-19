using Newtonsoft.Json;
using System;

namespace ItemsOrdersGenerator.Classes.View
{
    /// <summary>
    /// Кортеж с минимальным и максимальным значениями
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class MinMaxTupleJson<T1, T2> : Tuple<T1, T2>
    {
        /// <summary>
        /// Минимальное значение
        /// </summary>
        [JsonProperty(PropertyName = "Min")]
        [JsonRequired]
        public new T1 Item1 { get; }

        /// <summary>
        /// Максимальное значение
        /// </summary>
        [JsonProperty(PropertyName = "Max")]
        [JsonRequired]
        public new T2 Item2 { get; }

        /// <summary>
        /// Стандартный конструктор кортежа
        /// </summary>
        /// <param name="item1">Минимальное значение</param>
        /// <param name="item2">Максимальное значение</param>
        public MinMaxTupleJson(T1 item1, T2 item2) : base(item1, item2)
        {
            Item1 = base.Item1;
            Item2 = base.Item2;
        }
    }

    /// <summary>
    /// Класс кортежа, используемый, в основном, для обозначения времени "от" и "до"
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class FromToTupleJson<T1, T2> : Tuple<T1, T2>
    {
        /// <summary>
        /// Значение "от"
        /// </summary>
        [JsonProperty(PropertyName = "From")]
        [JsonRequired]
        public new T1 Item1 { get; }

        /// <summary>
        /// Значение "до"
        /// </summary>
        [JsonProperty(PropertyName = "To")]
        [JsonRequired]
        public new T2 Item2 { get; }

        /// <summary>
        /// Стандартный конструктор кортежа
        /// </summary>
        /// <param name="item1">Значение "от"</param>
        /// <param name="item2">Значение "до"</param>
        public FromToTupleJson(T1 item1, T2 item2) : base(item1, item2)
        {
            Item1 = base.Item1;
            Item2 = base.Item2;
        }
    }
}
