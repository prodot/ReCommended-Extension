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

namespace ReCommendedExtension.Analyzers.ExpressionResult;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use expression result" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseExpressionResultSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "UseExpressionResult";

    public required ICSharpTreeNode Expression { get; init; }

    public required ExpressionResultReplacements Replacements { get; init; }

    public override DocumentRange CalculateRange() => Expression.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(UseExpressionResultSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => $"Replace with '{highlighting.Replacements.Main.TrimToSingleLineWithMaxLength(120)}'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

                var expression = ModificationUtil
                    .ReplaceChild(highlighting.Expression, factory.CreateExpression($"({highlighting.Replacements.Main})"))
                    .TryRemoveParentheses(factory);

                if (expression is IUnaryOperatorExpression unaryOperatorExpression)
                {
                    unaryOperatorExpression.Operand.TryRemoveParentheses(factory);
                }
            }

            return null;
        }
    }

    [QuickFix]
    public sealed class AlternativeFix(UseExpressionResultSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => highlighting.Replacements.Alternative is { };

        public override string Text
        {
            get
            {
                Debug.Assert(highlighting.Replacements.Alternative is { });

                return $"Replace with '{highlighting.Replacements.Alternative.TrimToSingleLineWithMaxLength(120)}'";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

                var expression = ModificationUtil
                    .ReplaceChild(highlighting.Expression, factory.CreateExpression($"({highlighting.Replacements.Alternative})"))
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