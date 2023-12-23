using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions;

// todo: remove compiler directive "MUST_DISPOSE_RESOURCE_NO_TASK_LIKE" when [MustDisposeResource] supports task-like method and parameters (https://youtrack.jetbrains.com/issue/RSRP-495289/MustDisposeResource-should-support-task-like-method-and-parameters)

public abstract class AnnotateWithMustDisposeResourceBase(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    static bool IsDisposable(IType type, ITreeNode context)
        => type.GetTypeElement() is { } typeElement
            && (typeElement.IsDisposable(context.GetPsiModule()) && !type.IsTask() && !type.IsGenericTask()
                || type is IDeclaredType declaredType && declaredType.GetTypeElement() is IStruct { IsByRefLike: true } s && s.HasDisposeMethods())

#if !MUST_DISPOSE_RESOURCE_NO_TASK_LIKE
            || type.IsTasklike(context)
            && type.GetTasklikeUnderlyingType(context).GetTypeElement() is { } awaitedTypeElement
            && awaitedTypeElement.IsDisposable(context.GetPsiModule())
            && !awaitedTypeElement.Type().IsTask()
            && !awaitedTypeElement.Type().IsGenericTask()
#endif
    ;

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

        IMethod method => IsDisposable(method.ReturnType, context) && !IsAnyBaseMethodAnnotated(method),

        ILocalFunction localFunction => IsDisposable(localFunction.ReturnType, context),

        IParameter { Kind: ParameterKind.REFERENCE or ParameterKind.OUTPUT } parameter => IsDisposable(parameter.Type, context)
            && !IsParameterOfAnyBaseMethodAnnotated(parameter),

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