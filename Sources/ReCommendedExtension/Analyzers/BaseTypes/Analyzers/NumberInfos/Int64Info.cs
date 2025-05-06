using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class Int64Info() : SignedIntegerInfo<long>(PredefinedType.LONG_FQN)
{
    static readonly int maxValueStringLength = long.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.Int64;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override long? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("long").GetText();
                }

                var result = constant.GetText();

                if (result is [.. var rest, 'u' or 'U'])
                {
                    return $"{rest}L";
                }

                return $"{result}L";
            }

            return constant.Cast("long").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("long").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "0L";

    internal override bool AreEqual(long x, long y) => x == y;

    internal override bool IsZero(long value) => value == 0;

    internal override bool AreMinMaxValues(long min, long max) => (min, max) == (long.MinValue, long.MaxValue);
}