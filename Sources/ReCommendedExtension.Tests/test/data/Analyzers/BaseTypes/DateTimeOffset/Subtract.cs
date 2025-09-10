using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void Operator(DateTimeOffset dateTimeOffset, DateTimeOffset dateTimeOffsetValue, TimeSpan timeSpanValue)
        {
            var result1 = dateTimeOffset.Subtract(dateTimeOffsetValue);
            var result2 = dateTimeOffset.Subtract(timeSpanValue);
        }

        public void NoDetection(DateTimeOffset dateTimeOffset, DateTimeOffset dateTimeOffsetValue, TimeSpan timeSpanValue)
        {
            dateTimeOffset.Subtract(dateTimeOffsetValue);
            dateTimeOffset.Subtract(timeSpanValue);
        }
    }
}