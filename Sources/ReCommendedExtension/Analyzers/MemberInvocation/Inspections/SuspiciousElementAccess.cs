using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record SuspiciousElementAccess : Inspection
{
    public static SuspiciousElementAccess ByIndexWhenArg0IsDistinctCollection { get; } = new()
    {
        Condition =
            args => args is [{ Value: { } qualifier }, ..]
                && qualifier.Type() is var type
                && !IsIndexableCollectionOrString(type, qualifier)
                && IsDistinctCollection(type, qualifier),
        Message = _ => "The collection doesn't guarantee ordering, so retrieving the item by its index could result in unpredictable behavior.",
    };

    public static SuspiciousElementAccess FirstWhenArg0IsDistinctCollection { get; } = new()
    {
        Condition =
            args => args is [{ Value: { } qualifier }, ..]
                && qualifier.Type() is var type
                && !IsIndexableCollectionOrString(type, qualifier)
                && IsDistinctCollection(type, qualifier),
        Message = _ => """The collection doesn't guarantee ordering, so retrieving the "first" item could result in unpredictable behavior.""",
    };

    public static SuspiciousElementAccess LastWhenArg0IsDistinctCollection { get; } = new()
    {
        Condition =
            args => args is [{ Value: { } qualifier }, ..]
                && qualifier.Type() is var type
                && !IsIndexableCollectionOrString(type, qualifier)
                && IsDistinctCollection(type, qualifier),
        Message = _ => """The collection doesn't guarantee ordering, so retrieving the "last" item could result in unpredictable behavior.""",
    };

    public required Func<TreeNodeCollection<ICSharpArgument?>, bool> Condition { get; init; }
}