using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
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

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    [ElementProblemAnalyzer(
        typeof(ICSharpTreeNode),
        HighlightingTypes = new[] { typeof(RedundantAssertionStatementSuggestion), typeof(RedundantInlineAssertionSuggestion) })]
    public sealed class ControlFlowAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
    {
        [Pure]
        [CanBeNull]
        static ICSharpExpression TryGetOtherOperand(
            [NotNull] IEqualityExpression equalityExpression,
            EqualityExpressionType equalityType,
            [NotNull] TokenNodeType operandNodeType)
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
        static bool IsLiteral([CanBeNull] IExpression expression, [NotNull] TokenNodeType tokenType)
            => (expression as ICSharpLiteralExpression)?.Literal?.GetTokenType() == tokenType;

        [Pure]
        static CSharpControlFlowNullReferenceState GetExpressionNullReferenceStateByNullableContext(
            [NotNull] CSharpCompilerNullableInspector nullabilityInspector,
            [NotNull] ICSharpExpression expression)
        {
            var type = expression.Type();
            if (expression.IsDefaultValueOf(type))
            {
                switch (type.Classify)
                {
                    case TypeClassification.VALUE_TYPE:
                        return type.IsNullable() ? CSharpControlFlowNullReferenceState.NULL : CSharpControlFlowNullReferenceState.NOT_NULL;

                    case TypeClassification.REFERENCE_TYPE: return CSharpControlFlowNullReferenceState.NULL;

                    case TypeClassification.UNKNOWN: return CSharpControlFlowNullReferenceState.UNKNOWN; // unconstrained generic type

                    default: goto case TypeClassification.UNKNOWN;
                }
            }

            var closure = expression.GetContainingNode<ICSharpClosure>();
            if (closure != null)
            {
                nullabilityInspector = nullabilityInspector.GetClosureAnalysisResult(closure) as CSharpCompilerNullableInspector;
            }

            var controlFlowGraph = nullabilityInspector?.ControlFlowGraph;

            var edge = controlFlowGraph?.GetLeafElementsFor(expression).LastOrDefault()?.Exits.FirstOrDefault();
            if (edge != null)
            {
                var nullableContext = nullabilityInspector.GetContext(edge);

                switch (nullableContext?.ExpressionAnnotation)
                {
                    case NullableAnnotation.NotAnnotated:
                    case NullableAnnotation.NotNullable:
                        return CSharpControlFlowNullReferenceState.NOT_NULL;

                    case NullableAnnotation.Annotated:
                    case NullableAnnotation.Nullable:
                        return CSharpControlFlowNullReferenceState.MAY_BE_NULL; // todo: distinguish if the expression is "null" or just "may be null" here

                    default: return CSharpControlFlowNullReferenceState.UNKNOWN;
                }
            }

            return CSharpControlFlowNullReferenceState.UNKNOWN;
        }

        [NotNull]
        readonly NullnessProvider nullnessProvider;

        [NotNull]
        readonly AssertionMethodAnnotationProvider assertionMethodAnnotationProvider;

        [NotNull]
        readonly AssertionConditionAnnotationProvider assertionConditionAnnotationProvider;

        [NotNull]
        readonly NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer;

        public ControlFlowAnalyzer(
            [NotNull] CodeAnnotationsCache codeAnnotationsCache,
            [NotNull] NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
        {
            nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            assertionMethodAnnotationProvider = codeAnnotationsCache.GetProvider<AssertionMethodAnnotationProvider>();
            assertionConditionAnnotationProvider = codeAnnotationsCache.GetProvider<AssertionConditionAnnotationProvider>();

            this.nullableReferenceTypesDataFlowAnalysisRunSynchronizer = nullableReferenceTypesDataFlowAnalysisRunSynchronizer;
        }

        void AnalyzeAssertions(
            [NotNull] ElementProblemAnalyzerData data,
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] ICSharpTreeNode rootNode,
            [NotNull] ICSharpControlFlowGraph controlFlowGraph)
        {
            var assertions = Assertion.CollectAssertions(assertionMethodAnnotationProvider, assertionConditionAnnotationProvider, rootNode);

            assertions.ExceptWith(
                from highlightingInfo in consumer.Highlightings
                where highlightingInfo != null
                let redundantAssertionHighlighting = highlightingInfo.Highlighting as RedundantAssertionSuggestion
                where redundantAssertionHighlighting != null
                select redundantAssertionHighlighting.Assertion);

            if (assertions.Count == 0)
            {
                return; // no (new) assertions found
            }

            CSharpCompilerNullableInspector nullabilityInspector;
            CSharpControlFlowGraphInspector inspector;
            HashSet<IAsExpression> alwaysSuccessTryCastExpressions;

            if (rootNode.IsNullableWarningsContextEnabled())
            {
                nullabilityInspector =
                    (CSharpCompilerNullableInspector)nullableReferenceTypesDataFlowAnalysisRunSynchronizer.RunNullableAnalysisAndGetResults(
                        rootNode,
                        null, // wrong [NotNull] annotation in R# code
                        ValueAnalysisMode.OFF);
                inspector = null;
                alwaysSuccessTryCastExpressions = null;
            }
            else
            {
                nullabilityInspector = null;
                inspector = CSharpControlFlowGraphInspector.Inspect(controlFlowGraph, data.GetValueAnalysisMode());
                alwaysSuccessTryCastExpressions =
                    new HashSet<IAsExpression>(inspector.AlwaysSuccessTryCastExpressions ?? Array.Empty<IAsExpression>());
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
            [NotNull] IHighlightingConsumer context,
            [CanBeNull] CSharpCompilerNullableInspector nullabilityInspector,
            [CanBeNull] CSharpControlFlowGraphInspector inspector,
            [CanBeNull][ItemNotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            [NotNull] Assertion assertion,
            bool isKnownToBeTrue)
        {
            if (assertion is AssertionStatement assertionStatement)
            {
                if (nullabilityInspector != null &&
                    nullabilityInspector.ConditionIsAlwaysTrueOrFalseExpressions.TryGetValue(assertionStatement.Expression, out var value))
                {
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
                Debug.Assert(CSharpTokenType.TRUE_KEYWORD != null);
                Debug.Assert(CSharpTokenType.FALSE_KEYWORD != null);
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
                    Debug.Assert(CSharpTokenType.NULL_KEYWORD != null);
                    var expression = TryGetOtherOperand(equalityExpression, EqualityExpressionType.NE, CSharpTokenType.NULL_KEYWORD);
                    if (expression != null)
                    {
                        switch (GetExpressionNullReferenceState(
                            nullabilityInspector,
                            inspector,
                            alwaysSuccessTryCastExpressions,
                            expression))
                        {
                            case CSharpControlFlowNullReferenceState.NOT_NULL:
                                if (isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is true here.",
                                            assertionStatement));
                                }
                                break;

                            case CSharpControlFlowNullReferenceState.NULL:
                                if (!isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is false here.",
                                            assertionStatement));
                                }
                                break;
                        }
                    }

                    // pattern: Assert(x == null); when x is known to be null or not null
                    expression = TryGetOtherOperand(equalityExpression, EqualityExpressionType.EQEQ, CSharpTokenType.NULL_KEYWORD);
                    if (expression != null)
                    {
                        switch (GetExpressionNullReferenceState(nullabilityInspector, inspector, alwaysSuccessTryCastExpressions, expression))
                        {
                            case CSharpControlFlowNullReferenceState.NOT_NULL:
                                if (!isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is false here.",
                                            assertionStatement));
                                }
                                break;

                            case CSharpControlFlowNullReferenceState.NULL:
                                if (isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is true here.",
                                            assertionStatement));
                                }
                                break;
                        }
                    }
                }
            }
        }

        void AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
            [NotNull] IHighlightingConsumer context,
            [CanBeNull] CSharpCompilerNullableInspector nullabilityInspector,
            [CanBeNull] CSharpControlFlowGraphInspector inspector,
            [CanBeNull][ItemNotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            [NotNull] Assertion assertion,
            bool isKnownToBeNull)
        {
            if (assertion is AssertionStatement assertionStatement)
            {
                // pattern: Assert(null);
                Debug.Assert(CSharpTokenType.NULL_KEYWORD != null);
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
                    case CSharpControlFlowNullReferenceState.NOT_NULL:
                        if (!isKnownToBeNull)
                        {
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is not null here.",
                                    assertionStatement));
                        }
                        break;

                    case CSharpControlFlowNullReferenceState.NULL:
                        if (isKnownToBeNull)
                        {
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is null here.",
                                    assertionStatement));
                        }
                        break;
                }
            }

            if (!isKnownToBeNull &&
                assertion is InlineAssertion inlineAssertion &&
                GetExpressionNullReferenceState(
                    nullabilityInspector,
                    inspector,
                    alwaysSuccessTryCastExpressions,
                    inlineAssertion.QualifierExpression) ==
                CSharpControlFlowNullReferenceState.NOT_NULL)
            {
                context.AddHighlighting(
                    new RedundantInlineAssertionSuggestion("Assertion is redundant because the expression is not null here.", inlineAssertion));
            }
        }

        [Pure]
        CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(
            [CanBeNull] CSharpCompilerNullableInspector nullabilityInspector,
            [CanBeNull] CSharpControlFlowGraphInspector inspector,
            [CanBeNull][ItemNotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            [NotNull] ICSharpExpression expression)
        {
            if (nullabilityInspector != null)
            {
                return GetExpressionNullReferenceStateByNullableContext(nullabilityInspector, expression);
            }

            Debug.Assert(inspector != null);
            Debug.Assert(alwaysSuccessTryCastExpressions != null);

            while (true)
            {
                switch (expression)
                {
                    case IReferenceExpression referenceExpression:
                        if (referenceExpression is IConditionalAccessExpression conditionalAccessExpression &&
                            conditionalAccessExpression.HasConditionalAccessSign)
                        {
                            var referenceState = GetExpressionNullReferenceStateByAnnotations(referenceExpression);
                            if (referenceState == CSharpControlFlowNullReferenceState.NOT_NULL)
                            {
                                expression = conditionalAccessExpression.ConditionalQualifier;
                                continue;
                            }
                        }

                        var nullReferenceState = inspector.GetExpressionNullReferenceState(referenceExpression, true);

                        if (nullReferenceState == CSharpControlFlowNullReferenceState.UNKNOWN)
                        {
                            nullReferenceState = GetExpressionNullReferenceStateByAnnotations(referenceExpression);
                        }

                        return nullReferenceState;

                    case IAsExpression asExpression when alwaysSuccessTryCastExpressions.Contains(asExpression):
                        return CSharpControlFlowNullReferenceState.NOT_NULL;

                    case IObjectCreationExpression _: return CSharpControlFlowNullReferenceState.NOT_NULL;

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
        CSharpControlFlowNullReferenceState GetExpressionNullReferenceStateByAnnotations([NotNull] IReferenceExpression referenceExpression)
        {
            switch (referenceExpression.Reference.Resolve().DeclaredElement)
            {
                case IFunction function when nullnessProvider.GetInfo(function) == CodeAnnotationNullableValue.NOT_NULL:
                    return CSharpControlFlowNullReferenceState.NOT_NULL;

                case ITypeOwner typeOwner when !typeOwner.Type.IsDelegateType():
                    if (typeOwner is IAttributesOwner attributesOwner &&
                        nullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL)
                    {
                        return CSharpControlFlowNullReferenceState.NOT_NULL;
                    }

                    goto default;

                default: return CSharpControlFlowNullReferenceState.UNKNOWN;
            }
        }

        protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var controlFlowGraph = (ICSharpControlFlowGraph)ControlFlowBuilder.GetGraph(element);
            if (controlFlowGraph != null)
            {
                AnalyzeAssertions(data, consumer, element, controlFlowGraph);
            }
        }
    }
}