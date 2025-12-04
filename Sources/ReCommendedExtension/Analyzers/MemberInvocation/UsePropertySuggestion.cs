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
using ReCommendedExtension.Analyzers.MemberInvocation.Inspections;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use the suggested property" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UsePropertySuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "UseProperty";

    public required IInvocationExpression InvocationExpression { get; init; }

    public required IReferenceExpression InvokedExpression { get; init; }

    public required string PropertyName { get; init; }

    internal TargetType? EnsureTargetType { get; init; }

    public override DocumentRange CalculateRange()
        => InvocationExpression.GetDocumentRange().SetStartTo(InvokedExpression.Reference.GetDocumentRange().StartOffset);

    [QuickFix]
    public sealed class Fix(UsePropertySuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => $"Replace with '{highlighting.PropertyName}'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

                var conditionalAccess = highlighting.InvokedExpression.HasConditionalAccessSign ? "?" : "";

                var castType = highlighting.EnsureTargetType is { } targetType
                    ? highlighting.InvocationExpression.TryGetTargetType(false) is var type && highlighting.InvokedExpression.HasConditionalAccessSign
                        ? targetType.IsNullableType(type) ? null : targetType.Name
                        : targetType.IsType(type) || targetType.IsNullableType(type)
                            ? null
                            : targetType.Name
                    : null;

                ModificationUtil.ReplaceChild(
                    highlighting.InvocationExpression,
                    castType is { }
                        ? factory.CreateExpression(
                            $"({castType}{conditionalAccess})$0{conditionalAccess}.{highlighting.PropertyName}",
                            highlighting.InvokedExpression.QualifierExpression)
                        : factory.CreateExpression(
                            $"$0{conditionalAccess}.{highlighting.PropertyName}",
                            highlighting.InvokedExpression.QualifierExpression));
            }

            return null;
        }
    }
}