using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.AsyncVoid;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.CodeSmell, "Avoid 'async void'" + ZoneMarker.Suffix, "", Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class AvoidAsyncVoidWarning(string message, ITypeUsage typeUsage) : Highlighting(message)
{
    const string SeverityId = "AvoidAsyncVoid";

    public required ITypeOwnerDeclaration Declaration { get; init; }

    public override DocumentRange CalculateRange() => typeUsage.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(AvoidAsyncVoidWarning highlighting) : QuickFixBase
    {
        readonly IDeclaredType taskType = highlighting.Declaration.GetPredefinedType().Task;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => $"Change return type to '{taskType.GetClrName().ShortName}'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                highlighting.Declaration.SetType(taskType.ToIType());
            }

            return null;
        }
    }
}