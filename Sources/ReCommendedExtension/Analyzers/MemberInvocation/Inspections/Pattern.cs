using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal abstract record Pattern : Inspection
{
    public static PatternByArgument ByArgument { get; } = new() { Message = _ => "Use pattern." };

    public static PatternByArguments Arg0BetweenArg1Arg2 { get; } = new()
    {
        TryGetReplacement = args
            => args is [{ Value: { } arg0Value }, { Value: { } arg1Value }, { Value: { } arg2Value }]
            && arg1Value.TryGetCharConstant() is { }
            && arg2Value.TryGetCharConstant() is { }
                ? new PatternReplacement { Expression = arg0Value, Pattern = $">= {arg1Value.GetText()} and <= {arg2Value.GetText()}" }
                : null,
        Message = _ => "Use pattern.",
    };

    [Pure]
    static PatternReplacement? TryGetReplacementForBinaryExpression(
        IInvocationExpression invocationExpression,
        ICSharpExpression qualifier,
        Func<BinaryOperatorExpression, string?> tryGetPattern)
        => BinaryOperatorExpression.TryFrom(invocationExpression) is { } binaryExpression
            && !binaryExpression.Expression.IsUsedAsStatement()
            && tryGetPattern(binaryExpression) is { } pattern
                ? new PatternReplacement { Expression = qualifier, Pattern = pattern }
                : null;

    public static PatternByBinaryExpression IsFirstConstantCharacterWhenComparingToZero { get; } = new()
    {
        TryGetReplacement = (invocationExpression, qualifier, args) => args[0] is { Value: { } value } && value.TryGetCharConstant() is { }
            ? TryGetReplacementForBinaryExpression(
                invocationExpression,
                qualifier,
                binaryExpression => binaryExpression switch
                {
                    (InvocationExpression, Operator.Equal, Number { Value: 0 }) => $"[{value.GetText()}, ..]",
                    (Number { Value: 0 }, Operator.Equal, InvocationExpression) => $"[{value.GetText()}, ..]",

                    (InvocationExpression, Operator.NotEqual, Number { Value: 0 }) => $"not [{value.GetText()}, ..]",
                    (Number { Value: 0 }, Operator.NotEqual, InvocationExpression) => $"not [{value.GetText()}, ..]",

                    _ => null,
                })
            : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByBinaryExpression IsFirstConstantNonCharacterWhenComparingToZero { get; } = new()
    {
        TryGetReplacement = (invocationExpression, qualifier, args) => args[0] is { Value: { } value } && value.TryGetCharConstant() == null
            ? TryGetReplacementForBinaryExpression(
                invocationExpression,
                qualifier,
                binaryExpression => binaryExpression switch
                {
                    (InvocationExpression, Operator.Equal, Number { Value: 0 }) => $"[var firstChar, ..] && firstChar == {value.GetText()}",
                    (Number { Value: 0 }, Operator.Equal, InvocationExpression) => $"[var firstChar, ..] && firstChar == {value.GetText()}",

                    (InvocationExpression, Operator.NotEqual, Number { Value: 0 }) => $"not [var firstChar, ..] || firstChar != {value.GetText()}",
                    (Number { Value: 0 }, Operator.NotEqual, InvocationExpression) => $"not [var firstChar, ..] || firstChar != {value.GetText()}",

                    _ => null,
                })
            : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstConstantCharacter { get; } = new()
    {
        TryGetReplacement =
            (qualifier, args) => args[0] is { Value: { } value } && value.TryGetCharConstant() is { }
                ? new PatternReplacement { Expression = qualifier, Pattern = $"[{value.GetText()}, ..]" }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstNonConstantCharacter { get; } = new()
    {
        TryGetReplacement =
            (qualifier, args) => args[0] is { Value: { } value } && value.TryGetCharConstant() == null
                ? new PatternReplacement { Expression = qualifier, Pattern = $"[var firstChar, ..] && firstChar == {value.GetText()}" }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstCharacterByCaseSensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args)
            => args[0] is { Value: { } value }
            && value.TryGetStringConstant() is [var character]
            && args[1]?.Value.TryGetStringComparisonConstant() == StringComparison.Ordinal
                ? new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = $"[{character.ToLiteralString(value.GetCSharpLanguageLevel())}, ..]",
                }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstCharacterByCaseInsensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args) =>
        {
            if (args[0] is { Value: { } value }
                && value.TryGetStringConstant() is [var character]
                && args[1]?.Value.TryGetStringComparisonConstant() == StringComparison.OrdinalIgnoreCase)
            {
                var lowerCaseCharacter = char.ToLowerInvariant(character);
                var upperCaseCharacter = char.ToUpperInvariant(character);

                var languageLevel = value.GetCSharpLanguageLevel();

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = lowerCaseCharacter == upperCaseCharacter
                        ? $"[{character.ToLiteralString(languageLevel)}, ..]"
                        : $"[{lowerCaseCharacter.ToLiteralString(languageLevel)} or {upperCaseCharacter.ToLiteralString(languageLevel)}, ..]",
                };
            }
            return null;
        },
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastConstantCharacter { get; } = new()
    {
        TryGetReplacement =
            (qualifier, args) => args[0] is { Value: { } value } && value.TryGetCharConstant() is { }
                ? new PatternReplacement { Expression = qualifier, Pattern = $"[.., {value.GetText()}]" }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastNonConstantCharacter { get; } = new()
    {
        TryGetReplacement =
            (qualifier, args) => args[0] is { Value: { } value } && value.TryGetCharConstant() == null
                ? new PatternReplacement { Expression = qualifier, Pattern = $"[.., var lastChar] && lastChar == {value.GetText()}" }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastCharacterByCaseSensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args)
            => args[0] is { Value: { } value }
            && value.TryGetStringConstant() is [var character]
            && args[1]?.Value.TryGetStringComparisonConstant() == StringComparison.Ordinal
                ? new PatternReplacement
                {
                    Expression = qualifier, Pattern = $"[.., {character.ToLiteralString(value.GetCSharpLanguageLevel())}]",
                }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastCharacterByCaseInsensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args) =>
        {
            if (args[0] is { Value: { } value }
                && value.TryGetStringConstant() is [var character]
                && args[1]?.Value.TryGetStringComparisonConstant() == StringComparison.OrdinalIgnoreCase)
            {
                var lowerCaseCharacter = char.ToLowerInvariant(character);
                var upperCaseCharacter = char.ToUpperInvariant(character);

                var languageLevel = value.GetCSharpLanguageLevel();

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = lowerCaseCharacter == upperCaseCharacter
                        ? $"[.., {character.ToLiteralString(languageLevel)}]"
                        : $"[.., {lowerCaseCharacter.ToLiteralString(languageLevel)} or {upperCaseCharacter.ToLiteralString(languageLevel)}]",
                };
            }
            return null;
        },
        Message = _ => "Use list pattern.",
    };

    public CSharpLanguageLevel? MinimumLanguageVersion { get; init; }
}