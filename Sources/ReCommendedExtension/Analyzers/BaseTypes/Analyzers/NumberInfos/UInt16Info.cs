using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class UInt16Info() : UnsignedIntegerInfo<ushort>(PredefinedType.USHORT_FQN)
{
    static readonly int maxValueStringLength = ushort.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.UInt16;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override ushort? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("ushort").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("ushort").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "(ushort)0";

    internal override bool AreEqual(ushort x, ushort y) => x == y;

    internal override bool IsZero(ushort value) => value == 0;

    internal override bool AreMinMaxValues(ushort min, ushort max) => (min, max) == (ushort.MinValue, ushort.MaxValue);
}