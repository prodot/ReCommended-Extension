using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class UInt32Info() : UnsignedIntegerInfo<uint>(PredefinedType.UINT_FQN)
{
    static readonly int maxValueStringLength = uint.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.UInt32;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override uint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("uint").GetText();
                }

                return $"{constant.GetText()}u";
            }

            return constant.Cast("uint").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("uint").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "0u";

    internal override bool AreEqual(uint x, uint y) => x == y;

    internal override bool IsZero(uint value) => value == 0;

    internal override bool AreMinMaxValues(uint min, uint max) => (min, max) == (uint.MinValue, uint.MaxValue);
}