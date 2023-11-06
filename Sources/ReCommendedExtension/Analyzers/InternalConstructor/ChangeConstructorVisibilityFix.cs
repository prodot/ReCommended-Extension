using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.InternalConstructor;

[QuickFix]
public sealed class ChangeConstructorVisibilityFix : QuickFixBase
{
    readonly InternalConstructorVisibilitySuggestion highlighting;

    public ChangeConstructorVisibilityFix(InternalConstructorVisibilitySuggestion highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => highlighting.Visibility switch
        {
            AccessRights.PROTECTED => $"Make constructor '{highlighting.ConstructorDeclaration.DeclaredName}' protected.",
            AccessRights.PROTECTED_AND_INTERNAL => $"Make constructor '{highlighting.ConstructorDeclaration.DeclaredName}' private protected.",

            _ => throw new NotSupportedException(),
        };

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            highlighting.ConstructorDeclaration.SetAccessRights(highlighting.Visibility);
        }

        return _ => { };
    }
}