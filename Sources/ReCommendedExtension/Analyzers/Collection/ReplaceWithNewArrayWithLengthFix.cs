using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Collection;

[QuickFix]
public sealed class ReplaceWithNewArrayWithLengthFix(ArrayWithDefaultValuesInitializationSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace array initialization with '{highlighting.SuggestedCode}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            ICSharpTreeNode node = highlighting.ArrayInitializer.Parent switch
            {
                ITypeOwnerDeclaration => highlighting.ArrayInitializer,
                IArrayCreationExpression creationExpression => creationExpression,

                _ => throw new NotSupportedException(),
            };

            var factory = CSharpElementFactory.GetInstance(node);

            ModificationUtil.ReplaceChild(node, factory.CreateExpression(highlighting.SuggestedCode));
        }

        return _ => { };
    }
}