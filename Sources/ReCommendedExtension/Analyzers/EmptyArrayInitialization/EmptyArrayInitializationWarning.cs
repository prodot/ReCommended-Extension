using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.EmptyArrayInitialization;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    $"Use '{nameof(Array)}.{nameof(Array.Empty)}<T>()' for empty arrays" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class EmptyArrayInitializationWarning(string message, ICSharpTreeNode treeNode, IType arrayElementType) : Highlighting(message)
{
    const string SeverityId = "EmptyArrayInitialization";

    internal ICSharpTreeNode TreeNode { get; } = treeNode;

    internal IType ArrayElementType { get; } = arrayElementType;

    public override DocumentRange CalculateRange() => TreeNode.GetDocumentRange();
}