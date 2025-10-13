using System.Text;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument;

[QuickFix]
public sealed class UseOtherArgumentFix(UseOtherArgumentSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var builder = new StringBuilder();

            builder.Append($"Replace with '{highlighting.Replacement}'");

            if (highlighting is { AdditionalArgument: { } } or { RedundantArgument : { } })
            {
                builder.Append(" (and ");

                if (highlighting.AdditionalArgument is { })
                {
                    builder.Append($"add '{highlighting.AdditionalArgument}'");
                }

                if (highlighting is { AdditionalArgument: { }, RedundantArgument: { } })
                {
                    builder.Append(", ");
                }

                if (highlighting.RedundantArgument is { })
                {
                    builder.Append($"remove '{highlighting.RedundantArgument.Value?.GetText()}'");
                }

                builder.Append(')');
            }

            return builder.ToString();
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Argument);

            var argument = ModificationUtil.ReplaceChild(
                highlighting.Argument,
                factory.CreateArgument(highlighting.ParameterKind, highlighting.ParameterName, factory.CreateExpression(highlighting.Replacement)));

            highlighting.RedundantArgument?.Remove();

            if (highlighting.AdditionalArgument is { })
            {
                var comma = ModificationUtil.AddChildAfter(argument, CSharpTokenType.COMMA.CreateTreeElement());

                ModificationUtil.AddChildAfter(
                    comma,
                    factory.CreateArgument(
                        ParameterKind.VALUE,
                        highlighting.AdditionalArgumentParameterName,
                        factory.CreateExpression(highlighting.AdditionalArgument)));
            }
        }

        return _ => { };
    }
}