using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate methods, parameters, properties, and fields with [HandlesResourceDisposal] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method, a parameter, a property, or a field with the [HandlesResourceDisposal] attribute.")]

public sealed class AnnotateWithHandlesResourceDisposal(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    bool IsAttribute(IAttributeInstance attribute) => attribute.GetAttributeShortName() == AnnotationAttributeTypeName;

    [Pure]
    bool IsAnyBaseMethodAnnotated(IMethod method)
        => method.GetAllSuperMembers().Any(baseMethod => baseMethod.Member.GetAttributeInstances(AttributesSource.Self).Any(IsAttribute));

    [Pure]
    bool IsParameterOfAnyBaseMethodAnnotated(IParameter parameter)
    {
        if (parameter.ContainingParametersOwner is IMethod method)
        {
            var parameterIndex = method.Parameters.IndexOf(parameter);

            foreach (var member in method.GetAllSuperMembers())
            {
                var baseMethod = (IMethod)member.Member;

                if (baseMethod.Parameters.ElementAtOrDefault(parameterIndex) is { } baseMethodParameter
                    && TypeEqualityComparer.Default.Equals(parameter.Type, baseMethodParameter.Type)
                    && parameter.Kind == baseMethodParameter.Kind
                    && baseMethodParameter.GetAttributeInstances(AttributesSource.Self).Any(IsAttribute))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Pure]
    bool IsAnyBasePropertyAnnotated(IProperty property)
        => property.GetAllSuperMembers().Any(baseProperty => baseProperty.Member.GetAttributeInstances(AttributesSource.Self).Any(IsAttribute));

    protected override string AnnotationAttributeTypeName => nameof(HandlesResourceDisposalAttribute);

    protected override bool AllowsInheritedMethods => true;

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IMethod { IsStatic: false, ContainingType: { } } method => method.GetAccessRights() is not (AccessRights.PRIVATE or AccessRights.NONE)
                && (!IsAnyBaseMethodAnnotated(method)
                    && method is { ContainingType.IsDisposable : true, IsDisposeMethod: false, IsDisposeAsyncMethod: false }
                    || method is
                    {
                        ContainingType: IStruct { IsByRefLike: true },
                        IsDisposeMethodByConvention: false,
                        IsDisposeAsyncMethodByConvention: false,
                    }),

            IParameter { Kind: ParameterKind.VALUE or ParameterKind.INPUT or ParameterKind.READONLY_REFERENCE or ParameterKind.REFERENCE } parameter
                => parameter.Type.IsDisposable() && !IsParameterOfAnyBaseMethodAnnotated(parameter),

            IProperty property => property.Type.IsDisposable() && !IsAnyBasePropertyAnnotated(property),

            IField field => field.Type.IsDisposable(),

            _ => false,
        };
}