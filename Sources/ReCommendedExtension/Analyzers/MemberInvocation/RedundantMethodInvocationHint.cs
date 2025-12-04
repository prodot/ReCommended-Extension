using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Method invocation is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantMethodInvocationHint(string message) : Highlighting(message)
{
    const string SeverityId = "RedundantMethodInvocation";

    public required IInvocationExpression InvocationExpression { get; init; }

    public required IReferenceExpression InvokedExpression { get; init; }

    [Pure]
    bool RemoveEntireInvocationExpression()
        => InvocationExpression.IsUsedAsStatement && InvokedExpression.QualifierExpression is IReferenceExpression;

    public override DocumentRange CalculateRange()
    {
        if (RemoveEntireInvocationExpression())
        {
            return InvocationExpression.GetDocumentRange();
        }

        var startOffset = InvokedExpression.Reference.GetDocumentRange().StartOffset;

        return InvocationExpression.GetDocumentRange().SetStartTo(startOffset);
    }

    [QuickFix]
    public sealed class Fix(RedundantMethodInvocationHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove method invocation";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                if (highlighting.RemoveEntireInvocationExpression())
                {
                    ModificationUtil.DeleteChildRange(
                        highlighting.InvocationExpression,
                        highlighting.InvocationExpression.GetNextNonWhitespaceToken() is { } nextToken
                        && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON
                            ? nextToken
                            : highlighting.InvocationExpression);
                }
                else
                {
                    var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

                    ModificationUtil
                        .ReplaceChild(
                            highlighting.InvocationExpression,
                            factory.CreateExpression("($0)", highlighting.InvokedExpression.QualifierExpression))
                        .TryRemoveParentheses(factory);
                }
            }

            return null;
        }
    }
}