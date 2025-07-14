using System;

namespace Test
{
    public class TimeSpans
    {
        public void ParseExact(string input, IFormatProvider formatProvider)
        {
            var result = TimeSpan.ParseExact(input, "c", format{caret}Provider);
        }
    }
}