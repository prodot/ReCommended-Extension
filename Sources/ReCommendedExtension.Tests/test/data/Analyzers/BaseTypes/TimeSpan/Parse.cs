using System;

namespace Test
{
    public class TimeSpans
    {
        public void RedundantArgument(string input)
        {
            var result = TimeSpan.Parse(input, null);
        }

        public void NoDetection(string input, IFormatProvider formatProvider)
        {
            var result = TimeSpan.Parse(input, formatProvider);
        }
    }
}