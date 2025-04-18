using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.ControlFlow;

[ElementProblemAnalyzer(
    typeof(ICSharpTreeNode),
    HighlightingTypes = [typeof(RedundantAssertionStatementSuggestion), typeof(RedundantInlineAssertionSuggestion)])]
public sealed class ControlFlowAnalyzer(
    CodeAnnotationsCache codeAnnotationsCache,
    NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer) : ElementProblemAnalyzer<ICSharpTreeNode>
{
    [Pure]
    static ICSharpExpression? TryGetOtherOperand(
        IEqualityExpression equalityExpression,
        EqualityExpressionType equalityType,
        TokenNodeType operandNodeType)
    {
        if (equalityExpression.EqualityType == equalityType)
        {
            if (IsLiteral(equalityExpression.RightOperand, operandNodeType))
            {
                return equalityExpression.LeftOperand;
            }

            if (IsLiteral(equalityExpression.LeftOperand, operandNodeType))
            {
                return equalityExpression.RightOperand;
            }
        }

        return null;
    }

    [Pure]
    static bool IsLiteral(IExpression? expression, TokenNodeType tokenType)
        => (expression as ICSharpLiteralExpression)?.Literal.GetTokenType() == tokenType;

    readonly Lazy<NullnessProvider> nullnessProvider = codeAnnotationsCache.GetLazyProvider<NullnessProvider>();

    readonly Lazy<AssertionMethodAnnotationProvider> assertionMethodAnnotationProvider =
        codeAnnotationsCache.GetLazyProvider<AssertionMethodAnnotationProvider>();

    readonly Lazy<AssertionConditionAnnotationProvider> assertionConditionAnnotationProvider =
        codeAnnotationsCache.GetLazyProvider<AssertionConditionAnnotationProvider>();

    void AnalyzeAssertions(
        ElementProblemAnalyzerData data,
        IHighlightingConsumer consumer,
        ICSharpTreeNode rootNode,
        ICSharpControlFlowGraph controlFlowGraph)
    {
        var assertions = Assertion.CollectAssertions(assertionMethodAnnotationProvider.Value, assertionConditionAnnotationProvider.Value, rootNode);

        if (assertions.Count == 0)
        {
            return; // no (new) assertions found
        }

        CSharpCompilerNullableInspector? nullabilityInspector;
        CSharpControlFlowGraphInspector? inspector;
        HashSet<IAsExpression>? alwaysSuccessTryCastExpressions;

        if (rootNode.IsNullableWarningsContextEnabled())
        {
            nullabilityInspector = rootNode.TryGetNullableInspector(nullableReferenceTypesDataFlowAnalysisRunSynchronizer);
            inspector = null;
            alwaysSuccessTryCastExpressions = null;
        }
        else
        {
            nullabilityInspector = null;
            inspector = CSharpControlFlowGraphInspector.Inspect(controlFlowGraph, data.GetValueAnalysisMode());
            alwaysSuccessTryCastExpressions = [..inspector.AlwaysSuccessTryCastExpressions];
        }

        foreach (var assertion in assertions)
        {
            switch (assertion.AssertionConditionType)
            {
                case AssertionConditionType.IS_TRUE:
                    AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(
                        consumer,
                        nullabilityInspector,
                        inspector,
                        alwaysSuccessTryCastExpressions,
                        assertion,
                        true);
                    break;

                case AssertionConditionType.IS_FALSE:
                    AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(
                        consumer,
                        nullabilityInspector,
                        inspector,
                        alwaysSuccessTryCastExpressions,
                        assertion,
                        false);
                    break;

                case AssertionConditionType.IS_NOT_NULL:
                    AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
                        consumer,
                        nullabilityInspector,
                        inspector,
                        alwaysSuccessTryCastExpressions,
                        assertion,
                        false);
                    break;

                case AssertionConditionType.IS_NULL:
                    AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
                        consumer,
                        nullabilityInspector,
                        inspector,
                        alwaysSuccessTryCastExpressions,
                        assertion,
                        true);
                    break;
            }
        }
    }

    void AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(
        IHighlightingConsumer context,
        CSharpCompilerNullableInspector? nullabilityInspector,
        CSharpControlFlowGraphInspector? inspector,
        HashSet<IAsExpression>? alwaysSuccessTryCastExpressions,
        Assertion assertion,
        bool isKnownToBeTrue)
    {
        if (assertion is AssertionStatement assertionStatement)
        {
            if (nullabilityInspector is { }
                && nullabilityInspector.ConditionIsAlwaysTrueOrFalseExpressions.TryGetValue(assertionStatement.Expression, out var t))
            {
                var (value, _) = t;

                switch (value)
                {
                    case ConstantExpressionValue.TRUE when isKnownToBeTrue:
                        context.AddHighlighting(
                            new RedundantAssertionStatementSuggestion(
                                "Assertion is redundant because the expression is true here.",
                                assertionStatement));
                        return;

                    case ConstantExpressionValue.FALSE when !isKnownToBeTrue:
                        context.AddHighlighting(
                            new RedundantAssertionStatementSuggestion(
                                "Assertion is redundant because the expression is false here.",
                                assertionStatement));
                        return;
                }
            }

            // pattern: Assert(true); or Assert(false);
            if (IsLiteral(assertionStatement.Expression, isKnownToBeTrue ? CSharpTokenType.TRUE_KEYWORD : CSharpTokenType.FALSE_KEYWORD))
            {
                context.AddHighlighting(
                    new RedundantAssertionStatementSuggestion(
                        $"Assertion is redundant because the expression is {(isKnownToBeTrue ? "true" : "false")} here.",
                        assertionStatement));
            }

            if (assertionStatement.Expression is IEqualityExpression equalityExpression)
            {
                // pattern: Assert(x != null); when x is known to be null or not null
                if (TryGetOtherOperand(equalityExpression, EqualityExpressionType.NE, CSharpTokenType.NULL_KEYWORD) is { } expression)
                {
                    switch (GetExpressionNullReferenceState(nullabilityInspector, inspector, alwaysSuccessTryCastExpressions, expression))
                    {
                        case CSharpControlFlowNullReferenceState.NOT_NULL when isKnownToBeTrue:
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is true here.",
                                    assertionStatement));
                            break;

                        case CSharpControlFlowNullReferenceState.NULL when !isKnownToBeTrue:
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is false here.",
                                    assertionStatement));
                            break;
                    }
                }

                // pattern: Assert(x == null); when x is known to be null or not null
                expression = TryGetOtherOperand(equalityExpression, EqualityExpressionType.EQEQ, CSharpTokenType.NULL_KEYWORD);
                if (expression is { })
                {
                    switch (GetExpressionNullReferenceState(nullabilityInspector, inspector, alwaysSuccessTryCastExpressions, expression))
                    {
                        case CSharpControlFlowNullReferenceState.NOT_NULL when !isKnownToBeTrue:
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is false here.",
                                    assertionStatement));
                            break;

                        case CSharpControlFlowNullReferenceState.NULL when isKnownToBeTrue:
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is true here.",
                                    assertionStatement));
                            break;
                    }
                }
            }
        }
    }

    void AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
        IHighlightingConsumer context,
        CSharpCompilerNullableInspector? nullabilityInspector,
        CSharpControlFlowGraphInspector? inspector,
        HashSet<IAsExpression>? alwaysSuccessTryCastExpressions,
        Assertion assertion,
        bool isKnownToBeNull)
    {
        if (assertion is AssertionStatement assertionStatement)
        {
            // pattern: Assert(null);
            if (isKnownToBeNull && IsLiteral(assertionStatement.Expression, CSharpTokenType.NULL_KEYWORD))
            {
                context.AddHighlighting(
                    new RedundantAssertionStatementSuggestion("Assertion is redundant because the expression is null here.", assertionStatement));
            }

            // pattern: Assert(x); when x is known to be null or not null
            switch (GetExpressionNullReferenceState(
                nullabilityInspector,
                inspector,
                alwaysSuccessTryCastExpressions,
                assertionStatement.Expression))
            {
                case CSharpControlFlowNullReferenceState.NOT_NULL when !isKnownToBeNull:
                    context.AddHighlighting(
                        new RedundantAssertionStatementSuggestion(
                            "Assertion is redundant because the expression is not null here.",
                            assertionStatement));
                    break;

                case CSharpControlFlowNullReferenceState.NULL when isKnownToBeNull:
                    context.AddHighlighting(
                        new RedundantAssertionStatementSuggestion(
                            "Assertion is redundant because the expression is null here.",
                            assertionStatement));
                    break;
            }
        }

        if (!isKnownToBeNull
            && assertion is InlineAssertion inlineAssertion
            && GetExpressionNullReferenceState(
                nullabilityInspector,
                inspector,
                alwaysSuccessTryCastExpressions,
                inlineAssertion.QualifierExpression)
            == CSharpControlFlowNullReferenceState.NOT_NULL)
        {
            context.AddHighlighting(
                new RedundantInlineAssertionSuggestion("Assertion is redundant because the expression is not null here.", inlineAssertion));
        }
    }

    [Pure]
    CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(
        CSharpCompilerNullableInspector? nullabilityInspector,
        CSharpControlFlowGraphInspector? inspector,
        HashSet<IAsExpression>? alwaysSuccessTryCastExpressions,
        ICSharpExpression expression)
    {
        if (nullabilityInspector is { })
        {
            return expression.GetNullReferenceStateByNullableContext(nullabilityInspector);
        }

        Debug.Assert(inspector is { });
        Debug.Assert(alwaysSuccessTryCastExpressions is { });

        var exp = expression;

        while (true)
        {
            switch (exp)
            {
                case IReferenceExpression referenceExpression:
                    if (referenceExpression is IConditionalAccessExpression { HasConditionalAccessSign: true } conditionalAccessExpression
                        && GetExpressionNullReferenceStateByAnnotations(referenceExpression) == CSharpControlFlowNullReferenceState.NOT_NULL)
                    {
                        exp = conditionalAccessExpression.ConditionalQualifier;
                        continue;
                    }

                    var nullReferenceState = inspector.GetExpressionNullReferenceState(referenceExpression, true);

                    if (nullReferenceState == CSharpControlFlowNullReferenceState.UNKNOWN)
                    {
                        nullReferenceState = GetExpressionNullReferenceStateByAnnotations(referenceExpression);
                    }

                    return nullReferenceState;

                case IAsExpression asExpression when alwaysSuccessTryCastExpressions.Contains(asExpression):
                case IObjectCreationExpression:
                    return CSharpControlFlowNullReferenceState.NOT_NULL;

                case IInvocationExpression invocationExpression:
                    if (invocationExpression.InvokedExpression is IReferenceExpression invokedExpression)
                    {
                        return GetExpressionNullReferenceStateByAnnotations(invokedExpression);
                    }

                    goto default;

                default: return CSharpControlFlowNullReferenceState.UNKNOWN;
            }
        }
    }

    [Pure]
    CSharpControlFlowNullReferenceState GetExpressionNullReferenceStateByAnnotations(IReferenceExpression referenceExpression)
    {
        switch (referenceExpression.Reference.Resolve().DeclaredElement)
        {
            case IFunction function:
            {
                var (annotationNullableValue, _, _) = nullnessProvider.Value.GetInfo(function);
                if (annotationNullableValue == CodeAnnotationNullableValue.NOT_NULL)
                {
                    return CSharpControlFlowNullReferenceState.NOT_NULL;
                }

                goto default;
            }

            case ITypeOwner typeOwner when !typeOwner.Type.IsDelegateType():
                if (typeOwner is IAttributesOwner attributesOwner)
                {
                    var (annotationNullableValue, _, _) = nullnessProvider.Value.GetInfo(attributesOwner);
                    if (annotationNullableValue == CodeAnnotationNullableValue.NOT_NULL)
                    {
                        return CSharpControlFlowNullReferenceState.NOT_NULL;
                    }
                }

                goto default;

            default: return CSharpControlFlowNullReferenceState.UNKNOWN;
        }
    }

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if ((ICSharpControlFlowGraph?)ControlFlowBuilder.GetGraph(element) is { } controlFlowGraph)
        {
            AnalyzeAssertions(data, consumer, element, controlFlowGraph);
        }
    }
}