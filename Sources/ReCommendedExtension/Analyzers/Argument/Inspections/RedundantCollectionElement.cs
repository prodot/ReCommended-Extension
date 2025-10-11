using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal abstract record RedundantCollectionElement : Inspection
{
    const byte nonEquivalenceGroupId = 0xFF;

    /// <param name="collectionCreation"></param>
    /// <param name="getEquivalentGroupId">
    ///     The callback must return a valid group id (0 to 7) for strings that belong to an equivalence group or the
    ///     <see cref="nonEquivalenceGroupId"/> for strings that are not part of an equivalence group.
    /// </param>
    [Pure]
    static IEnumerable<(IInitializerElement, bool isEquivalent)> Iterate(
        CollectionCreation collectionCreation,
        Func<string, byte> getEquivalentGroupId)
    {
        var groupsSeenMask = (byte)0; // bit n set => group n already encountered
        var set = null as HashSet<string>;

        foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
        {
            var groupId = getEquivalentGroupId(s);

            if (groupId != nonEquivalenceGroupId)
            {
                Debug.Assert(groupId <= 7);

                var bit = unchecked((byte)(1 << groupId));
                if ((groupsSeenMask & bit) != 0)
                {
                    yield return (element, true);
                }
                else
                {
                    groupsSeenMask |= bit;
                }
            }
            else
            {
                set ??= new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                if (!set.Add(s))
                {
                    yield return (element, false);
                }
            }
        }
    }

    [Pure]
    static IEnumerable<IInitializerElement> Iterate<T>(IEnumerable<(IInitializerElement, T)> sequence, HashSet<T> set)
    {
        foreach (var (element, item) in sequence)
        {
            if (!set.Add(item))
            {
                yield return element;
            }
        }
    }

    public static DuplicateCollectionElement Char { get; } = new()
    {
        Selector = collectionCreation => collectionCreation.Count > 1
            ? Iterate(collectionCreation.ElementsWithCharConstants, new HashSet<char>(collectionCreation.Count))
            : [],
        Message = "The character is already passed.",
    };

    public static DuplicateCollectionElement String { get; } = new()
    {
        Selector = collectionCreation => collectionCreation.Count > 1
            ? Iterate(collectionCreation.ElementsWithStringConstants, new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal))
            : [],
        Message = "The string is already passed.",
    };

    public static DuplicateEquivalentCollectionElement StringDateTimeFormats { get; } = new()
    {
        Selector = collectionCreation => collectionCreation.Count > 1
            ? Iterate(
                collectionCreation,
                s => s switch
                {
                    "o" or "O" => 0,
                    "r" or "R" => 1,
                    "m" or "M" => 2,
                    "y" or "Y" => 3,
                    _ => nonEquivalenceGroupId,
                })
            : [],
        Message = "The string is already passed.",
        MessageEquivalentElement = "The equivalent string is already passed.",
    };

    public static DuplicateEquivalentCollectionElement StringTimeOnlyFormats { get; } = new()
    {
        Selector = collectionCreation => collectionCreation.Count > 1
            ? Iterate(
                collectionCreation,
                s => s switch
                {
                    "o" or "O" => 0,
                    "r" or "R" => 1,
                    _ => nonEquivalenceGroupId,
                })
            : [],
        Message = "The string is already passed.",
        MessageEquivalentElement = "The equivalent string is already passed.",
    };

    public static DuplicateEquivalentCollectionElement StringTimeSpanFormats { get; } = new()
    {
        Selector =
            collectionCreation => collectionCreation.Count > 1
                ? Iterate(collectionCreation, s => s is "c" or "t" or "T" ? (byte)0 : nonEquivalenceGroupId)
                : [],
        Message = "The string is already passed.",
        MessageEquivalentElement = "The equivalent string is already passed.",
    };

    public int ParameterIndex { get; init; } = -1;
}