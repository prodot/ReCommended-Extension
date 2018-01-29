using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        XamlBindingWithoutModeHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Binding expression without explicitly set 'Mode'" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, XamlLanguage.Name)]
    public sealed class XamlBindingWithoutModeHighlighting : Highlighting
    {
        internal const string SeverityId = "XamlBindingWithoutMode";

        [NotNull]
        readonly IXmlIdentifier nameNode;

        internal XamlBindingWithoutModeHighlighting([NotNull] string message, [NotNull] IXmlIdentifier nameNode) : base(message)
            => this.nameNode = nameNode;

        public override DocumentRange CalculateRange() => nameNode.GetDocumentRange();
    }
}