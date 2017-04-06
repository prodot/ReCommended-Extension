using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Search;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IMethodDeclaration), HighlightingTypes = new[] { typeof(AvoidAsyncVoidHighlighting) })]
    public sealed class AsyncVoidMethodAnalyzer : ElementProblemAnalyzer<IMethodDeclaration>
    {
        static bool IsPublicSurfaceArea([NotNull] IMethod method)
        {
            switch (method.AccessibilityDomain?.DomainType)
            {
                case AccessibilityDomain.AccessibilityDomainType.PUBLIC:
                case AccessibilityDomain.AccessibilityDomainType.PROTECTED:
                case AccessibilityDomain.AccessibilityDomainType.PROTECTED_OR_INTERNAL:
                    return true;

                default:
                    return false;
            }
        }

        protected override void Run(IMethodDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (!element.IsAsync || !element.IsVoidMethodDeclaration())
            {
                return; // not an "async void" method
            }

            var method = element.DeclaredElement;
            if (method == null)
            {
                return; // cannot analyze
            }

            if (method.GetImmediateSuperMembers().Any())
            {
                consumer.AddHighlighting(new AvoidAsyncVoidHighlighting("'void' method overridden or implemented as 'async void'.", element));
                return;
            }

            // find usages
            var psiServices = method.GetPsiServices();

            Debug.Assert(SearchDomainFactory.Instance != null);

            var solutionSearchDomain = SearchDomainFactory.Instance.CreateSearchDomain(psiServices.Solution, false);
            var references = psiServices.Finder.FindReferences(method, solutionSearchDomain, NullProgressIndicator.Create());

            if (IsPublicSurfaceArea(method))
            {
                if (references.Length > 0)
                {
                    consumer.AddHighlighting(new AvoidAsyncVoidHighlighting("'async void' public surface area method with detected usages.", element));
                }
                else
                {
                    var implicitUseAnnotationProvider = psiServices.GetCodeAnnotationsCache().GetProvider<ImplicitUseAnnotationProvider>();

                    Debug.Assert(implicitUseAnnotationProvider != null);

                    var useKindFlags = implicitUseAnnotationProvider.IsImplicitlyUsed(method);
                    if (useKindFlags == null)
                    {
                        // [UsedImplicitly] annotation not applied
                        consumer.AddHighlighting(
                            new AvoidAsyncVoidHighlighting("'async void' public surface area method without detected usages.", element));
                    }
                }
            }
            else
            {
                var count = references.Count(reference => !reference.AssertNotNull().IsEventTarget());
                if (count > 0)
                {
                    consumer.AddHighlighting(
                        new AvoidAsyncVoidHighlighting(
                            string.Format(
                                "'async void' method used {0} time{1} not as a direct event handler.",
                                count.ToString(),
                                count == 1 ? "" : "s"),
                            element));
                }
            }
        }
    }
}