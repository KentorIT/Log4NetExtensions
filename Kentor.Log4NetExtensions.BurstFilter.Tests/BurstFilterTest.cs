using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kentor.Log4NetExtensions.Tests
{
    [TestClass]
    public class BurstFilterTest
    {
        private const int bufferLengthSeconds = 10;

        private static readonly DateTime defaultTestDate = new DateTime(2000, 01, 01);
        private static log4net.Core.LoggingEvent GetLoggingEventWithOffset(int offsetSeconds)
        {
            return new log4net.Core.LoggingEvent(
                new log4net.Core.LoggingEventData
                {
                    TimeStamp = defaultTestDate.AddSeconds(offsetSeconds)
                });
        }

        [TestMethod]
        public void DenyWhenBufferIsFull()
        {
            var filter = new BurstFilter
            {
                BurstLength = TimeSpan.FromSeconds(bufferLengthSeconds),
                BurstSize = 2,
            };
            var result1 = filter.Decide(GetLoggingEventWithOffset(0));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result1);
            var result2 = filter.Decide(GetLoggingEventWithOffset(1));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result2);
            var result3 = filter.Decide(GetLoggingEventWithOffset(2));
            Assert.AreEqual(log4net.Filter.FilterDecision.Deny, result3);
        }

        [TestMethod]
        public void NeutralWithEnoughDelay()
        {
            var filter = new BurstFilter
            {
                BurstLength = TimeSpan.FromSeconds(bufferLengthSeconds),
                BurstSize = 2,
            };
            var result1 = filter.Decide(GetLoggingEventWithOffset(0));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result1);
            var result2 = filter.Decide(GetLoggingEventWithOffset(1));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result2);

            var result3 = filter.Decide(GetLoggingEventWithOffset(11));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result3);
        }

        [TestMethod]
        public void ContinueLoggingOnLongBursts()
        {
            var filter = new BurstFilter
            {
                BurstLength = TimeSpan.FromSeconds(bufferLengthSeconds),
                BurstSize = 2,
            };
            var result1 = filter.Decide(GetLoggingEventWithOffset(0));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result1);
            var result2 = filter.Decide(GetLoggingEventWithOffset(5));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result2);
            for (var i = 6; i <= 9; i++)
            {
                var resultN = filter.Decide(GetLoggingEventWithOffset(i));
                Assert.AreEqual(log4net.Filter.FilterDecision.Deny, resultN);
            }

            // After 10 seconds the first slot should be cleared
            var result10 = filter.Decide(GetLoggingEventWithOffset(10));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result10);
            // and once again full
            var result11 = filter.Decide(GetLoggingEventWithOffset(11));
            Assert.AreEqual(log4net.Filter.FilterDecision.Deny, result11);
            var result12 = filter.Decide(GetLoggingEventWithOffset(12));
            Assert.AreEqual(log4net.Filter.FilterDecision.Deny, result12);
            var result13 = filter.Decide(GetLoggingEventWithOffset(13));
            Assert.AreEqual(log4net.Filter.FilterDecision.Deny, result13);
            var result14 = filter.Decide(GetLoggingEventWithOffset(14));
            Assert.AreEqual(log4net.Filter.FilterDecision.Deny, result14);
            // And finally the event after 5 seconds is gone too
            var result15 = filter.Decide(GetLoggingEventWithOffset(15));
            Assert.AreEqual(log4net.Filter.FilterDecision.Neutral, result15);
        }

        [TestMethod]
        public void CheckDefaultValues()
        {
            var filter = new BurstFilter();
            Assert.AreEqual(60, filter.BurstSize);
            Assert.AreEqual(TimeSpan.FromMinutes(1), filter.BurstLength);
        }

        [TestMethod]
        public void ChangeValues()
        {
            var filter = new BurstFilter();
            filter.BurstLength = TimeSpan.FromMinutes(3);
            filter.BurstSize = 4;
            Assert.AreEqual(4, filter.BurstSize);
            Assert.AreEqual(TimeSpan.FromMinutes(3), filter.BurstLength);
        }
    }
}
