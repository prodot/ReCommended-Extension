using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class NumberAnalyzer<N>(NumberInfo<N> numberInfo) : ElementProblemAnalyzer<IInvocationExpression> where N : struct
{
    private protected NumberInfo<N> NumberInfo => numberInfo;

    private protected virtual void Analyze(IInvocationExpression element, IMethod method, IHighlightingConsumer consumer) { }

    protected sealed override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
            } method)
        {
            Analyze(element, method, consumer);
        }
    }
}