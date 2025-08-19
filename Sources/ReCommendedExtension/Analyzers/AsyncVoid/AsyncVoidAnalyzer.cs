using JetBrains.Application.Progress;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;

namespace ReCommendedExtension.Analyzers.AsyncVoid;

[ElementProblemAnalyzer(typeof(ICSharpDeclaration), HighlightingTypes = [typeof(AsyncVoidFunctionExpressionWarning), typeof(AvoidAsyncVoidWarning)])]
public sealed class AsyncVoidAnalyzer : ElementProblemAnalyzer<ICSharpDeclaration>
{
    [Pure]
    static bool IsPublicSurfaceArea(IMethod method)
        => method.AccessibilityDomain.DomainType is AccessibilityDomain.AccessibilityDomainType.PUBLIC
            or AccessibilityDomain.AccessibilityDomainType.PROTECTED
            or AccessibilityDomain.AccessibilityDomainType.PROTECTED_OR_INTERNAL;

    /// <summary>
    /// Returns the object creation expression <c>new EventHandler(</c><paramref name="argument"/><c>)</c> if used in the pattern
    /// <c>new EventHandler(</c><paramref name="argument"/><c>)</c> or <c>null</c>.
    /// </summary>
    [Pure]
    static IObjectCreationExpression? TryGetDelegateCreation(ICSharpArgument argument)
    {
        var argumentList = argument.Parent as IArgumentList;

        if (argumentList is null or { Arguments: not [_] })
        {
            return null;
        }

        return argumentList.Parent as IObjectCreationExpression;
    }

    [Pure]
    static bool IsEventTarget(IReference reference)
    {
        switch (reference.GetTreeNode().Parent)
        {
            case IAssignmentExpression assignmentExpression: return IsEventSubscriptionOrUnSubscription(assignmentExpression);

            case ICSharpArgument argument:
            {
                if (TryGetDelegateCreation(argument) is { Parent: IAssignmentExpression assignmentExpression })
                {
                    return IsEventSubscriptionOrUnSubscription(assignmentExpression);
                }
                break;
            }
        }

        if (reference is IReferenceToDelegateCreation referenceToDelegateCreation)
        {
            return referenceToDelegateCreation.IsEventSubscription;
        }

        return false;
    }

    /// <summary>
    /// Returns <c>true</c> if used in the pattern <c>button.Click ±= </c><paramref name="assignmentExpression"/><c>;</c>.
    /// </summary>
    [Pure]
    static bool IsEventSubscriptionOrUnSubscription(IAssignmentExpression assignmentExpression)
    {
        if (assignmentExpression.AssignmentType is not (AssignmentType.PLUSEQ or AssignmentType.MINUSEQ))
        {
            return false;
        }

        if (assignmentExpression.OperatorOperands is not [_, _])
        {
            return false;
        }

        return (assignmentExpression.Dest as IReferenceExpression)?.Reference.Resolve().DeclaredElement is IEvent;
    }

    static void Analyze(IMethodDeclaration methodDeclaration, IHighlightingConsumer consumer)
    {
        if (!methodDeclaration.IsAsync || !methodDeclaration.Type.IsVoid())
        {
            return; // not an "async void" method
        }

        var method = methodDeclaration.DeclaredElement;
        if (method == null)
        {
            return; // cannot analyze
        }

        if (method.GetImmediateSuperMembers().Any())
        {
            consumer.AddHighlighting(new AvoidAsyncVoidWarning("'void' method overridden or implemented as 'async void'.", methodDeclaration));
            return;
        }

        // find usages
        var psiServices = method.GetPsiServices();

        var solutionSearchDomain = SearchDomainFactory.Instance.CreateSearchDomain(psiServices.Solution, false);
        var references = psiServices.SingleThreadedFinder.FindReferences(method, solutionSearchDomain, NullProgressIndicator.Create());

        if (IsPublicSurfaceArea(method))
        {
            if (references is [])
            {
                var implicitUseAnnotationProvider = psiServices.GetCodeAnnotationsCache().GetProvider<ImplicitUseAnnotationProvider>();

                if (implicitUseAnnotationProvider.CalculateTypeMemberImplicitlyUsedFlags(method).IsEmpty)
                {
                    // [UsedImplicitly] annotation not applied
                    consumer.AddHighlighting(
                        new AvoidAsyncVoidWarning("'async void' public surface area method without detected usages.", methodDeclaration));
                }
            }
            else
            {
                consumer.AddHighlighting(
                    new AvoidAsyncVoidWarning("'async void' public surface area method with detected usages.", methodDeclaration));
            }
        }
        else
        {
            var count = references.Count(reference => !IsEventTarget(reference));
            if (count > 0)
            {
                consumer.AddHighlighting(
                    new AvoidAsyncVoidWarning(
                        $"'async void' method used {count.ToString()} time{(count == 1 ? "" : "s")} not as a direct event handler.",
                        methodDeclaration));
            }
        }
    }

