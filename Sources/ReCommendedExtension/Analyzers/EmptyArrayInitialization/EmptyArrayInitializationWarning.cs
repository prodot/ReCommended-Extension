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
public sealed record EmptyArrayInitializationWarning : Highlighting
{
    const string SeverityId = "EmptyArrayInitialization";

    internal EmptyArrayInitializationWarning(string message, ICSharpTreeNode treeNode, IType arrayElementType) : base(message)
    {
        TreeNode = treeNode;
        ArrayElementType = arrayElementType;
    }

    internal ICSharpTreeNode TreeNode { get; }

    internal IType ArrayElementType { get; }

    public override DocumentRange CalculateRange() => TreeNode.GetDocumentRange();
}