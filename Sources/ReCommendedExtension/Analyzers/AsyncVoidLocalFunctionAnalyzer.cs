using System.Diagnostics;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Search;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(ILocalFunctionDeclaration), HighlightingTypes = new[] { typeof(AvoidAsyncVoidHighlighting) })]
    public sealed class AsyncVoidLocalFunctionAnalyzer : ElementProblemAnalyzer<ILocalFunctionDeclaration>
    {
        protected override void Run(ILocalFunctionDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (!element.IsAsync || !element.IsVoidMethodDeclaration())
            {
                return; // not an "async void" local function
            }

            var psiServices = element.GetPsiServices();

            Debug.Assert(SearchDomainFactory.Instance != null);

            var solutionSearchDomain = SearchDomainFactory.Instance.CreateSearchDomain(psiServices.Solution, false);
            var references = psiServices.Finder.FindReferences(element.DeclaredElement, solutionSearchDomain, NullProgressIndicator.Create());
            var count = references.Count(reference => !reference.AssertNotNull().IsEventTarget());
            if (count > 0)
            {
                consumer.AddHighlighting(
                    new AvoidAsyncVoidHighlighting(
                        string.Format(
                            "'async void' local function used {0} time{1} not as a direct event handler.",
                            count.ToString(),
                            count == 1 ? "" : "s"),
                        element));
            }
        }
    }
}