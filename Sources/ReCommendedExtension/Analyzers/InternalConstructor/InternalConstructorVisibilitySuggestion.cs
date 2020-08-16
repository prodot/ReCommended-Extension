using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.InternalConstructor
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Make internal constructor in abstract class protected or private protected" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class InternalConstructorVisibilitySuggestion : Highlighting
    {
        const string SeverityId = "InternalConstructorVisibility";

        [NotNull]
        readonly ITokenNode modifierTokenNode;

        internal InternalConstructorVisibilitySuggestion(
            [NotNull] string message,
            [NotNull] ITokenNode modifierTokenNode,
            [NotNull] IConstructorDeclaration constructorDeclaration,
            AccessRights visibility) : base(message)
        {
            this.modifierTokenNode = modifierTokenNode;
            ConstructorDeclaration = constructorDeclaration;
            Visibility = visibility;
        }

        [NotNull]
        internal IConstructorDeclaration ConstructorDeclaration { get; }

        internal AccessRights Visibility { get; }

        public override DocumentRange CalculateRange() => modifierTokenNode.GetDocumentRange();
    }
}