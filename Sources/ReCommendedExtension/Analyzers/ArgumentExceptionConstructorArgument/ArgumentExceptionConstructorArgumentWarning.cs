using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Parameter name used for the exception message" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ArgumentExceptionConstructorArgumentWarning : Highlighting
{
    const string SeverityId = "ArgumentExceptionConstructorArgument";

    readonly ICSharpArgument argument;

    public ArgumentExceptionConstructorArgumentWarning(string message, ICSharpArgument argument) : base(message) => this.argument = argument;

    public override DocumentRange CalculateRange() => argument.GetDocumentRange();
}