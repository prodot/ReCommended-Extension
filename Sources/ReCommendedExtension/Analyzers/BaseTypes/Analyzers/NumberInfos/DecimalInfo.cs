using System.Globalization;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class DecimalInfo() : FractionalNumberInfo<decimal>(PredefinedType.DECIMAL_FQN)
{
    static readonly int maxValueStringLength = decimal.MaxValue.ToString(NumberFormatInfo.InvariantInfo).Length;

    internal override TypeCode? TypeCode => System.TypeCode.Decimal;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override NumberStyles DefaultNumberStyles => NumberStyles.Number;

    internal override bool CanUseEqualityOperator => true;

    internal override bool SupportsCaseInsensitiveGeneralFormatSpecifierWithoutPrecision => true;

    internal override decimal? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Decimal, DecimalValue: var value }:
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("decimal").GetText();
                }

                var result = constant.GetText();

                var magnitude = result is ['-' or '+', .. var m] ? m : result;
                if (magnitude is ['0', 'x' or 'X' or 'b' or 'B', ..])
                {
                    return constant.Cast("decimal").GetText();
                }

                return result switch
                {
                    [.. var rest, 'u' or 'U', 'l' or 'L'] => $"{rest}m",
                    [.. var rest, 'l' or 'L', 'u' or 'U'] => $"{rest}m",
                    [.. var rest, 'u' or 'U'] => $"{rest}m",
                    [.. var rest, 'l' or 'L'] => $"{rest}m",

                    _ => $"{result}m",
                };
            }

            return constant.Cast("decimal").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("decimal").GetText();

    internal override bool AreEqual(decimal x, decimal y) => x == y;

    internal override bool AreMinMaxValues(decimal min, decimal max) => (min, max) == (decimal.MinValue, decimal.MaxValue);

    internal override RoundTripFormatSpecifierSupport GetRoundTripFormatSpecifier(string precisionSpecifier, out string? replacement)
    {
        replacement = null;
        return RoundTripFormatSpecifierSupport.Unsupported;
    }
}