using System.Numerics;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class IntegerAnalyzer<N>(IClrTypeName clrTypeName) : NumberAnalyzer<N>(clrTypeName) where N : struct
{
    /// <remarks>
    /// <c>T.DivRem(0, right)</c> → <c>(0, 0)</c><para/>
    /// <c>T.DivRem(left, 1)</c> → <c>(left, 0)</c>
    /// </remarks>
    void AnalyzeDivRem(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument leftArgument,
        ICSharpArgument rightArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetConstant(rightArgument.Value, out _) is { } right)
        {
            if (TryGetConstant(leftArgument.Value, out _) is { } left && IsZero(left) && !IsZero(right))
            {
                var replacement = invocationExpression.TryGetTargetType().IsValueTuple(out var t1TypeArgument, out var t2TypeArgument)
                    && t1TypeArgument.IsClrType(ClrTypeName)
                    && t2TypeArgument.IsClrType(ClrTypeName)
                        ? "(0, 0)"
                        : $"(Quotient: {CastZero()}, Remainder: {CastZero()})";

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion("The expression is always (0, 0).", invocationExpression, replacement));

                return;
            }

            if (IsOne(right) && leftArgument.Value is { } leftValue)
            {
                var replacement = invocationExpression.TryGetTargetType().IsValueTuple(out var t1TypeArgument, out var t2TypeArgument)
                    && t1TypeArgument.IsClrType(ClrTypeName)
                    && t2TypeArgument.IsClrType(ClrTypeName)
                        ? $"({leftValue.GetText()}, 0)"
                        : TryGetConstant(leftArgument.Value, out var leftImplicitlyConverted) is { } && leftImplicitlyConverted
                            ? $"(Quotient: {CastConstant(leftValue, leftImplicitlyConverted)}, Remainder: {CastZero()})"
                            : $"(Quotient: {leftValue.GetText()}, Remainder: {CastZero()})";

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always the same as the first argument with no remainder.",
                        invocationExpression,
                        replacement));
            }
        }
    }

    [Pure]
    private protected abstract string CastZero();

    [Pure]
    private protected abstract bool IsZero(N value);

    [Pure]
    private protected abstract bool IsOne(N value);

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(ClrTypeName))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName) { }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case "DivRem": // todo: nameof(IBinaryInteger<T>.DivRem) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var leftType }, { Type: var rightType }], [var leftArgument, var rightArgument])
                                    when leftType.IsClrType(ClrTypeName) && rightType.IsClrType(ClrTypeName):

                                    AnalyzeDivRem(consumer, element, leftArgument, rightArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        if (method.ContainingType.IsClrType(ClrTypeNames.Math) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case nameof(Math.DivRem):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var leftType }, { Type: var rightType }], [var leftArgument, var rightArgument])
                            when leftType.IsClrType(ClrTypeName) && rightType.IsClrType(ClrTypeName):

                            AnalyzeDivRem(consumer, element, leftArgument, rightArgument);
                            break;
                    }
                    break;
            }
        }
    }
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class SByteAnalyzer() : IntegerAnalyzer<sbyte>(PredefinedType.SBYTE_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.SByte;

    private protected override sbyte? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= sbyte.MinValue and <= sbyte.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((sbyte)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            return $"(sbyte){result}";
        }

        return result;
    }

    private protected override string CastZero() => "(sbyte)0";

    private protected override bool AreEqual(sbyte x, sbyte y) => x == y;

    private protected override bool IsZero(sbyte value) => value == 0;

    private protected override bool IsOne(sbyte value) => value == 1;

    private protected override bool AreMinMaxValues(sbyte min, sbyte max) => (min, max) == (sbyte.MinValue, sbyte.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class Int16Analyzer() : IntegerAnalyzer<short>(PredefinedType.SHORT_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.Int16;

    private protected override short? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= short.MinValue and <= short.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((short)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            return $"(short){result}";
        }

        return result;
    }

    private protected override string CastZero() => "(short)0";

    private protected override bool AreEqual(short x, short y) => x == y;

    private protected override bool IsZero(short value) => value == 0;

    private protected override bool IsOne(short value) => value == 1;

    private protected override bool AreMinMaxValues(short min, short max) => (min, max) == (short.MinValue, short.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class UInt16Analyzer() : IntegerAnalyzer<ushort>(PredefinedType.USHORT_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.UInt16;

    private protected override ushort? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= ushort.MinValue and <= ushort.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((ushort)value);

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            return $"(ushort){result}";
        }

        return result;
    }

    private protected override string CastZero() => "(ushort)0";

    private protected override bool AreEqual(ushort x, ushort y) => x == y;

    private protected override bool IsZero(ushort value) => value == 0;

    private protected override bool IsOne(ushort value) => value == 1;

    private protected override bool AreMinMaxValues(ushort min, ushort max) => (min, max) == (ushort.MinValue, ushort.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class Int32Analyzer() : IntegerAnalyzer<int>(PredefinedType.INT_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.Int32;

    private protected override int? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = false;
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

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted || result is ['\'', .., '\''])
        {
            return $"(int){result}";
        }

        return result;
    }

    private protected override string CastZero() => "0";

    private protected override bool AreEqual(int x, int y) => x == y;

    private protected override bool IsZero(int value) => value == 0;

    private protected override bool IsOne(int value) => value == 1;

    private protected override bool AreMinMaxValues(int min, int max) => (min, max) == (int.MinValue, int.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class UInt32Analyzer() : IntegerAnalyzer<uint>(PredefinedType.UINT_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.UInt32;

    private protected override uint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((uint)value);

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            if (result is ['\'', .., '\''])
            {
                return $"(uint){result}";
            }

            if (constant is ICSharpLiteralExpression)
            {
                return $"{result}u";
            }

            return $"(uint){result}";
        }

        return result;
    }

    private protected override string CastZero() => "0u";

    private protected override bool AreEqual(uint x, uint y) => x == y;

    private protected override bool IsZero(uint value) => value == 0;

    private protected override bool IsOne(uint value) => value == 1;

    private protected override bool AreMinMaxValues(uint min, uint max) => (min, max) == (uint.MinValue, uint.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class Int64Analyzer() : IntegerAnalyzer<long>(PredefinedType.LONG_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.Int64;

    private protected override long? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = false;
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

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (result is ['\'', .., '\''])
                {
                    return $"(long){result}";
                }

                if (result is [.. var rest, 'u' or 'U'])
                {
                    return $"{rest}L";
                }

                return $"{result}L";
            }

            return $"(long){result}";
        }

        return result;
    }

    private protected override string CastZero() => "0L";

    private protected override bool AreEqual(long x, long y) => x == y;

    private protected override bool IsZero(long value) => value == 0;

    private protected override bool IsOne(long value) => value == 1;

    private protected override bool AreMinMaxValues(long min, long max) => (min, max) == (long.MinValue, long.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class UInt64Analyzer() : IntegerAnalyzer<ulong>(PredefinedType.ULONG_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.UInt64;

    private protected override ulong? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((ulong)value);

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((ulong)value);

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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (result is ['\'', .., '\''])
                {
                    return $"(ulong){result}";
                }

                return result switch
                {
                    [.. var rest, 'l' or 'L'] => $"{rest}ul",
                    [.. var rest, 'u' or 'U'] => $"{rest}ul",

                    _ => $"{result}ul",
                };
            }

            return $"(ulong){result}";
        }

        return result;
    }

    private protected override string CastZero() => "0ul";

    private protected override bool AreEqual(ulong x, ulong y) => x == y;

    private protected override bool IsZero(ulong value) => value == 0;

    private protected override bool IsOne(ulong value) => value == 1;

    private protected override bool AreMinMaxValues(ulong min, ulong max) => (min, max) == (ulong.MinValue, ulong.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
public sealed class Int128Analyzer() : IntegerAnalyzer<Int128Analyzer.Int128>(ClrTypeNames.Int128)
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

        public override string ToString()
        {
            if (upper == 0)
            {
                return lower.ToString();
            }

            return ToBigInteger().ToString();
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => $"(Int128){constant.GetText()}";

    private protected override string CastZero() => "(Int128)0";

    private protected override bool AreEqual(Int128 x, Int128 y) => x == y;

    private protected override bool IsZero(Int128 value) => value == 0;

    private protected override bool IsOne(Int128 value) => value == 1;

    private protected override bool AreMinMaxValues(Int128 min, Int128 max) => (min, max) == (Int128.MinValue, Int128.MaxValue);
}

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion)])]
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

        public override string ToString()
        {
            if (upper == 0)
            {
                return lower.ToString();
            }

            return ToBigInteger().ToString();
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => $"(UInt128){constant.GetText()}";

    private protected override string CastZero() => "(UInt128)0";

    private protected override bool AreEqual(UInt128 x, UInt128 y) => x == y;

    private protected override bool IsZero(UInt128 value) => value == 0;

    private protected override bool IsOne(UInt128 value) => value == 1;

    private protected override bool AreMinMaxValues(UInt128 min, UInt128 max) => (min, max) == (UInt128.MinValue, UInt128.MaxValue);
}