using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.InternalConstructor;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Make internal constructor in abstract class protected or private protected" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class InternalConstructorVisibilitySuggestion(string message, ITokenNode modifierTokenNode) : Highlighting(message)
{
    const string SeverityId = "InternalConstructorVisibility";

    public required IConstructorDeclaration ConstructorDeclaration { get; init; }

    public required AccessRights Visibility { get; init; }

    public override DocumentRange CalculateRange() => modifierTokenNode.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(InternalConstructorVisibilitySuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
            => highlighting.Visibility switch
            {
                AccessRights.PROTECTED => $"Make constructor '{highlighting.ConstructorDeclaration.DeclaredName}' protected.",
                AccessRights.PROTECTED_AND_INTERNAL => $"Make constructor '{highlighting.ConstructorDeclaration.DeclaredName}' private protected.",

                _ => throw new NotSupportedException(),
            };

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                highlighting.ConstructorDeclaration.SetAccessRights(highlighting.Visibility);
            }

            return null;
        }
    }
}