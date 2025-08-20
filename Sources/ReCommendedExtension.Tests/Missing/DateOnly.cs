using System.Globalization;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Missing;

public readonly record struct DateOnly : IFormattable
{
    public static DateOnly MaxValue => FromDateTime(DateTime.MaxValue);

    public static DateOnly MinValue => FromDateTime(DateTime.MinValue);

    [Pure]
    public static DateOnly FromDateTime(DateTime dateTime) => new(dateTime.Date);

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static DateOnly Parse(string s, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
        => FromDateTime(DateTime.Parse(s, provider, style));

    [Pure]
    public static DateOnly Parse(string s, IFormatProvider? provider) => FromDateTime(DateTime.Parse(s, provider));

    [Pure]
    public static DateOnly Parse(string s) => FromDateTime(DateTime.Parse(s));

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static DateOnly Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None)
        => FromDateTime(DateTime.Parse(s.ToString(), provider, style));

    [Pure]
    public static DateOnly Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => FromDateTime(DateTime.Parse(s.ToString(), provider));

    [Pure]
    public static DateOnly ParseExact(string s, string format) => ParseExact(s, [format], null);

    [Pure]
    public static DateOnly ParseExact(string s, string format, IFormatProvider? provider, DateTimeStyles style)
        => ParseExact(s, [format], provider, style);

    [Pure]
    public static DateOnly ParseExact(string s, string[] formats) => ParseExact(s, formats, null);

    [Pure]
    public static DateOnly ParseExact(string s, string[] formats, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
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
    public static DateOnly ParseExact(ReadOnlySpan<char> s, string[] formats) => ParseExact(s.ToString(), formats, null);

    [Pure]
    public static DateOnly ParseExact(ReadOnlySpan<char> s, string[] formats, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
        => ParseExact(s.ToString(), formats, provider, style);

    [Pure]
    public static bool TryParseExact(
        [NotNullWhen(true)] string? s,
        [NotNullWhen(true)] string?[]? formats,
        IFormatProvider? provider,
        DateTimeStyles style,
        out DateOnly result)
    {
        if (s is { } && formats is { })
        {
            foreach (var format in formats)
            {
                DateTime date;
                var dateResult = format switch
                {
                    "d" or "D" or "m" or "M" or "y" or "Y" => DateTime.TryParseExact(s, format, provider, style, out date),
                    "o" or "O" => DateTime.TryParseExact(s, "yyyy'-'MM'-'dd", null, style, out date),
                    "r" or "R" => DateTime.TryParseExact(s, "ddd, dd MMM yyyy", null, style, out date),

                    _ => throw new NotSupportedException("Not used for testing."),
                };

                if (dateResult)
                {
                    result = FromDateTime(date);
                    return true;
                }
            }
        }

        result = default;
        return false;
    }

    readonly DateTime date;

    DateOnly(DateTime date)
    {
        Assert.AreEqual(TimeSpan.Zero, date.TimeOfDay);

        this.date = date;
    }

    public DateOnly([ValueRange(1, 9_999)] int year, [ValueRange(1, 12)] int month, [ValueRange(1, 31)] int day)
        => date = new DateTime(year, month, day);

    [Pure]
    public DateTime ToDateTime(TimeOnly time) => date + time.ToTimeSpan();

    [Pure]
    public DateOnly AddDays(int value) => FromDateTime(date.AddDays(value));

    public override string ToString() => date.ToString("d");

    [Pure]
    public string ToString(string? format) => ToString(format, null);

    [Pure]
    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString(string? format, IFormatProvider? provider)
        => format switch
        {
            null or "" or "d" => date.ToString("d", provider),
            "D" or "m" or "M" or "y" or "Y" => date.ToString(format, provider),
            "o" or "O" => $"{date:yyyy'-'MM'-'dd}",
            "r" or "R" => $"{date:ddd, dd MMM yyyy}",

            _ => throw new NotSupportedException("Not used for testing."),
        };
}