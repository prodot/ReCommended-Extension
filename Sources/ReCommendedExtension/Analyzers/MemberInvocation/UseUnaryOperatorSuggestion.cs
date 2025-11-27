using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use unary operator" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseUnaryOperatorSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "UseUnaryOperator";

    public required IInvocationExpression InvocationExpression { get; init; }

    public required string Operand { get; init; }

    public required string Operator { get; init; }

    public override DocumentRange CalculateRange() => InvocationExpression.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(UseUnaryOperatorSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                var operand = highlighting.Operand.TrimToSingleLineWithMaxLength(120);

                return $"Replace with '{highlighting.Operator}{operand}'";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

                var expression = ModificationUtil
                    .ReplaceChild(
                        highlighting.InvocationExpression,
                        factory.CreateExpression($"({highlighting.Operator}({highlighting.Operand}))"))
                    .TryRemoveParentheses(factory);

                if (expression is IUnaryOperatorExpression unaryOperatorExpression)
                {
                    unaryOperatorExpression.Operand.TryRemoveParentheses(factory);
                }
            }

            return null;
        }
    }
}