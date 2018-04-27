using System;
using System.Diagnostics;
using JetBrains.Annotations;
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
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.QuickFixes
{
    [QuickFix]
    public sealed class ReplaceWithNewArrayWithLengthFix : QuickFixBase
    {
        [NotNull]
        readonly ArrayWithDefaultValuesInitializationHighlighting highlighting;

        public ReplaceWithNewArrayWithLengthFix([NotNull] ArrayWithDefaultValuesInitializationHighlighting highlighting)
            => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                Debug.Assert(CSharpLanguage.Instance != null);

                return string.Format(
                    "Replace array initialization with 'new {0}[{1}]'",
                    highlighting.ArrayElementType.GetPresentableName(CSharpLanguage.Instance),
                    highlighting.ElementCount.ToString());
            }
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                ICSharpTreeNode node;
                switch (highlighting.ArrayInitializer.Parent)
                {
                    case ITypeOwnerDeclaration _:
                        node = highlighting.ArrayInitializer;
                        break;

                    case IArrayCreationExpression creationExpression:
                        node = creationExpression;
                        break;

                    default: throw new NotSupportedException();
                }

                var factory = CSharpElementFactory.GetInstance(node);

                Debug.Assert(CSharpLanguage.Instance != null);

                ModificationUtil.ReplaceChild(
                    node,
                    factory.CreateExpression(
                        string.Format(
                            "new {0}[{1}]",
                            highlighting.ArrayElementType.GetPresentableName(CSharpLanguage.Instance),
                            highlighting.ElementCount.ToString())));
            }

            return _ => { };
        }
    }
}