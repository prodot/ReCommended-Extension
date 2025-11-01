using System.Globalization;
using System.Numerics;
using System.Text;

namespace ReCommendedExtension.Extensions.NumberInfos;

/// <remarks>
/// Original code from <see href="https://github.com/dotnet/dotnet"/><para/>
/// License: MIT<para/>
/// Copyright (c) .NET Foundation and Contributors
/// </remarks>
public readonly record struct Int128 : IFormattable // todo: remove when available (used mostly for testing)
{
    public static Int128 MinValue => new(0x8000_0000_0000_0000, 0);

    public static Int128 MaxValue => new(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);

    public static implicit operator Int128(ushort value) => new(0, value);

    public static implicit operator Int128(int value)
    {
        var lower = (long)value;
        return new Int128(unchecked((ulong)(lower >> 63)), unchecked((ulong)lower));
    }

    public static implicit operator Int128(long value) => new(unchecked((ulong)(value >> 63)), unchecked((ulong)value));

    public static implicit operator Int128(uint value) => new(0, value);

    public static implicit operator Int128(ulong value) => new(0, value);

    public static Int128 operator -(Int128 value) => 0 - value;

    public static Int128 operator -(Int128 x, Int128 y)
    {
        unchecked
        {
            var lower = x.lower - y.lower;
            var upper = x.upper - y.upper;

            if (lower > x.lower)
            {
                upper--;
            }

            return new Int128(upper, lower);
        }
    }

    public static bool operator <(Int128 x, Int128 y)
        => unchecked((long)x.upper) < unchecked((long)y.upper) || x.upper == y.upper && x.lower < y.lower;

    public static bool operator <=(Int128 x, Int128 y)
        => unchecked((long)x.upper) < unchecked((long)y.upper) || x.upper == y.upper && x.lower <= y.lower;

    public static bool operator >(Int128 x, Int128 y)
        => unchecked((long)x.upper) > unchecked((long)y.upper) || x.upper == y.upper && x.lower > y.lower;

    public static bool operator >=(Int128 x, Int128 y)
        => unchecked((long)x.upper) > unchecked((long)y.upper) || x.upper == y.upper && x.lower >= y.lower;

    public static Int128 operator |(Int128 x, Int128 y) => new(x.upper | y.upper, x.lower | y.lower);

    public static Int128 operator <<(Int128 value, int shiftAmount)
    {
        shiftAmount &= 0x7F;

        if ((shiftAmount & 0x40) != 0)
        {
            return new Int128(value.lower << shiftAmount, 0);
        }

        if (shiftAmount != 0)
        {
            return new Int128(value.upper << shiftAmount | value.lower >> (64 - shiftAmount), value.lower << shiftAmount);
        }

        return value;
    }

    public static Int128 operator >>> (Int128 value, int shiftAmount)
    {
        shiftAmount &= 0x7F;

        if ((shiftAmount & 0x40) != 0)
        {
            return new Int128(0, value.upper >> shiftAmount);
        }

        if (shiftAmount != 0)
        {
            return new Int128(value.upper >> shiftAmount, value.lower >> shiftAmount | value.upper << (64 - shiftAmount));
        }

        return value;
    }

    [Pure]
    public static Int128 Clamp(Int128 value, Int128 min, Int128 max)
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
    public static bool IsNegative(Int128 value) => unchecked((long)value.upper) < 0;

    [Pure]
    public static bool IsPositive(Int128 value) => unchecked((long)value.upper) >= 0;

    [Pure]
    public static Int128 Max(Int128 x, Int128 y) => x >= y ? x : y;

    [Pure]
    public static Int128 Min(Int128 x, Int128 y) => x <= y ? x : y;

    [Pure]
    public static Int128 MaxMagnitude(Int128 x, Int128 y)
    {
        var absX = x;

        if (IsNegative(absX))
        {
            absX = -absX;

            if (IsNegative(absX))
            {
                return x;
            }
        }

        var absY = y;

        if (IsNegative(absY))
        {
            absY = -absY;

            if (IsNegative(absY))
            {
                return y;
            }
        }

        if (absX > absY)
        {
            return x;
        }

        if (absX == absY)
        {
            return IsNegative(x) ? y : x;
        }

        return y;
    }

    [Pure]
    public static Int128 MinMagnitude(Int128 x, Int128 y)
    {
        var absX = x;

        if (IsNegative(absX))
        {
            absX = unchecked(-absX);

            if (IsNegative(absX))
            {
                return y;
            }
        }

        var absY = y;

        if (IsNegative(absY))
        {
            absY = unchecked(-absY);

            if (IsNegative(absY))
            {
                return x;
            }
        }

        if (absX < absY)
        {
            return x;
        }

        if (absX == absY)
        {
            return IsNegative(x) ? x : y;
        }

        return y;
    }

    [Pure]
    public static (Int128 Quotient, Int128 Remainder) DivRem(Int128 left, Int128 right)
    {
        if (left.upper == 0 && right.upper == 0)
        {
            var quotient = left.lower / right.lower;
            return (quotient, unchecked(left.lower - quotient * right.lower));
        }

        {
            var quotient = BigInteger.DivRem(left.ToBigInteger(), right.ToBigInteger(), out var remainder);
            return (new Int128(quotient), new Int128(remainder));
        }
    }

    [Pure]
    public static Int128 RotateLeft(Int128 value, int rotateAmount) => value << rotateAmount | value >>> (128 - rotateAmount);

    [Pure]
    public static Int128 RotateRight(Int128 value, int rotateAmount) => value >>> rotateAmount | value << (128 - rotateAmount);

    [Pure]
    public static Int128 Parse(string s) => Parse(s, NumberStyles.Integer, provider: null);

    [Pure]
    public static Int128 Parse(string s, NumberStyles style) => Parse(s, style, provider: null);

    [Pure]
    public static Int128 Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

    [Pure]
    public static Int128 Parse(string s, NumberStyles style, IFormatProvider? provider) => new(BigInteger.Parse(s, style, provider));

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, out Int128 result) => TryParse(s, NumberStyles.Integer, null, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Int128 result)
        => TryParse(s, NumberStyles.Integer, provider, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Int128 result)
    {
        if (BigInteger.TryParse(s, style, provider, out var value) && value >= MinValue.ToBigInteger() && value <= MaxValue.ToBigInteger())
        {
            result = new Int128(value);
            return true;
        }

        result = default;
        return false;
    }

    readonly ulong lower;
    readonly ulong upper;

    Int128(ulong upper, ulong lower)
    {
        this.lower = lower;
        this.upper = upper;
    }

    Int128(BigInteger value)
    {
        if (value < MinValue.ToBigInteger() || value > MaxValue.ToBigInteger())
        {
            throw new OverflowException();
        }

        lower = (ulong)(value & ulong.MaxValue);
        upper = (ulong)(value >> 64 & ulong.MaxValue);
    }

    [Pure]
    BigInteger ToBigInteger()
    {
        var bigInt = (BigInteger)upper << 64 | lower;

        if ((upper & MinValue.upper) != 0)
        {
            bigInt -= BigInteger.One << 128;
        }

        return bigInt;
    }

    public override int GetHashCode() => (lower, upper).GetHashCode();

    public bool Equals(Int128 other) => (lower, upper) == (other.lower, other.upper);

    public override string ToString() => ToString(null, NumberFormatInfo.CurrentInfo);

    [Pure]
    public string ToString(string? format) => ToString(format, NumberFormatInfo.CurrentInfo);

    [Pure]
    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString(string? format, IFormatProvider? provider)
    {
        if (upper == 0)
        {
            return lower.ToString(format, provider);
        }

        var value = ToBigInteger();

        var s = format is { } ? value.ToString(format, provider) ?? value.ToString(provider) : value.ToString(provider);

        if (format is ['G' or 'g' or 'D' or 'd', _, ..]) // remove leading zeros
        {
            var builder = new StringBuilder(s);

            if (this < 0)
            {
                while (builder is [_, '0', ..])
                {
                    builder.Remove(1, 1);
                }
            }
            else
            {
                while (builder is ['0', ..])
                {
                    builder.Remove(0, 1);
                }
            }

            s = builder.ToString();
        }

        return s;
    }
}