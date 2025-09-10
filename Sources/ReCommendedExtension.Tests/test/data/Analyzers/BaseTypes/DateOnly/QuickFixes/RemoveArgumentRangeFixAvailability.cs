using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void Parse(string s)
        {
            var result = DateOnly.Parse(s, null, DateTimeStyles.None);
        }

        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = DateOnly.ParseExact(s, format, null, DateTimeStyles.None);
            var result2 = DateOnly.ParseExact(s, formats, null, DateTimeStyles.None);
            var result3 = DateOnly.ParseExact(s1, formats, null, DateTimeStyles.None);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, string format, string[] formats, out DateOnly result)
        {
            var result11 = DateOnly.TryParseExact(s, format, null, DateTimeStyles.None, out result);
            var result12 = DateOnly.TryParseExact(s1, format, null, DateTimeStyles.None, out result);

            var result21 = DateOnly.TryParseExact(s, formats, null, DateTimeStyles.None, out result);
            var result22 = DateOnly.TryParseExact(s1, formats, null, DateTimeStyles.None, out result);
        }
    }
}