using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan offset, Calendar calendar, string s, ReadOnlySpan<char> s1, IFormatProvider provider, string format, out DateTimeOffset result)
        {
            var result11 = new DateTimeOffset(year, month, day, hour, minute, second, 0, offset);
            var result12 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, offset);
            var result13 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);

            DateTimeOffset result21 = new(year, month, day, hour, minute, second, 0, offset);
            DateTimeOffset result22 = new(year, month, day, hour, minute, second, millisecond, 0, offset);
            DateTimeOffset result23 = new(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);

            var result31 = DateTimeOffset.Parse(s, null);
            var result32 = DateTimeOffset.Parse(s1, null);
            var result33 = DateTimeOffset.Parse(s, provider, DateTimeStyles.None);

            var result41 = DateTimeOffset.TryParse(s, null, out result);
            var result42 = DateTimeOffset.TryParse(s1, null, out result);
            var result43 = DateTimeOffset.TryParse(s, provider, DateTimeStyles.None, out result);
            var result44 = DateTimeOffset.TryParse(s1, provider, DateTimeStyles.None, out result);

            var result51 = DateTimeOffset.ParseExact(s, format, provider, DateTimeStyles.None);
        }

        public void RedundantCollectionElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.ParseExact(s, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style);
            var result12 = DateTimeOffset.ParseExact(s1, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style);

            var result21 = DateTimeOffset.TryParseExact(s, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style, out result);
            var result22 = DateTimeOffset.TryParseExact(s1, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style, out result);
        }

        public void NoDetection(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, TimeSpan offset, Calendar calendar, string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, string format, out DateTimeOffset result)
        {
            var result11 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, offset);
            var result12 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, microsecond, offset);
            var result13 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, microsecond, calendar, offset);

            var result21 = DateTimeOffset.Parse(s, provider);
            var result22 = DateTimeOffset.Parse(s1, provider);
            var result23 = DateTimeOffset.Parse(s, provider, style);

            var result31 = DateTimeOffset.TryParse(s, provider, out result);
            var result32 = DateTimeOffset.TryParse(s1, provider, out result);
            var result33 = DateTimeOffset.TryParse(s, provider, style, out result);
            var result34 = DateTimeOffset.TryParse(s1, provider, style, out result);

            var result41 = DateTimeOffset.ParseExact(s, format, provider, style);

            var result51 = DateTimeOffset.ParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style);
            var result52 = DateTimeOffset.ParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style);

            var result61 = DateTimeOffset.TryParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style, out result);
            var result62 = DateTimeOffset.TryParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style, out result);
        }
    }
}