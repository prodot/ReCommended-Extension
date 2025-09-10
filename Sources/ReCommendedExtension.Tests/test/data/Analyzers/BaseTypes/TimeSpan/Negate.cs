using System;

namespace Test
{
    public class TimeSpans
    {
        public void Operator(TimeSpan timeSpan)
        {
            var result = timeSpan.Negate();
        }

        public void NoDetection(TimeSpan timeSpan)
        {
            timeSpan.Negate();
        }
    }
}