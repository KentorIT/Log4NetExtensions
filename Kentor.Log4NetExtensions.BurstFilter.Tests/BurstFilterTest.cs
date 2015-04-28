using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Kentor.Log4NetExtensions.Tests
{
    [TestClass]
    public class BurstFilterTest
    {
        private const int bufferLengthMilliseconds = 25;

        [TestMethod]
        public void DenyWhenBufferIsFull()
        {
            var filter = new BurstFilter
            {
                BurstLength = TimeSpan.FromMilliseconds(bufferLengthMilliseconds),
                BurstSize = 2,
            };
            var result1 = filter.Decide(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData()));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result1);
            var result2 = filter.Decide(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData()));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result2);
            var result3 = filter.Decide(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData()));
            Assert.AreEqual(log4net.Filter.FilterDecision.Deny, result3);
        }

        [TestMethod]
        public void NeutralWithEnoughDelay()
        {
            var filter = new BurstFilter
            {
                BurstLength = TimeSpan.FromMilliseconds(bufferLengthMilliseconds),
                BurstSize = 2,
            };
            var result1 = filter.Decide(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData()));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result1);
            var result2 = filter.Decide(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData()));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result2);

            Thread.Sleep(2 * bufferLengthMilliseconds);

            var result3 = filter.Decide(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData()));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result3);
        }
    }
}
