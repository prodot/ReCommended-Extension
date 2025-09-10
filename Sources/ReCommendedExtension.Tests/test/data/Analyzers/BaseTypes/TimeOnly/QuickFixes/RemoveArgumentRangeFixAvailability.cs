using System;
using System.Globalization;

namespace Test
{
    public class TimesOnly
    {
        public void Parse(string s)
        {
            var result = TimeOnly.Parse(s, null, DateTimeStyles.None);
        }

        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = TimeOnly.ParseExact(s, format, null, DateTimeStyles.None);
            var result2 = TimeOnly.ParseExact(s, formats, null, DateTimeStyles.None);
            var result3 = TimeOnly.ParseExact(s1, formats, null, DateTimeStyles.None);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, string format, string[] formats, out TimeOnly result)
        {
            var result11 = TimeOnly.TryParseExact(s, format, null, DateTimeStyles.None, out result);
            var result12 = TimeOnly.TryParseExact(s1, format, null, DateTimeStyles.None, out result);

            var result21 = TimeOnly.TryParseExact(s, formats, null, DateTimeStyles.None, out result);
            var result22 = TimeOnly.TryParseExact(s1, formats, null, DateTimeStyles.None, out result);
        }
    }
}