using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate types, methods, and parameters with [MustDisposeResource] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a type, a method, or a parameter with the [MustDisposeResource] attribute.")]
public sealed class AnnotateWithMustDisposeResource(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    static bool IsDisposable(ITypeElement type, IPsiModule psiModule)
        => type.IsDescendantOf(PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(psiModule))
            || type.IsDescendantOf(PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(psiModule));

    [Pure]
    static bool IsDisposable(IType type, ITreeNode context)
        => type.GetTypeElement() is { } typeElement && IsDisposable(typeElement, context.GetPsiModule()) && !type.IsTask() && !type.IsGenericTask()
            || type.IsTasklike(context)
            && type.GetTasklikeUnderlyingType(context).GetTypeElement() is { } asyncTypeElement
            && IsDisposable(asyncTypeElement, context.GetPsiModule());

    protected override string AnnotationAttributeTypeName => nameof(MustDisposeResourceAttribute);

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IClass type => IsDisposable(type, context.GetPsiModule()),
            IConstructor { ContainingType: IClass } constructor => IsDisposable(constructor.ContainingType, context.GetPsiModule()),
            IMethod method => IsDisposable(method.ReturnType, context),
            IParameter { Kind: ParameterKind.REFERENCE or ParameterKind.OUTPUT } parameter => IsDisposable(parameter.Type, context),

            _ => false,
        };

    protected override IAttribute? TryGetAttributeToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        => ownerDeclaration.Attributes.FirstOrDefault(
            attribute =>
            {
                var shortName = attribute.GetAttributeType().GetClrName().ShortName;

                return shortName == MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName
                    || shortName == PureAnnotationProvider.PureAttributeShortName;
            });
}