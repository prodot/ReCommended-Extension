using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseBinaryOperationSuggestion),
        typeof(UseExpressionResultSuggestion),
        typeof(UseCharRangePatternSuggestion),
        typeof(RedundantArgumentHint),
    ])]
public sealed class CharAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    /// <remarks>
    /// <c>character.Equals(obj)</c> → <c>character == obj</c>
    /// </remarks>
    static void AnalyzeEquals_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperationSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    objArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>character.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>character.GetTypeCode()</c> → <c>TypeCode.Char</c>
    /// </remarks>
    static void AnalyzeGetTypeCode(IHighlightingConsumer consumer, IInvocationExpression invocationExpression)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TypeCode)}.{nameof(TypeCode.Char)}.",
                    invocationExpression,
                    $"{nameof(TypeCode)}.{nameof(TypeCode.Char)}"));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiDigit(c)</c> → <c>c is >= '0' and &lt;= '9'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiDigit(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('0', '9')]));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiDigit(c)</c> → <c>c is >= '0' and &lt;= '9' or >= 'A' and &lt;= 'F' or >= 'a' and &lt;= 'f'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiHexDigit(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('0', '9'), new CharRange('A', 'F'), new CharRange('a', 'f')]));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiDigitLower(c)</c> → <c>c is >= '0' and &lt;= '9' or >= 'a' and &lt;= 'f'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiHexDigitLower(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('0', '9'), new CharRange('a', 'f')]));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiDigitUpper(c)</c> → <c>c is >= '0' and &lt;= '9' or >= 'A' and &lt;= 'F'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiHexDigitUpper(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('0', '9'), new CharRange('A', 'F')]));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiLetter(c)</c> → <c>c is >= 'A' and &lt;= 'Z' or >= 'a' and &lt;= 'z'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiLetter(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('A', 'Z'), new CharRange('a', 'z')]));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiLetterLower(c)</c> → <c>c is >= 'a' and &lt;= 'z'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiLetterLower(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('a', 'z')]));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiLetterOrDigit(c)</c> → <c>c is >= 'A' and &lt;= 'Z' or >= 'a' and &lt;= 'z' or >= '0' and &lt;= '9'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiLetterOrDigit(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('A', 'Z'), new CharRange('a', 'z'), new CharRange('0', '9')]));
        }
    }

    /// <remarks>
    /// <c>char.IsAsciiLetterUpper(c)</c> → <c>c is >= 'A' and &lt;= 'Z'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsAsciiLetterUpper(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument cArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange('A', 'Z')]));
        }
    }

    /// <remarks>
    /// <c>char.IsBetween(c, 'a', 'c')</c> → <c>c is >= 'a' and &lt;= 'c'</c> (C# 9)
    /// </remarks>
    static void AnalyzeIsBetween(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument cArgument,
        ICSharpArgument minInclusiveArgument,
        ICSharpArgument maxInclusiveArgument)
    {
        if (invocationExpression.GetLanguageVersion() >= CSharpLanguageLevel.CSharp90
            && !invocationExpression.IsUsedAsStatement()
            && minInclusiveArgument.Value is { }
            && maxInclusiveArgument.Value is { }
            && minInclusiveArgument.Value.TryGetCharConstant() is { }
            && maxInclusiveArgument.Value.TryGetCharConstant() is { }
            && cArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseCharRangePatternSuggestion(
                    "Use relational and logical patterns.",
                    invocationExpression,
                    cArgument.Value,
                    [new CharRange(minInclusiveArgument.Value.GetText(), maxInclusiveArgument.Value.GetText())]));
        }
    }

    /// <remarks>
    /// <c>character.ToString(provider)</c> → <c>character.ToString()</c>
    /// </remarks>
    static void AnalyzeToString_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (PredefinedType.CHAR_FQN.HasMethod(
            new MethodSignature { Name = nameof(bool.ToString), ParameterTypes = [] },
            invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing a format provider is redundant.", providerArgument));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
            } method
            && method.ContainingType.IsClrType(PredefinedType.CHAR_FQN))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case nameof(char.Equals):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var objType }], [var objArgument]) when objType.IsChar():
                                    AnalyzeEquals_Char(consumer, element, invokedExpression, objArgument);
                                    break;

                                case ([{ Type: var objType }], [var objArgument]) when objType.IsObject():
                                    AnalyzeEquals_Object(consumer, element, objArgument);
                                    break;
                            }
                            break;

                        case nameof(char.GetTypeCode):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([], []): AnalyzeGetTypeCode(consumer, element); break;
                            }
                            break;

                        case nameof(char.ToString):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var providerType }], [var providerArgument]) when providerType.IsIFormatProvider():
                                    AnalyzeToString_IFormatProvider(consumer, element, providerArgument);
                                    break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case "IsAsciiDigit": // todo: nameof(char.IsAsciiDigit) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiDigit(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsAsciiHexDigit": // todo: nameof(char.IsAsciiHexDigit) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiHexDigit(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsAsciiHexDigitLower": // todo: nameof(char.IsAsciiHexDigitLower) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiHexDigitLower(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsAsciiHexDigitUpper": // todo: nameof(char.IsAsciiHexDigitUpper) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiHexDigitUpper(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsAsciiLetter": // todo: nameof(char.IsAsciiLetter) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiLetter(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsAsciiLetterLower": // todo: nameof(char.IsAsciiLetterLower) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiLetterLower(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsAsciiLetterOrDigit": // todo: nameof(char.IsAsciiLetterOrDigit) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiLetterOrDigit(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsAsciiLetterUpper": // todo: nameof(char.IsAsciiLetterUpper) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }], [var cArgument]) when cType.IsChar():
                                    AnalyzeIsAsciiLetterUpper(consumer, element, cArgument);
                                    break;
                            }
                            break;

                        case "IsBetween": // todo: nameof(char.IsBetween) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var cType }, { Type: var minInclusiveType }, { Type: var maxInclusiveType }], [
                                    var cArgument, var minInclusiveArgument, var maxInclusiveArgument,
                                ]) when cType.IsChar() && minInclusiveType.IsChar() && maxInclusiveType.IsChar():
                                    AnalyzeIsBetween(consumer, element, cArgument, minInclusiveArgument, maxInclusiveArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}