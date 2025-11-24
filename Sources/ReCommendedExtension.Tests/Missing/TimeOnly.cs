using System.Globalization;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Missing;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
public readonly record struct TimeOnly : IFormattable
{
    public static TimeOnly MinValue => new(0);

    public static TimeOnly MaxValue => new(TimeSpan.TicksPerDay - 1);

    [Pure]
    public static TimeOnly FromDateTime(DateTime dateTime) => new(dateTime.TimeOfDay);

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static TimeOnly Parse(string s, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
        => FromDateTime(DateTime.Parse(s, provider, style));

    [Pure]
    public static TimeOnly Parse(string s, IFormatProvider? provider) => FromDateTime(DateTime.Parse(s, provider));

    [Pure]
    public static TimeOnly Parse(string s) => FromDateTime(DateTime.Parse(s));

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static TimeOnly Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None)
        => FromDateTime(DateTime.Parse(s.ToString(), provider, style));

    [Pure]
    public static TimeOnly Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => FromDateTime(DateTime.Parse(s.ToString(), provider));

    [Pure]
    public static TimeOnly ParseExact(string s, string format) => ParseExact(s, [format], null);

    [Pure]
    public static TimeOnly ParseExact(string s, string format, IFormatProvider? provider, DateTimeStyles style)
        => ParseExact(s, [format], provider, style);

    [Pure]
    public static TimeOnly ParseExact(string s, string[] formats) => ParseExact(s, formats, null);

    [Pure]
    public static TimeOnly ParseExact(string s, string[] formats, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        if (TryParseExact(s, formats, provider, style, out var result))
        {
            return result;
        }

        throw new FormatException();
    }

    [Pure]
    public static TimeOnly ParseExact(ReadOnlySpan<char> s, string[] formats) => ParseExact(s.ToString(), formats, null);

    [Pure]
    public static TimeOnly ParseExact(ReadOnlySpan<char> s, string[] formats, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
        => ParseExact(s.ToString(), formats, provider, style);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
    {
        if (DateTime.TryParse(s, provider, style, out var dateTime))
        {
            result = FromDateTime(dateTime);
            return true;
        }

        result = default;
        return false;
    }

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TimeOnly result)
        => TryParse(s, provider, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, out TimeOnly result) => TryParse(s, null, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
        => TryParse(s.ToString(), provider, style, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TimeOnly result)
        => TryParse(s.ToString(), provider, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out TimeOnly result) => TryParse(s.ToString(), null, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParseExact(
        [NotNullWhen(true)] string? s,
        [NotNullWhen(true)] string? format,
        IFormatProvider? provider,
        DateTimeStyles style,
        out TimeOnly result)
        => TryParseExact(s, [format], provider, style, out result);

    [Pure]
    public static bool TryParseExact([NotNullWhen(true)] string? s, [NotNullWhen(true)] string? format, out TimeOnly result)
        => TryParseExact(s, [format], null, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParseExact([NotNullWhen(true)] string? s, [NotNullWhen(true)] string?[]? formats, out TimeOnly result)
        => TryParseExact(s, formats, null, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParseExact(
        [NotNullWhen(true)] string? s,
        [NotNullWhen(true)] string?[]? formats,
        IFormatProvider? provider,
        DateTimeStyles style,
        out TimeOnly result)
    {
        if (s is { } && formats is { })
        {
            foreach (var format in formats)
            {
                DateTime time;
                var timeResult = format switch
                {
                    "t" or "T" => DateTime.TryParseExact(s, format, provider, style, out time),
                    "o" or "O" => DateTime.TryParseExact(s, @"HH:mm:ss.fffffff", null, style, out time),
                    "r" or "R" => DateTime.TryParseExact(s, "HH':'mm':'ss", null, style, out time),

                    _ => throw new NotSupportedException("Not used for testing."),
                };

                if (timeResult)
                {
                    result = FromDateTime(time);
                    return true;
                }
            }
        }

        result = default;
        return false;
    }

    [Pure]
    public static bool TryParseExact(
        ReadOnlySpan<char> s,
        ReadOnlySpan<char> format,
        IFormatProvider? provider,
        DateTimeStyles style,
        out TimeOnly result)
        => TryParseExact(s.ToString(), [format.ToString()], provider, style, out result);

    [Pure]
    public static bool TryParseExact(ReadOnlySpan<char> s, ReadOnlySpan<char> format, out TimeOnly result)
        => TryParseExact(s.ToString(), [format.ToString()], null, DateTimeStyles.None, out result);

    [Pure]
    public static bool TryParseExact(
        ReadOnlySpan<char> s,
        [NotNullWhen(true)] string?[]? formats,
        IFormatProvider? provider,
        DateTimeStyles style,
        out TimeOnly result)
        => TryParseExact(s.ToString(), formats, provider, style, out result);

    [Pure]
    public static bool TryParseExact(ReadOnlySpan<char> s, [NotNullWhen(true)] string?[]? formats, out TimeOnly result)
        => TryParseExact(s.ToString(), formats, null, DateTimeStyles.None, out result);

    readonly TimeSpan timeOfDay;

    TimeOnly(TimeSpan timeOfDay)
    {
        Assert.GreaterOrEqual(timeOfDay, TimeSpan.Zero);
        Assert.Less(timeOfDay, TimeSpan.FromHours(24));

        this.timeOfDay = timeOfDay;
    }

    public TimeOnly([ValueRange(0, TimeSpan.TicksPerDay - 1)] long ticks) : this(new TimeSpan(ticks)) { }

    public TimeOnly([ValueRange(0, 23)] int hour, [ValueRange(0, 59)] int minute) : this(new TimeSpan(hour, minute, 0)) { }

    public TimeOnly([ValueRange(0, 23)] int hour, [ValueRange(0, 59)] int minute, [ValueRange(0, 59)] int second) : this(
        new TimeSpan(hour, minute, second)) { }

    public TimeOnly(
        [ValueRange(0, 23)] int hour,
        [ValueRange(0, 59)] int minute,
        [ValueRange(0, 59)] int second,
        [ValueRange(0, 999)] int millisecond) : this(new TimeSpan(0, hour, minute, second, millisecond)) { }

    public TimeOnly(
        [ValueRange(0, 23)] int hour,
        [ValueRange(0, 59)] int minute,
        [ValueRange(0, 59)] int second,
        [ValueRange(0, 999)] int millisecond,
        [ValueRange(0, 999)] int microsecond) : this(TimeSpan._Ctor(0, hour, minute, second, millisecond, microsecond)) { }

    [Pure]
    public TimeOnly Add(TimeSpan value, out int wrappedDays)
    {
        var ticks = timeOfDay.Ticks + value.Ticks % TimeSpan.TicksPerDay;
        wrappedDays = (int)(value.Ticks / TimeSpan.TicksPerDay);

        switch (ticks)
        {
            case < 0:
                ticks += TimeSpan.TicksPerDay;
                wrappedDays--;
                break;

            case >= TimeSpan.TicksPerDay:
                ticks -= TimeSpan.TicksPerDay;
                wrappedDays++;
                break;
        }

        return new TimeOnly(ticks);
    }

    [Pure]
    public TimeOnly Add(TimeSpan value) => Add(value, out _);

    [Pure]
    public TimeOnly AddHours(double value, out int wrappedDays) => Add(TimeSpan.FromHours(value), out wrappedDays);

    [Pure]
    public TimeOnly AddHours(double value) => Add(TimeSpan.FromHours(value), out _);

    [Pure]
    public TimeOnly AddMinutes(double value, out int wrappedDays) => Add(TimeSpan.FromMinutes(value), out wrappedDays);

    [Pure]
    public TimeOnly AddMinutes(double value) => Add(TimeSpan.FromMinutes(value), out _);

    [Pure]
    public TimeSpan ToTimeSpan() => timeOfDay;

    public override string ToString() => ToString("t", null);

    [Pure]
    public string ToString(string? format) => ToString(format, null);

    [Pure]
    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString(string? format, IFormatProvider? provider)
    {
        var dateTime = new DateTime(2025, 1, 1) + timeOfDay;

        return format switch
        {
            null or "" or "t" => dateTime.ToString("t", provider),
            "T" => dateTime.ToString("T", provider),
            "o" or "O" => $"{dateTime:HH:mm:ss.fffffff}",
            "r" or "R" => $"{dateTime:HH':'mm':'ss}",

            _ => throw new NotSupportedException("Not used for testing."),
        };
    }
}