namespace ReCommendedExtension.Analyzers.BaseTypes;

public readonly record struct CharRange
{
    internal CharRange(char from, char to)
    {
        From = $"'{from}'";
        To = $"'{to}'";
    }

    internal CharRange(string from, string to)
    {
        From = from;
        To = to;
    }

    public string From { get; }

    public string To { get; }
}