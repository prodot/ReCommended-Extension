namespace ReCommendedExtension.Tests;

[Flags]
public enum MissingStringSplitOptions
{
    None = StringSplitOptions.None,
    RemoveEmptyEntries = StringSplitOptions.RemoveEmptyEntries,
    TrimEntries = 2,
}