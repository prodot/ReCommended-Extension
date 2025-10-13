using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record OtherArgument : Inspection
{
    [Pure]
    static bool IsCollectionCreationWithOnlyInvariantConstants(ICSharpArgument arg, Func<string, bool> isInvariantConstant)
    {
        if (CollectionCreation.TryFrom(arg.Value) is { Count: > 0 } collectionCreation)
        {
            foreach (var s in collectionCreation.AllElementsAsStringConstants)
            {
                if (s == null || !isInvariantConstant(s))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public static OtherArgument NullFormatProviderForInvariantDateTimeOnlyFormat { get; } = new()
    {
        TryGetReplacement = arg => arg.Value is { } && !arg.Value.IsDefaultValue() ? "null" : null,
        FurtherArgumentCondition = new ArgumentCondition { Condition = arg => arg.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" },
        Message = "The format provider is ignored (pass null instead).",
    };

    public static OtherArgument NullFormatProviderForInvariantDateTimeOnlyFormatCollection { get; } = new()
    {
        TryGetReplacement = arg => arg.Value is { } && !arg.Value.IsDefaultValue() ? "null" : null,
        FurtherArgumentCondition = new ArgumentCondition
        {
            Condition = arg => IsCollectionCreationWithOnlyInvariantConstants(arg, s => s is "o" or "O" or "r" or "R"),
        },
        Message = "The format provider is ignored (pass null instead).",
    };

    public static OtherArgument NullFormatProviderForInvariantDateTimeFormat { get; } = new()
    {
        TryGetReplacement = arg => arg.Value is { } && !arg.Value.IsDefaultValue() ? "null" : null,
        FurtherArgumentCondition =
            new ArgumentCondition { Condition = arg => arg.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" or "s" or "u" },
        Message = "The format provider is ignored (pass null instead).",
    };

    public static OtherArgument NullFormatProviderForInvariantDateTimeFormatCollection { get; } = new()
    {
        TryGetReplacement = arg => arg.Value is { } && !arg.Value.IsDefaultValue() ? "null" : null,
        FurtherArgumentCondition = new ArgumentCondition
        {
            Condition = arg => IsCollectionCreationWithOnlyInvariantConstants(arg, s => s is "o" or "O" or "r" or "R" or "s" or "u"),
        },
        Message = "The format provider is ignored (pass null instead).",
    };

    public static OtherArgument NullFormatProviderForInvariantTimeSpanFormat { get; } = new()
    {
        TryGetReplacement = arg => arg.Value is { } && !arg.Value.IsDefaultValue() ? "null" : null,
        FurtherArgumentCondition = new ArgumentCondition { Condition = arg => arg.Value.TryGetStringConstant() is "c" or "t" or "T" },
        Message = "The format provider is ignored (pass null instead).",
    };

    public static OtherArgument SingleCollectionElement { get; } = new()
    {
        TryGetReplacement = arg => CollectionCreation.TryFrom(arg.Value) is { Count: 1 } collectionCreation
            ? collectionCreation.SingleElement.GetText()
            : null,
        Message = "The only collection element should be passed directly.",
    };

    public static OtherArgument Char { get; } = new()
    {
        TryGetReplacement = arg => arg.Value.TryGetStringConstant() is [var character] ? $"'{character.ToString()}'" : null,
        Message = "The only character should be passed directly.",
    };

    public static OtherArgument CharWithCurrentCulture { get; } = new()
    {
        TryGetReplacement = arg => arg.Value.TryGetStringConstant() is [var character] ? $"'{character.ToString()}'" : null,
        Message = "The only character should be passed directly.",
        AdditionalArgument = $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}",
    };

    public static OtherArgument CharForStringComparisonOrdinal { get; } = new()
    {
        TryGetReplacement = arg => arg.Value.TryGetStringConstant() is [var character] ? $"'{character.ToString()}'" : null,
        FurtherArgumentCondition = new ArgumentCondition
        {
            Condition = arg => arg.Value.TryGetStringComparisonConstant() == StringComparison.Ordinal,
        },
        Message = "The only character should be passed directly.",
    };

    public static OtherArgument CharForOne { get; } = new()
    {
        TryGetReplacement = arg => arg.Value.TryGetStringConstant() is [var character] ? $"'{character.ToString()}'" : null,
        FurtherArgumentCondition = new ArgumentCondition { Condition = arg => arg.Value.TryGetInt32Constant() == 1 },
        Message = "The only character should be passed directly.",
    };

    public int ParameterIndex { get; init; } = -1;

    public required Func<ICSharpArgument, string?> TryGetReplacement { get; init; }

    public ArgumentCondition? FurtherArgumentCondition { get; init; }

    public ReplacementSignature? ReplacementSignature { get; init; }

    public string? AdditionalArgument { get; private init; }

    public int? RedundantArgumentIndex { get; init; }
}