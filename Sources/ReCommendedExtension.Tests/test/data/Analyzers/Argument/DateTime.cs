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
            var result12 = new DateTime(year, month, day, 0, 0, 0, calendar);

            DateTime result21 = new(year, month, day, 0, 0, 0);
            DateTime result22 = new(year, month, day, 0, 0, 0, calendar);
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
            var result63 = new DateTime(year, month, day, 0, second: 0, minute: 0);
        }
    }
}