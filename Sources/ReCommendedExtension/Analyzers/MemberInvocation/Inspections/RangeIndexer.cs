using JetBrains.ReSharper.Psi;
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
            args => args is [{ Value: { AsInt32Constant: null or > 0 } value }] ? new RangeIndexerReplacement("", value.GetText()) : null,
        Message = _ => "Use the range indexer.",
    };

    public static RangeIndexer FromArg1ToEndWhenArg0Zero { get; } = new()
    {
        TryGetReplacement =
            args => args is [{ Value.AsInt32Constant: 0 }, { Value: { } value }] ? new RangeIndexerReplacement(value.GetText(), "") : null,
        Message = _ => "Use the range indexer.",
    };

    public static RangeIndexer FromArg1WhenArg0IsIndexableCollectionOrString { get; } = new()
    {
        TryGetReplacement = args
            => args is [{ Value: { } qualifier }, { Value: { } index }]
            && qualifier.Type() is var type
            && IsIndexableCollectionOrString(type, qualifier, out var hasAccessibleIndexer)
            && hasAccessibleIndexer
                ? new RangeIndexerReplacement(index.GetText()) { CanThrowOtherException = type.IsGenericArray() || type.IsString() }
                : null,
        Message = _ => "Use the indexer.",
    };

    public static RangeIndexer ZeroWhenArg0IsIndexableCollectionOrString { get; } = new()
    {
        TryGetReplacement = args
            => args is [{ Value: { } qualifier }]
            && qualifier.Type() is var type
            && IsIndexableCollectionOrString(type, qualifier, out var hasAccessibleIndexer)
            && hasAccessibleIndexer
                ? new RangeIndexerReplacement("0") { CanThrowOtherException = true }
                : null,
        Message = _ => "Use the indexer.",
    };

    public static RangeIndexer LastWhenArg0IsIndexableCollectionOrString { get; } = new()
    {
        TryGetReplacement = args
            => args is [{ Value: { } qualifier }]
            && qualifier.Type() is var type
            && IsIndexableCollectionOrString(type, qualifier, out var hasAccessibleIndexer)
            && hasAccessibleIndexer
                ? new RangeIndexerReplacement("^1") { CanThrowOtherException = true }
                : null,
        Message = _ => "Use the indexer.",
    };

    public CSharpLanguageLevel? MinimumLanguageVersion { get; init; }

    public required Func<TreeNodeCollection<ICSharpArgument?>, RangeIndexerReplacement?> TryGetReplacement { get; init; }

    public bool EnsureExtensionInvokedAsExtension { get; init; }

    public bool EnsureNoTypeArguments { get; init; }
}