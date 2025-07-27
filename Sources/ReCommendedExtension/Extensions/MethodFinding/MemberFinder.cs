using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal static class MemberFinder
{
    [Pure]
    static bool HasMember(
        this IClrTypeName clrTypeName,
        Func<ITypeElement, IEnumerable<IParametersOwner>> getMembers,
        IReadOnlyList<ParameterType> parameterTypes,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
    {
        parameterNames = [];

        if (clrTypeName.TryGetTypeElement(psiModule) is { } typeElement)
        {
            foreach (var member in getMembers(typeElement))
            {
                if (parameterTypes is [])
                {
                    return true;
                }

                var continueWithNextMember = false;

                if (returnParameterNames)
                {
                    parameterNames = new string[parameterTypes.Count];
                }

                for (var i = 0; i < parameterTypes.Count; i++)
                {
                    if (parameterTypes[i].IsSameAs(member.Parameters[i].Type, psiModule))
                    {
                        if (returnParameterNames)
                        {
                            parameterNames[i] = member.Parameters[i].ShortName;
                        }
                        continue;
                    }

                    continueWithNextMember = true;
                    break;
                }

                if (continueWithNextMember)
                {
                    parameterNames = [];
                    continue;
                }

                return true;
            }
        }

        return false;
    }

    [Pure]
    public static bool HasConstructor(
        this IClrTypeName clrTypeName,
        ConstructorSignature signature,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
        => clrTypeName.HasMember(
            typeElement =>
                from constructor in typeElement.Constructors
                where constructor is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && constructor.Parameters.Count == signature.ParameterTypes.Count
                select constructor,
            signature.ParameterTypes,
            returnParameterNames,
            out parameterNames,
            psiModule);

    [Pure]
    public static bool HasConstructor(this IClrTypeName clrTypeName, ConstructorSignature signature, IPsiModule psiModule)
        => clrTypeName.HasConstructor(signature, false, out _, psiModule);

    [Pure]
    public static bool HasProperty(this IClrTypeName clrTypeName, PropertySignature signature, IPsiModule psiModule)
        => clrTypeName.HasMember(
            typeElement =>
                from property in typeElement.Properties
                where property is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && property.IsStatic == signature.IsStatic
                    && property.ShortName == signature.Name
                select property,
            [],
            false,
            out _,
            psiModule);

    [Pure]
    public static bool HasMethod(
        this IClrTypeName clrTypeName,
        MethodSignature signature,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
        => clrTypeName.HasMember(
            typeElement =>
                from method in typeElement.Methods
                where method is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && method.IsStatic == signature.IsStatic
                    && method.ShortName == signature.Name
                    && method.TypeParametersCount == signature.GenericParametersCount
                    && method.Parameters.Count == signature.ParameterTypes.Count
                select method,
            signature.ParameterTypes,
            returnParameterNames,
            out parameterNames,
            psiModule);

    [Pure]
    public static bool HasMethod(this IClrTypeName clrTypeName, MethodSignature signature, IPsiModule psiModule)
        => clrTypeName.HasMethod(signature, false, out _, psiModule);
}