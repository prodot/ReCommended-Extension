using JetBrains.Util;

namespace ReCommendedExtension.Extensions;

internal static class StringExtensions
{
    extension(string attributeShortName)
    {
        [Pure]
        public string WithoutSuffix()
        {
            Debug.Assert(attributeShortName.EndsWith("Attribute", StringComparison.Ordinal));

            return attributeShortName[..^"Attribute".Length];
        }
    }

    extension(string value)
    {
        [Pure]
        public string WithFirstCharacterUpperCased()
        {
            Debug.Assert(value is [>= 'a' and <= 'z', ..]);

            return $"{value[0].ToUpperFast().ToString()}{value[1..]}";
        }
    }
}