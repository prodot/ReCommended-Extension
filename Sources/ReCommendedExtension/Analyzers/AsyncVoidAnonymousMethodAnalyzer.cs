using System.Diagnostics;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IAnonymousMethodExpression), HighlightingTypes = new[] { typeof(AsyncVoidFunctionExpressionHighlighting) })]
    public sealed class AsyncVoidAnonymousMethodAnalyzer : ElementProblemAnalyzer<IAnonymousMethodExpression>
    {
        protected override void Run(IAnonymousMethodExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (!element.IsAsync || !element.ReturnType.IsVoid())
            {
                return; // not an "async delegate (...) { ... }" that returns void
            }

            if (element.Parent is IAssignmentExpression assignmentExpression && assignmentExpression.IsEventSubscriptionOrUnSubscription())
            {
                return; // direct event target
            }

            Debug.Assert(element.AsyncKeyword != null);

            consumer.AddHighlighting(
                new AsyncVoidFunctionExpressionHighlighting(
                    "'async void' anonymous method expression not used as a direct event handler.",
                    element.AsyncKeyword,
                    () => element.SetAsync(false)));
        }
    }
}