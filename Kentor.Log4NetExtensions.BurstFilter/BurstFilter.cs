using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.Log4NetExtensions
{
    public class BurstFilter : log4net.Filter.FilterSkeleton
    {
        public BurstFilter()
        {
            BurstLength = TimeSpan.FromMinutes(1);
            BurstSize = 60;
        }
        private readonly ConcurrentQueue<DateTime> queuedEvents = new ConcurrentQueue<DateTime>();
        public override log4net.Filter.FilterDecision Decide(log4net.Core.LoggingEvent loggingEvent)
        {
            var logTime = loggingEvent.TimeStamp;
            DateTime firstLogTime;
            if (queuedEvents.Count + 1 > BurstSize)
            {
                if (queuedEvents.TryPeek(out firstLogTime))
                {
                    if (firstLogTime.Add(BurstLength) > logTime)
                    {
                        return log4net.Filter.FilterDecision.Deny;
                    }
                    else
                    {
                        // Trim peaked event
                        queuedEvents.TryDequeue(out firstLogTime);
                    }
                }
            }

            queuedEvents.Enqueue(logTime);
            return log4net.Filter.FilterDecision.Neutral;
        }

        public TimeSpan BurstLength { get; set; }
        public int BurstSize { get; set; }
    }
}
