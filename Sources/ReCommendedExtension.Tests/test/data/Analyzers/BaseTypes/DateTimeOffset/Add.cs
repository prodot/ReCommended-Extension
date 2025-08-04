using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void Operator(DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
        {
            var result = dateTimeOffset.Add(timeSpan);
        }

        public void NoDetection(DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
        {
            dateTimeOffset.Add(timeSpan);
        }
    }
}