using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class ByteInfo() : UnsignedIntegerInfo<byte>(PredefinedType.BYTE_FQN)
{
    static readonly int maxValueStringLength = byte.MaxValue.ToString().Length;

    internal override TypeCode? TypeCode => System.TypeCode.Byte;

    internal override int? MaxValueStringLength => maxValueStringLength;

    internal override byte? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= byte.MinValue and <= byte.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((byte)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    internal override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("byte").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression) => expression.Cast("byte").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => "(byte)0";

    internal override bool AreEqual(byte x, byte y) => x == y;

    internal override bool IsZero(byte value) => value == 0;

    internal override bool AreMinMaxValues(byte min, byte max) => (min, max) == (byte.MinValue, byte.MaxValue);
}