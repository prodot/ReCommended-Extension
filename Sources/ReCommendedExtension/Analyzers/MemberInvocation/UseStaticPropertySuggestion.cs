using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use the suggested static property" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseStaticPropertySuggestion(string message, IReferenceExpressionReference invokingReference) : Highlighting(message)
{
    const string SeverityId = "UseStaticProperty";

    public required ICSharpExpression? QualifierExpression { get; init; }

    public required IReferenceExpression ReferenceExpression { get; init; }

    public required string PropertyName { get; init; }

    public override DocumentRange CalculateRange()
        => ReferenceExpression.GetDocumentRange().SetStartTo(invokingReference.GetDocumentRange().StartOffset);

    [QuickFix]
    public sealed class Fix(UseStaticPropertySuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => highlighting.ReferenceExpression.Parent is { };

        public override string Text => $"Replace with '{highlighting.PropertyName}'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

                Debug.Assert(highlighting.ReferenceExpression.Parent is { });

                ModificationUtil.ReplaceChild(
                    highlighting.ReferenceExpression.Parent,
                    highlighting.QualifierExpression is { }
                        ? factory.CreateExpression($"$0.{highlighting.PropertyName}", highlighting.QualifierExpression)
                        : factory.CreateExpression(highlighting.PropertyName));
            }

            return null;
        }
    }
}