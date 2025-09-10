using System;

namespace Test
{
    public class TimeSpans
    {
        public void Operator(TimeSpan timeSpan, double divisor, TimeSpan ts)
        {
            var result1 = timeSpan.Divide(divisor);
            var result2 = timeSpan.Divide(ts);
        }

        public void NoDetection(TimeSpan timeSpan, double divisor, TimeSpan ts)
        {
            timeSpan.Divide(divisor);
            timeSpan.Divide(ts);
        }
    }
}