using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IThrowStatement), HighlightingTypes = new[] { typeof(UnthrowableExceptionHighlighting) })]
    public sealed class UnthrowableExceptionAnalyzerForThrowStatements : ElementProblemAnalyzer<IThrowStatement>
    {

        protected override void Run(IThrowStatement element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
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