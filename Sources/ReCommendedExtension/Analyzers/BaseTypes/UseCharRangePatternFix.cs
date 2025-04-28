using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class UseCharRangePatternFix(UseCharRangePatternSuggestion highlighting) : QuickFixBase
{
    string Pattern => string.Join(" or ", from range in highlighting.Ranges select $">= {range.From} and <= {range.To}");

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => $"Replace with '{highlighting.Expression.GetText().TrimToSingleLineWithMaxLength(120)} is {Pattern.TrimToSingleLineWithMaxLength(120)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var expression = ModificationUtil
                .ReplaceChild(highlighting.InvocationExpression, factory.CreateExpression($"(($0) is {Pattern})", highlighting.Expression))
                .TryRemoveParentheses(factory);

            if (expression is IIsExpression isExpression)
            {
                isExpression.Operand.TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}