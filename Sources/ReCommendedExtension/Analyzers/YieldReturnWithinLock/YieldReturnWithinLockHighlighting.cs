using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.YieldReturnWithinLock;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        YieldReturnWithinLockHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "'yield return' used inside the 'lock' block" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.YieldReturnWithinLock
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class YieldReturnWithinLockHighlighting : Highlighting
    {
        internal const string SeverityId = "YieldReturnWithinLock";

        [NotNull]
        readonly IYieldStatement yieldReturnStatement;

        internal YieldReturnWithinLockHighlighting([NotNull] string message, [NotNull] IYieldStatement yieldReturnStatement) : base(message)
            => this.yieldReturnStatement = yieldReturnStatement;

        public override DocumentRange CalculateRange()
            => yieldReturnStatement.YieldKeyword.GetDocumentRange().JoinRight(yieldReturnStatement.ReturnKeyword.GetDocumentRange());
    }
}