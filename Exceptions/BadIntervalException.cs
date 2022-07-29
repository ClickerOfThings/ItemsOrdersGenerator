using ItemOrderDemonstration;
using ItemsOrdersGenerator.Helpers;
using System;
using System.Text;

namespace ItemsOrdersGenerator.Exceptions
{
    /// <summary>
    /// Исключение неправильного интервала
    /// </summary>
    public class BadIntervalException : Exception
    {
        public override string Message { get; }

        public BadIntervalException(TimeSpan minimalTime, TimeSpan maximumTime, TimeSpan interval, int timeWindows)
        {
            string maxAllowedIntervalString =
                TimeSpanHelper.GetOnePartIntervalBetween(minimalTime, maximumTime, timeWindows)
                    .ToString(Program.TIME_FORMAT);
            string currentIntervalString = interval.ToString(Program.TIME_FORMAT);

            Message = "Слишком большой промежуток, либо слишком большое количество временных окон. " +
                    $"Максимально возможный промежуток между минимумом и максимумом - {maxAllowedIntervalString}" +
                    $", текущий промежуток - {currentIntervalString}";
        }
    }
}
