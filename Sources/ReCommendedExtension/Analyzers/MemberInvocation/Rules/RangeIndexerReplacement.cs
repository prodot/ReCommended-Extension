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

    internal string Index { get; }

    internal string IndexDisplayText { get; }

    internal bool CanThrowOtherException { get; init; }
}