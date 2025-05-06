using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class Int16Info() : SignedIntegerInfo<short>(PredefinedType.SHORT_FQN)
{
    static readonly int maxValueStringLength = short.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.Int16;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override short? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("short").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("short").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "(short)0";

    internal override bool AreEqual(short x, short y) => x == y;

    internal override bool IsZero(short value) => value == 0;

    internal override bool AreMinMaxValues(short min, short max) => (min, max) == (short.MinValue, short.MaxValue);
}