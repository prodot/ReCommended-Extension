using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseType;

[ElementProblemAnalyzer(typeof(IClassLikeDeclaration), HighlightingTypes = [typeof(RemoveRedundantBaseTypeDeclarationHint)])]
public sealed class BaseTypeAnalyzer : ElementProblemAnalyzer<IClassLikeDeclaration>
{
    protected override void Run(IClassLikeDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is IClassDeclaration or IRecordDeclaration { IsStruct: false }
            && element.SuperTypes.FirstOrDefault().IsObject()
            && element.ExtendsList is { ExtendedTypes: [_, ..] } baseTypes)
        {
            consumer.AddHighlighting(new RemoveRedundantBaseTypeDeclarationHint("'object' is the default base type.") { BaseTypes = baseTypes });
        }
    }
}