using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(AvoidAsyncVoidHighlighting.SeverityId, null, HighlightingGroupIds.CodeSmell, "Avoid 'async void'" + ZoneMarker.Suffix,
        "", Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class AvoidAsyncVoidHighlighting : Highlighting
    {
        internal const string SeverityId = "AvoidAsyncVoid";

        internal AvoidAsyncVoidHighlighting([NotNull] string message, [NotNull] IMethodDeclaration methodDeclaration) : base(message)
        {
            MethodDeclaration = methodDeclaration;
        }

        [NotNull]
        internal IMethodDeclaration MethodDeclaration { get; }

        public override DocumentRange CalculateRange() => MethodDeclaration.TypeUsage.GetDocumentRange();
    }
}