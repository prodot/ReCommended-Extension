using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

[QuickFix]
public sealed class RemoveAttributeFix : QuickFixBase
{
    readonly AttributeHighlighting highlighting;

    public RemoveAttributeFix(NotAllowedAnnotationWarning highlighting) => this.highlighting = highlighting;

    public RemoveAttributeFix(ConflictingAnnotationWarning highlighting) => this.highlighting = highlighting;

    public RemoveAttributeFix(RedundantAnnotationSuggestion highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove attribute";

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            highlighting.AttributesOwnerDeclaration.RemoveAttribute(highlighting.Attribute);
        }

        return null;
    }
}