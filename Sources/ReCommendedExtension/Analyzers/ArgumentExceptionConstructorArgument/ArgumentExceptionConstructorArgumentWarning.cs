using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Parameter name used for the exception message" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ArgumentExceptionConstructorArgumentWarning : Highlighting
    {
        const string SeverityId = "ArgumentExceptionConstructorArgument";

        [NotNull]
        readonly ICSharpArgument argument;

        public ArgumentExceptionConstructorArgumentWarning([NotNull] string message, [NotNull] ICSharpArgument argument) : base(message)
            => this.argument = argument;

        public override DocumentRange CalculateRange() => argument.GetDocumentRange();
    }
}