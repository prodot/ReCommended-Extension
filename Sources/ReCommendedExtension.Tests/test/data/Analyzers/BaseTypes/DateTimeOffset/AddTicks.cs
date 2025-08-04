using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void RedundantMethodInvocation(DateTimeOffset dateTimeOffset)
        {
            var result = dateTimeOffset.AddTicks(0);
        }

        public void NoDetection(DateTimeOffset dateTimeOffset, long value)
        {
            var result = dateTimeOffset.AddTicks(value);

            dateTimeOffset.AddTicks(0);
        }
    }
}