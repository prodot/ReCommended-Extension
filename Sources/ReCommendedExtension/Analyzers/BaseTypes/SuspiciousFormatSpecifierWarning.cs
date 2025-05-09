using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Suspicious format specifier" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class SuspiciousFormatSpecifierWarning : Highlighting
{
    const string SeverityId = "SuspiciousFormatSpecifier";

    readonly ICSharpArgument? formatArgument;
    readonly IInterpolatedStringInsert? insert;

    public SuspiciousFormatSpecifierWarning(string message, ICSharpArgument formatArgument) : base(message) => this.formatArgument = formatArgument;

    public SuspiciousFormatSpecifierWarning(string message, IInterpolatedStringInsert insert) : base(message) => this.insert = insert;

    public override DocumentRange CalculateRange()
    {
        if (formatArgument is { })
        {
            return formatArgument.Value.GetDocumentRange();
        }

        if (insert is { })
        {
            return insert.FormatSpecifier.GetDocumentRange().ExtendLeft(-1);
        }

        throw new NotSupportedException();
    }
}