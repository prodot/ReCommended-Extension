using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider? provider, out DateOnly result)
        {
            var result11 = DateOnly.TryParse(s, provider, DateTimeStyles.None, out result);
            var result12 = DateOnly.TryParse(s, null, out result);

            var result21 = DateOnly.TryParse(s1, provider, DateTimeStyles.None, out result);
            var result22 = DateOnly.TryParse(s1, null, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result11 = DateOnly.TryParse(s, provider, style, out result);
            var result12 = DateOnly.TryParse(s, provider, out result);

            var result21 = DateOnly.TryParse(s1, provider, style, out result);
            var result22 = DateOnly.TryParse(s1, provider, out result);
        }
    }
}