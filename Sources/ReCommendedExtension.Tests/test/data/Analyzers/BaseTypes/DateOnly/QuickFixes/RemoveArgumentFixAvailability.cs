using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void Parse(string s, ReadOnlySpan<char> s1)
        {
            var result1 = DateOnly.Parse(s, null);
            var result2 = DateOnly.Parse(s1, null);
        }

        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = DateOnly.ParseExact(s, format, null);
            var result2 = DateOnly.ParseExact(s, formats, null);
            var result3 = DateOnly.ParseExact(s1, formats, null);
        }

        public void ToString(DateOnly dateOnly, string format, IFormatProvider provider)
        {
            var result11 = dateOnly.ToString(null as string);
            var result12 = dateOnly.ToString("");
            var result13 = dateOnly.ToString("d");

            var result21 = dateOnly.ToString(null as IFormatProvider);

            var result31 = dateOnly.ToString(format, null);
            var result32 = dateOnly.ToString(null, provider);
            var result33 = dateOnly.ToString("", provider);
            var result34 = dateOnly.ToString("d", provider);
            var result35 = dateOnly.ToString("o", provider);
            var result36 = dateOnly.ToString("O", provider);
            var result37 = dateOnly.ToString("r", provider);
            var result38 = dateOnly.ToString("R", provider);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, IFormatProvider? provider, out DateOnly result)
        {
            var result11 = DateOnly.TryParse(s, provider, DateTimeStyles.None, out result);
            var result12 = DateOnly.TryParse(s, null, out result);

            var result21 = DateOnly.TryParse(s1, provider, DateTimeStyles.None, out result);
            var result22 = DateOnly.TryParse(s1, null, out result);
        }
    }
}