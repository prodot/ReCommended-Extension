using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = TimeOnly.ParseExact(s, "R", provider, style);
            var result12 = TimeOnly.ParseExact(s, "R", provider);

            var result21 = TimeOnly.ParseExact(s, [format]);

            var result31 = TimeOnly.ParseExact(s, [format], provider, style);
            var result32 = TimeOnly.ParseExact(s, ["o", "r"], provider, style);

            var result41 = TimeOnly.ParseExact(s1, ["o", "r"], provider, style);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
        {
            var result1 = TimeOnly.TryParseExact(s, "R", provider, style, out result);
            var result2 = TimeOnly.TryParseExact(s, [format], out result);
            var result3 = TimeOnly.TryParseExact(s, [format], provider, style, out result);
            var result4 = TimeOnly.TryParseExact(s, ["o", "r"], provider, style, out result);
            var result5 = TimeOnly.TryParseExact(s1, ["o", "r"], provider, style, out result);
        }
    }
}