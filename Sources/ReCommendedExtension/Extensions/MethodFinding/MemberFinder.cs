using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal static class MemberFinder
{
    [Pure]
    static bool HasMember(
        [InstantHandle] this IEnumerable<IParametersOwner> members,
        IReadOnlyList<Parameter> parameters,
        bool returnParameterNames,
        out string[] parameterNames)
    {
        parameterNames = [];

        foreach (var member in members)
        {
            if (parameters is [])
            {
                return true;
            }

            var continueWithNextMember = false;

            if (returnParameterNames)
            {
                parameterNames = new string[parameters.Count];
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var memberParameter = member.Parameters[i];

                if (parameter.Kind == memberParameter.Kind && parameter.IsType(memberParameter.Type))
                {
                    if (returnParameterNames)
                    {
                        parameterNames[i] = memberParameter.ShortName;
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

        return false;
    }

    [Pure]
    public static bool HasConstructor(
        this ITypeElement typeElement,
        ConstructorSignature signature,
        bool returnParameterNames,
        out string[] parameterNames)
        => (
            from constructor in typeElement.Constructors
            where constructor is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                && constructor.Parameters.Count == signature.Parameters.Count
            select constructor).HasMember(signature.Parameters, returnParameterNames, out parameterNames);

    [Pure]
    public static bool HasConstructor(
        this IClrTypeName clrTypeName,
        ConstructorSignature signature,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
    {
        if (clrTypeName.TryGetTypeElement(psiModule) is { } typeElement)
        {
            return typeElement.HasConstructor(signature, returnParameterNames, out parameterNames);
        }

        parameterNames = [];
        return false;
    }

    [Pure]
    public static bool HasConstructor(this ITypeElement typeElement, ConstructorSignature signature)
        => typeElement.HasConstructor(signature, false, out _);

    [Pure]
    public static bool HasConstructor(this IClrTypeName clrTypeName, ConstructorSignature signature, IPsiModule psiModule)
        => clrTypeName.TryGetTypeElement(psiModule) is { } typeElement && typeElement.HasConstructor(signature);

    [Pure]
    public static bool HasProperty(this ITypeElement typeElement, PropertySignature signature)
        => (
            from property in typeElement.Properties
            where property is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                && property.IsStatic == signature.IsStatic
                && property.ShortName == signature.Name
            select property).HasMember([], false, out _);

    [Pure]
    public static bool HasProperty(this IClrTypeName clrTypeName, PropertySignature signature, IPsiModule psiModule)
        => clrTypeName.TryGetTypeElement(psiModule) is { } typeElement && typeElement.HasProperty(signature);

    [Pure]
    public static bool HasMethod(this ITypeElement typeElement, MethodSignature signature, bool returnParameterNames, out string[] parameterNames)
        => (
            from method in typeElement.Methods
            where method is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                && method.IsStatic == signature.IsStatic
                && method.ShortName == signature.Name
                && method.TypeParametersCount == signature.GenericParametersCount
                && method.Parameters.Count == signature.Parameters.Count
            select method).HasMember(signature.Parameters, returnParameterNames, out parameterNames);

    [Pure]
    public static bool HasMethod(
        this IClrTypeName clrTypeName,
        MethodSignature signature,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
    {
        if (clrTypeName.TryGetTypeElement(psiModule) is { } typeElement)
        {
            return typeElement.HasMethod(signature, returnParameterNames, out parameterNames);
        }

        parameterNames = [];
        return false;
    }

    [Pure]
    public static bool HasMethod(this ITypeElement typeElement, MethodSignature signature) => typeElement.HasMethod(signature, false, out _);

    [Pure]
    public static bool HasMethod(this IClrTypeName clrTypeName, MethodSignature signature, IPsiModule psiModule)
        => clrTypeName.TryGetTypeElement(psiModule) is { } typeElement && typeElement.HasMethod(signature);
}