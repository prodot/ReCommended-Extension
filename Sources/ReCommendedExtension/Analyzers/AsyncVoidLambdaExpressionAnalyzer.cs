using System.Diagnostics;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(ILambdaExpression), HighlightingTypes = new[] { typeof(AsyncVoidFunctionExpressionHighlighting) })]
    public sealed class AsyncVoidLambdaExpressionAnalyzer : ElementProblemAnalyzer<ILambdaExpression>
    {
        protected override void Run(ILambdaExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (!element.IsAsync || !element.ReturnType.IsVoid())
            {
                return; // not an "async (...) => ..." expression that returns void
            }

            var assignmentExpression = element.Parent as IAssignmentExpression;
            if (assignmentExpression != null && assignmentExpression.IsEventSubscriptionOrUnSubscription())
            {
                return; // direct event target
            }

            Debug.Assert(element.AsyncKeyword != null);

            consumer.AddHighlighting(
                new AsyncVoidFunctionExpressionHighlighting(
                    "'async void' lambda expression not used as a direct event handler.",
                    element.AsyncKeyword,
                    () => element.SetAsync(false)));
        }
    }
}