using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Collection;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use 'new T[n]' for arrays and collection expressions with default values" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class ArrayWithDefaultValuesInitializationSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "ArrayWithDefaultValuesInitialization";

    public required string SuggestedCode { get; init; }

    public required ICSharpTreeNode TreeNode { get; init; }

    public override DocumentRange CalculateRange() => TreeNode.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(ArrayWithDefaultValuesInitializationSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => $"Replace with '{highlighting.SuggestedCode}'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var node = highlighting.TreeNode.Parent as IArrayCreationExpression ?? highlighting.TreeNode;

                var factory = CSharpElementFactory.GetInstance(node);

                ModificationUtil.ReplaceChild(node, factory.CreateExpression(highlighting.SuggestedCode));
            }

            return null;
        }
    }
}