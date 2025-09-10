using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class ReplaceNullableValueWithTypeCastFix(ReplaceNullableValueWithTypeCastSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });
            Debug.Assert(CSharpLanguage.Instance is { });

            var type = highlighting
                .ReferenceExpression.QualifierExpression.Type()
                .Unlift()
                .GetPresentableName(CSharpLanguage.Instance)
                .TrimToSingleLineWithMaxLength(120);
            var expression = highlighting.ReferenceExpression.QualifierExpression.GetText().TrimToSingleLineWithMaxLength(120);

            return $"Use '({type}){expression}'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });

            var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

            ModificationUtil
                .ReplaceChild(
                    highlighting.ReferenceExpression,
                    factory.CreateExpression(
                        "(($0)$1)",
                        highlighting.ReferenceExpression.QualifierExpression.Type().Unlift(),
                        highlighting.ReferenceExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}