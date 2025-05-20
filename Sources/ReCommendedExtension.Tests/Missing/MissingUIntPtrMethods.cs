using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

[SuppressMessage("ReSharper", "UnusedParameter.Global")]
internal static class MissingUIntPtrMethods // todo: consider implementing IFormattable
{
    [Pure]
    public static nuint Clamp(nuint value, nuint min, nuint max)
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
    public static (nuint Quotient, nuint Remainder) DivRem(nuint left, nuint right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static nuint Max(nuint x, nuint y) => x >= y ? x : y;

    [Pure]
    public static nuint Min(nuint x, nuint y) => x <= y ? x : y;

    [Pure]
    public static nuint Parse(string s) => Parse(s, NumberStyles.Integer, provider: null);

    [Pure]
    public static nuint Parse(string s, NumberStyles style) => Parse(s, style, provider: null);

    [Pure]
    public static nuint Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

    [Pure]
    public static nuint Parse(string s, NumberStyles style, IFormatProvider? provider)
        => UIntPtr.Size switch
        {
            8 => (nuint)ulong.Parse(s, style, provider),
            4 => uint.Parse(s, style, provider),

            _ => throw new PlatformNotSupportedException(),
        };

    [Pure]
    public static nuint Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static nuint Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static nuint Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static nuint Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => Parse(s.ToString(), style, provider);

    [Pure]
    public static nuint RotateLeft(nuint value, int offset)
        => UIntPtr.Size switch
        {
            8 => (nuint)MissingUInt64Methods.RotateLeft(value, offset),
            4 => MissingUInt32Methods.RotateLeft((uint)value, offset),

            _ => throw new PlatformNotSupportedException(),
        };

    [Pure]
    public static nuint RotateRight(nuint value, int offset)
        => UIntPtr.Size switch
        {
            8 => (nuint)MissingUInt64Methods.RotateRight(value, offset),
            4 => MissingUInt32Methods.RotateRight((uint)value, offset),

            _ => throw new PlatformNotSupportedException(),
        };

    [Pure]
    public static string ToString(this nuint value, string? format) => value.ToString(format, NumberFormatInfo.CurrentInfo);

    [Pure]
    public static string ToString(this nuint value, IFormatProvider? provider) => value.ToString(null, provider);

    [Pure]
    public static string ToString(this nuint value, string? format, IFormatProvider? provider) => value.ToString();

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, out nuint result) => TryParse(s, NumberStyles.Number, null, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out nuint result)
        => TryParse(s, NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out nuint result)
    {
        switch (IntPtr.Size)
        {
            case 8:
                if (ulong.TryParse(s, style, provider, out var uint64Value))
                {
                    result = (nuint)uint64Value;
                    return true;
                }
                break;

            case 4:
                if (uint.TryParse(s, style, provider, out var uint32Value))
                {
                    result = uint32Value;
                    return true;
                }
                break;

            default: throw new PlatformNotSupportedException();
        }

        result = default;
        return false;
    }

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out nuint result)
        => TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out nuint result)
        => TryParse(s.ToString(), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out nuint result) => TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out nuint result)
        => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out nuint result)
        => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out nuint result) => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}