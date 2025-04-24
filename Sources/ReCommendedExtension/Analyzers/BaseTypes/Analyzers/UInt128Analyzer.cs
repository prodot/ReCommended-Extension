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
public sealed class UInt128Analyzer() : IntegerAnalyzer<UInt128Analyzer.UInt128>(ClrTypeNames.UInt128)
{
    private protected override TypeCode? TryGetTypeCode() => null;

    public readonly record struct UInt128 // todo: remove when available (used only for testing)
    {
        public static UInt128 MinValue => new(0, 0);

        public static UInt128 MaxValue => new(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);

        public static explicit operator UInt128(int value)
        {
            var lower = (long)value;
            return new UInt128(unchecked((ulong)(lower >> 63)), unchecked((ulong)lower));
        }

        public static implicit operator UInt128(long value) => new(unchecked((ulong)(value >> 63)), unchecked((ulong)value));

        public static implicit operator UInt128(uint value) => new(0, value);

        public static implicit operator UInt128(ulong value) => new(0, value);

        public static bool operator <(UInt128 x, UInt128 y) => x.upper < y.upper || x.upper == y.upper && x.lower < y.lower;

        public static bool operator <=(UInt128 x, UInt128 y) => x.upper < y.upper || x.upper == y.upper && x.lower <= y.lower;

        public static bool operator >(UInt128 x, UInt128 y) => x.upper > y.upper || x.upper == y.upper && x.upper > y.upper;

        public static bool operator >=(UInt128 x, UInt128 y) => x.upper > y.upper || x.upper == y.upper && x.lower >= y.lower;

        public static UInt128 operator |(UInt128 x, UInt128 y) => new(x.upper | y.upper, x.lower | y.lower);

        public static UInt128 operator <<(UInt128 value, int shiftAmount)
        {
            shiftAmount &= 0x7F;

            if ((shiftAmount & 0x40) != 0)
            {
                return new UInt128(value.lower << shiftAmount, 0);
            }

            if (shiftAmount != 0)
            {
                return new UInt128(value.upper << shiftAmount | value.lower >> (64 - shiftAmount), value.lower << shiftAmount);
            }

            return value;
        }

        public static UInt128 operator >>>(UInt128 value, int shiftAmount)
        {
            shiftAmount &= 0x7F;

            if ((shiftAmount & 0x40) != 0)
            {
                return new UInt128(0, value.upper >> shiftAmount);
            }

            if (shiftAmount != 0)
            {
                return new UInt128(value.upper >> shiftAmount, value.lower >> shiftAmount | value.upper << (64 - shiftAmount));
            }

            return value;
        }

        [Pure]
        public static UInt128 Clamp(UInt128 value, UInt128 min, UInt128 max)
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
        public static UInt128 Max(UInt128 x, UInt128 y) => x >= y ? x : y;

        [Pure]
        public static UInt128 Min(UInt128 x, UInt128 y) => x <= y ? x : y;

        [Pure]
        public static (UInt128 Quotient, UInt128 Remainder) DivRem(UInt128 left, UInt128 right)
        {
            if (left.upper == 0 && right.upper == 0)
            {
                var quotient = left.lower / right.lower;
                return (quotient, unchecked(left.lower - quotient * right.lower));
            }

            {
                var quotient = BigInteger.DivRem(left.ToBigInteger(), right.ToBigInteger(), out var remainder);
                return (new UInt128(quotient), new UInt128(remainder));
            }
        }

        [Pure]
        public static UInt128 Parse(string s) => Parse(s, NumberStyles.Integer, provider: null);

        [Pure]
        public static UInt128 Parse(string s, NumberStyles style) => Parse(s, style, provider: null);

        [Pure]
        public static UInt128 Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

        [Pure]
        public static UInt128 Parse(string s, NumberStyles style, IFormatProvider? provider) => new(BigInteger.Parse(s, style, provider));

        [Pure]
        public static UInt128 RotateLeft(UInt128 value, int rotateAmount) => value << rotateAmount | value >>> (128 - rotateAmount);

        [Pure]
        public static UInt128 RotateRight(UInt128 value, int rotateAmount) => value >>> rotateAmount | value << (128 - rotateAmount);

        [Pure]
        public static bool TryParse([NotNullWhen(true)] string? s, out UInt128 result) => TryParse(s, NumberStyles.Integer, null, out result);

        [Pure]
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UInt128 result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        [Pure]
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out UInt128 result)
        {
            if (BigInteger.TryParse(s, style, provider, out var value) && value >= 0 && value <= MaxValue.ToBigInteger())
            {
                result = new UInt128(value);
                return true;
            }

            result = default;
            return false;
        }

        readonly ulong lower;
        readonly ulong upper;

        UInt128(ulong upper, ulong lower)
        {
            this.lower = lower;
            this.upper = upper;
        }

        UInt128(BigInteger value)
        {
            if (value < 0 || value > MaxValue.ToBigInteger())
            {
                throw new OverflowException();
            }

            lower = (ulong)(value & ulong.MaxValue);
            upper = (ulong)(value >> 64 & ulong.MaxValue);
        }

        [Pure]
        BigInteger ToBigInteger() => (BigInteger)upper << 64 | lower;

        public override int GetHashCode() => (lower, upper).GetHashCode();

        public bool Equals(UInt128 other) => (lower, upper) == (other.lower, other.upper);

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

    private protected override UInt128? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Long, LongValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => constant.Cast("UInt128").GetText();

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("UInt128").GetText();

    private protected override string CastZero(CSharpLanguageLevel languageLevel) => "(UInt128)0";

    private protected override bool AreEqual(UInt128 x, UInt128 y) => x == y;

    private protected override bool IsZero(UInt128 value) => value == 0;

    private protected override bool AreMinMaxValues(UInt128 min, UInt128 max) => (min, max) == (UInt128.MinValue, UInt128.MaxValue);
}