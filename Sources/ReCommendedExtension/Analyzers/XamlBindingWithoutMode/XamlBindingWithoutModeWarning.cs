using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace ReCommendedExtension.Analyzers.XamlBindingWithoutMode
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Binding expression without explicitly set 'Mode'" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, XamlLanguage.Name)]
    public sealed class XamlBindingWithoutModeWarning : Highlighting
    {
        const string SeverityId = "XamlBindingWithoutMode";

        [NotNull]
        readonly IXmlIdentifier nameNode;

        internal XamlBindingWithoutModeWarning([NotNull] string message, [NotNull] IXmlIdentifier nameNode) : base(message)
            => this.nameNode = nameNode;

        public override DocumentRange CalculateRange() => nameNode.GetDocumentRange();
    }
}