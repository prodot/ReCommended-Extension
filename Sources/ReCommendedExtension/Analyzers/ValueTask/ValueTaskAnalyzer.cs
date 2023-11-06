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

namespace ReCommendedExtension.Analyzers.ValueTask;

[ElementProblemAnalyzer(
    typeof(ICSharpTreeNode),
    HighlightingTypes = new[] { typeof(PossibleMultipleConsumptionWarning), typeof(IntentionalBlockingAttemptWarning) })]
public sealed class ValueTaskAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
{
    sealed class Inspector : CSharpControlFlowGraphInspector
    {
        public static Inspector Inspect(
            ICSharpControlFlowGraph controlFlowGraph,
            ValueAnalysisMode analysisMode,
            bool shouldDisableValueAnalysisIfNullableWarningsEnabled = true,
            Func<ITreeNode, ICSharpControlFlowGraph>? graphBuilder = null)
        {
            var declaration = controlFlowGraph.Declaration;
            Debug.Assert(declaration is { });

            var containingFile = (ICSharpFile?)declaration.GetContainingFile();

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

        readonly Dictionary<ICSharpExpression, bool> isPossibleConsumptionCache = new();

        Inspector(
            ICSharpControlFlowGraph controlFlowGraph,
            CSharpControlFlowContextFactory factory,
            Func<ITreeNode, ICSharpControlFlowGraph>? graphBuilder) : base(controlFlowGraph, factory, graphBuilder) { }

        IResolveContext ResolveContext => ContextFactory.ResolveContext;

        IEnumerable<ITreeNode> GetAccessExpressionsThroughLocalFunctionCalls(
            CSharpControlFlowContext context,
            ITreeNode expression,
            VariableInfo info)
        {
            var accessExpressionsThroughLocalFunctionCalls =
                GetAccessExpressionsThroughLocalFunctionCalls(context, expression, info, new HashSet<ITreeNode>());

            return accessExpressionsThroughLocalFunctionCalls;
        }

        bool IsPossibleConsumptionUsage(ICSharpExpression ex, IDeclaredElement? element)
        {
            var type = element.Type();
            if (!type.IsValueTask() && !type.IsGenericValueTask())
            {
                return false; // not a ValueTask or ValueTask<T>
            }

            var parenthesizedExpression = ex.GetContainingParenthesizedExpression();
            if (BinaryExpressionNavigator.GetByLeftOperand(parenthesizedExpression) is { }
                || BinaryExpressionNavigator.GetByRightOperand(parenthesizedExpression) is { }
                || IsExpressionNavigator.GetByOperand(parenthesizedExpression) is { }
                || AsExpressionNavigator.GetByOperand(parenthesizedExpression) is { }
                || SwitchStatementNavigator.GetByGoverningExpression(parenthesizedExpression) is { }
                || AssignmentExpressionNavigator.GetByDest(parenthesizedExpression) is { })
            {
                return false;
            }

            var qualifierExpression = ReferenceExpressionNavigator.GetByQualifierExpression(parenthesizedExpression) ?? ex as IReferenceExpression;
            if (qualifierExpression is { })
            {
                switch (qualifierExpression.Reference.Resolve(ResolveContext).DeclaredElement)
                {
                    case IProperty:
                    case IMethod method
                        when method.IsOverridesObjectGetHashCode()
                        || method.IsOverridesObjectEquals()
                        || method.IsOverridesObjectToString()
                        || method.IsIEquatableEqualsMethod()
                        || method.ShortName == nameof(GetType) && method.ContainingType.IsObjectClass():
                        return false; // is well-known pure method or property
                }
            }

            if (InvocationExpressionNavigator.GetByArgument(CSharpArgumentNavigator.GetByValue(parenthesizedExpression)) is { } byArgument)
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

        bool IsPossibleConsumption(CSharpControlFlowContext context, ICSharpExpression expression, VariableInfo info)
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
            IControlFlowElement controlFlowElement,
            CSharpControlFlowContext context,
            VariableInfo info,
            ControlFlowAccessType access,
            ITreeNode? accessExpression)
        {
            // the ControlFlowVariableAccessState.ACCESSED_AS_ENUMERABLE is intentionally "misused" here

            if ((access & ControlFlowAccessType.Read) != 0
                && (access & ControlFlowAccessType.Partial) == 0
                && accessExpression is ICSharpExpression expression
                && IsPossibleConsumption(context, expression, info))
            {
                if ((context[info] & ControlFlowVariableAccessState.ACCESSED_AS_ENUMERABLE) != 0)
                {
                    Debug.Assert(info.DeclaredElement is { });

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

        public OneToSetMap<Pair<IDeclaredElement, ITreeNode>, ICSharpExpression> PossibleMultipleConsumption { get; } = new();

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

    static void AnalyzeMultipleConsumptions(ICSharpDeclaration declaration, IHighlightingConsumer consumer, ValueAnalysisMode valueAnalysisMode)
    {
        if (declaration is not (IAnonymousFunctionExpression
            or ICSharpFunctionDeclaration
            or IExpressionBodyOwnerDeclaration
            or IQueryParameterPlatform))
        {
            return;
        }

        // build control flow graph of the method
        var controlFlowGraph = (ICSharpControlFlowGraph?)ControlFlowBuilder.GetGraph(declaration);
        if (controlFlowGraph is not { })
        {
            return;
        }

        // inspect the control flow graph
        var inspector = Inspector.Inspect(controlFlowGraph, valueAnalysisMode);

        // show warnings
        foreach (var (_, usages) in inspector.PossibleMultipleConsumption)
        {
            foreach (var usage in usages)
            {
                Debug.Assert(CSharpLanguage.Instance is { });

                consumer.AddHighlighting(
                    new PossibleMultipleConsumptionWarning(
                        $"Possible multiple consumption of {usage.Type().GetPresentableName(CSharpLanguage.Instance)}.",
                        usage));
            }
        }
    }

    static void AnalyzeBlockingAttempt(IInvocationExpression invocationExpression, IHighlightingConsumer consumer)
    {
        if (invocationExpression.InvokedExpression is IReferenceExpression invocationExpressionInvokedExpression
            && invocationExpression.Reference.Resolve().DeclaredElement is IMethod { ShortName: "GetResult", TypeParameters: [], Parameters: [] }
            && invocationExpression.GetInvokedReferenceExpressionQualifier() is IInvocationExpression
            {
                InvokedExpression: IReferenceExpression qualifierInvokedExpression,
            } qualifier)
        {
            var qualifierType = qualifier.Type();
            if ((qualifierType.IsClrType(ClrTypeNames.ValueTaskAwaiter) || qualifierType.IsClrType(ClrTypeNames.GenericValueTaskAwaiter))
                && qualifier.Reference.Resolve().DeclaredElement is IMethod { ShortName: "GetAwaiter", TypeParameters: [], Parameters: [] })
            {
                var valueTaskExpression = qualifier.GetInvokedReferenceExpressionQualifier();
                var valueTaskType = valueTaskExpression?.Type();
                if (valueTaskType.IsValueTask() || valueTaskType.IsGenericValueTask())
                {
                    Debug.Assert(valueTaskExpression is { });
                    Debug.Assert(CSharpLanguage.Instance is { });

                    consumer.AddHighlighting(
                        new IntentionalBlockingAttemptWarning(
                            $"Blocking on {valueTaskType.GetPresentableName(CSharpLanguage.Instance)} with 'GetAwaiter().GetResult()' might not block.",
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