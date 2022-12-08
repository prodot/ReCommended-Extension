using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.ControlFlow.Impl;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve.Managed;
using JetBrains.ReSharper.Psi.Resolve.Managed;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.ValueTask
{
    [ElementProblemAnalyzer(
        typeof(ICSharpTreeNode),
        HighlightingTypes = new[] { typeof(PossibleMultipleConsumptionWarning), typeof(IntentionalBlockingAttemptWarning) })]
    public sealed class ValueTaskAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
    {
        sealed class Inspector : CSharpControlFlowGraphInspector
        {
            [NotNull]
            public static Inspector Inspect(
                [NotNull] ICSharpControlFlowGraph controlFlowGraph,
                ValueAnalysisMode analysisMode,
                bool shouldDisableValueAnalysisIfNullableWarningsEnabled = true,
                [CanBeNull] Func<ITreeNode, ICSharpControlFlowGraph> graphBuilder = null)
            {
                var declaration = controlFlowGraph.Declaration;
                Debug.Assert(declaration != null);

                var containingFile = (ICSharpFile)declaration.GetContainingFile();

                var forceClosuresCollection = false;
                if ((uint)analysisMode > 0U && shouldDisableValueAnalysisIfNullableWarningsEnabled)
                {
                    var file = containingFile;
                    var treeTextRange = declaration.GetTreeTextRange();
                    ref var local = ref treeTextRange;
                    if (file.IsNullableWarningsEnabledEverywhereIn(in local))
                    {
                        analysisMode = ValueAnalysisMode.OFF;
                        forceClosuresCollection = true;
                    }
                }

                var universalContext = new UniversalContext(declaration);
                var factory = new CSharpControlFlowContextFactory(
                    controlFlowGraph,
                    universalContext,
                    analysisMode,
                    ExecutionBehavior.InstantExecution,
                    collectClosures: forceClosuresCollection);

                var flowGraphInspector = new Inspector(controlFlowGraph, factory, graphBuilder);

                flowGraphInspector.Inspect();
                return flowGraphInspector;
            }

            [NotNull]
            readonly Dictionary<ICSharpExpression, bool> isPossibleConsumptionCache = new Dictionary<ICSharpExpression, bool>();

            Inspector(
                [NotNull] ICSharpControlFlowGraph controlFlowGraph,
                [NotNull] CSharpControlFlowContextFactory factory,
                [CanBeNull] Func<ITreeNode, ICSharpControlFlowGraph> graphBuilder) : base(controlFlowGraph, factory, graphBuilder) { }

            [NotNull]
            IResolveContext ResolveContext => ContextFactory.ResolveContext;

            [NotNull]
            [ItemNotNull]
            IEnumerable<ITreeNode> GetAccessExpressionsThroughLocalFunctionCalls(
                [NotNull] CSharpControlFlowContext context,
                [NotNull] ITreeNode expression,
                [NotNull] VariableInfo info)
            {
                var accessExpressionsThroughLocalFunctionCalls =
                    GetAccessExpressionsThroughLocalFunctionCalls(context, expression, info, new HashSet<ITreeNode>());
                Debug.Assert(accessExpressionsThroughLocalFunctionCalls != null);

                return accessExpressionsThroughLocalFunctionCalls;
            }

            bool IsPossibleConsumptionUsage([NotNull] ICSharpExpression ex, [CanBeNull] IDeclaredElement element)
            {
                var type = element.Type();
                if (!type.IsValueTask() && !type.IsGenericValueTask())
                {
                    return false; // not a ValueTask or ValueTask<T>
                }

                var parenthesizedExpression = ex.GetContainingParenthesizedExpression();
                if (BinaryExpressionNavigator.GetByLeftOperand(parenthesizedExpression) != null
                    || BinaryExpressionNavigator.GetByRightOperand(parenthesizedExpression) != null
                    || IsExpressionNavigator.GetByOperand(parenthesizedExpression) != null
                    || AsExpressionNavigator.GetByOperand(parenthesizedExpression) != null
                    || SwitchStatementNavigator.GetByGoverningExpression(parenthesizedExpression) != null
                    || AssignmentExpressionNavigator.GetByDest(parenthesizedExpression) != null)
                {
                    return false;
                }

                var qualifierExpression =
                    ReferenceExpressionNavigator.GetByQualifierExpression(parenthesizedExpression) ?? ex as IReferenceExpression;
                if (qualifierExpression != null)
                {
                    switch (qualifierExpression.Reference.Resolve(ResolveContext).DeclaredElement)
                    {
                        case IProperty _:
                        case IMethod method
                            when method.IsOverridesObjectGetHashCode()
                            || method.IsOverridesObjectEquals()
                            || method.IsOverridesObjectToString()
                            || method.IsIEquatableEqualsMethod()
                            || method.ShortName == nameof(GetType) && method.ContainingType.IsObjectClass():
                            return false; // is well-known pure method or property
                    }
                }

                var byArgument = InvocationExpressionNavigator.GetByArgument(CSharpArgumentNavigator.GetByValue(parenthesizedExpression));
                if (byArgument != null)
                {
                    var method = byArgument.InvocationExpressionReference.Resolve(ResolveContext).DeclaredElement as IMethod;
                    if (method.IsObjectEqualsMethod() || method.IsObjectReferenceEqualsMethod() || method.IsIEquatableEqualsMethod())
                    {
                        return false; // is an argument of the well-known pure method
                    }
                }

                if (ex is IInvocationExpression)
                {
                    return false;
                }

                return true;
            }

            bool IsPossibleConsumption(
                [NotNull] CSharpControlFlowContext context,
                [NotNull] ICSharpExpression expression,
                [NotNull] VariableInfo info)
            {
                foreach (var localFunctionCall in GetAccessExpressionsThroughLocalFunctionCalls(context, expression, info))
                {
                    if (localFunctionCall is ICSharpExpression accessExpression)
                    {
                        if (isPossibleConsumptionCache.TryGetValue(accessExpression, out var flag))
                        {
                            if (flag)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            flag = IsPossibleConsumptionUsage(accessExpression, info.DeclaredElement);
                            isPossibleConsumptionCache[expression] = flag;
                            if (flag)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            void OnPatchAccessInfo(
                [NotNull] IControlFlowElement controlFlowElement,
                [NotNull] CSharpControlFlowContext context,
                [NotNull] VariableInfo info,
                ControlFlowAccessType access,
                [CanBeNull] ITreeNode accessExpression)
            {
                // the ControlFlowVariableAccessState.ACCESSED_AS_ENUMERABLE is intentionally "misused" here

                if ((access & ControlFlowAccessType.Read) != 0
                    && (access & ControlFlowAccessType.Partial) == 0
                    && accessExpression is ICSharpExpression expression
                    && IsPossibleConsumption(context, expression, info))
                {
                    if ((context[info] & ControlFlowVariableAccessState.ACCESSED_AS_ENUMERABLE) != 0)
                    {
                        foreach (var recentAssignments in context.GetRecentAssignmentsList(info))
                        {
                            var key = Pair.Of((IDeclaredElement)info.DeclaredElement, recentAssignments);

                            if (!PossibleMultipleConsumption.ContainsKey(key))
                            {
                                PossibleMultipleConsumption.AddRange(
                                    key,
                                    from usage in context.GetUsagesOfRecentAssignments(info, recentAssignments).OfType<ICSharpExpression>()
                                    from e in GetAccessExpressionsThroughLocalFunctionCalls(context, usage, info)
                                        .OfType<ICSharpExpression>()
                                        .Where(accessSource => IsPossibleConsumption(context, accessSource, info))
                                    select e);
                            }

                            foreach (var localFunctionCall in GetAccessExpressionsThroughLocalFunctionCalls(context, expression, info))
                            {
                                if (localFunctionCall is ICSharpExpression e && IsPossibleConsumption(context, e, info))
                                {
                                    PossibleMultipleConsumption.Add(key, e);
                                }
                            }
                        }
                    }

                    context[info] |= ControlFlowVariableAccessState.ACCESSED_AS_ENUMERABLE;
                }
            }

            [NotNull]
            public OneToSetMap<Pair<IDeclaredElement, ITreeNode>, ICSharpExpression> PossibleMultipleConsumption { get; } =
                new OneToSetMap<Pair<IDeclaredElement, ITreeNode>, ICSharpExpression>();

            public override void Inspect()
            {
                PatchAccessInfo += OnPatchAccessInfo;
                try
                {
                    base.Inspect();
                }
                finally
                {
                    PatchAccessInfo -= OnPatchAccessInfo;
                }
            }
        }

        static void AnalyzeMultipleConsumptions(
            [NotNull] ICSharpDeclaration declaration,
            [NotNull] IHighlightingConsumer consumer,
            ValueAnalysisMode valueAnalysisMode)
        {
            switch (declaration)
            {
                case IAnonymousFunctionExpression _:
                case ICSharpFunctionDeclaration _:
                case IExpressionBodyOwnerDeclaration _:
                case IQueryParameterPlatform _:
                    break;

                default: return;
            }

            // build control flow graph of the method
            var controlFlowGraph = (ICSharpControlFlowGraph)ControlFlowBuilder.GetGraph(declaration);
            if (controlFlowGraph == null)
            {
                return;
            }

            // inspect the control flow graph
            var inspector = Inspector.Inspect(controlFlowGraph, valueAnalysisMode);

            // show warnings
            foreach (var (_, usages) in inspector.PossibleMultipleConsumption)
            {
                Debug.Assert(usages != null);

                foreach (var usage in usages)
                {
                    Debug.Assert(usage != null);
                    Debug.Assert(CSharpLanguage.Instance != null);

                    consumer.AddHighlighting(
                        new PossibleMultipleConsumptionWarning(
                            "Possible multiple consumption of " + usage.Type().GetPresentableName(CSharpLanguage.Instance) + ".",
                            usage));
                }
            }
        }

        static void AnalyzeBlockingAttempt([NotNull] IInvocationExpression invocationExpression, [NotNull] IHighlightingConsumer consumer)
        {
            if (invocationExpression.InvokedExpression is IReferenceExpression invocationExpressionInvokedExpression
                && invocationExpression.Reference.Resolve().DeclaredElement is IMethod getResultMethod
                && getResultMethod.ShortName == "GetResult"
                && getResultMethod.TypeParameters.Count == 0
                && getResultMethod.Parameters.Count == 0
                && invocationExpression.GetInvokedReferenceExpressionQualifier() is IInvocationExpression qualifier
                && qualifier.InvokedExpression is IReferenceExpression qualifierInvokedExpression)
            {
                var qualifierType = qualifier.Type();
                if ((qualifierType.IsClrType(ClrTypeNames.ValueTaskAwaiter) || qualifierType.IsClrType(ClrTypeNames.GenericValueTaskAwaiter))
                    && qualifier.Reference.Resolve().DeclaredElement is IMethod getAwaiterMethod
                    && getAwaiterMethod.ShortName == "GetAwaiter"
                    && getAwaiterMethod.TypeParameters.Count == 0
                    && getAwaiterMethod.Parameters.Count == 0)
                {
                    var valueTaskExpression = qualifier.GetInvokedReferenceExpressionQualifier();
                    var valueTaskType = valueTaskExpression?.Type();
                    if (valueTaskType.IsValueTask() || valueTaskType.IsGenericValueTask())
                    {
                        Debug.Assert(CSharpLanguage.Instance != null);

                        consumer.AddHighlighting(
                            new IntentionalBlockingAttemptWarning(
                                "Blocking on "
                                + valueTaskType.GetPresentableName(CSharpLanguage.Instance)
                                + " with 'GetAwaiter().GetResult()' might not block.",
                                invocationExpression.InvokedExpression,
                                valueTaskExpression,
                                qualifierInvokedExpression,
                                invocationExpressionInvokedExpression));
                    }
                }
            }
        }

        protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            switch (element)
            {
                case ICSharpDeclaration declaration:
                    AnalyzeMultipleConsumptions(declaration, consumer, data.GetValueAnalysisMode());
                    break;

                case IInvocationExpression invocationExpression:
                    AnalyzeBlockingAttempt(invocationExpression, consumer);
                    break;
            }
        }
    }
}