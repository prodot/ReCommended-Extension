using JetBrains.ReSharper.Feature.Services.ContextActions;
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

[ContextAction(
    Group = "C#",
    Name = "Annotate method with [MustUseReturnValue] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method with the [MustUseReturnValue] attribute.")]
public sealed class AnnotateWithMustUseReturnValue(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
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
            IMethod method => !method.ReturnType.IsVoid() && !IsDisposable(method.ReturnType, context) && !IsAnyBaseMethodAnnotated(method),

            ILocalFunction localFunction => !localFunction.ReturnType.IsVoid() && !IsDisposable(localFunction.ReturnType, context),

            _ => false,
        };

    protected override IAttribute[] GetAttributesToReplace(IAttributesOwnerDeclaration ownerDeclaration)
        => (
            from attribute in ownerDeclaration.Attributes
            let shortName = attribute.GetAttributeType().GetClrName().ShortName
            where shortName == PureAnnotationProvider.PureAttributeShortName
                || shortName == MustDisposeResourceAnnotationProvider.MustDisposeResourceAttributeShortName
            select attribute).ToArray();
}