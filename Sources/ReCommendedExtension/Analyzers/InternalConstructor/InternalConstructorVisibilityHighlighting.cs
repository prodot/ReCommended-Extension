using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.InternalConstructor;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        InternalConstructorVisibilityHighlighting.SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Make internal constructor in abstract class protected or private protected" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.InternalConstructor
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class InternalConstructorVisibilityHighlighting : Highlighting
    {
        internal const string SeverityId = "InternalConstructorVisibility";

        [NotNull]
        readonly ITokenNode modifierTokenNode;

        internal InternalConstructorVisibilityHighlighting(
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