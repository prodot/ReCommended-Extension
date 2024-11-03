using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Strings;

[QuickFix]
public sealed class UseOtherMethodFix(UseOtherMethodSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var negation = highlighting.IsNegated ? "!" : "";
            var arguments = string.Join(", ", highlighting.Arguments);

            return $"Replace with '{negation}{highlighting.OtherMethodName}({arguments.TrimToSingleLineWithMaxLength(120)})'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var negation = highlighting.IsNegated ? "!" : "";
            var arguments = string.Join(", ", highlighting.Arguments);

            ModificationUtil
                .ReplaceChild(
                    highlighting.BinaryExpression as ITreeNode ?? highlighting.InvocationExpression,
                    factory.CreateExpression(
                        $"({negation}$0.{highlighting.OtherMethodName}({arguments}))",
                        highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}