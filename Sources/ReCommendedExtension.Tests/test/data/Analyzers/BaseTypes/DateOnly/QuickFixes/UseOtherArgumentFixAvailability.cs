using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateOnly.ParseExact(s, "R", provider, style);
            var result12 = DateOnly.ParseExact(s, "R", provider);

            var result21 = DateOnly.ParseExact(s, [format]);

            var result31 = DateOnly.ParseExact(s, [format], provider, style);
            var result32 = DateOnly.ParseExact(s, ["o", "r"], provider, style);

            var result41 = DateOnly.ParseExact(s1, ["o", "r"], provider, style);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result1 = DateOnly.TryParseExact(s, "R", provider, style, out result);
            var result2 = DateOnly.TryParseExact(s, [format], out result);
            var result3 = DateOnly.TryParseExact(s, [format], provider, style, out result);
            var result4 = DateOnly.TryParseExact(s, ["o", "r"], provider, style, out result);
            var result5 = DateOnly.TryParseExact(s1, ["o", "r"], provider, style, out result);
        }
    }
}