using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult()
        {
            var result1 = TimeSpan.FromTicks(0);
            var result2 = TimeSpan.FromTicks(long.MinValue);
            var result3 = TimeSpan.FromTicks(long.MaxValue);
        }

        public void NoDetection(long ticks)
        {
            var result = TimeSpan.FromTicks(ticks);

            TimeSpan.FromTicks(0);
            TimeSpan.FromTicks(long.MinValue);
            TimeSpan.FromTicks(long.MaxValue);
        }
    }
}