using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Redundant invocation of the property change notifiers from the constructor" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class NotifyPropertyChangedInvocatorFromConstructorWarning(string message) : Highlighting(message)
{
    const string SeverityId = "NotifyPropertyChangedInvocatorFromConstructor";

    public required IInvocationExpression InvocationExpression { get; init; }

    public override DocumentRange CalculateRange() => InvocationExpression.GetHighlightingRange();

    [QuickFix]
    public sealed class Fix(NotifyPropertyChangedInvocatorFromConstructorWarning highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove invocation";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                ModificationUtil.DeleteChildRange(
                    highlighting.InvocationExpression,
                    highlighting.InvocationExpression.GetNextNonWhitespaceToken() is { } nextToken
                    && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON
                        ? nextToken
                        : highlighting.InvocationExpression);
            }

            return null;
        }
    }
}