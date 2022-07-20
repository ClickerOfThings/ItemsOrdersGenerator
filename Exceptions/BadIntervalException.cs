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
        private const string BAD_INTERVAL_MSG =
                    "Слишком большой промежуток, либо слишком большое количество временных окон. " +
                    "Максимально возможный промежуток между минимумом и максимумом - %0" +
                    ", текущий промежуток - %1";
        public override string Message { get; }

        public BadIntervalException(TimeSpan minimalTime, TimeSpan maximumTime, TimeSpan interval, int timeWindows)
        {
            StringBuilder resultErrorMessage = new StringBuilder(BAD_INTERVAL_MSG);
            Message = resultErrorMessage
                .Replace("%0", TimeSpanHelper.GetOnePartIntervalBetween(minimalTime, maximumTime, timeWindows)
                    .ToString(Program.TIME_FORMAT))
                .Replace("%1", interval.ToString(Program.TIME_FORMAT))
                .ToString();
        }
    }
}
