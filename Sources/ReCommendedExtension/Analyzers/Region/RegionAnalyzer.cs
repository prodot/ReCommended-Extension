using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Region;

[ElementProblemAnalyzer(
    typeof(IStartRegion),
    HighlightingTypes = [typeof(RegionWithinTypeMemberBodyWarning), typeof(RegionWithSingleElementSuggestion)])]
public sealed class RegionAnalyzer : ElementProblemAnalyzer<IStartRegion>
{
    protected override void Run(IStartRegion element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.EndRegion is { })
        {
            var nonWhitespaceNodes = 0;

            foreach (var node in TreeRange.Build(element, element.EndRegion).Skip(1).TakeWhile(n => n != element.EndRegion))
            {
                if (node is { NodeType: TokenNodeType { IsWhitespace: true } })
                {
                    continue;
                }

                nonWhitespaceNodes++;

                if (nonWhitespaceNodes > 1)
                {
                    break;
                }
            }

            if (nonWhitespaceNodes == 1)
            {
                consumer.AddHighlighting(new RegionWithSingleElementSuggestion("The region contains a single element.", element));
            }
        }

        if (element.GetContainingNode<IBlock>() is { })
        {
            consumer.AddHighlighting(new RegionWithinTypeMemberBodyWarning("The region is contained within a type member body.", element));
        }
    }
}