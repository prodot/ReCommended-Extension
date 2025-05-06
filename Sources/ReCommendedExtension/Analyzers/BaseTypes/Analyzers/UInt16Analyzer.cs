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
public sealed class UInt16Analyzer() : UnsignedIntegerAnalyzer<ushort>(PredefinedType.USHORT_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.UInt16;

    private protected override ushort? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("ushort").GetText();
        }

        return constant.GetText();
    }

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("ushort").GetText();

    private protected override string CastZero(CSharpLanguageLevel languageLevel) => "(ushort)0";

    private protected override bool AreEqual(ushort x, ushort y) => x == y;

    private protected override bool IsZero(ushort value) => value == 0;

    private protected override bool AreMinMaxValues(ushort min, ushort max) => (min, max) == (ushort.MinValue, ushort.MaxValue);

    static readonly int MaxValueStringLength = ushort.MaxValue.ToString().Length;

    private protected override int? TryGetMaxValueStringLength() => MaxValueStringLength;
}