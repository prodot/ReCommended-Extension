using System;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Await
{
    [QuickFix]
    public sealed class RemoveAsyncAwaitFix : QuickFixBase
    {
        [NotNull]
        readonly RedundantAwaitHighlighting highlighting;

        public RemoveAsyncAwaitFix([NotNull] RedundantAwaitHighlighting highlighting) => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append("Remove 'async'/'await'");

                if (highlighting.QuickFixRemovesConfigureAwait)
                {
                    builder.Append($"/'{ClrMethodsNames.ConfigureAwait}(...)'");
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
                if (highlighting.AttributesOwnerDeclaration != null && !highlighting.AttributesOwnerDeclaration.OverridesInheritedMember())
                {
                    var codeAnnotationsConfiguration =
                        highlighting.AttributesOwnerDeclaration.GetPsiServices().GetComponent<CodeAnnotationsConfiguration>();

                    var attributeType = codeAnnotationsConfiguration.GetAttributeTypeForElement(
                        highlighting.AttributesOwnerDeclaration,
                        NullnessProvider.NotNullAttributeShortName);

                    if (attributeType != null)
                    {
                        var attribute = factory.CreateAttribute(attributeType);

                        highlighting.AttributesOwnerDeclaration.AddAttributeAfter(
                            attribute,
                            highlighting.AttributesOwnerDeclaration.AttributesEnumerable.LastOrDefault());
                    }
                }

                // remove 'async'
                highlighting.RemoveAsync();

                if (highlighting.StatementToBeReplacedWithReturnStatement != null)
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
}