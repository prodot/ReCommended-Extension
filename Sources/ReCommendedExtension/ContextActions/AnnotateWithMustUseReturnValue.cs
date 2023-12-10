using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate method with [MustUseReturnValue] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method with the [MustUseReturnValue] attribute.")]
public sealed class AnnotateWithMustUseReturnValue(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    protected override string AnnotationAttributeTypeName => nameof(MustUseReturnValueAttribute);

    protected override string TextSuffix => "with observable state changes";

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement is IMethod method && !method.ReturnType.IsVoid();

    protected override IAttribute? TryGetAttributeToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        => ownerDeclaration.Attributes.FirstOrDefault(
            attribute => attribute.GetAttributeType().GetClrName().ShortName == PureAnnotationProvider.PureAttributeShortName);
}