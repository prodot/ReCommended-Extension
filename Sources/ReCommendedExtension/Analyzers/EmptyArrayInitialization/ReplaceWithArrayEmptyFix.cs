using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.EmptyArrayInitialization;

[QuickFix]
public sealed class ReplaceWithArrayEmptyFix : QuickFixBase
{
    readonly EmptyArrayInitializationWarning highlighting;

    public ReplaceWithArrayEmptyFix(EmptyArrayInitializationWarning highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            Debug.Assert(CSharpLanguage.Instance is { });

            var typeName = highlighting.ArrayElementType.GetPresentableName(CSharpLanguage.Instance);

            return $"Replace with '{nameof(Array)}.{nameof(Array.Empty)}<{typeName}>()'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.TreeNode);

            ModificationUtil.ReplaceChild(
                highlighting.TreeNode,
                factory.CreateExpression(
                    $"$0.{nameof(Array.Empty)}<$1>()",
                    EmptyArrayInitializationAnalyzer.TryGetArrayType(highlighting.TreeNode.GetPsiModule()),
                    highlighting.ArrayElementType));
        }

        return _ => { };
    }
}