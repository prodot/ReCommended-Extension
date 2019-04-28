using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.InternalConstructor
{
    [QuickFix]
    public sealed class ChangeConstructorVisibilityFix : QuickFixBase
    {
        [NotNull]
        readonly InternalConstructorVisibilityHighlighting highlighting;

        public ChangeConstructorVisibilityFix([NotNull] InternalConstructorVisibilityHighlighting highlighting) => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                switch (highlighting.Visibility)
                {
                    case AccessRights.PROTECTED: return $"Make constructor '{highlighting.ConstructorDeclaration.DeclaredName}' protected.";
                    case AccessRights.PROTECTED_AND_INTERNAL:
                        return $"Make constructor '{highlighting.ConstructorDeclaration.DeclaredName}' private protected.";

                    default: throw new NotSupportedException();
                }
            }
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                highlighting.ConstructorDeclaration.SetAccessRights(highlighting.Visibility);
            }

            return _ => { };
        }
    }
}