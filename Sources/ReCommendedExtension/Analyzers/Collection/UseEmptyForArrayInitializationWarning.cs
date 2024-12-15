using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Collection;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    $"Use '{nameof(Array)}.{nameof(Array.Empty)}<T>()' for empty arrays" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseEmptyForArrayInitializationWarning(string message, ICSharpTreeNode treeNode, IType arrayItemType) : Highlighting(message)
{
    const string SeverityId = "UseEmptyForArrayInitialization";

    internal ICSharpTreeNode TreeNode => treeNode;

    internal IType ArrayItemType => arrayItemType;

    public override DocumentRange CalculateRange() => TreeNode.GetDocumentRange();
}