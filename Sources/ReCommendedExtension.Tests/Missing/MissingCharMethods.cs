namespace ReCommendedExtension.Tests.Missing;

/// <remarks>
/// Original code from <see href="https://github.com/dotnet/dotnet"/><para/>
/// License: MIT<para/>
/// Copyright (c) .NET Foundation and Contributors
/// </remarks>
internal static class MissingCharMethods
{
    extension(char)
    {
        [Pure]
        public static bool IsAsciiDigit(char c) => IsBetween(c, '0', '9');

        [Pure]
        public static bool IsAsciiHexDigit(char c) => IsBetween(c, '0', '9') || IsBetween(c, 'A', 'F') || IsBetween(c, 'a', 'f');

        [Pure]
        public static bool IsAsciiHexDigitLower(char c) => IsBetween(c, '0', '9') || IsBetween(c, 'a', 'f');

        [Pure]
        public static bool IsAsciiHexDigitUpper(char c) => IsBetween(c, '0', '9') || IsBetween(c, 'A', 'F');

        [Pure]
        public static bool IsAsciiLetter(char c) => IsBetween(c, 'A', 'Z') || IsBetween(c, 'a', 'z');

        [Pure]
        public static bool IsAsciiLetterLower(char c) => IsBetween(c, 'a', 'z');

        [Pure]
        public static bool IsAsciiLetterOrDigit(char c) => IsBetween(c, 'A', 'Z') || IsBetween(c, 'a', 'z') || IsBetween(c, '0', '9');

        [Pure]
        public static bool IsAsciiLetterUpper(char c) => IsBetween(c, 'A', 'Z');

        [Pure]
        public static bool IsBetween(char c, char minInclusive, char maxInclusive)
            => unchecked((uint)(c - minInclusive)) <= unchecked((uint)(maxInclusive - minInclusive));
    }
}