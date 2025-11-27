using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

[ElementProblemAnalyzer(typeof(ISpecificCatchClause), HighlightingTypes = [typeof(CatchClauseWithoutVariableHint)])]
public sealed class CatchClauseWithoutVariableAnalyzer : ElementProblemAnalyzer<ISpecificCatchClause>
{
    protected override void Run(ISpecificCatchClause element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.ExceptionType.IsSystemException() && element.ExceptionDeclaration == null)
        {
            consumer.AddHighlighting(
                new CatchClauseWithoutVariableHint("Redundant declaration without an exception variable.") { CatchClause = element });
        }
    }
}