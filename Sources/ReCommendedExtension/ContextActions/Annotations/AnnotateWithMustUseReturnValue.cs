using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate methods with [MustUseReturnValue] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method with the [MustUseReturnValue] attribute.")]
public sealed class AnnotateWithMustUseReturnValue(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    bool IsAnyBaseMethodAnnotated(IMethod method)
        => method
            .GetAllSuperMembers()
            .Any(
                baseMethod => baseMethod
                    .Member.GetAttributeInstances(AttributesSource.Self)
                    .Any(attribute => attribute.GetAttributeShortName() == AnnotationAttributeTypeName));

    protected override string AnnotationAttributeTypeName => nameof(MustUseReturnValueAttribute);

    protected override bool AllowsInheritedMethods => true;

    protected override string TextSuffix => "with observable state changes";

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
            IMethod method => !method.ReturnType.IsVoid()
                && !method.ReturnType.IsDisposable(context)
                && !IsAnyBaseMethodAnnotated(method),
#else
            IMethod method => !method.ReturnType.IsVoid()
                && !method.ReturnType.IsDisposable(context)
                && !method.ReturnType.IsTasklikeOfIsDisposable(context)
                && !IsAnyBaseMethodAnnotated(method),
#endif

            ILocalFunction localFunction => !localFunction.ReturnType.IsVoid() && !localFunction.ReturnType.IsDisposable(context),

            _ => false,
        };

    protected override IAttribute[] GetAttributesToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        =>
        [
            ..
            from attribute in ownerDeclaration.Attributes
            let shortName = attribute.GetAttributeType().GetClrName().ShortName
            where shortName == PureAnnotationProvider.PureAttributeShortName
                || shortName == MustDisposeResourceAnnotationProvider.MustDisposeResourceAttributeShortName
            select attribute,
        ];
}