using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseType;

[ElementProblemAnalyzer(typeof(IClassLikeDeclaration), HighlightingTypes = [typeof(RemoveRedundantBaseTypeDeclarationSuggestion)])]
public sealed class BaseTypeAnalyzer : ElementProblemAnalyzer<IClassLikeDeclaration>
{
    protected override void Run(IClassLikeDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is IClassDeclaration or IRecordDeclaration { IsStruct: false })
        {
            var firstBaseType = element.SuperTypes.FirstOrDefault();
            if (firstBaseType.IsObject())
            {
                var baseTypes = element.ExtendsList;
                Debug.Assert(baseTypes is { ExtendedTypes: [_, ..] });

                consumer.AddHighlighting(
                    new RemoveRedundantBaseTypeDeclarationSuggestion("'object' is default base type.", baseTypes));
            }
        }
    }
}