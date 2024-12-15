using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate type parameter, parameter, and return value with [DefaultEqualityUsage] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a type parameter, a parameter, or a return value with the [DefaultEqualityUsage] attribute.")]
public sealed class AnnotateWithDefaultEqualityUsage(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    protected override string AnnotationAttributeTypeName => nameof(DefaultEqualityUsageAttribute);

    protected override bool AnnotateMethodReturnValue => true;

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement is ITypeParameter or IParameter || declaredElement is IMethod method && !method.ReturnType.IsVoid();
}