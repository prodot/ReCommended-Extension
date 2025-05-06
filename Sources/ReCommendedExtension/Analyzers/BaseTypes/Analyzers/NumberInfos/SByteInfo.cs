using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class SByteInfo() : SignedIntegerInfo<sbyte>(PredefinedType.SBYTE_FQN)
{
    static readonly int maxValueStringLength = sbyte.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.SByte;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override sbyte? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("sbyte").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("sbyte").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "(sbyte)0";

    internal override bool AreEqual(sbyte x, sbyte y) => x == y;

    internal override bool IsZero(sbyte value) => value == 0;

    internal override bool AreMinMaxValues(sbyte min, sbyte max) => (min, max) == (sbyte.MinValue, sbyte.MaxValue);
}