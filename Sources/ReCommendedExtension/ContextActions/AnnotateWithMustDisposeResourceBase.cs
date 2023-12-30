using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions;

public abstract class AnnotateWithMustDisposeResourceBase(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    protected abstract bool IsTypeAnnotated(ITypeElement type);

    [Pure]
    protected abstract bool IsAnyBaseTypeAnnotated(ITypeElement type);

    [Pure]
    protected abstract bool IsAnyBaseMethodAnnotated(IMethod method);

    [Pure]
    protected abstract bool IsParameterOfAnyBaseMethodAnnotated(IParameter parameter);

    protected sealed override string AnnotationAttributeTypeName => nameof(MustDisposeResourceAttribute);

    protected sealed override bool AllowsInheritedMethods => true;

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context) => declaredElement switch
    {
        IClass type => type.IsDisposable(context.GetPsiModule()) && !IsAnyBaseTypeAnnotated(type),

        IConstructor { ContainingType: IClass or IStruct { IsByRefLike: false } } constructor =>
            constructor.ContainingType.IsDisposable(context.GetPsiModule())
            && !IsTypeAnnotated(constructor.ContainingType)
            && !IsAnyBaseTypeAnnotated(constructor.ContainingType),

        IConstructor { ContainingType: IStruct { IsByRefLike: true } s } => s.HasDisposeMethods(),

#if MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
        IMethod method => method.ReturnType.IsDisposable(context) && !IsAnyBaseMethodAnnotated(method),

        ILocalFunction localFunction => localFunction.ReturnType.IsDisposable(context),

        IParameter { Kind: ParameterKind.REFERENCE or ParameterKind.OUTPUT } parameter => parameter.Type.IsDisposable(context)
            && !IsParameterOfAnyBaseMethodAnnotated(parameter),
#else
        IMethod method => (method.ReturnType.IsDisposable(context) || method.ReturnType.IsTasklikeOfIsDisposable(context))
            && !IsAnyBaseMethodAnnotated(method),

        ILocalFunction localFunction => localFunction.ReturnType.IsDisposable(context) || localFunction.ReturnType.IsTasklikeOfIsDisposable(context),

        IParameter { Kind: ParameterKind.REFERENCE or ParameterKind.OUTPUT } parameter => (parameter.Type.IsDisposable(context)
                || parameter.Type.IsTasklikeOfIsDisposable(context))
            && !IsParameterOfAnyBaseMethodAnnotated(parameter),
#endif

        _ => false,
    };

    protected sealed override IAttribute[] GetAttributesToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        => (
            from attribute in ownerDeclaration.Attributes
            let shortName = attribute.GetAttributeType().GetClrName().ShortName
            where shortName == MustDisposeResourceAnnotationProvider.MustDisposeResourceAttributeShortName
                || shortName == MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName
                || shortName == PureAnnotationProvider.PureAttributeShortName
            select attribute).ToArray();
}