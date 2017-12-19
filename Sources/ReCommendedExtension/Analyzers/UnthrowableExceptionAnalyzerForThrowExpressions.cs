using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IThrowExpression), HighlightingTypes = new[] { typeof(UnthrowableExceptionHighlighting) })]
    public sealed class UnthrowableExceptionAnalyzerForThrowExpressions : ElementProblemAnalyzer<IThrowExpression>
    {

        protected override void Run(IThrowExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.Exception != null)
            {
                var reason = UnthrowableExceptions.TryGetReason(element.Exception);
                if (reason != null)
                {
                    consumer.AddHighlighting(new UnthrowableExceptionHighlighting(reason, element.Exception));
                }
            }
        }
    }
}