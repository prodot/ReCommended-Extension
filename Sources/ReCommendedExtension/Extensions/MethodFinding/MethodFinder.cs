using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal static class MethodFinder
{
    [Pure]
    public static bool HasMethod(
        this IClrTypeName clrTypeName,
        MethodSignature signature,
        bool returnParameterNames,
        out string[] parameterNames,
        IPsiModule psiModule)
    {
        parameterNames = [];

        if (clrTypeName.TryGetTypeElement(psiModule) is { } typeElement)
        {
            foreach (var method in typeElement.Methods)
            {
                if (method is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && method.IsStatic == signature.IsStatic
                    && method.ShortName == signature.Name
                    && method.TypeParametersCount == signature.GenericParametersCount
                    && method.Parameters.Count == signature.ParameterTypes.Count)
                {
                    if (signature.ParameterTypes is [])
                    {
                        return true;
                    }

                    var continueWithNextMethod = false;

                    if (returnParameterNames)
                    {
                        parameterNames = new string[signature.ParameterTypes.Count];
                    }

                    for (var i = 0; i < signature.ParameterTypes.Count; i++)
                    {
                        if (signature.ParameterTypes[i].IsSameAs(method.Parameters[i].Type, psiModule))
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
    public static bool HasMethod(this IClrTypeName clrTypeName, MethodSignature signature, IPsiModule psiModule)
        => clrTypeName.HasMethod(signature, false, out _, psiModule);
}