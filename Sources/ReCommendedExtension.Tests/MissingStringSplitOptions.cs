namespace ReCommendedExtension.Tests;

[Flags]
internal enum MissingStringSplitOptions
{
    None = StringSplitOptions.None,
    RemoveEmptyEntries = StringSplitOptions.RemoveEmptyEntries,
    TrimEntries = 2,
}