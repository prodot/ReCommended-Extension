using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateOnly.ParseExact(s, "R", provider, style);
            var result12 = DateOnly.ParseExact(s, "R", provider);

            var result21 = DateOnly.ParseExact(s, [format]);

            var result31 = DateOnly.ParseExact(s, [format], provider, style);
            var result32 = DateOnly.ParseExact(s, ["o", "r"], provider, style);
        }
    }
}