using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.Analyzers.Annotation
{
    [QuickFix]
    public sealed class RemoveAttributeFix : QuickFixBase
    {
        [NotNull]
        readonly AttributeHighlighting highlighting;

        public RemoveAttributeFix([NotNull] NotAllowedAnnotationWarning highlighting) => this.highlighting = highlighting;

        public RemoveAttributeFix([NotNull] ConflictingAnnotationWarning highlighting) => this.highlighting = highlighting;

        public RemoveAttributeFix([NotNull] RedundantAnnotationSuggestion highlighting) => this.highlighting = highlighting;

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                highlighting.AttributesOwnerDeclaration.RemoveAttribute(highlighting.Attribute);
            }

            return _ => { };
        }

        public override string Text => "Remove attribute";
    }
}