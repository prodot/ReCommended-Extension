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
    typeof(ICSharpInvocationInfo),
    HighlightingTypes = [typeof(RedundantMethodInvocationHint), typeof(UseBinaryOperatorSuggestion), typeof(UseExpressionResultSuggestion)])]
public sealed class DateTimeOffsetAnalyzer : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
    /// <remarks>
    /// <c>dateTimeOffset.Add(timeSpan)</c> → <c>dateTimeOffset + timeSpan</c>
    /// </remarks>
    static void AnalyzeAdd_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument timeSpanArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && timeSpanArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '+' operator.",
                    invocationExpression,
                    "+",
                    invokedExpression.QualifierExpression.GetText(),
                    timeSpanArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.AddTicks(0)</c> → <c>dateTimeOffset</c>
    /// </remarks>
    static void AnalyzeAddTicks_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(DateTimeOffset.AddTicks)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Equals(other)</c> → <c>dateTimeOffset == other</c>
    /// </remarks>
    static void AnalyzeEquals_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument otherArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && otherArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    otherArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>DateTimeOffset.Equals(first, second)</c> → <c>first == second</c>
    /// </remarks>
    static void AnalyzeEquals_DateTimeOffset_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument firstArgument,
        ICSharpArgument secondArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && firstArgument.Value is { } && secondArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    firstArgument.Value.GetText(),
                    secondArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Subtract(value)</c> → <c>dateTimeOffset - value</c>
    /// </remarks>
    static void AnalyzeSubtract_DateTimeOffset(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '-' operator.",
                    invocationExpression,
                    "-",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTimeOffset.Subtract(value)</c> → <c>dateTimeOffset - value</c>
    /// </remarks>
    static void AnalyzeSubtract_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '-' operator.",
                    invocationExpression,
                    "-",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    protected override void Run(ICSharpInvocationInfo element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression,
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
                } method
                && method.ContainingType.IsClrType(PredefinedType.DATETIMEOFFSET_FQN):

                switch (invokedExpression, method)
                {
                    case ({ QualifierExpression: { } }, { IsStatic: false }):
                        switch (method.ShortName)
                        {
                            case nameof(DateTimeOffset.Add):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var timeSpanType }], [{ } timeSpanArgument]) when timeSpanType.IsTimeSpan():
                                        AnalyzeAdd_TimeSpan(consumer, invocationExpression, invokedExpression, timeSpanArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.AddTicks):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsLong():
                                        AnalyzeAddTicks_Int64(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var otherType }], [{ } otherArgument]) when otherType.IsDateTimeOffset():
                                        AnalyzeEquals_DateTimeOffset(consumer, invocationExpression, invokedExpression, otherArgument);
                                        break;

                                    case ([{ Type: var objType }], [{ } objArgument]) when objType.IsObject():
                                        AnalyzeEquals_Object(consumer, invocationExpression, objArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTimeOffset.Subtract):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsDateTimeOffset():
                                        AnalyzeSubtract_DateTimeOffset(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;

                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsTimeSpan():
                                        AnalyzeSubtract_TimeSpan(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;
                                }
                                break;
                        }
                        break;

                    case (_, { IsStatic: true }):
                        switch (method.ShortName)
                        {
                            case nameof(DateTimeOffset.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var firstType }, { Type: var secondType }], [{ } firstArgument, { } secondArgument])
                                        when firstType.IsDateTimeOffset() && secondType.IsDateTimeOffset():

                                        AnalyzeEquals_DateTimeOffset_DateTimeOffset(consumer, invocationExpression, firstArgument, secondArgument);
                                        break;
                                }
                                break;
                        }
                        break;
                }
                break;
        }
    }
}