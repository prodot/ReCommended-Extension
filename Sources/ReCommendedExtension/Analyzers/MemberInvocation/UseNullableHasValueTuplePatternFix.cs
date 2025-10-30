using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[QuickFix]
public sealed class UseNullableHasValueTuplePatternFix(UseNullableHasValueAlternativeSuggestion highlighting) : QuickFixBase
{
    [NonNegativeValue]
    int tupleLength;

    public override bool IsAvailable(IUserDataHolder cache)
    {
        Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });

        return highlighting.ReferenceExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80
            && highlighting.ReferenceExpression.QualifierExpression.Type().Unlift().IsValueTuple(out tupleLength)
            && tupleLength >= 2;
    }

    public override string Text
    {
        get
        {
            Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });
            Debug.Assert(tupleLength >= 2);

            var expression = highlighting.ReferenceExpression.QualifierExpression.GetText().TrimToSingleLineWithMaxLength(120);
            var pattern = $"({string.Join(", ", Enumerable.Repeat('_', tupleLength))})";

            return $"Replace with '{expression} is {pattern}'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

            var pattern = $"({string.Join(", ", Enumerable.Repeat('_', tupleLength))})";

            ModificationUtil
                .ReplaceChild(
                    highlighting.ReferenceExpression,
                    factory.CreateExpression($"($0 is {pattern})", highlighting.ReferenceExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}