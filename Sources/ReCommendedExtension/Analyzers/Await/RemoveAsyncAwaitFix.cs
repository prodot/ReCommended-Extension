using System.Text;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Await;

[Obsolete] // todo: remove
[QuickFix]
public sealed class RemoveAsyncAwaitFix : QuickFixBase
{
    readonly RedundantAwaitSuggestion highlighting;

    public RemoveAsyncAwaitFix(RedundantAwaitSuggestion highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var builder = new StringBuilder();

            builder.Append("Remove 'async'/'await'");

            if (highlighting.ConfigureAwaitArgument is { })
            {
                builder.Append("/'");
                builder.Append(nameof(Task.ConfigureAwait));
                builder.Append('(');
                builder.Append(highlighting.ConfigureAwaitArgument);
                builder.Append(")'");
            }

            return builder.ToString();
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.AwaitExpression);

            // add [NotNull] annotation
            if (highlighting.AttributesOwnerDeclaration is { }
                && !highlighting.AttributesOwnerDeclaration.IsNullableAnnotationsContextEnabled()
                && !highlighting.AttributesOwnerDeclaration.OverridesInheritedMember()
                && highlighting.AttributesOwnerDeclaration.Attributes.All(
                    a => a.GetAttributeType().GetClrName().ShortName != NullnessProvider.NotNullAttributeShortName))
            {
                var codeAnnotationsConfiguration =
                    highlighting.AttributesOwnerDeclaration.GetPsiServices().GetComponent<CodeAnnotationsConfiguration>();

                var attributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                    highlighting.AttributesOwnerDeclaration,
                    NullnessProvider.NotNullAttributeShortName);
                if (attributeType is { })
                {
                    var attribute = factory.CreateAttribute(attributeType);

                    highlighting.AttributesOwnerDeclaration.AddAttributeAfter(
                        attribute,
                        highlighting.AttributesOwnerDeclaration.Attributes.LastOrDefault());
                }
            }

            // remove 'async'
            highlighting.RemoveAsync();

            if (highlighting.StatementToBeReplacedWithReturnStatement is { })
            {
                // replace 'await' with 'return' (and remove 'ConfigureAwait' if available)
                ModificationUtil.ReplaceChild(
                    highlighting.StatementToBeReplacedWithReturnStatement,
                    factory.CreateStatement("return $0;", highlighting.ExpressionToReturn));
            }
            else
            {
                // remove 'await' (and 'ConfigureAwait' if available)
                ModificationUtil.ReplaceChild(highlighting.AwaitExpression, factory.CreateExpression("$0", highlighting.ExpressionToReturn));
            }
        }

        return _ => { };
    }
}