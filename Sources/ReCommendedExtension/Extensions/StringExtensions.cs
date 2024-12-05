using JetBrains.Util;

namespace ReCommendedExtension.Extensions;

internal static class StringExtensions
{
    [Pure]
    public static string WithoutSuffix(this string attributeShortName)
    {
        Debug.Assert(attributeShortName.EndsWith("Attribute", StringComparison.Ordinal));

        return attributeShortName[..^"Attribute".Length];
    }

    [Pure]
    public static string WithFirstCharacterUpperCased(this string value)
    {
        Debug.Assert(value is [>= 'a' and <= 'z', ..]);

        return $"{value[0].ToUpperFast().ToString()}{value[1..]}";
    }
}