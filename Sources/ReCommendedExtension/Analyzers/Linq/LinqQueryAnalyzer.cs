using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Linq;

[ElementProblemAnalyzer(typeof(IQueryExpression), HighlightingTypes = [typeof(RedundantLinqQueryHint)])]
public sealed class LinqQueryAnalyzer : ElementProblemAnalyzer<IQueryExpression>
{
    protected override void Run(IQueryExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.From is { CastType: null, Declaration: var declaration, Expression: var expression }
            && (expression.Type().IsGenericIEnumerable() || element.TryGetTargetType().IsGenericIEnumerable() || element.Parent is ISpreadElement)
            && element.Clauses is [IQuerySelectClause { Expression.Value: IReferenceExpression referenceExpression } selectClause]
            && referenceExpression.Reference.Resolve().DeclaredElement?.GetSingleDeclaration() == declaration)
        {
            var highlighting = new RedundantLinqQueryHint("No-op LINQ query is redundant.", element, expression);

            var documentRange = element.GetDocumentRange();

            consumer.AddHighlighting(highlighting, documentRange.SetEndTo(element.From.InKeyword.GetDocumentEndOffset()));
            consumer.AddHighlighting(highlighting, documentRange.SetStartTo(selectClause.SelectKeyword.GetDocumentStartOffset()));
        }
    }
}