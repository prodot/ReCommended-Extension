using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.ControlFlow;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Redundant assertion statement" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RedundantAssertionStatementSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "RedundantAssertionStatement";

    public required AssertionStatement Assertion { get; init; }

    public override DocumentRange CalculateRange() => Assertion.Statement.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(RedundantAssertionStatementSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove assertion";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var statement = ((AssertionStatement)highlighting.Assertion).Statement;

                ModificationUtil.DeleteChildRange(
                    statement,
                    statement.GetNextNonWhitespaceToken() is { } nextToken && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON
                        ? nextToken
                        : statement);
            }

            return null;
        }
    }
}