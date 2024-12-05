using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Strings;

[QuickFix]
public sealed class UseRangeIndexerFix(UseRangeIndexerSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => $"Replace with '[{highlighting.StartIndexArgument.TrimToSingleLineWithMaxLength(120)}..{highlighting.EndIndexArgument.TrimToSingleLineWithMaxLength(120)}]'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var conditionalAccess = highlighting.InvokedExpression.HasConditionalAccessSign ? "?" : "";

            var startIndex = highlighting.StartIndexArgument != "" ? $"({highlighting.StartIndexArgument})" : "";
            var endIndex = highlighting.EndIndexArgument != "" ? $"({highlighting.EndIndexArgument})" : "";

            ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"$0{conditionalAccess}[{startIndex}..{endIndex}]", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveRangeIndexParentheses(factory);
        }

        return _ => { };
    }
}