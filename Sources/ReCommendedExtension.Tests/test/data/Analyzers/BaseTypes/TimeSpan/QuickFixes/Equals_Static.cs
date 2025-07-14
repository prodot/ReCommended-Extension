using System;

namespace Test
{
    public class TimeSpans
    {
        public void Equals(TimeSpan timeSpan, TimeSpan obj)
        {
            var result = !TimeSpan.Equals{caret}(timeSpan, obj);
        }
    }
}