using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
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
        typeof(RedundantArgumentHint),
        typeof(UseFloatingPointPatternSuggestion),
        typeof(PassOtherFormatSpecifierSuggestion),
        typeof(RedundantFormatPrecisionSpecifierHint),
    ])]
public sealed class SingleAnalyzer() : FloatingPointNumberAnalyzer<float>(PredefinedType.FLOAT_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.Single;

    private protected override string TryGetNanConstant() => $"float.{nameof(float.NaN)}";

    private protected override float? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Float, FloatValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
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

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted) => throw new NotSupportedException();

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("float").GetText();

    private protected override RoundTripFormatSpecifierSupport GetRoundTripFormatSpecifier(string precisionSpecifier, out string? replacement)
    {
        replacement = "G9";
        return RoundTripFormatSpecifierSupport.ToBeReplaced;
    }
}