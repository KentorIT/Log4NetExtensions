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
        private static readonly ConcurrentQueue<DateTime> queuedEvents = new ConcurrentQueue<DateTime>();
        public override log4net.Filter.FilterDecision Decide(log4net.Core.LoggingEvent loggingEvent)
        {
            var utcNow = DateTime.UtcNow;
            queuedEvents.Enqueue(utcNow);
            DateTime firstTimeUtc = DateTime.MinValue;
            bool hasEntry = false;
            while (queuedEvents.Count > BurstSize)
            {
                if (queuedEvents.TryDequeue(out firstTimeUtc))
                {
                    hasEntry = true;
                }
            }
            if (hasEntry)
            {
                if (firstTimeUtc.Add(BurstLength) > utcNow)
                {
                    return log4net.Filter.FilterDecision.Deny;
                }
            }
            return log4net.Filter.FilterDecision.Neutral;
        }

        public TimeSpan BurstLength { get; set; }
        public int BurstSize { get; set; }
    }
}
