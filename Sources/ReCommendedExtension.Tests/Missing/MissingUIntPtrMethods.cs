using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingUIntPtrMethods
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
}