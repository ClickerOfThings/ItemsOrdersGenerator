using ItemOrderDemonstration;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ItemsOrdersGenerator.Classes.Model
{
    /// <summary>
    /// Класс запроса в заказе
    /// </summary>
    [XmlType("demand")]
    public class Demand
    {
        /// <summary>
        /// Начало время запроса (не сериализируется в xml файле)
        /// </summary>
        [XmlIgnore]
        public TimeSpan From { get; set; }
        /// <summary>
        /// View-свойство начала времени запроса, который сериализируется 
        /// в xml файл с форматированием из метода <see cref="FormatTimeSpanValues(TimeSpan)"/>
        /// </summary>
        [XmlAttribute("from")]
        public string FromView
        {
            get
            {
                return FormatTimeSpanValues(From);
            }
            set
            {
                From = TimeSpan.Parse(value);
            }
        }
        /// <summary>
        /// Окончание время запроса (не сериализируется в xml файле)
        /// </summary>
        [XmlIgnore]
        public TimeSpan To { get; set; }
        /// <summary>
        /// View-свойство конца времени запроса, который сериализируется 
        /// в xml файл с форматированием из метода <see cref="FormatTimeSpanValues(TimeSpan)"/>
        /// </summary>
        [XmlAttribute("to")]
        public string ToView
        {
            get
            {
                return FormatTimeSpanValues(To);
            }
            set
            {
                To = TimeSpan.Parse(value);
            }
        }
        /// <summary>
        /// Позиция товара
        /// </summary>
        [XmlElement("position")]
        public Position Position { get; set; }
        /// <summary>
        /// Форматирование значений из TimeSpan для форматирования в xml файл заказов
        /// </summary>
        /// <param name="tsToProcess">Объект TimeSpan, который необходимо отформатировать</param>
        /// <returns>Отформатированная строка из объекта TimeSpan</returns>
        /// <remarks>
        /// <para>
        /// Форматирование времени: если <paramref name="tsToProcess"/> пустой, то пустая строка;
        /// </para>
        /// <para>
        /// если <paramref name="tsToProcess"/> имеет только час, то час;
        /// </para>
        /// <para>
        /// если <paramref name="tsToProcess"/> имеет и час и минуты, то в формате чч:мм (если час меньше десяти, то 
        /// выводится одна цифра часа).
        /// </para>
        /// </remarks>
        private string FormatTimeSpanValues(TimeSpan tsToProcess)
        {
            if (tsToProcess.Hours == 0 && tsToProcess.Minutes == 0)
                return string.Empty;
            if (tsToProcess.Minutes == 0)
                return tsToProcess.Hours.ToString();
            else
            {
                StringBuilder resultTimeFormatted = new StringBuilder(string.Format("{0:00}", tsToProcess.Hours));
                resultTimeFormatted.Append(":");
                resultTimeFormatted.Append(string.Format("{0:00}", tsToProcess.Minutes));
                return resultTimeFormatted.ToString();
            }
        }
        /// <summary>
        /// Итератор случайных временных отрезков
        /// </summary>
        /// <param name="randObj">Объект генерации случайных чисел</param>
        /// <param name="minimumTime">Минимальное время, в пределе которого будут создаваться временные отрезки</param>
        /// <param name="maximumTime">Максимальное время, в пределе которого будут создаваться временные отрезки</param>
        /// <param name="intervalBetweenFromTo">Интервал между отрезками времени от и до</param>
        /// <param name="timeWindowsCount">Количество временных окон</param>
        /// <returns>Итератор случайных временных отрезков</returns>
        /// <exception cref="BadIntervalException">Невозможно разделить всё время (от минимального к максимальному) 
        /// на равные временные отрезки</exception>
        public static IEnumerable<Tuple<TimeSpan, TimeSpan>> RandomTimespans(Random randObj,
            TimeSpan minimumTime, TimeSpan maximumTime,
            TimeSpan intervalBetweenFromTo,
            int timeWindowsCount)
        {
            if (!CheckIfIntervalMeetsRange(minimumTime, maximumTime, intervalBetweenFromTo, timeWindowsCount))
                throw new BadIntervalException(minimumTime, maximumTime, intervalBetweenFromTo, timeWindowsCount);

            TimeSpan from, to;
            TimeSpan interval = (maximumTime - minimumTime) / timeWindowsCount;

            for (int i = 0; i < timeWindowsCount; i++)
            {
                while (true)
                {
                    TimeSpan currentFragmentFrom = minimumTime + interval * i;
                    TimeSpan currentFragmentTo = minimumTime + interval * (i + 1);

                    from = RandomTimespan(randObj,
                        currentFragmentFrom.Ticks,
                        currentFragmentTo.Ticks);
                    from = from.Subtract(new TimeSpan(0, from.Minutes % 5, 0));

                    currentFragmentFrom = from.Add(intervalBetweenFromTo);
                    if (currentFragmentFrom > currentFragmentTo)
                        continue;

                    to = RandomTimespan(randObj,
                        currentFragmentFrom.Ticks,
                        currentFragmentTo.Ticks);
                    to = to.Subtract(new TimeSpan(0, to.Minutes % 5, 0));

                    break;
                }
                yield return new Tuple<TimeSpan, TimeSpan>(from, to);
            }
        }
        /// <summary>
        /// Случайное время в пределах временного отрезка
        /// </summary>
        /// <param name="randObj">Объект генерации случайных чисел</param>
        /// <param name="ticksFrom">Тики минимального времени, в промежутке которого будет сгенерировано время</param>
        /// <param name="ticksTo">Тики максимального времени, в промежутке которого будет сгенерировано время</param>
        /// <returns>Случайно сгенерирванное время без секунд и мс в пределах 
        /// от <paramref name="ticksFrom"/> до <paramref name="ticksTo"/></returns>
        private static TimeSpan RandomTimespan(Random randObj, long ticksFrom, long ticksTo)
        {
            double randomNumber = randObj.NextDouble();
            TimeSpan resultTimepan = new TimeSpan((long)(ticksFrom + (ticksTo - ticksFrom) * randomNumber));
            return resultTimepan.Subtract(new TimeSpan(0, 0, 0, resultTimepan.Seconds, resultTimepan.Milliseconds));
        }
        /// <summary>
        /// Получить одну часть от деления временного отрезка на <paramref name="divideIntoParts"/>
        /// </summary>
        /// <param name="from">Начало временного отрезка</param>
        /// <param name="to">Конец временного отрезка</param>
        /// <param name="divideIntoParts">Количество частейЮ на которое требуется разделить временной отрезок</param>
        /// <returns>Часть деления временного отрезка</returns>
        public static TimeSpan GetOnePartIntervalBetween(TimeSpan from, TimeSpan to, int divideIntoParts)
            => (to - from) / divideIntoParts;
        /// <summary>
        /// Проверить, возможен ли интервал во временном промежутке с определённым количеством временных окон
        /// </summary>
        /// <param name="minimumTime">Временной промежуток от</param>
        /// <param name="maximumTime">Временной промежуток до</param>
        /// <param name="intervalBetweenFromTo">Интервал между промежутками от и до</param>
        /// <param name="timeWindowsCount">Количество временных окон</param>
        /// <returns>true если интервал возможно поместить в промежуток, false если нет</returns>
        public static bool CheckIfIntervalMeetsRange(TimeSpan minimumTime, TimeSpan maximumTime,
            TimeSpan intervalBetweenFromTo,
            int timeWindowsCount)
        {
            TimeSpan interval = GetOnePartIntervalBetween(minimumTime, maximumTime, timeWindowsCount);
            return interval > intervalBetweenFromTo;
        }
    }
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
                .Replace("%0", Demand.GetOnePartIntervalBetween(minimalTime, maximumTime, timeWindows)
                    .ToString(Program.TIME_FORMAT))
                .Replace("%1", interval.ToString(Program.TIME_FORMAT))
                .ToString();
        }
    }
}
