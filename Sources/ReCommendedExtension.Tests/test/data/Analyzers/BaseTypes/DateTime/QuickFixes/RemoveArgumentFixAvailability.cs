using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void _Constructors(long ticks, DateOnly date, TimeOnly time, int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, Calendar calendar, DateTimeKind kind)
        {
            var result11 = new DateTime(ticks, DateTimeKind.Unspecified);
            var result12 = new DateTime(date, time, DateTimeKind.Unspecified);
            var result13 = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            var result14 = new DateTime(year, month, day, hour, minute, second, 0);
            var result15 = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
            var result16 = new DateTime(year, month, day, hour, minute, second, 0, kind);
            var result17 = new DateTime(year, month, day, hour, minute, second, 0, calendar);
            var result18 = new DateTime(year, month, day, hour, minute, second, millisecond, calendar, DateTimeKind.Unspecified);
            var result19 = new DateTime(year, month, day, hour, minute, second, millisecond, 0);
            var result1A = new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, DateTimeKind.Unspecified);
            var result1B = new DateTime(year, month, day, hour, minute, second, millisecond, 0, kind);
            var result1C = new DateTime(year, month, day, hour, minute, second, millisecond, 0, calendar);
            var result1D = new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, calendar, DateTimeKind.Unspecified);
            var result1E = new DateTime(year, month, day, hour, minute, second, millisecond, 0, calendar, kind);

            DateTime result21 = new(ticks, DateTimeKind.Unspecified);
            DateTime result22 = new(date, time, DateTimeKind.Unspecified);
            DateTime result23 = new(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            DateTime result24 = new(year, month, day, hour, minute, second, 0);
            DateTime result25 = new(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
            DateTime result26 = new(year, month, day, hour, minute, second, 0, kind);
            DateTime result27 = new(year, month, day, hour, minute, second, 0, calendar);
            DateTime result28 = new(year, month, day, hour, minute, second, millisecond, calendar, DateTimeKind.Unspecified);
            DateTime result29 = new(year, month, day, hour, minute, second, millisecond, 0);
            DateTime result2A = new(year, month, day, hour, minute, second, millisecond, microsecond, DateTimeKind.Unspecified);
            DateTime result2B = new(year, month, day, hour, minute, second, millisecond, 0, kind);
            DateTime result2C = new(year, month, day, hour, minute, second, millisecond, 0, calendar);
            DateTime result2D = new(year, month, day, hour, minute, second, millisecond, microsecond, calendar, DateTimeKind.Unspecified);
            DateTime result2E = new(year, month, day, hour, minute, second, millisecond, 0, calendar, kind);
        }

        public void GetDateTimeFormats(DateTime dateTime, char format)
        {
            var result1 = dateTime.GetDateTimeFormats(null);
            var result2 = dateTime.GetDateTimeFormats(format, null);
        }

        public void Parse(string s, ReadOnlySpan<char> s1, IFormatProvider provider)
        {
            var result11 = DateTime.Parse(s, null);
            var result12 = DateTime.Parse(s, provider, DateTimeStyles.None);

            var result21 = DateTime.Parse(s1, null);
        }

        public void ParseExact(string s, string format, IFormatProvider provider)
        {
            var result = DateTime.ParseExact(s, format, provider, DateTimeStyles.None);
        }

        public void ToString(DateTime dateTime, string format, IFormatProvider provider)
        {
            var result11 = dateTime.ToString(null as string);
            var result12 = dateTime.ToString("");

            var result21 = dateTime.ToString(null as IFormatProvider);

            var result31 = dateTime.ToString(null, provider);
            var result32 = dateTime.ToString("", provider);
            var result33 = dateTime.ToString(format, null);
            var result34 = dateTime.ToString("O", provider);
            var result35 = dateTime.ToString("o", provider);
            var result36 = dateTime.ToString("R", provider);
            var result37 = dateTime.ToString("r", provider);
            var result38 = dateTime.ToString("s", provider);
            var result39 = dateTime.ToString("u", provider);
        }

        public void RedundantATryParsergument(string s, ReadOnlySpan<char> s1, IFormatProvider provider, out DateTime result)
        {
            var result11 = DateTime.TryParse(s, null, out result);
            var result12 = DateTime.TryParse(s1, null, out result);

            var result21 = DateTime.TryParse(s, provider, DateTimeStyles.None, out result);
            var result22 = DateTime.TryParse(s1, provider, DateTimeStyles.None, out result);
        }
    }
}