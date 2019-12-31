using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.UnthrowableException;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        UnthrowableExceptionWarning.SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Exception should never be thrown" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.UnthrowableException
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class UnthrowableExceptionWarning : Highlighting
    {
        internal const string SeverityId = "UnthrowableException";

        [NotNull]
        readonly ICSharpExpression thrownStatementExpression;

        internal UnthrowableExceptionWarning([NotNull] string message, [NotNull] ICSharpExpression thrownStatementExpression) : base(message)
            => this.thrownStatementExpression = thrownStatementExpression;

        public override DocumentRange CalculateRange() => thrownStatementExpression.GetDocumentRange();
    }
}