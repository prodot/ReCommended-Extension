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

namespace ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization
{
    [QuickFix]
    public sealed class ReplaceWithNewArrayWithLengthFix : QuickFixBase
    {
        [NotNull]
        readonly ArrayWithDefaultValuesInitializationSuggestion highlighting;

        public ReplaceWithNewArrayWithLengthFix([NotNull] ArrayWithDefaultValuesInitializationSuggestion highlighting)
            => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                Debug.Assert(CSharpLanguage.Instance != null);

                return $"Replace array initialization with '{highlighting.SuggestedCode}'";
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

                ModificationUtil.ReplaceChild(node, factory.CreateExpression(highlighting.SuggestedCode));
            }

            return _ => { };
        }
    }
}