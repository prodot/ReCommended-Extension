using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.Method.Inspections;
using ReCommendedExtension.Analyzers.Method.Rules;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;
using MethodSignature = ReCommendedExtension.Extensions.MethodFinding.MethodSignature;

namespace ReCommendedExtension.Analyzers.Method;

[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(RedundantMethodInvocationHint), typeof(UseOtherMethodSuggestion), typeof(UseBinaryOperatorSuggestion)])]
public sealed class MethodAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
    void Analyze(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpExpression qualifier,
        IReferenceExpression invokedExpression,
        Rules.Method method,
        string methodName,
        ITypeElement containingType,
        IList<IParameter> resolvedParameters,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        foreach (var inspection in method.Inspections)
        {
            switch (inspection)
            {
                case RedundantMethodInvocation redundantMethodInvocation
                    when (!redundantMethodInvocation.IsPureMethod || !invocationExpression.IsUsedAsStatement())
                    && redundantMethodInvocation.Condition(qualifier, arguments, nullableReferenceTypesDataFlowAnalysisRunSynchronizer):
                {
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(inspection.Message(methodName), invocationExpression, invokedExpression));

                    break;
                }

                case OtherMethodInvocation otherMethodInvocation:
                {
                    Debug.Assert(otherMethodInvocation.ReplacementMethod is { });
                    Debug.Assert(invokedExpression.QualifierExpression is { });

                    if (otherMethodInvocation.TryGetReplacement(invocationExpression, arguments) is { } replacement
                        && (otherMethodInvocation.QualifierCanBeNull
                            || invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                        && containingType.HasMethod(
                            new MethodSignature
                            {
                                Name = otherMethodInvocation.ReplacementMethod.Name,
                                Parameters = otherMethodInvocation.ReplacementMethod.Parameters,
                                IsStatic = method.Signature.IsStatic,
                                GenericParametersCount = otherMethodInvocation.ReplacementMethod.GenericParametersCount,
                            }))
                    {
                        var highlighting = new UseOtherMethodSuggestion(
                            inspection.Message(otherMethodInvocation.ReplacementMethod.Name),
                            qualifier,
                            new ReplacedMethodInvocation { Name = otherMethodInvocation.ReplacementMethod.Name, Replacement = replacement });

                        switch (replacement.Context)
                        {
                            case MethodInvocationContext.Standalone:
                                Debug.Assert(replacement.OriginalExpression == invocationExpression);

                                consumer.AddHighlighting(
                                    highlighting,
                                    invocationExpression.GetDocumentRange().SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset));
                                break;

                            case MethodInvocationContext.BinaryLeftOperand:
                                Debug.Assert(replacement.OriginalExpression == invocationExpression.Parent);
                                Debug.Assert(
                                    invocationExpression.Parent is IBinaryExpression { LeftOperand: var leftOperand }
                                    && leftOperand == invocationExpression);

                                consumer.AddHighlighting(
                                    highlighting,
                                    replacement
                                        .OriginalExpression.GetDocumentRange()
                                        .SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset));
                                break;

                            case MethodInvocationContext.BinaryRightOperand:
                                Debug.Assert(replacement.OriginalExpression == invocationExpression.Parent);
                                Debug.Assert(
                                    invocationExpression.Parent is IBinaryExpression { RightOperand: var rightOperand }
                                    && rightOperand == invocationExpression);

                                var binaryExpression = (IBinaryExpression)replacement.OriginalExpression;

                                if (binaryExpression.OperatorReference is { })
                                {
                                    consumer.AddHighlighting(
                                        highlighting,
                                        binaryExpression
                                            .LeftOperand.GetDocumentRange()
                                            .JoinRight(binaryExpression.OperatorReference.GetDocumentRange()));
                                    consumer.AddHighlighting(
                                        highlighting,
                                        invocationExpression
                                            .GetDocumentRange()
                                            .SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset));
                                }
                                break;
                        }
                    }

                    break;
                }

                case BinaryOperator binaryOperator when !invocationExpression.IsUsedAsStatement():
                {
                    Debug.Assert(binaryOperator.Operator is { });

                    if (binaryOperator.TryGetOperands(qualifier, arguments) is { } operands)
                    {
                        consumer.AddHighlighting(
                            new UseBinaryOperatorSuggestion(
                                inspection.Message(binaryOperator.Operator),
                                invocationExpression,
                                operands,
                                binaryOperator.Operator,
                                binaryOperator.HighlightInvokedMethodOnly ? invokedExpression : null));
                    }
                    break;
                }
            }
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is
            {
                InvokedExpression: IReferenceExpression
                {
                    QualifierExpression: { } qualifierExpression, Reference: var reference,
                } invokedExpression,
            }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, ContainingType: { } containingType,
            } resolvedMethod
            && RuleDefinitions.TryGetMethod(containingType, resolvedMethod) is { } method
            && element.TryGetArgumentsInDeclarationOrder() is { } arguments)
        {
            Analyze(
                consumer,
                element,
                qualifierExpression,
                invokedExpression,
                method,
                resolvedMethod.ShortName,
                containingType,
                resolvedMethod.Parameters,
                arguments);
        }
    }
}