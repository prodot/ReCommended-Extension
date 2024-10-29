using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Collection;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use 'new T[n]' for arrays and collection expressions with default values" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class ArrayWithDefaultValuesInitializationSuggestion(string message, string? suggestedCode, ICSharpTreeNode treeNode)
    : Highlighting(message)
{
    const string SeverityId = "ArrayWithDefaultValuesInitialization";

    internal string? SuggestedCode => suggestedCode;

    internal ICSharpTreeNode TreeNode => treeNode;

    public override DocumentRange CalculateRange() => TreeNode.GetDocumentRange();
}