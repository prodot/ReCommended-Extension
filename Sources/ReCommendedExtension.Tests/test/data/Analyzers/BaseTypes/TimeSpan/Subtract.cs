using System;

namespace Test
{
    public class TimeSpans
    {
        public void Operator(TimeSpan timeSpan, TimeSpan ts)
        {
            var result = timeSpan.Subtract(ts);
        }

        public void NoDetection(TimeSpan timeSpan, TimeSpan ts)
        {
            timeSpan.Subtract(ts);
        }
    }
}