using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class UIntPtrInfo() : UnsignedIntegerInfo<nuint>(PredefinedType.UINTPTR_FQN)
{
    internal override TypeCode? TypeCode => null;

    internal override int? MaxValueStringLength => null;

    internal override nuint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return (nuint)value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

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
            return constant.Cast(constant.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nuint" : "UIntPtr").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression)
        => expression.Cast(expression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nuint" : "UIntPtr").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => languageLevel >= CSharpLanguageLevel.CSharp90 ? "(nuint)0" : "(UIntPtr)0";

    internal override bool AreEqual(nuint x, nuint y) => x == y;

    internal override bool IsZero(nuint value) => value == 0;

    internal override bool AreMinMaxValues(nuint min, nuint max) => false; // nuint.MinValue and nuint.MaxValue are platform-dependent
}