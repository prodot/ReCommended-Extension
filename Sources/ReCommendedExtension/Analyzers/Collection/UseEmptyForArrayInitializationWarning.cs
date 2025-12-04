using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
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
    HighlightingGroupIds.BestPractice,
    $"Use '{nameof(Array)}.{nameof(Array.Empty)}<T>()' for empty arrays" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseEmptyForArrayInitializationWarning(string message) : Highlighting(message)
{
    const string SeverityId = "UseEmptyForArrayInitialization";

    public required ICSharpTreeNode TreeNode { get; init; }

    public required IType ArrayItemType { get; init; }

    public override DocumentRange CalculateRange() => TreeNode.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(UseEmptyForArrayInitializationWarning highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                Debug.Assert(CSharpLanguage.Instance is { });

                var typeName = highlighting.ArrayItemType.GetPresentableName(CSharpLanguage.Instance);

                return $"Replace with '{nameof(Array)}.{nameof(Array.Empty)}<{typeName}>()'";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.TreeNode);

                ModificationUtil.ReplaceChild(
                    highlighting.TreeNode,
                    factory.CreateExpression(
                        $"$0.{nameof(Array.Empty)}<$1>()",
                        PredefinedType.ARRAY_FQN.TryGetTypeElement(highlighting.TreeNode.GetPsiModule()),
                        highlighting.ArrayItemType));
            }

            return null;
        }
    }
}