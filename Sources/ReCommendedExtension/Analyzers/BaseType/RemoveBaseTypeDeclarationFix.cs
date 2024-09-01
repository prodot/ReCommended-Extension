using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseType;

[QuickFix]
public sealed class RemoveBaseTypeDeclarationFix(RemoveRedundantBaseTypeDeclarationSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove redundant 'object'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            Debug.Assert(highlighting.BaseTypes.ExtendedTypes is [_, ..]);

            highlighting.BaseTypes.RemoveExtendedType(highlighting.BaseTypes.ExtendedTypes[0]);
        }

        return _ => { };
    }
}