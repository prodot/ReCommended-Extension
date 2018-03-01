using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IYieldStatement), HighlightingTypes = new[] { typeof(YieldReturnWithinLockHighlighting) })]
    public sealed class YieldReturnWithinLockAnalyzer : ElementProblemAnalyzer<IYieldStatement>
    {
        protected override void Run(IYieldStatement element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            for (var treeNode = element.Parent; treeNode != null; treeNode = treeNode.Parent)
            {
                switch (treeNode)
                {
                    case ILockStatement _:
                        consumer.AddHighlighting(new YieldReturnWithinLockHighlighting("'yield return' used inside the 'lock' block.", element));
                        return;

                    case ILocalFunctionDeclaration _:
                        return;
                }
            }
        }
    }
}