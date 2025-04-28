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
public sealed class ByteAnalyzer() : UnsignedIntegerAnalyzer<byte>(PredefinedType.BYTE_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.Byte;

    private protected override byte? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("byte").GetText();
        }

        return constant.GetText();
    }

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("byte").GetText();

    private protected override string CastZero(CSharpLanguageLevel languageLevel) => "(byte)0";

    private protected override bool AreEqual(byte x, byte y) => x == y;

    private protected override bool IsZero(byte value) => value == 0;

    private protected override bool AreMinMaxValues(byte min, byte max) => (min, max) == (byte.MinValue, byte.MaxValue);
}