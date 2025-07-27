using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The argument range is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantArgumentRangeHint(string message, ICSharpArgument firstArgument, ICSharpArgument lastArgument) : Highlighting(message)
{
    const string SeverityId = "RedundantArgumentRange";

    internal ICSharpArgument FirstArgument => firstArgument;

    internal ICSharpArgument LastArgument => lastArgument;

    public override DocumentRange CalculateRange() => firstArgument.GetDocumentRange().Join(lastArgument.GetDocumentRange());
}