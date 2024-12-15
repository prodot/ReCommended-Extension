using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions;

internal static class AttributesOwnerExtensions
{
    [Pure]
    public static IEnumerable<T> WithoutObsolete<T>(this IEnumerable<T> attributesOwners) where T : class, IAttributesOwner
        =>
            from attributesOwner in attributesOwners
            where !attributesOwner.HasAttributeInstance(PredefinedType.OBSOLETE_ATTRIBUTE_CLASS, false)
            select attributesOwner;
}