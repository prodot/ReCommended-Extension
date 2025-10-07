using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Argument;

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
public sealed class RedundantArgumentRangeHint(string message, IReadOnlyList<ICSharpArgument> arguments) : Highlighting(message)
{
    const string SeverityId = "RedundantArgumentRange";

    internal IReadOnlyList<ICSharpArgument> Arguments => arguments;

    public override DocumentRange CalculateRange() => arguments[0].GetDocumentRange().Join(arguments[^1].GetDocumentRange());
}