using System;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Impl.Tree;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.DelegateInvoke
{
    [ElementProblemAnalyzer(typeof(IReferenceExpression), HighlightingTypes = new[] { typeof(RedundantDelegateInvokeSuggestion) })]
    public sealed class DelegateInvokeAnalyzer : ElementProblemAnalyzer<IReferenceExpression>
    {
        protected override void Run(IReferenceExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.QualifierExpression != null &&
                !element.HasConditionalAccessSign &&
                element.Reference.Resolve().DeclaredElement.IsDelegateInvokeMethod() &&
                element.GetNextMeaningfulSibling() is CSharpTokenBase token &&
                token.GetTokenType() == CSharpTokenType.LPARENTH)
            {
                consumer.AddHighlighting(new RedundantDelegateInvokeSuggestion($"Redundant '{nameof(Action.Invoke)}' expression.", element));
            }
        }
    }
}