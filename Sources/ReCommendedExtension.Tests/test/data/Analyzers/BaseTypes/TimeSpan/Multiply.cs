using System;

namespace Test
{
    public class TimeSpans
    {
        public void Operator(TimeSpan timeSpan, double factor)
        {
            var result = timeSpan.Multiply(factor);
        }

        public void NoDetection(TimeSpan timeSpan, double factor)
        {
            timeSpan.Multiply(factor);
        }
    }
}