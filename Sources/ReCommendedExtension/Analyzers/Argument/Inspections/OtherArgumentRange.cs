using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record OtherArgumentRange : Inspection
{
    public static OtherArgumentRange Chars { get; } = new()
    {
        TryGetReplacements = args
            => args[0].Value.TryGetStringConstant() is [var c0] && args[1].Value.TryGetStringConstant() is [var c1]
                ? [c0.ToLiteralString(args[0].GetCSharpLanguageLevel()), c1.ToLiteralString(args[1].GetCSharpLanguageLevel())]
                : null,
        Message = "The only character should be passed directly.",
    };

    public static OtherArgumentRange CharsForStringComparisonOrdinal { get; } = new()
    {
        TryGetReplacements =
            args => args[0].Value.TryGetStringConstant() is [var c0] && args[1].Value.TryGetStringConstant() is [var c1]
                ? [c0.ToLiteralString(args[0].GetCSharpLanguageLevel()), c1.ToLiteralString(args[1].GetCSharpLanguageLevel())]
                : null,
        FurtherArgumentCondition = new ArgumentCondition
        {
            Condition = arg => arg.Value.TryGetStringComparisonConstant() == StringComparison.Ordinal,
        },
        Message = "The only character should be passed directly.",
    };

    public Range ParameterIndexRange { get; init; } = ..;

    public required Func<IReadOnlyList<ICSharpArgument>, IReadOnlyList<string>?> TryGetReplacements { get; init; }

    public ArgumentCondition? FurtherArgumentCondition { get; init; }

    public ReplacementSignatureRange? ReplacementSignature { get; init; }

    public int? RedundantArgumentIndex { get; init; }
}