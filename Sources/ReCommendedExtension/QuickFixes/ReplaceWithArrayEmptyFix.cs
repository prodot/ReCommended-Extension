using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Analyzers;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.QuickFixes
{
    [QuickFix]
    public sealed class ReplaceWithArrayEmptyFix : QuickFixBase
    {
        [NotNull]
        readonly EmptyArrayInitializationHighlighting highlighting;

        public ReplaceWithArrayEmptyFix([NotNull] EmptyArrayInitializationHighlighting highlighting) => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                Debug.Assert(CSharpLanguage.Instance != null);

                return string.Format(
                    "Replace with '{0}.{1}<{2}>()'",
                    nameof(Array),
                    EmptyArrayInitializationAnalyzer.ArrayEmptyMethodName,
                    highlighting.ArrayElementType.GetPresentableName(CSharpLanguage.Instance));
            }
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.TreeNode);

                ModificationUtil.ReplaceChild(
                    highlighting.TreeNode,
                    factory.CreateExpression(
                        $"$0.{EmptyArrayInitializationAnalyzer.ArrayEmptyMethodName}<$1>()",
                        EmptyArrayInitializationAnalyzer.GetArrayType(highlighting.TreeNode.GetPsiModule()),
                        highlighting.ArrayElementType));
            }

            return _ => { };
        }
    }
}