namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

public record struct RangeIndexerReplacement
{
    internal RangeIndexerReplacement(string index)
    {
        Index = index;
        IndexDisplayText = index;
    }

    internal RangeIndexerReplacement(string startIndex, string endIndex)
    {
        Index = $"{(startIndex != "" ? $"({startIndex})" : "")}..{(endIndex != "" ? $"({endIndex})" : "")}";
        IndexDisplayText = $"{startIndex}..{endIndex}";
    }

    public string Index { get; }

    public string IndexDisplayText { get; }
}