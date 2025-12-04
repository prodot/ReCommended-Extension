using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal abstract record Pattern : Inspection
{
    [Pure]
    static string? TryGetItemDefaultValue(IType collectionType, ITreeNode context)
    {
        if (collectionType.IsString() && PredefinedType.CHAR_FQN.TryGetTypeElement(context.GetPsiModule()) is { } charTypeElement)
        {
            return TypeFactory.CreateType(charTypeElement).TryGetDefaultValueLiteral(context);
        }

        if (collectionType is IArrayType(var elementType, 1))
        {
            return elementType.TryGetDefaultValueLiteral(context);
        }

        if (collectionType is IDeclaredType { GenericParameterTypes: [{ } itemType] })
        {
            return itemType.TryGetDefaultValueLiteral(context);
        }

        return null;
    }

    public static PatternByArgument ByArgument { get; } = new() { Message = _ => "Use pattern." };

    public static PatternByArguments Arg0BetweenArg1Arg2 { get; } = new()
    {
        TryGetReplacement = args
            => args is [{ Value: { } arg0Value }, { Value: { AsCharConstant: { } } arg1Value }, { Value: { AsCharConstant: { } } arg2Value }]
                ? new PatternReplacement { Expression = arg0Value, Pattern = $"is >= {arg1Value.GetText()} and <= {arg2Value.GetText()}" }
                : null,
        Message = _ => "Use pattern.",
    };

    [Pure]
    static PatternReplacement? TryGetReplacementForBinaryExpression(
        IInvocationExpression invocationExpression,
        ICSharpExpression qualifier,
        Func<BinaryOperatorExpression, string?> tryGetPattern,
        Func<BinaryOperatorExpression, string?>? tryGetPatternDisplayText = null)
        => BinaryOperatorExpression.TryFrom(invocationExpression) is { Expression.IsUsedAsStatement: false } binaryExpression
            && tryGetPattern(binaryExpression) is { } pattern
                ? new PatternReplacement
                {
                    Expression = qualifier, Pattern = pattern, PatternDisplayText = tryGetPatternDisplayText?.Invoke(binaryExpression),
                }
                : null;

    public static PatternByBinaryExpression IsFirstConstantCharacterWhenComparingToZero { get; } = new()
    {
        TryGetReplacement = (invocationExpression, qualifier, args) => args is [{ Value: { AsCharConstant: { } } value }]
            ? TryGetReplacementForBinaryExpression(
                invocationExpression,
                qualifier,
                binaryExpression => binaryExpression switch
                {
                    (InvocationExpression, Operator.Equal, Number { Value: 0 }) => $"is [{value.GetText()}, ..]",
                    (Number { Value: 0 }, Operator.Equal, InvocationExpression) => $"is [{value.GetText()}, ..]",

                    (InvocationExpression, Operator.NotEqual, Number { Value: 0 }) => $"is not [{value.GetText()}, ..]",
                    (Number { Value: 0 }, Operator.NotEqual, InvocationExpression) => $"is not [{value.GetText()}, ..]",

                    _ => null,
                })
            : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByBinaryExpression IsFirstConstantNonCharacterWhenComparingToZero { get; } = new()
    {
        TryGetReplacement = (invocationExpression, qualifier, args) => args is [{ Value: { AsCharConstant: null } value }]
            ? TryGetReplacementForBinaryExpression(
                invocationExpression,
                qualifier,
                binaryExpression => binaryExpression switch
                {
                    (InvocationExpression, Operator.Equal, Number { Value: 0 }) => $"is [var firstChar, ..] && firstChar == ({value.GetText()})",
                    (Number { Value: 0 }, Operator.Equal, InvocationExpression) => $"is [var firstChar, ..] && firstChar == ({value.GetText()})",

                    (InvocationExpression, Operator.NotEqual, Number { Value: 0 }) => $"is not [var firstChar, ..] || firstChar != ({
                        value.GetText()
                    })",
                    (Number { Value: 0 }, Operator.NotEqual, InvocationExpression) => $"is not [var firstChar, ..] || firstChar != ({
                        value.GetText()
                    })",

                    _ => null,
                },
                binaryExpression => binaryExpression switch
                {
                    (InvocationExpression, Operator.Equal, Number { Value: 0 }) => $"is [var firstChar, ..] && firstChar == {value.GetText()}",
                    (Number { Value: 0 }, Operator.Equal, InvocationExpression) => $"is [var firstChar, ..] && firstChar == {value.GetText()}",

                    (InvocationExpression, Operator.NotEqual, Number { Value: 0 }) => $"is not [var firstChar, ..] || firstChar != {value.GetText()}",
                    (Number { Value: 0 }, Operator.NotEqual, InvocationExpression) => $"is not [var firstChar, ..] || firstChar != {value.GetText()}",

                    _ => null,
                })
            : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstConstantCharacter { get; } = new()
    {
        TryGetReplacement =
            (qualifier, args) => args is [{ Value: { AsCharConstant: { } } value }]
                ? new PatternReplacement { Expression = qualifier, Pattern = $"is [{value.GetText()}, ..]" }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstNonConstantCharacter { get; } = new()
    {
        TryGetReplacement = (qualifier, args) => args is [{ Value: { AsCharConstant: null } value }]
            ? new PatternReplacement
            {
                Expression = qualifier,
                Pattern = $"is [var firstChar, ..] && firstChar == ({value.GetText()})",
                PatternDisplayText = $"is [var firstChar, ..] && firstChar == {value.GetText()}",
            }
            : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstCharacterByCaseSensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args)
            => args is [{ Value: { AsStringConstant: [var character] } value }, { Value.AsStringComparisonConstant: StringComparison.Ordinal }]
                ? new PatternReplacement
                {
                    Expression = qualifier, Pattern = $"is [{character.ToLiteralString(value.GetCSharpLanguageLevel())}, ..]",
                }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsFirstCharacterByCaseInsensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args) =>
        {
            if (args is
                [
                    { Value: { AsStringConstant: [var character] } value }, { Value.AsStringComparisonConstant: StringComparison.OrdinalIgnoreCase },
                ])
            {
                var lowerCaseCharacter = char.ToLowerInvariant(character);
                var upperCaseCharacter = char.ToUpperInvariant(character);

                var languageLevel = value.GetCSharpLanguageLevel();

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = lowerCaseCharacter == upperCaseCharacter
                        ? $"is [{character.ToLiteralString(languageLevel)}, ..]"
                        : $"is [{lowerCaseCharacter.ToLiteralString(languageLevel)} or {upperCaseCharacter.ToLiteralString(languageLevel)}, ..]",
                };
            }

            return null;
        },
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastConstantCharacter { get; } = new()
    {
        TryGetReplacement =
            (qualifier, args) => args is [{ Value: { AsCharConstant: { } } value }]
                ? new PatternReplacement { Expression = qualifier, Pattern = $"is [.., {value.GetText()}]" }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastNonConstantCharacter { get; } = new()
    {
        TryGetReplacement = (qualifier, args) => args is [{ Value: { AsCharConstant: null } value }]
            ? new PatternReplacement
            {
                Expression = qualifier,
                Pattern = $"is [.., var lastChar] && lastChar == ({value.GetText()})",
                PatternDisplayText = $"is [.., var lastChar] && lastChar == {value.GetText()}",
            }
            : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastCharacterByCaseSensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args)
            => args is [{ Value: { AsStringConstant: [var character] } value }, { Value.AsStringComparisonConstant: StringComparison.Ordinal }]
                ? new PatternReplacement
                {
                    Expression = qualifier, Pattern = $"is [.., {character.ToLiteralString(value.GetCSharpLanguageLevel())}]",
                }
                : null,
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments IsLastCharacterByCaseInsensitiveString { get; } = new()
    {
        TryGetReplacement = (qualifier, args) =>
        {
            if (args is
                [
                    { Value: { AsStringConstant: [var character] } value }, { Value.AsStringComparisonConstant: StringComparison.OrdinalIgnoreCase },
                ])
            {
                var lowerCaseCharacter = char.ToLowerInvariant(character);
                var upperCaseCharacter = char.ToUpperInvariant(character);

                var languageLevel = value.GetCSharpLanguageLevel();

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = lowerCaseCharacter == upperCaseCharacter
                        ? $"is [.., {character.ToLiteralString(languageLevel)}]"
                        : $"is [.., {lowerCaseCharacter.ToLiteralString(languageLevel)} or {upperCaseCharacter.ToLiteralString(languageLevel)}]",
                };
            }

            return null;
        },
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments FirstItemOrDefault { get; } = new()
    {
        TryGetReplacement = (qualifier, args) =>
        {
            var type = qualifier.Type();

            if (IsIndexableCollectionOrString(type, qualifier, out var hasAccessibleIndexer) && hasAccessibleIndexer)
            {
                var defaultValue = args is [_, { Value: { } value }, ..] ? value : null;

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = defaultValue is { }
                        ? $"is [var first, ..] ? first : ({defaultValue.GetText()})"
                        : $"is [var first, ..] ? first : {TryGetItemDefaultValue(type, qualifier) ?? "default"}",
                    PatternDisplayText = defaultValue is { }
                        ? $"is [var first, ..] ? first : {defaultValue.GetText()}"
                        : $"is [var first, ..] ? first : {TryGetItemDefaultValue(type, qualifier) ?? "default"}",
                    HighlightOnlyInvokedMethod = true,
                };
            }

            return null;
        },
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments LastItemOrDefault { get; } = new()
    {
        TryGetReplacement = (qualifier, args) =>
        {
            var type = qualifier.Type();

            if (IsIndexableCollectionOrString(type, qualifier, out var hasAccessibleIndexer) && hasAccessibleIndexer)
            {
                var defaultValue = args is [_, { Value: { } value }, ..] ? value : null;

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = defaultValue is { }
                        ? $"is [.., var last] ? last : ({defaultValue.GetText()})"
                        : $"is [.., var last] ? last : {TryGetItemDefaultValue(type, qualifier) ?? "default"}",
                    PatternDisplayText = defaultValue is { }
                        ? $"is [.., var last] ? last : {defaultValue.GetText()}"
                        : $"is [.., var last] ? last : {TryGetItemDefaultValue(type, qualifier) ?? "default"}",
                    HighlightOnlyInvokedMethod = true,
                };
            }

            return null;
        },
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments SingleItem { get; } = new()
    {
        TryGetReplacement = (qualifier, _) =>
        {
            var type = qualifier.Type();

            if (IsIndexableCollectionOrString(type, qualifier, out var hasAccessibleIndexer) && hasAccessibleIndexer)
            {
                var exceptionMessage = type.IsString()
                    ? "String is either empty or contains more than one character."
                    : "List is either empty or contains more than one element.";

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = $"""is [var item] ? item : throw new {nameof(InvalidOperationException)}("{exceptionMessage}")""",
                    PatternDisplayText = $"is [var item] ? item : throw new {nameof(InvalidOperationException)}(...)",
                    HighlightOnlyInvokedMethod = true,
                };
            }

            return null;
        },
        Message = _ => "Use list pattern.",
    };

    public static PatternByQualifierArguments SingleItemOrDefault { get; } = new()
    {
        TryGetReplacement = (qualifier, args) =>
        {
            var type = qualifier.Type();

            if (IsIndexableCollectionOrString(type, qualifier, out var hasAccessibleIndexer) && hasAccessibleIndexer)
            {
                var defaultValue = args is [_, { Value: { } value }, ..] ? value : null;

                var exceptionMessage = type.IsString() ? "String contains more than one character." : "List contains more than one element.";

                return new PatternReplacement
                {
                    Expression = qualifier,
                    Pattern = defaultValue is { }
                        ? $$"""switch { [] => ({{
                            defaultValue.GetText()
                        }}), [var item] => item, _ => throw new InvalidOperationException("{{
                            exceptionMessage
                        }}") }"""
                        : $$"""switch { [] => {{
                            TryGetItemDefaultValue(type, qualifier) ?? "default"
                        }}, [var item] => item, _ => throw new InvalidOperationException("{{
                            exceptionMessage
                        }}") }""",
                    PatternDisplayText = defaultValue is { }
                        ? $$"""switch { [] => {{defaultValue.GetText()}}, [var item] => item, _ => throw new InvalidOperationException(...) }"""
                        : $$"""switch { [] => {{
                            TryGetItemDefaultValue(type, qualifier) ?? "default"
                        }}, [var item] => item, _ => throw new InvalidOperationException(...) }""",
                    HighlightOnlyInvokedMethod = true,
                };
            }

            return null;
        },
        Message = _ => "Use switch expression.",
    };

    public CSharpLanguageLevel? MinimumLanguageVersion { get; init; }
}