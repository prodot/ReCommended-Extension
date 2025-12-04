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
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use binary operator" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseBinaryOperatorSuggestion(string message, IReferenceExpression? invokedExpression) : Highlighting(message)
{
    const string SeverityId = "UseBinaryOperator";

    public required IInvocationExpression InvocationExpression { get; init; }

    public required BinaryOperatorOperands Operands { get; init; }

    public required string Operator { get; init; }

    public override DocumentRange CalculateRange()
    {
        var documentRange = InvocationExpression.GetDocumentRange();

        if (invokedExpression is { })
        {
            return documentRange.SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset);
        }

        return documentRange;
    }

    [QuickFix]
    public sealed class Fix(UseBinaryOperatorSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                var leftOperand = highlighting.Operands.Left.TrimToSingleLineWithMaxLength(120);
                var rightOperand = highlighting.Operands.Right.TrimToSingleLineWithMaxLength(120);

                return $"Replace with '{leftOperand} {highlighting.Operator} {rightOperand}'";
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
                        factory.CreateExpression($"(({highlighting.Operands.Left}) {highlighting.Operator} ({highlighting.Operands.Right}))"))
                    .TryRemoveParentheses(factory);

                if (expression is IBinaryExpression binaryExpression)
                {
                    binaryExpression.LeftOperand.TryRemoveParentheses(factory);
                    binaryExpression.RightOperand.TryRemoveParentheses(factory);
                }
            }

            return null;
        }
    }
}