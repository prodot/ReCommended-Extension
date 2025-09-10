using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.ContextActions.Annotations;

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

    protected sealed override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IClass type => type.IsDisposable() && !IsAnyBaseTypeAnnotated(type),

            IStruct { IsByRefLike: false } type when context.MustDisposeResourceAttributeSupportsStructs() => type.IsDisposable(),

            IStruct { IsByRefLike: true } type when context.MustDisposeResourceAttributeSupportsStructs() => type.IsDisposable()
                || type.HasDisposeMethods(),

            IConstructor { ContainingType: (IClass or IStruct { IsByRefLike: false }) and var type } => type.IsDisposable()
                && !IsTypeAnnotated(type)
                && !IsAnyBaseTypeAnnotated(type),

            IConstructor { ContainingType: IStruct { IsByRefLike: true } type } => type.IsDisposable()
                || type.HasDisposeMethods() && !IsTypeAnnotated(type),

            IMethod method => (method.ReturnType.IsDisposable() || method.ReturnType.IsTasklikeOfDisposable(context))
                && !IsAnyBaseMethodAnnotated(method),

            ILocalFunction localFunction => localFunction.ReturnType.IsDisposable() || localFunction.ReturnType.IsTasklikeOfDisposable(context),

            IParameter { Kind: ParameterKind.REFERENCE or ParameterKind.OUTPUT } parameter => (parameter.Type.IsDisposable()
                    || parameter.Type.IsTasklikeOfDisposable(context))
                && !IsParameterOfAnyBaseMethodAnnotated(parameter),

            _ => false,
        };

    protected sealed override IAttribute[] GetAttributesToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        =>
        [
            ..
            from attribute in ownerDeclaration.Attributes
            where attribute.Target == AttributeTarget
            let shortName = attribute.GetAttributeType().GetClrName().ShortName
            where shortName == MustDisposeResourceAnnotationProvider.MustDisposeResourceAttributeShortName
                || shortName == MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName
                || shortName == PureAnnotationProvider.PureAttributeShortName
            select attribute,
        ];
}