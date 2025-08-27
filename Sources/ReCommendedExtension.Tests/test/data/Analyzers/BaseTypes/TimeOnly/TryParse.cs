using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider? provider, out TimeOnly result)
        {
            var result11 = TimeOnly.TryParse(s, provider, DateTimeStyles.None, out result);
            var result12 = TimeOnly.TryParse(s, null, out result);

            var result21 = TimeOnly.TryParse(s1, provider, DateTimeStyles.None, out result);
            var result22 = TimeOnly.TryParse(s1, null, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
        {
            var result11 = TimeOnly.TryParse(s, provider, style, out result);
            var result12 = TimeOnly.TryParse(s, provider, out result);

            var result21 = TimeOnly.TryParse(s1, provider, style, out result);
            var result22 = TimeOnly.TryParse(s1, provider, out result);
        }
    }
}