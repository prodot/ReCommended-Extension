using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(DateTime dateTime, long ticks, DateOnly date, TimeOnly time, int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, Calendar calendar, DateTimeKind kind, char f, string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, out DateTime result)
        {
            var result11 = new DateTime(ticks, DateTimeKind.Unspecified);
            var result12 = new DateTime(date, time, DateTimeKind.Unspecified);
            var result13 = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            var result14 = new DateTime(year, month, day, hour, minute, second, 0);
            var result15 = new DateTime(year, month, day, hour, minute, second, 0, DateTimeKind.Unspecified);
            var result16 = new DateTime(year, month, day, hour, minute, second, 0, calendar);
            var result17 = new DateTime(year, month, day, hour, minute, second, millisecond, calendar, DateTimeKind.Unspecified);
            var result18 = new DateTime(year, month, day, hour, minute, second, millisecond, 0);
            var result19 = new DateTime(year, month, day, hour, minute, second, millisecond, 0, calendar);
            var result1A = new DateTime(year, month, day, hour, minute, second, millisecond, 0, DateTimeKind.Unspecified);
            var result1B = new DateTime(year, month, day, hour, minute, second, millisecond, 0, calendar, DateTimeKind.Unspecified);
            
            DateTime result21 = new(ticks, DateTimeKind.Unspecified);
            DateTime result22 = new(date, time, DateTimeKind.Unspecified);
            DateTime result23 = new(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            DateTime result24 = new(year, month, day, hour, minute, second, 0);
            DateTime result25 = new(year, month, day, hour, minute, second, 0, DateTimeKind.Unspecified);
            DateTime result26 = new(year, month, day, hour, minute, second, 0, calendar);
            DateTime result27 = new(year, month, day, hour, minute, second, millisecond, calendar, DateTimeKind.Unspecified);
            DateTime result28 = new(year, month, day, hour, minute, second, millisecond, 0);
            DateTime result29 = new(year, month, day, hour, minute, second, millisecond, 0, calendar);
            DateTime result2A = new(year, month, day, hour, minute, second, millisecond, 0, DateTimeKind.Unspecified);
            DateTime result2B = new(year, month, day, hour, minute, second, millisecond, 0, calendar, DateTimeKind.Unspecified);

            var result31 = dateTime.GetDateTimeFormats(null);
            var result32 = dateTime.GetDateTimeFormats(f, null);

            var result41 = DateTime.Parse(s, null);
            var result42 = DateTime.Parse(s1, null);
            var result43 = DateTime.Parse(s, provider, DateTimeStyles.None);

            var result51 = DateTime.ParseExact(s, format, provider, DateTimeStyles.None);

            var result61 = DateTime.TryParse(s, null, out result);
            var result62 = DateTime.TryParse(s1, null, out result);
            var result63 = DateTime.TryParse(s, provider, DateTimeStyles.None, out result);
            var result64 = DateTime.TryParse(s1, provider, DateTimeStyles.None, out result);
        }

        public void RedundantArgumentRange(int year, int month, int day, Calendar calendar)
        {
            var result11 = new DateTime(year, month, day, 0, 0, 0);
            var result12 = new DateTime(year, month, day, hour: 0, minute: 0, second: 0);
            var result13 = new DateTime(year, month, second: 0, day: day, minute: 0, hour: 0);
            var result14 = new DateTime(year, month, day, 0, 0, 0, calendar);
            var result15 = new DateTime(year, month, day, hour: 0, minute: 0, second: 0, calendar: calendar);
            var result16 = new DateTime(year, month, second: 0, day: day, minute: 0, hour: 0, calendar: calendar);

            DateTime result21 = new(year, month, day, 0, 0, 0);
            DateTime result22 = new(year, month, day, hour: 0, minute: 0, second: 0);
            DateTime result23 = new(year, month, second: 0, day: day, minute: 0, hour: 0);
            DateTime result24 = new(year, month, day, 0, 0, 0, calendar);
            DateTime result25 = new(year, month, day, hour: 0, minute: 0, second: 0, calendar: calendar);
            DateTime result26 = new(year, month, second: 0, day: day, minute: 0, hour: 0, calendar: calendar);
        }

        public void RedundantCollectionElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            var result11 = DateTime.ParseExact(s, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style);
            var result12 = DateTime.ParseExact(s1, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style);

            var result21 = DateTime.TryParseExact(s, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style, out result);
            var result22 = DateTime.TryParseExact(s1, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style, out result);
        }

        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            var result11 = DateTime.ParseExact(s, "o", provider);
            var result12 = DateTime.ParseExact(s, "O", provider);
            var result13 = DateTime.ParseExact(s, "r", provider);
            var result14 = DateTime.ParseExact(s, "R", provider);
            var result15 = DateTime.ParseExact(s, "s", provider);
            var result16 = DateTime.ParseExact(s, "u", provider);
            var result17 = DateTime.ParseExact(s, "o", provider, style);
            var result18 = DateTime.ParseExact(s, "O", provider, style);
            var result19 = DateTime.ParseExact(s, "r", provider, style);
            var result1A = DateTime.ParseExact(s, "R", provider, style);
            var result1B = DateTime.ParseExact(s, "s", provider, style);
            var result1C = DateTime.ParseExact(s, "u", provider, style);
            var result1D = DateTime.ParseExact(s, [format], provider, style);
            var result1E = DateTime.ParseExact(s, ["o", "r", "s", "u"], provider, style);
            var result1F = DateTime.ParseExact(s1, ["o", "r", "s", "u"], provider, style);

            var result21 = DateTime.TryParseExact(s, "o", provider, style, out result);
            var result22 = DateTime.TryParseExact(s, "O", provider, style, out result);
            var result23 = DateTime.TryParseExact(s, "r", provider, style, out result);
            var result24 = DateTime.TryParseExact(s, "R", provider, style, out result);
            var result25 = DateTime.TryParseExact(s, "s", provider, style, out result);
            var result26 = DateTime.TryParseExact(s, "u", provider, style, out result);
            var result27 = DateTime.TryParseExact(s, [format], provider, style, out result);
            var result28 = DateTime.TryParseExact(s, ["o", "r", "s", "u"], provider, style, out result);
            var result29 = DateTime.TryParseExact(s1, ["o", "r", "s", "u"], provider, style, out result);
        }

        public void NoDetection(DateTime dateTime, long ticks, DateOnly date, TimeOnly time, int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, Calendar calendar, DateTimeKind kind, char f, string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            var result11 = new DateTime(ticks, kind);
            var result12 = new DateTime(date, time, kind);
            var result13 = new DateTime(year, month, day, hour, minute, second, kind);
            var result14 = new DateTime(year, month, day, hour, minute, second, millisecond);
            var result15 = new DateTime(year, month, day, hour, minute, second, millisecond, kind);
            var result16 = new DateTime(year, month, day, hour, minute, second, millisecond, calendar);
            var result17 = new DateTime(year, month, day, hour, minute, second, millisecond, calendar, kind);
            var result18 = new DateTime(year, month, day, hour, minute, second, millisecond, microsecond);
            var result19 = new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, calendar);
            var result1A = new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, kind);
            var result1B = new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, calendar, kind);

            var result21 = dateTime.GetDateTimeFormats(provider);
            var result22 = dateTime.GetDateTimeFormats(f, provider);

            var result31 = DateTime.Parse(s, provider);
            var result32 = DateTime.Parse(s1, provider);
            var result33 = DateTime.Parse(s, provider, style);

            var result41 = DateTime.ParseExact(s, format, provider, style);

            var result51 = DateTime.TryParse(s, provider, out result);
            var result52 = DateTime.TryParse(s1, provider, out result);
            var result53 = DateTime.TryParse(s, provider, style, out result);
            var result54 = DateTime.TryParse(s1, provider, style, out result);

            var result61 = new DateTime(year, month, day, hour, minute, second);
            var result62 = new DateTime(year, month, day, hour, minute, second, calendar);

            var result71 = DateTime.ParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style);
            var result72 = DateTime.ParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style);

            var result81 = DateTime.TryParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style, out result);
            var result82 = DateTime.TryParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style, out result);

            var result91 = DateTime.ParseExact(s, "o", null);
            var result92 = DateTime.ParseExact(s, "O", null);
            var result93 = DateTime.ParseExact(s, "r", null);
            var result94 = DateTime.ParseExact(s, "R", null);
            var result95 = DateTime.ParseExact(s, "s", null);
            var result96 = DateTime.ParseExact(s, "u", null);
            var result97 = DateTime.ParseExact(s, "o", null, style);
            var result98 = DateTime.ParseExact(s, "O", null, style);
            var result99 = DateTime.ParseExact(s, "r", null, style);
            var result9A = DateTime.ParseExact(s, "R", null, style);
            var result9B = DateTime.ParseExact(s, "s", null, style);
            var result9C = DateTime.ParseExact(s, "u", null, style);
            var result9D = DateTime.ParseExact(s, [format, format], provider, style);
            var result9E = DateTime.ParseExact(s, ["o", "r", "s", "u"], null, style);
            var result9F = DateTime.ParseExact(s, ["o", "r", "s", "u", "d"], provider, style);
            var result9G = DateTime.ParseExact(s1, ["o", "r", "s", "u"], null, style);
            var result9H = DateTime.ParseExact(s1, ["d", "o", "r", "s", "u"], provider, style);
        }
    }
}