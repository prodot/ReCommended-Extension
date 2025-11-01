using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record RangeIndexer : Inspection
{
    public static RangeIndexer FromStartToArg { get; } = new()
    {
        TryGetReplacement =
            args => args[0] is { Value: { } value } && value.TryGetInt32Constant() is null or > 0
                ? new RangeIndexerReplacement("", value.GetText())
                : null,
        Message = _ => "Use the range indexer.",
    };

    public static RangeIndexer FromArg1ToEndWhenArg0Zero { get; } = new()
    {
        TryGetReplacement =
            args => args[0]?.Value.TryGetInt32Constant() == 0 && args[1] is { Value: { } value }
                ? new RangeIndexerReplacement(value.GetText(), "")
                : null,
        Message = _ => "Use the range indexer.",
    };

    public CSharpLanguageLevel? MinimumLanguageVersion { get; init; }

    public required Func<TreeNodeCollection<ICSharpArgument?>, RangeIndexerReplacement?> TryGetReplacement { get; init; }
}