using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class Int32Info() : SignedIntegerInfo<int>(PredefinedType.INT_FQN)
{
    static readonly int maxValueStringLength = int.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.Int32;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override int? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("int").GetText();
                }
            }
            else
            {
                return constant.Cast("int").GetText();
            }
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("int").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "0";

    internal override bool AreEqual(int x, int y) => x == y;

    internal override bool IsZero(int value) => value == 0;

    internal override bool AreMinMaxValues(int min, int max) => (min, max) == (int.MinValue, int.MaxValue);
}