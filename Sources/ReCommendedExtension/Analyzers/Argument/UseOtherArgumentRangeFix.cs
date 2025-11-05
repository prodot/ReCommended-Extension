using System.Text;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument;

[QuickFix]
public sealed class UseOtherArgumentRangeFix(UseOtherArgumentRangeSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var builder = new StringBuilder();

            builder.Append(
                $"Replace with '{
                    string
                        .Join(", ", from argumentReplacement in highlighting.ArgumentReplacements select argumentReplacement.Replacement.Value)
                        .TrimToSingleLineWithMaxLength(120)
                }', respectively");

            if (highlighting.RedundantArgument is { })
            {
                builder.Append($" (and remove '{highlighting.RedundantArgument.Value?.GetText()}')");
            }

            return builder.ToString();
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            Debug.Assert(highlighting.ArgumentReplacements is [_, _, ..]);

            var factory = CSharpElementFactory.GetInstance(highlighting.ArgumentReplacements[0].Argument);

            foreach (var argumentReplacement in highlighting.ArgumentReplacements)
            {
                ModificationUtil.ReplaceChild(
                    argumentReplacement.Argument,
                    factory.CreateArgument(
                        argumentReplacement.Replacement.ParameterKind,
                        argumentReplacement.Replacement.ParameterName,
                        factory.CreateExpression(argumentReplacement.Replacement.Value)));
            }

            highlighting.RedundantArgument?.Remove();
        }

        return _ => { };
    }
}