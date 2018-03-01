using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        EmptyArrayInitializationHighlighting.SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Use 'Array.Empty<T>()' for empty arrays" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class EmptyArrayInitializationHighlighting : Highlighting
    {
        internal const string SeverityId = "EmptyArrayInitialization";

        internal EmptyArrayInitializationHighlighting(
            [NotNull] string message,
            [NotNull] ICSharpTreeNode treeNode,
            [NotNull] IType arrayElementType) : base(message)
        {
            TreeNode = treeNode;
            ArrayElementType = arrayElementType;
        }

        [NotNull]
        internal ICSharpTreeNode TreeNode { get; }

        [NotNull]
        internal IType ArrayElementType { get; }

        public override DocumentRange CalculateRange() => TreeNode.GetDocumentRange();
    }
}