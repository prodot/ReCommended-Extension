using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate types, methods, and parameters with [MustDisposeResource] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a type, a method, or a parameter with the [MustDisposeResource] attribute.")]
public sealed class AnnotateWithMustDisposeResource(ICSharpContextActionDataProvider provider) : AnnotateWithMustDisposeResourceBase(provider)
{
    [Pure]
    bool IsAttribute(IAttributeInstance attribute)
        => attribute.GetAttributeShortName() == AnnotationAttributeTypeName
            && (attribute.PositionParameterCount == 0
                || attribute.PositionParameterCount == 1
                && attribute.PositionParameter(0).ConstantValue is { Kind: ConstantValueKind.Bool, BoolValue: true });

    protected override bool IsAttribute(IAttribute attribute)
        => base.IsAttribute(attribute) && attribute.Arguments is [] or [{ Value.ConstantValue: { Kind: ConstantValueKind.Bool, BoolValue: true } }];

    protected override bool IsTypeAnnotated(ITypeElement type) => type.GetAttributeInstances(AttributesSource.Self).Any(IsAttribute);

    protected override bool IsAnyBaseTypeAnnotated(ITypeElement type) => type.GetAllSuperTypeElements().Any(IsTypeAnnotated);

    protected override bool IsAnyBaseMethodAnnotated(IMethod method)
        => method.GetAllSuperMembers().Any(baseMethod => baseMethod.Member.GetAttributeInstances(AttributesSource.Self).Any(IsAttribute));

    protected override bool IsParameterOfAnyBaseMethodAnnotated(IParameter parameter)
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
}