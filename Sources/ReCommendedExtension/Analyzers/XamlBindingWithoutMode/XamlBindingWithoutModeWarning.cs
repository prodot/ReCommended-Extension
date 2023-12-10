using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace ReCommendedExtension.Analyzers.XamlBindingWithoutMode;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Binding expression without explicitly set 'Mode'" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, XamlLanguage.Name)]
public sealed class XamlBindingWithoutModeWarning(string message, IXmlIdentifier nameNode) : Highlighting(message)
{
    const string SeverityId = "XamlBindingWithoutMode";

    public override DocumentRange CalculateRange() => nameNode.GetDocumentRange();
}