using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class UseStringListPatternFix(UseStringListPatternSuggestion highlighting) : QuickFixBase
{
    string Replacement
    {
        get
        {
            Debug.Assert(highlighting.Arguments is [_, ..]);

            if (highlighting.Arguments.All(a => a.isConstant))
            {
                var pattern = string.Join(" or ", from a in highlighting.Arguments select a.valueArgument);

                return highlighting.Kind switch
                {
                    ListPatternSuggestionKind.FirstCharacter => $"is [{pattern}, ..]",
                    ListPatternSuggestionKind.NotFirstCharacter => $"is not [{pattern}, ..]",
                    ListPatternSuggestionKind.LastCharacter => $"is [.., {pattern}]",
                    ListPatternSuggestionKind.NotLastCharacter => $"is not [.., {pattern}]",

                    _ => throw new NotSupportedException(),
                };
            }

            if (highlighting.Arguments is [var (value, _)])
            {
                return highlighting.Kind switch
                {
                    ListPatternSuggestionKind.FirstCharacter => $"is [var firstChar, ..] && firstChar == {value}",
                    ListPatternSuggestionKind.NotFirstCharacter => $"is not [var firstChar, ..] || firstChar != {value}",
                    ListPatternSuggestionKind.LastCharacter => $"is [.., var lastChar] && lastChar == {value}",
                    ListPatternSuggestionKind.NotLastCharacter => $"is not [.., var lastChar] || lastChar != {value}",

                    _ => throw new NotSupportedException(),
                };
            }

            return highlighting.Kind switch
            {
                ListPatternSuggestionKind.FirstCharacter => $"is [var firstChar, ..] && ({
                    string.Join(" || ", from a in highlighting.Arguments select $"firstChar == {a.valueArgument}")
                })",

                ListPatternSuggestionKind.NotFirstCharacter => $"is not [var firstChar, ..] || {
                    string.Join(" && ", from a in highlighting.Arguments select $"firstChar != {a.valueArgument}")
                }",

                ListPatternSuggestionKind.LastCharacter => $"is [.., var lastChar] && ({
                    string.Join(" || ", from a in highlighting.Arguments select $"firstChar == {a.valueArgument}")
                })",

                ListPatternSuggestionKind.NotLastCharacter => $"is not [.., var lastChar] || {
                    string.Join(" && ", from a in highlighting.Arguments select $"lastChar != {a.valueArgument}")
                }",

                _ => throw new NotSupportedException(),
            };
        }
    }

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{Replacement.TrimToSingleLineWithMaxLength(120)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var expression = ModificationUtil
                .ReplaceChild(
                    highlighting.BinaryExpression as ITreeNode ?? highlighting.InvocationExpression,
                    factory.CreateExpression($"(($0) {Replacement})", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);

            var patternOperand = expression switch
            {
                IIsExpression isExpression => isExpression.Operand,
                IBinaryExpression { LeftOperand: IIsExpression isExpression } => isExpression.Operand,
                _ => null,
            };

            patternOperand?.TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}