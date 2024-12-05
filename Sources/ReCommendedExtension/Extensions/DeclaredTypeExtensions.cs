using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Extensions;

internal static class DeclaredTypeExtensions
{
    [Pure]
    public static IType?[]? TryGetGenericParameterTypes(this IDeclaredType declaredType)
    {
        if (declaredType.GetTypeElement() is { } typeElement)
        {
            var elementTypes = new IType?[typeElement.TypeParametersCount];

            for (var i = 0; i < elementTypes.Length; i++)
            {
                if (CollectionTypeUtil.GetElementTypesForGenericType(declaredType, typeElement, i) is [var elementType])
                {
                    elementTypes[i] = elementType;
                }
            }

            return elementTypes;
        }

        return null;
    }
}