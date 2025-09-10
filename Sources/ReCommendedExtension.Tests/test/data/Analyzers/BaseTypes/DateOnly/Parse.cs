using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void RedundantArgumentRange(string s)
        {
            var result = DateOnly.Parse(s, null, DateTimeStyles.None);
        }

        public void RedundantArgument(string s, ReadOnlySpan<char> s1)
        {
            var result1 = DateOnly.Parse(s, null);
            var result2 = DateOnly.Parse(s1, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
        {
            var result1 = DateOnly.Parse(s, null, style);
            var result2 = DateOnly.Parse(s, provider, style);
            var result3 = DateOnly.Parse(s, provider);
            var result4 = DateOnly.Parse(s1, provider);
        }
    }
}