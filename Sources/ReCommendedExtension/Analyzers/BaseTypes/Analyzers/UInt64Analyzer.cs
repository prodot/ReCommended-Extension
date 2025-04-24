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
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion), typeof(RedundantArgumentHint)])]
public sealed class UInt64Analyzer() : IntegerAnalyzer<ulong>(PredefinedType.ULONG_FQN)
{
    private protected override TypeCode? TryGetTypeCode() => TypeCode.UInt64;

    private protected override ulong? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((ulong)value);

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and var value }:
                    implicitlyConverted = true;
                    return unchecked((ulong)value);

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
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

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("ulong").GetText();
                }

                var result = constant.GetText();

                return result switch
                {
                    [.. var rest, 'l' or 'L'] => $"{rest}ul",
                    [.. var rest, 'u' or 'U'] => $"{rest}ul",

                    _ => $"{result}ul",
                };
            }

            return constant.Cast("ulong").GetText();
        }

        return constant.GetText();
    }

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("ulong").GetText();

    private protected override string CastZero(CSharpLanguageLevel languageLevel) => "0ul";

    private protected override bool AreEqual(ulong x, ulong y) => x == y;

    private protected override bool IsZero(ulong value) => value == 0;

    private protected override bool AreMinMaxValues(ulong min, ulong max) => (min, max) == (ulong.MinValue, ulong.MaxValue);
}