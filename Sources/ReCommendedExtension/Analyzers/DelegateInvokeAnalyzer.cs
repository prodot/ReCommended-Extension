using System;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IReferenceExpression), HighlightingTypes = new[] { typeof(RedundantDelegateInvokeHighlighting) })]
    public sealed class DelegateInvokeAnalyzer : ElementProblemAnalyzer<IReferenceExpression>
    {
        protected override void Run(IReferenceExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.QualifierExpression != null &&
                !element.HasConditionalAccessSign &&
                element.Reference.Resolve().DeclaredElement.IsDelegateInvokeMethod())
            {
                consumer.AddHighlighting(new RedundantDelegateInvokeHighlighting($"Redundant '{nameof(Action.Invoke)}' expression.", element));
            }
        }
    }
}