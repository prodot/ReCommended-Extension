using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions;

internal static class AttributesOwnerExtensions
{
    extension<T>(IEnumerable<T> attributesOwners) where T : class, IAttributesOwner
    {
        [Pure]
        public IEnumerable<T> WithoutObsolete()
            =>
                from attributesOwner in attributesOwners
                where !attributesOwner.HasAttributeInstance(PredefinedType.OBSOLETE_ATTRIBUTE_CLASS, false)
                select attributesOwner;
    }
}