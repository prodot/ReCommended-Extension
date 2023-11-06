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
public sealed record XamlBindingWithoutModeWarning : Highlighting
{
    const string SeverityId = "XamlBindingWithoutMode";

    readonly IXmlIdentifier nameNode;

    internal XamlBindingWithoutModeWarning(string message, IXmlIdentifier nameNode) : base(message) => this.nameNode = nameNode;

    public override DocumentRange CalculateRange() => nameNode.GetDocumentRange();
}