    static void Analyze(ILocalFunctionDeclaration localFunctionDeclaration, IHighlightingConsumer consumer)
    {
        if (!localFunctionDeclaration.IsAsync || !localFunctionDeclaration.Type.IsVoid())
        {
            return; // not an "async void" local function
        }

        var psiServices = localFunctionDeclaration.GetPsiServices();

        var solutionSearchDomain = SearchDomainFactory.Instance.CreateSearchDomain(psiServices.Solution, false);
        var references = psiServices.SingleThreadedFinder.FindReferences(
            localFunctionDeclaration.DeclaredElement,
            solutionSearchDomain,
            NullProgressIndicator.Create());
        var count = references.Count(reference => !IsEventTarget(reference));
        if (count > 0)
        {
            consumer.AddHighlighting(
                new AvoidAsyncVoidWarning(
                    $"'async void' local function used {count.ToString()} time{(count == 1 ? "" : "s")} not as a direct event handler.",
                    localFunctionDeclaration));
        }
    }

    static void Analyze(ILambdaExpression lambdaExpression, IHighlightingConsumer consumer)
    {
        if (!lambdaExpression.IsAsync || !lambdaExpression.InferredReturnType.IsVoid())
        {
            return; // not an "async (...) => ..." expression that returns void
        }

        if (lambdaExpression.Parent is IAssignmentExpression assignmentExpression && IsEventSubscriptionOrUnSubscription(assignmentExpression))
        {
            return; // direct event target
        }

        if (lambdaExpression.AsyncKeyword is { })
        {
            consumer.AddHighlighting(
                new AsyncVoidFunctionExpressionWarning(
                    "'async void' lambda expression not used as a direct event handler.",
                    lambdaExpression.AsyncKeyword,
                    () => lambdaExpression.SetAsync(false)));
        }
    }

    static void Analyze(IAnonymousMethodExpression anonymousMethodExpression, IHighlightingConsumer consumer)
    {
        if (!anonymousMethodExpression.IsAsync || !anonymousMethodExpression.InferredReturnType.IsVoid())
        {
            return; // not an "async delegate (...) { ... }" that returns void
        }

        if (anonymousMethodExpression.Parent is IAssignmentExpression assignmentExpression
            && IsEventSubscriptionOrUnSubscription(assignmentExpression))
        {
            return; // direct event target
        }

        consumer.AddHighlighting(
            new AsyncVoidFunctionExpressionWarning(
                "'async void' anonymous method expression not used as a direct event handler.",
                anonymousMethodExpression.AsyncKeyword,
                () => anonymousMethodExpression.SetAsync(false)));
    }

    protected override void Run(ICSharpDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IMethodDeclaration methodDeclaration:
                Analyze(methodDeclaration, consumer);
                break;

            case ILocalFunctionDeclaration localFunctionDeclaration:
                Analyze(localFunctionDeclaration, consumer);
                break;

            case ILambdaExpression lambdaExpression:
                Analyze(lambdaExpression, consumer);
                break;

            case IAnonymousMethodExpression anonymousMethodExpression:
                Analyze(anonymousMethodExpression, consumer);
                break;
        }
    }
}