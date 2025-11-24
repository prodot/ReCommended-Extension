using System.Globalization;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingDateTimeMembers
{
    extension(DateTime dateTime)
    {
        [Pure]
        public static DateTime _Ctor(DateOnly date, TimeOnly time, DateTimeKind kind) => new(date.ToDateTime(time).Ticks, kind);

        [Pure]
        public static DateTime _Ctor(DateOnly date, TimeOnly time) => date.ToDateTime(time);

        [Pure]
        public static DateTime _Ctor(
            [ValueRange(1, 9_999)] int year,
            [ValueRange(1, 12)] int month,
            [ValueRange(1, 31)] int day,
            [ValueRange(0, 23)] int hour,
            [ValueRange(0, 59)] int minute,
            [ValueRange(0, 59)] int second,
            [ValueRange(0, 999)] int millisecond,
            [ValueRange(0, 999)] int microsecond,
            DateTimeKind kind)
        {
            const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

            return new DateTime(year, month, day, hour, minute, second, millisecond, kind) + TimeSpan.FromTicks(microsecond * ticksPerMicrosecond);
        }

        [Pure]
        public static DateTime _Ctor(
            [ValueRange(1, 9_999)] int year,
            [ValueRange(1, 12)] int month,
            [ValueRange(1, 31)] int day,
            [ValueRange(0, 23)] int hour,
            [ValueRange(0, 59)] int minute,
            [ValueRange(0, 59)] int second,
            [ValueRange(0, 999)] int millisecond,
            [ValueRange(0, 999)] int microsecond)
            => _Ctor(year, month, day, hour, minute, second, millisecond, microsecond, DateTimeKind.Unspecified);

        [Pure]
        public static DateTime _Ctor(
            [ValueRange(1, 9_999)] int year,
            [ValueRange(1, 12)] int month,
            [ValueRange(1, 31)] int day,
            [ValueRange(0, 23)] int hour,
            [ValueRange(0, 59)] int minute,
            [ValueRange(0, 59)] int second,
            [ValueRange(0, 999)] int millisecond,
            [ValueRange(0, 999)] int microsecond,
            Calendar calendar)
        {
            const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

            return new DateTime(year, month, day, hour, minute, second, millisecond, calendar)
                + TimeSpan.FromTicks(microsecond * ticksPerMicrosecond);
        }

        [Pure]
        public static DateTime _Ctor(
            [ValueRange(1, 9_999)] int year,
            [ValueRange(1, 12)] int month,
            [ValueRange(1, 31)] int day,
            [ValueRange(0, 23)] int hour,
            [ValueRange(0, 59)] int minute,
            [ValueRange(0, 59)] int second,
            [ValueRange(0, 999)] int millisecond,
            [ValueRange(0, 999)] int microsecond,
            Calendar calendar,
            DateTimeKind kind)
        {
            const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

            return new DateTime(year, month, day, hour, minute, second, millisecond, calendar, kind)
                + TimeSpan.FromTicks(microsecond * ticksPerMicrosecond);
        }

        [ValueRange(0, 999)]
        public int Microsecond
        {
            get
            {
                const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

                return (int)(dateTime.Ticks / ticksPerMicrosecond % 1_000);
            }
        }

        [Pure]
        public static DateTime Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => DateTime.Parse(s.ToString(), provider);

        [Pure]
        [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
        public static DateTime Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null, DateTimeStyles styles = DateTimeStyles.None)
            => DateTime.Parse(s.ToString(), DateTimeFormatInfo.GetInstance(provider), styles);

        [Pure]
        public static DateTime ParseExact(
            ReadOnlySpan<char> s,
            string[] formats,
            IFormatProvider? provider,
            DateTimeStyles style = DateTimeStyles.None)
            => DateTime.ParseExact(s.ToString(), formats, provider, style);

        [Pure]
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DateTime result)
            => DateTime.TryParse(s, provider, DateTimeStyles.None, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, DateTimeStyles styles, out DateTime result)
            => DateTime.TryParse(s.ToString(), provider, styles, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out DateTime result)
            => DateTime.TryParse(s.ToString(), provider, DateTimeStyles.None, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, out DateTime result) => DateTime.TryParse(s.ToString(), out result);

        [Pure]
        public static bool TryParseExact(
            ReadOnlySpan<char> s,
            [NotNullWhen(true)] string?[]? formats,
            IFormatProvider? provider,
            DateTimeStyles style,
            out DateTime result)
            => DateTime.TryParseExact(s.ToString(), formats, provider, style, out result);
    }
}