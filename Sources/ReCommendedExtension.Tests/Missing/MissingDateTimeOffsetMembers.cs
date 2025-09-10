using System.Globalization;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingDateTimeOffsetMembers
{
    [Pure]
    public static DateTimeOffset _Ctor(
        [ValueRange(1, 9_999)] int year,
        [ValueRange(1, 12)] int month,
        [ValueRange(1, 31)] int day,
        [ValueRange(0, 23)] int hour,
        [ValueRange(0, 59)] int minute,
        [ValueRange(0, 59)] int second,
        [ValueRange(0, 999)] int millisecond,
        [ValueRange(0, 999)] int microsecond,
        TimeSpan offset)
    {
        const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

        return new DateTimeOffset(year, month, day, hour, minute, second, millisecond, offset)
            + TimeSpan.FromTicks(microsecond * ticksPerMicrosecond);
    }

    [Pure]
    public static DateTimeOffset _Ctor(
        [ValueRange(1, 9_999)] int year,
        [ValueRange(1, 12)] int month,
        [ValueRange(1, 31)] int day,
        [ValueRange(0, 23)] int hour,
        [ValueRange(0, 59)] int minute,
        [ValueRange(0, 59)] int second,
        [ValueRange(0, 999)] int millisecond,
        [ValueRange(0, 999)] int microsecond,
        Calendar calendar,
        TimeSpan offset)
    {
        const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

        return new DateTimeOffset(year, month, day, hour, minute, second, millisecond, calendar, offset)
            + TimeSpan.FromTicks(microsecond * ticksPerMicrosecond);
    }

    [Pure]
    public static DateTimeOffset Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => DateTimeOffset.Parse(s.ToString(), provider);

    [Pure]
    public static DateTimeOffset Parse(ReadOnlySpan<char> s) => DateTimeOffset.Parse(s.ToString());

    [Pure]
    public static DateTimeOffset ParseExact(
        ReadOnlySpan<char> input,
        string[] formats,
        IFormatProvider? formatProvider,
        DateTimeStyles styles = DateTimeStyles.None)
        => DateTimeOffset.ParseExact(input.ToString(), formats, formatProvider, styles);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DateTimeOffset result)
        => DateTimeOffset.TryParse(s, provider, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? formatProvider, out DateTimeOffset result)
        => DateTimeOffset.TryParse(s.ToString(), formatProvider, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> input, out DateTimeOffset result) => DateTimeOffset.TryParse(input.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
        => DateTimeOffset.TryParse(input.ToString(), formatProvider, styles, out result);

    [Pure]
    public static bool TryParseExact(
        ReadOnlySpan<char> input,
        [NotNullWhen(true)] string?[]? formats,
        IFormatProvider? formatProvider,
        DateTimeStyles styles,
        out DateTimeOffset result)
        => DateTimeOffset.TryParseExact(input.ToString(), formats, formatProvider, styles, out result);
}