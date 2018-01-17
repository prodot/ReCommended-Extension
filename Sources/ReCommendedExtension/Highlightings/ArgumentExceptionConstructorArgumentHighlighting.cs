using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(ArgumentExceptionConstructorArgumentHighlighting.SeverityId, null, HighlightingGroupIds.CodeSmell,
        "Redundant invocation of the property change notifiers from the constructor" + ZoneMarker.Suffix, "", Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ArgumentExceptionConstructorArgumentHighlighting : Highlighting
    {
        internal const string SeverityId = "ArgumentExceptionConstructorArgument";

        [NotNull]
        readonly ICSharpArgument argument;

        public ArgumentExceptionConstructorArgumentHighlighting([NotNull] string message, [NotNull] ICSharpArgument argument) : base(message)
            => this.argument = argument;

        public override DocumentRange CalculateRange() => argument.GetDocumentRange();
    }
}