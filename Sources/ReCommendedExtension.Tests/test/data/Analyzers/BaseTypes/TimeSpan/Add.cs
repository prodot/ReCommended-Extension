using System;

namespace Test
{
    public class TimeSpans
    {
        public void Operator(TimeSpan timeSpan, TimeSpan ts)
        {
            var result = timeSpan.Add(ts);
        }

        public void NoDetection(TimeSpan timeSpan, TimeSpan ts)
        {
            timeSpan.Add(ts);
        }
    }
}