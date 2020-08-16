using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.EmptyArrayInitialization
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Use 'Array.Empty<T>()' for empty arrays" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class EmptyArrayInitializationWarning : Highlighting
    {
        const string SeverityId = "EmptyArrayInitialization";

        internal EmptyArrayInitializationWarning(
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