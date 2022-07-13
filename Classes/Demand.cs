using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ItemOrderDemonstration.Classes
{
    [XmlType("demand")]
    public class Demand
    {
        [XmlIgnore]
        public TimeSpan From { get; set; }
        [XmlAttribute("from")]
        public string FromView
        {
            get
            {
                return process_timespan(From);
            }
            set
            {
                From = TimeSpan.Parse(value);
            }
        }
        [XmlIgnore]
        public TimeSpan To { get; set; }
        [XmlAttribute("to")]
        public string ToView
        {
            get
            {
                return process_timespan(To);
            }
            set
            {
                To = TimeSpan.Parse(value);
            }
        }
        [XmlElement("position")]
        public Position Position { get; set; }

        private string process_timespan(TimeSpan tsToProcess)
        {
            if (tsToProcess.Hours == 0 && tsToProcess.Minutes == 0)
                return string.Empty;
            if (tsToProcess.Minutes <= 0)
                return tsToProcess.Hours.ToString();
            else
            {
                StringBuilder builder = new StringBuilder(String.Format("{0:00}", tsToProcess.Hours));
                builder.Append(":");
                builder.Append(String.Format("{0:00}", tsToProcess.Minutes));
                return builder.ToString();
            }
        }
        public static void ThrowIntervalException(TimeSpan minTime, TimeSpan maxTime, int timeWins, TimeSpan interval)
            => throw new BadIntervalException("Слишком большой промежуток, либо слишком большое количество временных окон. " +
                    "Максимально возможный промежуток между минимумом и максимумом - " + 
                    GetPartBetween(minTime, maxTime, timeWins).ToString(@"hh\:mm") + 
                    ", текущий промежуток - " +
                    interval.ToString(@"hh\:mm"));

        public static IEnumerable<Tuple<TimeSpan, TimeSpan>> RandomTimespans(Random randObj,
            TimeSpan minimumTime, TimeSpan maximumTime,
            TimeSpan intervalBetweenFromTo,
            int timeWindowsCount)
        {
            if (!(CheckIfIntervalMeetsRange(minimumTime, maximumTime, intervalBetweenFromTo, timeWindowsCount)))
                ThrowIntervalException(minimumTime, maximumTime, timeWindowsCount, intervalBetweenFromTo);

            TimeSpan from, to;
            TimeSpan interval = (maximumTime - minimumTime) / timeWindowsCount;
            TimeSpan fragment = minimumTime;
            for (int i = 0; i < timeWindowsCount; i++)
            {
                while (true)
                {
                    TimeSpan currentFragmentFrom = fragment + interval * i;
                    TimeSpan currentFragmentTo = fragment + interval * (i + 1);
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

        private static TimeSpan RandomTimespan(Random randObj, long ticksFrom, long ticksTo)
        {
            double ranNum = randObj.NextDouble();
            TimeSpan temp = new TimeSpan((long)(ticksFrom + (ticksTo - ticksFrom) * ranNum));
            return temp.Subtract(new TimeSpan(0, 0, 0, temp.Seconds, temp.Milliseconds));
        }
        public static TimeSpan GetPartBetween(TimeSpan from, TimeSpan to, int divide)
            => (to - from) / divide;
        public static bool CheckIfIntervalMeetsRange(TimeSpan minimumTime, TimeSpan maximumTime,
            TimeSpan intervalBetweenFromTo,
            int timeWindowsCount)
        {
            TimeSpan interval = GetPartBetween(minimumTime, maximumTime, timeWindowsCount);
            return interval > intervalBetweenFromTo;
        }
    }

    public class BadIntervalException : Exception
    {
        public BadIntervalException(string message) : base(message)
        {
        }
    }
}
