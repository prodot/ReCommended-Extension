using System.Text;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Tests.Missing;

internal static class Extensions
{
    [Pure]
    public static string ToString<T>(this T value, string? format)
        => value switch
        {
            Half v => v.ToString(format),
            Int128 v => v.ToString(format),
            UInt128 v => v.ToString(format),
            IFormattable formattable => formattable.ToString(format, null),
            nint v => v.ToString(format),
            nuint v => v.ToString(format),

            _ => throw new NotSupportedException(),
        };

    [Pure]
    public static string ToString<T>(this T value, IFormatProvider? provider)
        => value switch
        {
            Half v => v.ToString(provider),
            Int128 v => v.ToString(provider),
            UInt128 v => v.ToString(provider),
            IConvertible convertible => convertible.ToString(provider),
            IFormattable formattable => formattable.ToString(null, provider),
            nint v => v.ToString(provider),
            nuint v => v.ToString(provider),

            _ => throw new NotSupportedException(),
        };

    [Pure]
    public static string ToString<T>(this T value, string? format, IFormatProvider? provider)
        => value switch
        {
            IFormattable formattable => formattable.ToString(format, provider),
            nint v => v.ToString(format, provider),
            nuint v => v.ToString(format, provider),

            _ => throw new NotSupportedException(),
        };

    [Pure]
    public static byte[] AsUtf8Bytes(this string value) => Encoding.UTF8.GetBytes(value);
}