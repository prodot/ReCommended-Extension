using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class FloatingPointNumberAnalyzer<N>(IClrTypeName clrTypeName) : NumberAnalyzer<N>(clrTypeName) where N : struct
{
    private protected sealed override void AnalyzeEquals_Number(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        // avoid the "==" operator
    }

    private protected sealed override bool AreEqual(N x, N y) => false; // can only be checked by comparing literals

    private protected sealed override bool AreMinMaxValues(N min, N max) => false; // can only be checked by comparing literals
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(UseExpressionResultSuggestion)])]
public sealed class DoubleAnalyzer() : FloatingPointNumberAnalyzer<double>(PredefinedType.DOUBLE_FQN)
{
    private protected override double? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Double, DoubleValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Float, FloatValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: var value }:
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => throw new NotImplementedException();
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(UseExpressionResultSuggestion)])]
public sealed class SingleAnalyzer() : FloatingPointNumberAnalyzer<float>(PredefinedType.FLOAT_FQN)
{
    private protected override float? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Float, FloatValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: var value }:
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => throw new NotImplementedException();
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(UseExpressionResultSuggestion)])]
public sealed class HalfAnalyzer() : FloatingPointNumberAnalyzer<HalfAnalyzer.Half>(ClrTypeNames.Half)
{
    public readonly record struct Half // todo: remove when available (used only for testing)
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
    }

    private protected override Half? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => throw new NotImplementedException();
}