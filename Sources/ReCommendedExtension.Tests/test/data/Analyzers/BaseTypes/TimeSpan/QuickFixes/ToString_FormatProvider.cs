using System;

namespace Test
{
    public class TimeSpans
    {
        public void ToString(TimeSpan timeSpan, IFormatProvider formatProvider)
        {
            var result13 = timeSpan.ToString("c", format{caret}Provider);
        }
    }
}