using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void Add(DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
        {
            var result = dateTimeOffset.Add(timeSpan);
        }

        public void Equals(DateTimeOffset dateTimeOffset, DateTimeOffset other)
        {
            var result1 = dateTimeOffset.Equals(other);
            var result2 = DateTimeOffset.Equals(dateTimeOffset, other);
        }

        public void Subtract(DateTimeOffset dateTimeOffset, DateTimeOffset dateTimeOffsetValue, TimeSpan timeSpanValue)
        {
            var result1 = dateTimeOffset.Subtract(dateTimeOffsetValue);
            var result2 = dateTimeOffset.Subtract(timeSpanValue);
        }
    }
}