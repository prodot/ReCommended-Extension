using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Analyzers.Strings.MethodFinding;

internal static class MethodFinder
{
    [Pure]
    public static bool HasMethod(
        this IClrTypeName clrTypeName,
        string methodName,
        [NonNegativeValue] int genericParametersCount,
        IReadOnlyList<ParameterType> parameterTypes,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
    {
        parameterNames = [];

        if (clrTypeName.TryGetTypeElement(psiModule) is { } stringBuilderType)
        {
            foreach (var method in stringBuilderType.Methods)
            {
                if (method is { IsStatic: false, AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && method.ShortName == methodName
                    && method.TypeParametersCount == genericParametersCount
                    && method.Parameters.Count == parameterTypes.Count)
                {
                    if (parameterTypes is [])
                    {
                        return true;
                    }

                    var continueWithNextMethod = false;

                    if (returnParameterNames)
                    {
                        parameterNames = new string[parameterTypes.Count];
                    }

                    for (var i = 0; i < parameterTypes.Count; i++)
                    {
                        if (parameterTypes[i].IsSameAs(method.Parameters[i].Type, psiModule))
                        {
                            if (returnParameterNames)
                            {
                                parameterNames[i] = method.Parameters[i].ShortName;
                            }
                            continue;
                        }

                        continueWithNextMethod = true;
                        break;
                    }

                    if (continueWithNextMethod)
                    {
                        parameterNames = [];
                        continue;
                    }

                    return true;
                }
            }
        }

        return false;
    }

    [Pure]
    public static bool HasMethod(
        this IClrTypeName clrTypeName,
        string methodName,
        IReadOnlyList<ParameterType> parameterTypes,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
        => clrTypeName.HasMethod(methodName, 0, parameterTypes, returnParameterNames, out parameterNames, psiModule);

    [Pure]
    public static bool HasMethod(
        this IClrTypeName clrTypeName,
        string methodName,
        [NonNegativeValue] int genericParametersCount,
        IReadOnlyList<ParameterType> parameterTypes,
        IPsiModule psiModule)
        => clrTypeName.HasMethod(methodName, genericParametersCount, parameterTypes, false, out _, psiModule);

    [Pure]
    public static bool HasMethod(this IClrTypeName clrTypeName, string methodName, IReadOnlyList<ParameterType> parameterTypes, IPsiModule psiModule)
        => clrTypeName.HasMethod(methodName, 0, parameterTypes, false, out _, psiModule);
}