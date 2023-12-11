using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate method with [Pure] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method with the [Pure] attribute.")]
public sealed class AnnotateWithPure(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    protected override string AnnotationAttributeTypeName => nameof(PureAttribute);

    protected override string TextSuffix => "no observable state changes";

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement is IMethod method
            && (!method.ReturnType.IsVoid() || method.Parameters.Any(parameter => parameter.Kind == ParameterKind.OUTPUT))
            && method.Parameters.All(parameter => parameter.Kind != ParameterKind.REFERENCE);

    protected override IAttribute? TryGetAttributeToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        => ownerDeclaration.Attributes.FirstOrDefault(
            attribute =>
            {
                var shortName = attribute.GetAttributeType().GetClrName().ShortName;

                return shortName == MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName
                    || shortName == MustDisposeResourceAnnotationProvider.MustDisposeResourceAttributeShortName;
            });
}