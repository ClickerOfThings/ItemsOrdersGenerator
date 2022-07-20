using ItemsOrdersGenerator.Exceptions;
using System;
using System.Collections.Generic;

namespace ItemsOrdersGenerator.Helpers
{
    /// <summary>
    /// Вспомогательный класс для действий с классом <see cref="TimeSpan"/>
    /// </summary>
    internal static class TimeSpanHelper
    {
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
}
