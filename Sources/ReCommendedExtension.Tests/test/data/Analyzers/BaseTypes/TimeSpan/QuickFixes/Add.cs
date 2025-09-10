using System;

namespace Test
{
    public class TimeSpans
    {
        public void Add(TimeSpan timeSpan, TimeSpan ts)
        {
            var result = timeSpan.{caret}Add(ts);
        }
    }
}