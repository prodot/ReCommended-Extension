using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record OtherArgumentRange : Inspection
{
    public static OtherArgumentRange Chars { get; } = new()
    {
        TryGetReplacements = args
            => args is [{ Value.AsStringConstant: [var c0] } arg0, { Value.AsStringConstant: [var c1] } arg1, ..]
                ? [c0.ToLiteralString(arg0.GetCSharpLanguageLevel()), c1.ToLiteralString(arg1.GetCSharpLanguageLevel())]
                : null,
        Message = "The only character should be passed directly.",
    };

    public static OtherArgumentRange CharsForStringComparisonOrdinal { get; } = new()
    {
        TryGetReplacements =
            args => args is [{ Value.AsStringConstant: [var c0] } arg0, { Value.AsStringConstant: [var c1] } arg1, ..]
                ? [c0.ToLiteralString(arg0.GetCSharpLanguageLevel()), c1.ToLiteralString(arg1.GetCSharpLanguageLevel())]
                : null,
        FurtherArgumentCondition = new ArgumentCondition { Condition = arg => arg.Value.AsStringComparisonConstant == StringComparison.Ordinal },
        Message = "The only character should be passed directly.",
    };

    public Range ParameterIndexRange { get; init; } = ..;

    public required Func<IReadOnlyList<ICSharpArgument>, IReadOnlyList<string>?> TryGetReplacements { get; init; }

    public ArgumentCondition? FurtherArgumentCondition { get; init; }

    public ReplacementSignatureRange? ReplacementSignature { get; init; }

    public int? RedundantArgumentIndex { get; init; }
}