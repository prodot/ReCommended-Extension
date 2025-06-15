using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.ContextActions;

public abstract class ContextAction<N>(ICSharpContextActionDataProvider provider) : ContextActionBase where N : class, ITreeNode
{
    internal IPsiModule PsiModule => provider.PsiModule;

    internal TreeTextRange SelectedTreeRange => provider.SelectedTreeRange;

    internal NullableReferenceTypesDataFlowAnalysisRunSynchronizer NullableReferenceTypesDataFlowAnalysisRunSynchronizer
        => provider.PsiServices.GetComponent<NullableReferenceTypesDataFlowAnalysisRunSynchronizer>();

    public sealed override bool IsAvailable(IUserDataHolder cache)
    {
        var selectedElement = provider.GetSelectedElement<N>(true, false);

        return selectedElement is { } && IsAvailable(selectedElement);
    }

    protected abstract bool IsAvailable(N selectedElement);
}