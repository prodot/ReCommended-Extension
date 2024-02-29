using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    Group = "C#",
    Name = "Annotate method with [Pure] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method with the [Pure] attribute.")]
public sealed class AnnotateWithPure(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    static bool ReturnsAnyValueWithoutRefParameters(IParametersOwner parametersOwner)
    {
        if (parametersOwner.ReturnType.IsVoid())
        {
            var outParameter = false;

            foreach (var parameter in parametersOwner.Parameters)
            {
                switch (parameter.Kind)
                {
                    case ParameterKind.OUTPUT:
                        outParameter = true;
                        break;

                    case ParameterKind.REFERENCE: return false;
                }
            }

            return outParameter;
        }

        foreach (var parameter in parametersOwner.Parameters)
        {
            if (parameter.Kind == ParameterKind.REFERENCE)
            {
                return false;
            }
        }

        return true;
    }

    [Pure]
    bool IsAnyBaseMethodAnnotated(IMethod method)
        => method
            .GetAllSuperMembers()
            .Any(
                baseMethod => baseMethod
                    .Member.GetAttributeInstances(AttributesSource.Self)
                    .Any(attribute => attribute.GetAttributeShortName() == AnnotationAttributeTypeName));

    protected override string AnnotationAttributeTypeName => nameof(PureAttribute);

    protected override bool AllowsInheritedMethods => true;

    protected override string TextSuffix => "no observable state changes";

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IMethod method => ReturnsAnyValueWithoutRefParameters(method)
                && !method.ReturnType.IsDisposable(context)
                && !method.ReturnType.IsTasklikeOfIsDisposable(context)
                && !IsAnyBaseMethodAnnotated(method),

            ILocalFunction localFunction => ReturnsAnyValueWithoutRefParameters(localFunction)
                && !localFunction.ReturnType.IsDisposable(context)
                && !localFunction.ReturnType.IsTasklikeOfIsDisposable(context),

            _ => false,
        };

    protected override IAttribute[] GetAttributesToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        =>
        [
            ..
            from attribute in ownerDeclaration.Attributes
            let shortName = attribute.GetAttributeType().GetClrName().ShortName
            where shortName == MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName
                || shortName == MustDisposeResourceAnnotationProvider.MustDisposeResourceAttributeShortName
            select attribute,
        ];
}