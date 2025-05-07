namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

public sealed record NumberInfo<N> : NumberInfo where N : struct
{
    public required TryGetConstant<N> TryGetConstant { get; init; }

    public required Func<N, N, bool> AreEqual { get; init; }

    public required Func<N, N, bool> AreMinMaxValues { get; init; }

    internal Func<N, bool>? IsZero { get; init; }
}