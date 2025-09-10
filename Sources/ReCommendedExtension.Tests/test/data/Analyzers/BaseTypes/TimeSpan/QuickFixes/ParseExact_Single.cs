using System;

namespace Test
{
    public class TimeSpans
    {
        public void ParseExact(string input, string format, IFormatProvider formatProvider)
        {
            var result = TimeSpan.ParseExact(input, [for{caret}mat], formatProvider);
        }
    }
}