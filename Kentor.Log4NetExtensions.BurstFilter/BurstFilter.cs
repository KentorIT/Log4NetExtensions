using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.Log4NetExtensions
{
    /// <summary>
    /// Burst filter.
    /// Limits the number of <see cref="log4net.Core.LoggingEvent"/>s that will be logged during a given time interval.
    /// At most <see cref="BurstSize"/> <see cref="log4net.Core.LoggingEvent"/>s will be logged during a rolling time of <see cref="BurstLength"/>
    /// </summary>
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

        /// <summary>
        /// Gets or sets the burst length. Default value is one minute (00:01:00).
        /// </summary>
        /// <value>
        /// The burst length, a rolling interval where at most <see cref="BurstSize"/> <see cref="log4net.Core.LoggingEvent"/>s will be passed through
        /// </value>
        public TimeSpan BurstLength { get; set; }

        /// <summary>
        /// Gets or sets the burst size. Default value is 60.
        /// </summary>
        /// <value>
        /// The burst size, the maximum number of <see cref="log4net.Core.LoggingEvent"/>s will be passed through during <see cref="BurstLength"/>
        /// </value>
        public int BurstSize { get; set; }
    }
}
