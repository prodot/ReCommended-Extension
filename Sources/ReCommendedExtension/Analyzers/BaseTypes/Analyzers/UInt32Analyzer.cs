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
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperatorSuggestion), typeof(RedundantArgumentHint)])]
public sealed class UInt32Analyzer() : UnsignedIntegerAnalyzer<uint>(PredefinedType.UINT_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.UInt32;

    private protected override uint? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
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

    private protected override string Cast(ICSharpExpression expression) => $"(uint)({expression.GetText()})";

    private protected override string CastZero(CSharpLanguageLevel languageLevel) => "0u";

    private protected override bool AreEqual(uint x, uint y) => x == y;

    private protected override bool IsZero(uint value) => value == 0;

    private protected override bool AreMinMaxValues(uint min, uint max) => (min, max) == (uint.MinValue, uint.MaxValue);
}