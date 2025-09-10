using System;
using System.Globalization;

namespace Test
{
    public class TimesOnly
    {
        public void RedundantArgumentRange(string s)
        {
            var result = TimeOnly.Parse(s, null, DateTimeStyles.None);
        }

        public void RedundantArgument(string s, ReadOnlySpan<char> s1)
        {
            var result1 = TimeOnly.Parse(s, null);
            var result2 = TimeOnly.Parse(s1, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
        {
            var result1 = TimeOnly.Parse(s, null, style);
            var result2 = TimeOnly.Parse(s, provider, style);
            var result3 = TimeOnly.Parse(s, provider);
            var result4 = TimeOnly.Parse(s1, provider);
        }
    }
}