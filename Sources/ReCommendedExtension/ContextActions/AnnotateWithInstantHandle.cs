using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate parameter with [InstantHandle] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a parameter with the [InstantHandle] attribute.")]
public sealed class AnnotateWithInstantHandle(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    protected override string AnnotationAttributeTypeName => nameof(InstantHandleAttribute);

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement is IParameter parameter
            && (parameter.Type.IsGenericIEnumerable() || parameter.Type.IsIAsyncEnumerable() || parameter.Type.IsDelegateType());
}