using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Assertions;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(ICSharpTreeNode),
        HighlightingTypes = new[] { typeof(RedundantAssertionStatementHighlighting), typeof(RedundantInlineAssertionHighlighting) })]
    public sealed class ControlFlowAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
    {
        static void AnalyzeAssertions(
            [NotNull] ElementProblemAnalyzerData data,
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] ICSharpTreeNode rootNode,
            [NotNull] ICSharpControlFlowGraph controlFlowGraph)
        {
            var codeAnnotationsCache = rootNode.GetPsiServices().GetCodeAnnotationsCache();

            var nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            var assertionMethodAnnotationProvider = codeAnnotationsCache.GetProvider<AssertionMethodAnnotationProvider>();
            var assertionConditionAnnotationProvider = codeAnnotationsCache.GetProvider<AssertionConditionAnnotationProvider>();

            Debug.Assert(nullnessProvider != null);
            Debug.Assert(assertionMethodAnnotationProvider != null);
            Debug.Assert(assertionConditionAnnotationProvider != null);

            var assertions = Assertion.CollectAssertions(assertionMethodAnnotationProvider, assertionConditionAnnotationProvider, rootNode);

            assertions.ExceptWith(
                from highlightingInfo in consumer.Highlightings
                where highlightingInfo != null
                let redundantAssertionHighlighting = highlightingInfo.Highlighting as RedundantAssertionHighlighting
                where redundantAssertionHighlighting != null
                select redundantAssertionHighlighting.Assertion);

            if (assertions.Count == 0)
            {
                return; // no (new) assertions found
            }

            var inspector = CSharpControlFlowGraphInspector.Inspect((CSharpControlFlowGraph)controlFlowGraph, data.GetValueAnalysisMode());

            var alwaysSuccessTryCastExpressions = new HashSet<IAsExpression>(inspector.AlwaysSuccessTryCastExpressions ?? new IAsExpression[] { });

            foreach (var assertion in assertions)
            {
                switch (assertion.AssertionConditionType)
                {
                    case AssertionConditionType.IS_TRUE:
                        AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(
                            consumer,
                            nullnessProvider,
                            inspector,
                            alwaysSuccessTryCastExpressions,
                            assertion,
                            true);
                        break;

                    case AssertionConditionType.IS_FALSE:
                        AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(
                            consumer,
                            nullnessProvider,
                            inspector,
                            alwaysSuccessTryCastExpressions,
                            assertion,
                            false);
                        break;

                    case AssertionConditionType.IS_NOT_NULL:
                        AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
                            consumer,
                            nullnessProvider,
                            inspector,
                            alwaysSuccessTryCastExpressions,
                            assertion,
                            false);
                        break;

                    case AssertionConditionType.IS_NULL:
                        AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
                            consumer,
                            nullnessProvider,
                            inspector,
                            alwaysSuccessTryCastExpressions,
                            assertion,
                            true);
                        break;
                }
            }
        }

        static void AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(
            [NotNull] IHighlightingConsumer context,
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] CSharpControlFlowGraphInspector inspector,
            [NotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            [NotNull] Assertion assertion,
            bool isKnownToBeTrue)
        {
            var assertionStatement = assertion as AssertionStatement;
            if (assertionStatement != null)
            {
                // pattern: Assert(true); or Assert(false);
                Debug.Assert(CSharpTokenType.TRUE_KEYWORD != null);
                Debug.Assert(CSharpTokenType.FALSE_KEYWORD != null);
                if (IsLiteral(assertionStatement.Expression, isKnownToBeTrue ? CSharpTokenType.TRUE_KEYWORD : CSharpTokenType.FALSE_KEYWORD))
                {
                    context.AddHighlighting(
                        new RedundantAssertionStatementHighlighting(
                            $"Assertion is redundant because the expression is always {(isKnownToBeTrue ? "true" : "false")}.",
                            assertionStatement));
                }

                var equalityExpression = assertionStatement.Expression as IEqualityExpression;
                if (equalityExpression != null)
                {
                    // pattern: Assert(x != null); when x is known to be null or not null
                    Debug.Assert(CSharpTokenType.NULL_KEYWORD != null);
                    var expression = TryGetOtherOperand(equalityExpression, EqualityExpressionType.NE, CSharpTokenType.NULL_KEYWORD);
                    if (expression != null)
                    {
                        switch (GetExpressionNullReferenceState(nullnessProvider, inspector, alwaysSuccessTryCastExpressions, expression))
                        {
                            case CSharpControlFlowNullReferenceState.NOT_NULL:
                                if (isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementHighlighting(
                                            "Assertion is redundant because the expression is always true.",
                                            assertionStatement));
                                }
                                break;

                            case CSharpControlFlowNullReferenceState.NULL:
                                if (!isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementHighlighting(
                                            "Assertion is redundant because the expression is always false.",
                                            assertionStatement));
                                }
                                break;
                        }
                    }

                    // pattern: Assert(x == null); when x is known to be null or not null
                    expression = TryGetOtherOperand(equalityExpression, EqualityExpressionType.EQEQ, CSharpTokenType.NULL_KEYWORD);
                    if (expression != null)
                    {
                        switch (GetExpressionNullReferenceState(nullnessProvider, inspector, alwaysSuccessTryCastExpressions, expression))
                        {
                            case CSharpControlFlowNullReferenceState.NOT_NULL:
                                if (!isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementHighlighting(
                                            "Assertion is redundant because the expression is always false.",
                                            assertionStatement));
                                }
                                break;

                            case CSharpControlFlowNullReferenceState.NULL:
                                if (isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementHighlighting(
                                            "Assertion is redundant because the expression is always true.",
                                            assertionStatement));
                                }
                                break;
                        }
                    }
                }
            }
        }

        static void AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
            [NotNull] IHighlightingConsumer context,
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] CSharpControlFlowGraphInspector inspector,
            [NotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            [NotNull] Assertion assertion,
            bool isKnownToBeNull)
        {
            var assertionStatement = assertion as AssertionStatement;
            if (assertionStatement != null)
            {
                // pattern: Assert(null);
                Debug.Assert(CSharpTokenType.NULL_KEYWORD != null);
                if (isKnownToBeNull && IsLiteral(assertionStatement.Expression, CSharpTokenType.NULL_KEYWORD))
                {
                    context.AddHighlighting(
                        new RedundantAssertionStatementHighlighting(
                            "Assertion is redundant because the expression is always null.",
                            assertionStatement));
                }

                // pattern: Assert(x); when x is known to be null or not null
                switch (GetExpressionNullReferenceState(nullnessProvider, inspector, alwaysSuccessTryCastExpressions, assertionStatement.Expression))
                {
                    case CSharpControlFlowNullReferenceState.NOT_NULL:
                        if (!isKnownToBeNull)
                        {
                            context.AddHighlighting(
                                new RedundantAssertionStatementHighlighting(
                                    "Assertion is redundant because the expression is never null.",
                                    assertionStatement));
                        }
                        break;

                    case CSharpControlFlowNullReferenceState.NULL:
                        if (isKnownToBeNull)
                        {
                            context.AddHighlighting(
                                new RedundantAssertionStatementHighlighting(
                                    "Assertion is redundant because the expression is always null.",
                                    assertionStatement));
                        }
                        break;
                }
            }

            if (!isKnownToBeNull)
            {
                var inlineAssertion = assertion as InlineAssertion;
                if (inlineAssertion != null &&
                    GetExpressionNullReferenceState(
                        nullnessProvider,
                        inspector,
                        alwaysSuccessTryCastExpressions,
                        inlineAssertion.QualifierExpression) == CSharpControlFlowNullReferenceState.NOT_NULL)
                {
                    context.AddHighlighting(
                        new RedundantInlineAssertionHighlighting("Assertion is redundant because the expression is never null.", inlineAssertion));
                }
            }
        }

        [Pure]
        static CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] CSharpControlFlowGraphInspector inspector,
            [NotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            ICSharpExpression expression)
        {
            var referenceExpression = expression as IReferenceExpression;
            if (referenceExpression != null)
            {
                var nullReferenceState = inspector.GetExpressionNullReferenceState(referenceExpression, true);

                if (nullReferenceState == CSharpControlFlowNullReferenceState.UNKNOWN)
                {
                    return GetExpressionNullReferenceStateByAnnotations(nullnessProvider, referenceExpression);
                }

                return nullReferenceState;
            }

            var asExpression = expression as IAsExpression;
            if (asExpression != null && alwaysSuccessTryCastExpressions.Contains(asExpression))
            {
                return CSharpControlFlowNullReferenceState.NOT_NULL;
            }

            if (expression is IObjectCreationExpression)
            {
                return CSharpControlFlowNullReferenceState.NOT_NULL;
            }

            var invokedExpression = (expression as IInvocationExpression)?.InvokedExpression as IReferenceExpression;
            if (invokedExpression != null)
            {
                return GetExpressionNullReferenceStateByAnnotations(nullnessProvider, invokedExpression);
            }

            return CSharpControlFlowNullReferenceState.UNKNOWN;
        }

        [Pure]
        static CSharpControlFlowNullReferenceState GetExpressionNullReferenceStateByAnnotations(
            [NotNull] NullnessProvider nullnessProvider,
            [NotNull] IReferenceExpression referenceExpression)
        {
            var declaredElement = referenceExpression.Reference.GetResolveResult().DeclaredElement;

            var function = declaredElement as IFunction;
            if (function != null && nullnessProvider.GetInfo(function) == CodeAnnotationNullableValue.NOT_NULL)
            {
                return CSharpControlFlowNullReferenceState.NOT_NULL;
            }

            var typeOwner = declaredElement as ITypeOwner;
            if (typeOwner != null && !typeOwner.Type.IsDelegateType())
            {
                var attributesOwner = typeOwner as IAttributesOwner;
                if (attributesOwner != null && nullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL)
                {
                    return CSharpControlFlowNullReferenceState.NOT_NULL;
                }
            }

            return CSharpControlFlowNullReferenceState.UNKNOWN;
        }

        [Pure]
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
        static bool IsLiteral(IExpression expression, [NotNull] TokenNodeType tokenType)
            => (expression as ICSharpLiteralExpression)?.Literal?.GetTokenType() == tokenType;

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