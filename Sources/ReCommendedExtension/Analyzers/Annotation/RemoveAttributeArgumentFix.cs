using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.Analyzers.Annotation;

[QuickFix]
public sealed class RemoveAttributeArgumentFix(RedundantAnnotationArgumentSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

    public override string Text => "Remove attribute argument";

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        Debug.Assert(highlighting.Attribute.Arguments is [_]);

        using (WriteLockCookie.Create())
        {
            ModificationUtil.DeleteChildRange(highlighting.Attribute.LPar, highlighting.Attribute.RPar);
        }

        return null;
    }
}