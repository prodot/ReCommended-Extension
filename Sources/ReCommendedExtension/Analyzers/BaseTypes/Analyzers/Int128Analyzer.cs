using System.Globalization;
using System.Numerics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion), typeof(RedundantArgumentHint)])]
public sealed class Int128Analyzer() : SignedIntegerAnalyzer<Int128Analyzer.Int128>(ClrTypeNames.Int128)
{
    private protected override TypeCode? TryGetTypeCode() => null;

    public readonly record struct Int128 // todo: remove when available (used only for testing)
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

        public static Int128 operator >>>(Int128 value, int shiftAmount)
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
        public static Int128 Max(Int128 x, Int128 y) => x >= y ? x : y;

        [Pure]
        public static Int128 Min(Int128 x, Int128 y) => x <= y ? x : y;

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

        [Pure]
        public string ToString(string? format, IFormatProvider? provider)
        {
            if (upper == 0)
            {
                return lower.ToString(format, provider);
            }

            var value = ToBigInteger();

            return format is { } ? value.ToString(format, provider) ?? value.ToString(provider) : value.ToString(provider);
        }
    }

    private protected override Int128? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => constant.Cast("Int128").GetText();

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("Int128").GetText();

    private protected override string CastZero(CSharpLanguageLevel languageLevel) => "(Int128)0";

    private protected override bool AreEqual(Int128 x, Int128 y) => x == y;

    private protected override bool IsZero(Int128 value) => value == 0;

    private protected override bool AreMinMaxValues(Int128 min, Int128 max) => (min, max) == (Int128.MinValue, Int128.MaxValue);
}