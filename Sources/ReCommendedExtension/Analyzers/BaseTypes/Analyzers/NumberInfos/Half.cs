using System.Globalization;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

/// <remarks>
/// Original code from <see href="https://github.com/dotnet/dotnet"/><para/>
/// License: MIT<para/>
/// Copyright (c) .NET Foundation and Contributors
/// </remarks>
public readonly record struct Half // todo: remove when available (used mostly for testing)
{
    public static Half Epsilon => new(0x0001);

    public static Half PositiveInfinity => new(0x7C00);

    public static Half NegativeInfinity => new(0xFC00);

    public static Half NaN => new(0xFE00);

    public static Half MinValue => new(0xFBFF);

    public static Half MaxValue => new(0x7BFF);

    public static implicit operator Half(byte value) => (Half)(float)value;

    public static implicit operator Half(sbyte value) => (Half)(float)value;

    public static explicit operator Half(float value)
    {
        unchecked
        {
            const uint minExp = 0x3880_0000u;
            const uint exponent126 = 0x3f00_0000u;
            const uint singleBiasedExponentMask = 0x7F80_0000;
            const uint exponent13 = 0x0680_0000u;
            const float maxHalfValueBelowInfinity = 65520.0f;
            const uint exponentMask = 0x7C00;

            var bitValue = SingleToUInt32Bits(value);
            var sign = (bitValue & 0x8000_0000) >> 16;
            var realMask = float.IsNaN(value) ? 0u : ~0u;

            value = Math.Abs(value);
            value = Math.Min(maxHalfValueBelowInfinity, value);

            var exponentOffset0 = SingleToUInt32Bits(Math.Max(value, UInt32BitsToSingle(minExp))) & singleBiasedExponentMask + exponent13;

            value += UInt32BitsToSingle(exponentOffset0);
            bitValue = SingleToUInt32Bits(value);

            var maskedHalfExponentForNaN = ~realMask & exponentMask;

            bitValue -= exponent126;

            var newExponent = bitValue >> 13;

            bitValue &= realMask;
            bitValue += newExponent;
            bitValue &= ~maskedHalfExponentForNaN;
            bitValue |= maskedHalfExponentForNaN | sign;

            return new Half((ushort)bitValue);
        }
    }

    public static explicit operator float(Half value)
    {
        unchecked
        {
            const uint exponentLowerBound = 0x3880_0000u;
            const uint exponentOffset = 0x3800_0000u;
            const uint singleSignMask = 0x7FFFFFFFu;
            const uint halfExponentMask = 0x7C00;
            const int halfToSingleBitsMask = 0x0FFF_E000;

            var valueInInt16Bits = (short)value.bits;
            var sign = (uint)valueInInt16Bits & singleSignMask;
            var bitValueInProcess = (uint)valueInInt16Bits;
            var offsetExponent = bitValueInProcess & halfExponentMask;
            var subnormalMask = offsetExponent == 0u ? ~0u : 0u;
            var maskedExponentLowerBound = subnormalMask & exponentLowerBound;
            var offsetMaskedExponentLowerBound = exponentOffset | maskedExponentLowerBound;

            bitValueInProcess <<= 13;
            offsetMaskedExponentLowerBound <<= offsetExponent == halfExponentMask ? 1 : 0;
            bitValueInProcess &= halfToSingleBitsMask;
            bitValueInProcess += offsetMaskedExponentLowerBound;

            var absoluteValue = SingleToUInt32Bits(UInt32BitsToSingle(bitValueInProcess) - UInt32BitsToSingle(maskedExponentLowerBound));

            return UInt32BitsToSingle(absoluteValue | sign);
        }
    }

    [Pure]
    static uint SingleToUInt32Bits(float value) => BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);

    [Pure]
    static float UInt32BitsToSingle(uint value) => BitConverter.ToSingle(BitConverter.GetBytes(value), 0);

    [Pure]
    static bool AreZero(Half x, Half y) => ((x.bits | y.bits) & ~0x8000) == 0;

    [Pure]
    static bool IsNaNOrZero(Half value) => ((uint)value.bits - 1 & ~0x8000) >= 0x7C00;

    [Pure]
    static bool IsNaN(Half value) => ((uint)value.bits & ~0x8000) > 0x7C00;

    [Pure]
    public static Half Parse(string s) => Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);

    [Pure]
    public static Half Parse(string s, NumberStyles style) => Parse(s, style, null);

    [Pure]
    public static Half Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static Half Parse(string s, NumberStyles style, IFormatProvider? provider) => (Half)float.Parse(s, style, provider);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, out Half result)
        => TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, null, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Half result)
        => TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Half result)
    {
        if (float.TryParse(s, style, provider, out var value))
        {
            result = (Half)value;
            return true;
        }

        result = default;
        return false;
    }

    readonly ushort bits;

    Half(ushort bits) => this.bits = bits;

    public override int GetHashCode()
    {
        var bits = (uint)this.bits;

        if (IsNaNOrZero(this))
        {
            bits &= 0x7C00;
        }

        return unchecked((int)bits);
    }

    public bool Equals(Half other) => bits == other.bits || AreZero(this, other) || IsNaN(this) && IsNaN(other);

    public override string ToString() => ToString(null, NumberFormatInfo.CurrentInfo);

    [Pure]
    public string ToString(string? format) => ToString(format, NumberFormatInfo.CurrentInfo);

    [Pure]
    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    [Pure]
    public string ToString(string? format, IFormatProvider? provider) => ((float)this).ToString(format, provider);
}