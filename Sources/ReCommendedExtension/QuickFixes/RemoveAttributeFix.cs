using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.QuickFixes
{
    [QuickFix]
    public sealed class RemoveAttributeFix : QuickFixBase
    {
        [NotNull]
        readonly AttributeHighlighting highlighting;

        public RemoveAttributeFix([NotNull] NotAllowedAnnotationHighlighting highlighting) => this.highlighting = highlighting;

        public RemoveAttributeFix([NotNull] ConflictingAnnotationHighlighting highlighting) => this.highlighting = highlighting;

        public RemoveAttributeFix([NotNull] RedundantAnnotationHighlighting highlighting) => this.highlighting = highlighting;

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