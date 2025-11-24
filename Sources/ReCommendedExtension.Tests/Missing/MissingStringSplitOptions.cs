namespace ReCommendedExtension.Tests.Missing;

internal static class MissingStringSplitOptions
{
    extension(StringSplitOptions)
    {
        public static StringSplitOptions TrimEntries => (StringSplitOptions)2;
    }
}