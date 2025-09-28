using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void _Constructors(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, TimeSpan offset)
        {
            var result11 = new DateTimeOffset(year, month, day, hour, minute, second, 0, offset);
            var result12 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, offset);
            var result13 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);

            DateTimeOffset result21 = new(year, month, day, hour, minute, second, 0, offset);
            DateTimeOffset result22 = new(year, month, day, hour, minute, second, millisecond, 0, offset);
            DateTimeOffset result23 = new(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);
        }

        public void Parse(string input, ReadOnlySpan<char> s, IFormatProvider provider)
        {
            var result11 = DateTimeOffset.Parse(input, null);
            var result12 = DateTimeOffset.Parse(input, provider, DateTimeStyles.None);

            var result21 = DateTimeOffset.Parse(s, null);
        }

        public void ParseExact(string input, string format, IFormatProvider formatProvider)
        {
            var result = DateTimeOffset.ParseExact(input, format, formatProvider, DateTimeStyles.None);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, IFormatProvider provider, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.TryParse(s, null, out result);
            var result12 = DateTimeOffset.TryParse(s1, null, out result);

            var result21 = DateTimeOffset.TryParse(s, provider, DateTimeStyles.None, out result);
            var result22 = DateTimeOffset.TryParse(s1, provider, DateTimeStyles.None, out result);
        }
    }
}