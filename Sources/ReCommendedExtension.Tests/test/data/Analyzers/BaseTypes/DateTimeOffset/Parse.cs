using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void RedundantArgument(string input, ReadOnlySpan<char> s, IFormatProvider provider)
        {
            var result11 = DateTimeOffset.Parse(input, null);
            var result12 = DateTimeOffset.Parse(input, provider, DateTimeStyles.None);

            var result21 = DateTimeOffset.Parse(s, null);
        }

        public void NoDetection(string input, ReadOnlySpan<char> s, IFormatProvider provider, DateTimeStyles styles)
        {
            var result11 = DateTimeOffset.Parse(input, provider);
            var result12 = DateTimeOffset.Parse(input, provider, styles);

            var result21 = DateTimeOffset.Parse(s, provider);
        }
    }
}