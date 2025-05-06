using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

internal sealed class IntPtrInfo() : SignedIntegerInfo<nint>(PredefinedType.INTPTR_FQN)
{
    internal override TypeCode? TypeCode => null;

    internal override int? MaxValueStringLength => null;

    internal override nint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
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
            return constant.Cast(constant.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nint" : "IntPtr").GetText();
        }

        return constant.GetText();
    }

    internal override string Cast(ICSharpExpression expression)
        => expression.Cast(expression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nint" : "IntPtr").GetText();

    internal override string CastZero(CSharpLanguageLevel languageLevel) => languageLevel >= CSharpLanguageLevel.CSharp90 ? "(nint)0" : "(IntPtr)0";

    internal override bool AreEqual(nint x, nint y) => x == y;

    internal override bool IsZero(nint value) => value == 0;

    internal override bool AreMinMaxValues(nint min, nint max) => false; // nint.MinValue and nint.MaxValue are platform-dependent
}