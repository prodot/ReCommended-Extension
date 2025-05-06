using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class UInt64Info() : UnsignedIntegerInfo<ulong>(PredefinedType.ULONG_FQN)
{
    static readonly int maxValueStringLength = ulong.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.UInt64;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override ulong? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("ulong").GetText();
                }

                var result = constant.GetText();

                return result switch
                {
                    [.. var rest, 'l' or 'L'] => $"{rest}ul",
                    [.. var rest, 'u' or 'U'] => $"{rest}ul",

                    _ => $"{result}ul",
                };
            }

            return constant.Cast("ulong").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("ulong").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "0ul";

    internal override bool AreEqual(ulong x, ulong y) => x == y;

    internal override bool IsZero(ulong value) => value == 0;

    internal override bool AreMinMaxValues(ulong min, ulong max) => (min, max) == (ulong.MinValue, ulong.MaxValue);
}