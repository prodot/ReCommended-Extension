using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingIntPtrMethods
{
    [Pure]
    public static nint Clamp(nint value, nint min, nint max)
    {
        if (min > max)
        {
            throw new ArgumentException($"'{min}' cannot be greater than {max}.");
        }

        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    [Pure]
    public static (nint Quotient, nint Remainder) DivRem(nint left, nint right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static nint Max(nint x, nint y) => x >= y ? x : y;

    [Pure]
    public static nint Min(nint x, nint y) => x <= y ? x : y;

    [Pure]
    public static nint Parse(string s) => Parse(s, NumberStyles.Integer, provider: null);

    [Pure]
    public static nint Parse(string s, NumberStyles style) => Parse(s, style, provider: null);

    [Pure]
    public static nint Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

    [Pure]
    public static nint Parse(string s, NumberStyles style, IFormatProvider? provider)
        => IntPtr.Size switch
        {
            8 => (nint)long.Parse(s, style, provider),
            4 => int.Parse(s, style, provider),

            _ => throw new PlatformNotSupportedException(),
        };

    [Pure]
    public static nint Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static nint Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static nint Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static nint Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => Parse(s.ToString(), style, provider);

    [Pure]
    public static nint RotateLeft(nint value, int rotateAmount) => unchecked((nint)MissingUIntPtrMethods.RotateLeft((nuint)value, rotateAmount));

    [Pure]
    public static nint RotateRight(nint value, int rotateAmount) => unchecked((nint)MissingUIntPtrMethods.RotateRight((nuint)value, rotateAmount));

    [Pure]
    public static string ToString(this nint value, IFormatProvider? provider) => value.ToString(null, provider);

    [Pure]
    public static string ToString(this nint value, string? format, IFormatProvider? provider) => value.ToString(format);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, out nint result) => TryParse(s, NumberStyles.Number, null, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out nint result)
        => TryParse(s, NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out nint result)
    {
        switch (IntPtr.Size)
        {
            case 8:
                if (long.TryParse(s, style, provider, out var int64Value))
                {
                    result = (nint)int64Value;
                    return true;
                }
                break;

            case 4:
                if (int.TryParse(s, style, provider, out var int32Value))
                {
                    result = int32Value;
                    return true;
                }
                break;

            default: throw new PlatformNotSupportedException();
        }

        result = default;
        return false;
    }

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out nint result)
        => TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out nint result)
        => TryParse(s.ToString(), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out nint result) => TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out nint result)
        => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out nint result)
        => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out nint result) => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}