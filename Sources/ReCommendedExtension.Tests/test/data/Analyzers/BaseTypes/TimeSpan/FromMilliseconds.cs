using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult()
        {
            var result1 = TimeSpan.FromMilliseconds(0);
            var result2 = TimeSpan.FromMilliseconds(microseconds: 0, milliseconds: 0);
        }

        public void NoDetection(long milliseconds, long microseconds)
        {
            var result1 = TimeSpan.FromMilliseconds(milliseconds);
            var result2 = TimeSpan.FromMilliseconds(0, microseconds);

            TimeSpan.FromMilliseconds(0);
            TimeSpan.FromMilliseconds(microseconds: 0, milliseconds: 0);
        }
    }
}