using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Method;

[QuickFix]
public sealed class UseOtherMethodFix(UseOtherMethodSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var negation = highlighting.ReplacedMethodInvocation.Replacement.IsNegated ? "!" : "";
            var arguments = string.Join(", ", highlighting.ReplacedMethodInvocation.Replacement.Arguments);

            return $"Replace with '{negation}{highlighting.ReplacedMethodInvocation.Name}({arguments.TrimToSingleLineWithMaxLength(120)})'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.ReplacedMethodInvocation.Replacement.OriginalExpression);

            var negation = highlighting.ReplacedMethodInvocation.Replacement.IsNegated ? "!" : "";
            var arguments = string.Join(", ", highlighting.ReplacedMethodInvocation.Replacement.Arguments);

            ModificationUtil
                .ReplaceChild(
                    highlighting.ReplacedMethodInvocation.Replacement.OriginalExpression,
                    factory.CreateExpression($"({negation}$0.{highlighting.ReplacedMethodInvocation.Name}({arguments}))", highlighting.Qualifier))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}