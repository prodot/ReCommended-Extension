using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.Analyzers.Annotation;

[QuickFix]
public sealed class RemoveNullableAnnotationFix(RedundantNullableAnnotationHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

    public override string Text => "Make method return type not nullable";

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            ModificationUtil.ReplaceChild(highlighting.NullableTypeUsage, highlighting.NullableTypeUsage.UnderlyingType);
        }

        return null;
    }
}