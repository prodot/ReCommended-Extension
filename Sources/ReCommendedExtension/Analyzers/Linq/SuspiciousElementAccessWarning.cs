using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Linq;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Suspicious element access" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class SuspiciousElementAccessWarning(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression) : LinqHighlighting(message, invocationExpression, invokedExpression)
{
    const string SeverityId = "SuspiciousElementAccess";
}