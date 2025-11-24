using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.ExpressionResult.Inspections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Analyzers.ExpressionResult.Rules;

namespace ReCommendedExtension.Analyzers.ExpressionResult;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(ICSharpInvocationInfo), HighlightingTypes = [typeof(UseExpressionResultSuggestion)])]
public sealed class ExpressionResultAnalyzer(
    NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
    void Analyze(
        IHighlightingConsumer consumer,
        ICSharpInvocationInfo invocationInfo,
        ICSharpExpression? qualifier,
        Member member,
        InspectionContext context,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        foreach (var inspection in member.Inspections)
        {
            Debug.Assert(!inspection.EnsureQualifierNotNull || qualifier is { });

            if ((inspection.MinimumFrameworkVersion == null
                    || invocationInfo.PsiModule.TargetFrameworkId.Version >= inspection.MinimumFrameworkVersion)
                && inspection.TryGetReplacements(qualifier, arguments, context) is { } replacements
                && (!inspection.EnsureQualifierNotNull || qualifier!.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                && (!inspection.EnsureFirstArgumentNotNull
                    || arguments is [{ Value: { } firstArgValue }, ..]
                    && firstArgValue.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)))
            {
                consumer.AddHighlighting(new UseExpressionResultSuggestion(inspection.Message, invocationInfo, replacements));
            }
        }
    }

    protected override void Run(ICSharpInvocationInfo element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.IsUsedAsStatement)
        {
            return;
        }

        switch (element)
        {
            case IObjectCreationExpression { ConstructorReference: var reference } objectCreationExpression
                when reference.Resolve().DeclaredElement is IConstructor
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    IsStatic: false,
                    ContainingType: { } containingType,
                } resolvedConstructor
                && RuleDefinitions.TryGetConstructor(containingType, resolvedConstructor) is { } constructor
                && objectCreationExpression.TryGetArgumentsInDeclarationOrder() is { } arguments:
            {
                Analyze(
                    consumer,
                    element,
                    null,
                    constructor,
                    new InspectionContext(objectCreationExpression, resolvedConstructor.Parameters, containingType),
                    arguments);
                break;
            }

            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression { QualifierExpression: { } qualifierExpression, Reference: var reference },
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, ContainingType: { } containingType,
                } resolvedMethod
                && RuleDefinitions.TryGetMethod(containingType, resolvedMethod) is { } method
                && invocationExpression.TryGetArgumentsInDeclarationOrder() is { } arguments:
            {
                Analyze(
                    consumer,
                    element,
                    qualifierExpression,
                    method,
                    new InspectionContext(invocationExpression, resolvedMethod.Parameters, containingType),
                    arguments);
                break;
            }
        }
    }
}