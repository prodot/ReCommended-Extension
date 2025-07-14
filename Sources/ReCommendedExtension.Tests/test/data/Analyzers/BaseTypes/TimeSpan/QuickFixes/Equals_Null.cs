using System;

namespace Test
{
    public class TimeSpans
    {
        public void Equals(TimeSpan timeSpan)
        {
            var result = timeSpan.{caret}Equals(null);
        }
    }
}