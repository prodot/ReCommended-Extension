using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.YieldReturnWithinLock;

[ElementProblemAnalyzer(typeof(IYieldStatement), HighlightingTypes = [typeof(YieldReturnWithinLockWarning)])]
public sealed class YieldReturnWithinLockAnalyzer : ElementProblemAnalyzer<IYieldStatement>
{
    protected override void Run(IYieldStatement element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        for (var treeNode = element.Parent; treeNode is { }; treeNode = treeNode.Parent)
        {
            switch (treeNode)
            {
                case ILockStatement lockStatement when ClrTypeNames.Lock.TryGetTypeElement(element.GetPsiModule()) is not { } typeElement
                    || !TypeEqualityComparer.Default.Equals(lockStatement.Monitor.Type(), TypeFactory.CreateType(typeElement)):

                    consumer.AddHighlighting(new YieldReturnWithinLockWarning("'yield return' used inside the 'lock' block.", element));

                    return;

                case IAttributesOwnerDeclaration: return;
            }
        }
    }
}