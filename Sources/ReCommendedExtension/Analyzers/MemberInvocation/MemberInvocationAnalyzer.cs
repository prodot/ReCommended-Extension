using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Analyzers.MemberInvocation.Inspections;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MemberFinding;
using MethodSignature = ReCommendedExtension.Extensions.MemberFinding.MethodSignature;
using PropertySignature = ReCommendedExtension.Extensions.MemberFinding.PropertySignature;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IReferenceExpression),
    HighlightingTypes =
    [
        typeof(RedundantMethodInvocationHint),
        typeof(UseOtherMethodSuggestion),
        typeof(UseBinaryOperatorSuggestion),
        typeof(UseUnaryOperatorSuggestion),
        typeof(UsePatternSuggestion),
        typeof(UseNullableHasValueAlternativeSuggestion),
        typeof(ReplaceNullableValueWithTypeCastSuggestion),
        typeof(UseRangeIndexerSuggestion),
        typeof(UsePropertySuggestion),
        typeof(UseStaticPropertySuggestion),
    ])]
public sealed class MemberInvocationAnalyzer(
    NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IReferenceExpression>
{
    void Analyze(
        IHighlightingConsumer consumer,
        IInvocationExpression? invocationExpression,
        ICSharpExpression qualifier,
        IReferenceExpression invokedExpression,
        Member member,
        string memberName,
        ITypeElement containingType,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        foreach (var inspection in member.Inspections)
        {
            switch (inspection)
            {
                case RedundantMethodInvocation redundantMethodInvocation when invocationExpression is { }
                    && (!redundantMethodInvocation.IsPureMethod || !invocationExpression.IsUsedAsStatement())
                    && redundantMethodInvocation.Condition(qualifier, arguments)
                    && (!redundantMethodInvocation.EnsureFirstArgumentNotNull
                        || arguments is [{ Value: { } firstArgValue }, ..]
                        && firstArgValue.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)):
                {
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(inspection.Message(memberName), invocationExpression, invokedExpression));

                    break;
                }

                case OtherMethodInvocation otherMethodInvocation when invocationExpression is { }:
                {
                    Debug.Assert(otherMethodInvocation.ReplacementMethod is { });

                    if (otherMethodInvocation.TryGetReplacement(invocationExpression, arguments) is { } replacement
                        && (!otherMethodInvocation.EnsureQualifierNotNull
                            || qualifier.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                        && containingType.HasMethod(
                            new MethodSignature
                            {
                                Name = otherMethodInvocation.ReplacementMethod.Name,
                                Parameters = otherMethodInvocation.ReplacementMethod.Parameters,
                                IsStatic = member.Signature.IsStatic,
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

                case BinaryOperator binaryOperator when invocationExpression is { } && !invocationExpression.IsUsedAsStatement():
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

                case UnaryOperator unaryOperator when invocationExpression is { } && !invocationExpression.IsUsedAsStatement():
                {
                    Debug.Assert(unaryOperator.Operator is { });

                    if (unaryOperator.TryGetOperand(qualifier, arguments) is { } operand)
                    {
                        consumer.AddHighlighting(
                            new UseUnaryOperatorSuggestion(
                                inspection.Message(unaryOperator.Operator),
                                invocationExpression,
                                operand,
                                unaryOperator.Operator));
                    }
                    break;
                }

                case PatternByArgument pattern when invocationExpression is { } && !invocationExpression.IsUsedAsStatement():
                {
                    Debug.Assert(pattern is { MinimumLanguageVersion: { }, ParameterIndex: >= 0, Pattern: { } });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion)
                    {
                        var argument = arguments[pattern.ParameterIndex];

                        if (argument is { Value: { } value })
                        {
                            consumer.AddHighlighting(
                                new UsePatternSuggestion(
                                    inspection.Message(""),
                                    invocationExpression,
                                    new PatternReplacement { Expression = value, Pattern = pattern.Pattern }));
                        }
                    }
                    break;
                }

                case PatternByArguments pattern when invocationExpression is { } && !invocationExpression.IsUsedAsStatement():
                {
                    Debug.Assert(pattern.MinimumLanguageVersion is { });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion
                        && pattern.TryGetReplacement(arguments) is { } replacement)
                    {
                        consumer.AddHighlighting(new UsePatternSuggestion(inspection.Message(""), invocationExpression, replacement));
                    }
                    break;
                }

                case PatternByQualifierArguments pattern when invocationExpression is { } && !invocationExpression.IsUsedAsStatement():
                {
                    Debug.Assert(pattern.MinimumLanguageVersion is { });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion
                        && (!pattern.EnsureQualifierNotNull || qualifier.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                        && pattern.TryGetReplacement(qualifier, arguments) is { } replacement)
                    {
                        consumer.AddHighlighting(new UsePatternSuggestion(inspection.Message(""), invocationExpression, replacement));
                    }

                    break;
                }

                case PatternByBinaryExpression pattern when invocationExpression is { }:
                {
                    Debug.Assert(pattern.MinimumLanguageVersion is { });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion
                        && (!pattern.EnsureQualifierNotNull || qualifier.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                        && pattern.TryGetReplacement(invocationExpression, qualifier, arguments) is { } replacement)
                    {
                        consumer.AddHighlighting(new UsePatternSuggestion(inspection.Message(""), invocationExpression, replacement));
                    }

                    break;
                }

                case PropertyOfNullable propertyOfNullable when !invokedExpression.IsPropertyAssignment()
                    && !invokedExpression.IsWithinNameofExpression()
                    && (!propertyOfNullable.EnsureQualifierNotValueTuple || !qualifier.Type().Unlift().IsValueTuple(out _)):
                {
                    var highlighting = propertyOfNullable.Name switch
                    {
                        PropertyNameOfNullable.HasValue => new UseNullableHasValueAlternativeSuggestion(inspection.Message(""), invokedExpression),
                        PropertyNameOfNullable.Value => new ReplaceNullableValueWithTypeCastSuggestion(inspection.Message(""), invokedExpression),

                        _ => null as Highlighting,
                    };

                    if (highlighting is { })
                    {
                        consumer.AddHighlighting(highlighting);
                    }
                    break;
                }

                case PropertyOfString propertyOfString when invocationExpression is { }
                    && !invocationExpression.IsUsedAsStatement()
                    && (propertyOfString.MinimumFrameworkVersion == null
                        || invocationExpression.PsiModule.TargetFrameworkId.Version >= propertyOfString.MinimumFrameworkVersion)
                    && propertyOfString.Condition(arguments):
                {
                    Debug.Assert(propertyOfString.Name is { });

                    if (containingType.HasProperty(new PropertySignature { Name = propertyOfString.Name }))
                    {
                        consumer.AddHighlighting(
                            new UsePropertySuggestion(
                                inspection.Message(propertyOfString.Name),
                                invocationExpression,
                                invokedExpression,
                                propertyOfString.Name));
                    }
                    break;
                }

                case PropertyOfDateTime propertyOfDateTime when !invokedExpression.IsPropertyAssignment()
                    && !invokedExpression.IsWithinNameofExpression()
                    && invokedExpression.QualifierExpression is IReferenceExpression
                    {
                        Reference: var reference, QualifierExpression: var qualifierExpression,
                    }
                    && propertyOfDateTime.Condition(reference):
                {
                    Debug.Assert(propertyOfDateTime.Name is { });

                    if (containingType.HasProperty(new PropertySignature { Name = propertyOfDateTime.Name, IsStatic = true }))
                    {
                        consumer.AddHighlighting(
                            new UseStaticPropertySuggestion(
                                inspection.Message(propertyOfDateTime.Name),
                                reference,
                                qualifierExpression,
                                invokedExpression,
                                propertyOfDateTime.Name));
                    }

                    break;
                }

                case RangeIndexer rangeIndexer when invocationExpression is { }
                    && !invocationExpression.IsUsedAsStatement()
                    && (rangeIndexer.MinimumLanguageVersion == null
                        || invocationExpression.GetLanguageVersion() >= rangeIndexer.MinimumLanguageVersion)
                    && rangeIndexer.TryGetReplacement(arguments) is { } replacement:
                {
                    consumer.AddHighlighting(
                        new UseRangeIndexerSuggestion(inspection.Message(""), invocationExpression, invokedExpression, replacement));
                    break;
                }
            }
        }
    }

    protected override void Run(IReferenceExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { Reference: var reference, QualifierExpression: { } qualifierExpression })
        {
            switch (reference.Resolve().DeclaredElement)
            {
                case IMethod
                    {
                        AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, ContainingType: { } containingType,
                    } resolvedMethod when element.Parent is IInvocationExpression invocationExpression
                    && RuleDefinitions.TryGetMethod(containingType, resolvedMethod) is { } method
                    && invocationExpression.TryGetArgumentsInDeclarationOrder() is { } arguments:
                {
                    Analyze(
                        consumer,
                        invocationExpression,
                        qualifierExpression,
                        element,
                        method,
                        resolvedMethod.ShortName,
                        containingType,
                        arguments);
                    break;
                }

                case IProperty { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC } resolvedProperty:
                {
                    if (resolvedProperty.ContainingType is { } containingType
                        && RuleDefinitions.TryGetProperty(containingType, resolvedProperty) is { } property)
                    {
                        Analyze(consumer, null, qualifierExpression, element, property, resolvedProperty.ShortName, containingType, []);
                    }
                    break;
                }
            }
        }
    }
}