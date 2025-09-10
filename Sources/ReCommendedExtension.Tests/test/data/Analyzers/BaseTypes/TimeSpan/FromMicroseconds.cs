using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult()
        {
            var result = TimeSpan.FromMicroseconds(0);
        }

        public void NoDetection(long microseconds)
        {
            var result = TimeSpan.FromMicroseconds(microseconds);

            TimeSpan.FromMicroseconds(0);
        }
    }
}