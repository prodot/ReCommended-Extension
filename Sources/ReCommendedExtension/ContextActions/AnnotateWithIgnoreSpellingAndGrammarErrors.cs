using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate parameters with [IgnoreSpellingAndGrammarErrors] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a parameter with the [IgnoreSpellingAndGrammarErrors] attribute.")]

public sealed class AnnotateWithIgnoreSpellingAndGrammarErrors(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    protected override string AnnotationAttributeTypeName => nameof(IgnoreSpellingAndGrammarErrorsAttribute);

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement is IParameter { Kind: ParameterKind.VALUE or ParameterKind.INPUT };
}