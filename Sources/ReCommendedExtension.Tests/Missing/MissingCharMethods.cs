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
        public static bool IsAsciiHexDigitLower(char c) => char.IsBetween(c, '0', '9') || char.IsBetween(c, 'a', 'f');

        [Pure]
        public static bool IsAsciiHexDigitUpper(char c) => char.IsBetween(c, '0', '9') || char.IsBetween(c, 'A', 'F');
    }
}