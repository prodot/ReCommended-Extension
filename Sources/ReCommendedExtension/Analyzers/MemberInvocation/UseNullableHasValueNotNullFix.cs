using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[QuickFix]
public sealed class UseNullableHasValueNotNullFix(UseNullableHasValueAlternativeSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });

            var expression = highlighting.ReferenceExpression.QualifierExpression.GetText().TrimToSingleLineWithMaxLength(120);

            return $"Replace with '{expression} != null'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

            ModificationUtil
                .ReplaceChild(
                    highlighting.ReferenceExpression,
                    factory.CreateExpression("($0 != null)", highlighting.ReferenceExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}