using System.Text;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Collection;

[QuickFix]
public sealed class ReplaceWithCollectionExpressionFix(UseTargetTypedCollectionExpressionSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var builder = new StringBuilder();

            builder.Append("Replace with '[");

            if (highlighting is { SpreadItem: { } } or { Items: [_, ..] })
            {
                builder.Append("...");
            }

            builder.Append("]'");

            if (highlighting.OtherTypeNameHint is { })
            {
                builder.Append(" (");
                builder.Append(highlighting.OtherTypeNameHint);
                builder.Append(')');
            }

            return builder.ToString();
        }
    }

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

            if (highlighting.MethodReferenceToSetInferredTypeArguments is { })
            {
                // set inferred type arguments

                var (declaredElement, substitution) = highlighting.MethodReferenceToSetInferredTypeArguments.Reference.Resolve().Result;

                if (declaredElement is ITypeParametersOwner typeParametersOwner)
                {
                    highlighting.MethodReferenceToSetInferredTypeArguments.SetTypeArguments(
                        [..from typeParameter in typeParametersOwner.TypeParameters select substitution[typeParameter]]);
                }
            }

            var items = from item in highlighting.Items select item.GetText();

            if (highlighting.SpreadItem is { })
            {
                items = items.Prepend($"..{highlighting.SpreadItem.GetText()}");
            }

            ModificationUtil.ReplaceChild(highlighting.Expression, factory.CreateExpression($"[{string.Join(", ", items)}]"));
        }

        return null;
    }
}