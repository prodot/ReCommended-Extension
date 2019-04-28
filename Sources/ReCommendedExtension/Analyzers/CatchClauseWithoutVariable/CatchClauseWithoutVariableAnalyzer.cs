using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.CatchClauseWithoutVariable
{
    [ElementProblemAnalyzer(typeof(ISpecificCatchClause), HighlightingTypes = new[] { typeof(CatchClauseWithoutVariableHighlighting) })]
    public sealed class CatchClauseWithoutVariableAnalyzer : ElementProblemAnalyzer<ISpecificCatchClause>
    {
        protected override void Run(ISpecificCatchClause element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.ExceptionType.ToString() == "System.Exception" && element.ExceptionDeclaration == null)
            {
                consumer.AddHighlighting(new CatchClauseWithoutVariableHighlighting("Redundant declaration without exception variable.", element));
            }
        }
    }
}