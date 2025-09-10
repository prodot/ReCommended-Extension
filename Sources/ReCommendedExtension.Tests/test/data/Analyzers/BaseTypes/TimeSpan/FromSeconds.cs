using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult()
        {
            var result1 = TimeSpan.FromSeconds(0);
            var result2 = TimeSpan.FromSeconds(0, 0);
            var result3 = TimeSpan.FromSeconds(microseconds: 0, seconds: 0);
        }

        public void NoDetection(int seconds, int microseconds)
        {
            var result1 = TimeSpan.FromSeconds(seconds);
            var result2 = TimeSpan.FromSeconds(0, microseconds);

            TimeSpan.FromSeconds(0);
            TimeSpan.FromSeconds(0, 0);
            TimeSpan.FromSeconds(microseconds: 0, seconds: 0);
        }
    }
}