namespace ReCommendedExtension.Extensions;

internal static class CharExtensions
{
    [Pure]
    public static bool IsPrintable(this char c) => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c);
}