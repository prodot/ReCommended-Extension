using System;

namespace Test
{
    public class TimeSpans
    {
        public void RedundantArgument(string input, ReadOnlySpan<char> input1, out TimeSpan result)
        {
            var result1 = TimeSpan.TryParse(input, null, out result);
            var result2 = TimeSpan.TryParse(input1, null, out result);
        }

        public void NoDetection(string input, ReadOnlySpan<char> input1, IFormatProvider formatProvider, out TimeSpan result)
        {
            var result1 = TimeSpan.TryParse(input, formatProvider, out result);
            var result2 = TimeSpan.TryParse(input1, formatProvider, out result);
        }
    }
}