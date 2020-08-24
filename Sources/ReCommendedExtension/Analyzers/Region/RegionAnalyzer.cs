using System.Linq;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Region
{
    [ElementProblemAnalyzer(
        typeof(IStartRegion),
        HighlightingTypes = new[]
        {
            typeof(EmptyRegionSuggestion), typeof(RegionWithinTypeMemberBodyWarning), typeof(RegionWithSingleElementSuggestion),
        })]
    public sealed class RegionAnalyzer : ElementProblemAnalyzer<IStartRegion>
    {
        protected override void Run(IStartRegion element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.EndRegion != null)
            {
                var nonWhitespaceNodes = 0;
                foreach (var node in TreeRange.Build(element, element.EndRegion).Skip(1).TakeWhile(n => n != element.EndRegion))
                {
                    if (node?.NodeType is TokenNodeType tokenNodeType && tokenNodeType.IsWhitespace)
                    {
                        continue;
                    }

                    nonWhitespaceNodes++;

                    if (nonWhitespaceNodes > 1)
                    {
                        break;
                    }
                }

                switch (nonWhitespaceNodes)
                {
                    case 0:
                        consumer.AddHighlighting(new EmptyRegionSuggestion("The region is empty.", element));
                        break;

                    case 1:
                        consumer.AddHighlighting(new RegionWithSingleElementSuggestion("The region contains a single element.", element));
                        break;
                }
            }

            if (element.GetContainingNode<IBlock>() != null)
            {
                consumer.AddHighlighting(new RegionWithinTypeMemberBodyWarning("The region is contained within a type member body.", element));
            }
        }
    }
}