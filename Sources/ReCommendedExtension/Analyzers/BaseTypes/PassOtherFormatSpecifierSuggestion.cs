using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass other format specifier" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassOtherFormatSpecifierSuggestion : Highlighting
{
    const string SeverityId = "PassOtherFormatSpecifier";

    public PassOtherFormatSpecifierSuggestion(string message, ICSharpArgument formatArgument, string replacement) : base(message)
    {
        FormatArgument = formatArgument;
        Replacement = replacement;
    }

    public PassOtherFormatSpecifierSuggestion(string message, IInterpolatedStringInsert insert, string replacement) : base(message)
    {
        Insert = insert;
        Replacement = replacement;
    }

    internal ICSharpArgument? FormatArgument { get; }

    internal IInterpolatedStringInsert? Insert { get; }

    internal string Replacement { get; }

    public override DocumentRange CalculateRange()
    {
        if (FormatArgument is { })
        {
            return FormatArgument.Value.GetDocumentRange();
        }

        if (Insert is { })
        {
            return Insert.FormatSpecifier.GetDocumentRange().ExtendLeft(-1);
        }

        throw new NotSupportedException();
    }
}