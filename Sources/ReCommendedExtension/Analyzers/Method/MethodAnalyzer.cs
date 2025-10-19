using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.Method.Inspections;
using ReCommendedExtension.Analyzers.Method.Rules;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Method;

[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(RedundantMethodInvocationHint)])]
public sealed class MethodAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
    void Analyze(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpExpression qualifier,
        IReferenceExpression invokedExpression,
        Rules.Method method,
        string methodName,
        IList<IParameter> resolvedParameters,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        foreach (var inspection in method.Inspections)
        {
            switch (inspection)
            {
                case RedundantMethodInvocation redundantMethodInvocation
                    when (!redundantMethodInvocation.IsPureMethod || !invocationExpression.IsUsedAsStatement())
                    && redundantMethodInvocation.Condition(qualifier, arguments, nullableReferenceTypesDataFlowAnalysisRunSynchronizer):
                {
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(inspection.Message(methodName), invocationExpression, invokedExpression));

                    break;
                }
            }
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is
            {
                InvokedExpression: IReferenceExpression
                {
                    QualifierExpression: { } qualifierExpression, Reference: var reference,
                } invokedExpression,
            }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, ContainingType: { } containingType,
            } resolvedMethod
            && RuleDefinitions.TryGetMethod(containingType, resolvedMethod) is { } method
            && element.TryGetArgumentsInDeclarationOrder() is { } arguments)
        {
            Analyze(
                consumer,
                element,
                qualifierExpression,
                invokedExpression,
                method,
                resolvedMethod.ShortName,
                resolvedMethod.Parameters,
                arguments);
        }
    }
}