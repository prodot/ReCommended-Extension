using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(UseBinaryOperatorSuggestion),
        typeof(RedundantArgumentHint),
        typeof(SuspiciousFormatSpecifierWarning),
        typeof(RedundantFormatPrecisionSpecifierHint),
    ])]
public sealed class UIntPtrAnalyzer() : UnsignedIntegerAnalyzer<nuint>(PredefinedType.UINTPTR_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => null;

    private protected override nuint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        var result = constant.GetText();

        if (implicitlyConverted)
        {
            return constant.Cast(constant.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nuint" : "UIntPtr").GetText();
        }

        return result;
    }

    private protected override string Cast(ICSharpExpression expression)
        => expression.Cast(expression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 ? "nuint" : "UIntPtr").GetText();

    private protected override string CastZero(CSharpLanguageLevel languageLevel)
        => languageLevel >= CSharpLanguageLevel.CSharp90 ? "(nuint)0" : "(UIntPtr)0";

    private protected override bool AreEqual(nuint x, nuint y) => x == y;

    private protected override bool IsZero(nuint value) => value == 0;

    private protected override bool AreMinMaxValues(nuint min, nuint max) => false; // nuint.MinValue and nuint.MaxValue are platform-dependent

    private protected override int? TryGetMaxValueStringLength() => null;
}