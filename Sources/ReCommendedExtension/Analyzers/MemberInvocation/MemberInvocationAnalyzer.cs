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
        typeof(SuspiciousElementAccessWarning),
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
        bool isExtension,
        ITypeElement containingType,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        foreach (var inspection in member.Inspections)
        {
            switch (inspection)
            {
                case RedundantMethodInvocation redundantMethodInvocation when invocationExpression is { }
                    && (!redundantMethodInvocation.IsPureMethod || !invocationExpression.IsUsedAsStatement)
                    && redundantMethodInvocation.Condition(qualifier, arguments)
                    && (!redundantMethodInvocation.EnsureFirstArgumentNotNull
                        || arguments is [{ Value: { } firstArgValue }, ..]
                        && firstArgValue.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)):
                {
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(inspection.Message(memberName))
                        {
                            InvocationExpression = invocationExpression, InvokedExpression = invokedExpression,
                        });

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
                        var highlighting = new UseOtherMethodSuggestion(inspection.Message(otherMethodInvocation.ReplacementMethod.Name))
                        {
                            Qualifier = qualifier,
                            ReplacedMethodInvocation = new ReplacedMethodInvocation
                            {
                                Name = otherMethodInvocation.ReplacementMethod.Name, Replacement = replacement,
                            },
                        };

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

                case BinaryOperator binaryOperator when invocationExpression is { IsUsedAsStatement: false }:
                {
                    Debug.Assert(binaryOperator.Operator is { });

                    if (binaryOperator.TryGetOperands(qualifier, arguments) is { } operands)
                    {
                        consumer.AddHighlighting(
                            new UseBinaryOperatorSuggestion(
                                inspection.Message(binaryOperator.Operator),
                                binaryOperator.HighlightInvokedMethodOnly ? invokedExpression : null)
                            {
                                InvocationExpression = invocationExpression, Operands = operands, Operator = binaryOperator.Operator,
                            });
                    }
                    break;
                }

                case UnaryOperator unaryOperator when invocationExpression is { IsUsedAsStatement: false }:
                {
                    Debug.Assert(unaryOperator.Operator is { });

                    if (unaryOperator.TryGetOperand(qualifier, arguments) is { } operand)
                    {
                        consumer.AddHighlighting(
                            new UseUnaryOperatorSuggestion(inspection.Message(unaryOperator.Operator))
                            {
                                InvocationExpression = invocationExpression, Operand = operand, Operator = unaryOperator.Operator,
                            });
                    }
                    break;
                }

                case PatternByArgument pattern when invocationExpression is { IsUsedAsStatement: false }:
                {
                    Debug.Assert(pattern is { MinimumLanguageVersion: { }, ParameterIndex: >= 0, Pattern: { } });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion)
                    {
                        var argument = arguments[pattern.ParameterIndex];

                        if (argument is { Value: { } value })
                        {
                            consumer.AddHighlighting(
                                new UsePatternSuggestion(inspection.Message(""), invokedExpression)
                                {
                                    Expression = invocationExpression,
                                    Replacement = new PatternReplacement { Expression = value, Pattern = $"is {pattern.Pattern}" },
                                });
                        }
                    }
                    break;
                }

                case PatternByArguments pattern when invocationExpression is { IsUsedAsStatement: false }:
                {
                    Debug.Assert(pattern.MinimumLanguageVersion is { });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion
                        && pattern.TryGetReplacement(arguments) is { } replacement)
                    {
                        consumer.AddHighlighting(
                            new UsePatternSuggestion(inspection.Message(""), invokedExpression)
                            {
                                Expression = invocationExpression, Replacement = replacement,
                            });
                    }
                    break;
                }

                case PatternByQualifierArguments pattern when invocationExpression is { IsUsedAsStatement: false }:
                {
                    Debug.Assert(pattern.MinimumLanguageVersion is { });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion
                        && (!pattern.EnsureExtensionInvokedAsExtension || !isExtension || invocationExpression.IsInvokedAsExtension())
                        && (!pattern.EnsureNoTypeArguments || invocationExpression.TypeArguments is [])
                        && pattern.TryGetReplacement(qualifier, arguments) is { } replacement
                        && (!pattern.EnsureQualifierNotNull || qualifier.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)))
                    {
                        consumer.AddHighlighting(
                            new UsePatternSuggestion(inspection.Message(""), invokedExpression)
                            {
                                Expression = invocationExpression, Replacement = replacement,
                            });
                    }

                    break;
                }

                case PatternByBinaryExpression pattern when invocationExpression is { Parent: IBinaryExpression binaryExpression }:
                {
                    Debug.Assert(pattern.MinimumLanguageVersion is { });

                    if (invocationExpression.GetLanguageVersion() >= pattern.MinimumLanguageVersion
                        && pattern.TryGetReplacement(invocationExpression, qualifier, arguments) is { } replacement)
                    {
                        consumer.AddHighlighting(
                            new UsePatternSuggestion(inspection.Message(""), invokedExpression)
                            {
                                Expression = binaryExpression, Replacement = replacement,
                            });
                    }

                    break;
                }

                case PropertyOfNullable propertyOfNullable when invokedExpression is { IsPropertyAssignment: false, IsWithinNameofExpression: false }
                    && (!propertyOfNullable.EnsureQualifierNotValueTuple || !qualifier.Type().Unlift().IsValueTuple(out _)):
                {
                    var highlighting = propertyOfNullable.Name switch
                    {
                        PropertyNameOfNullable.HasValue => new UseNullableHasValueAlternativeSuggestion(inspection.Message(""))
                        {
                            ReferenceExpression = invokedExpression,
                        },
                        PropertyNameOfNullable.Value => new ReplaceNullableValueWithTypeCastSuggestion(inspection.Message(""))
                        {
                            ReferenceExpression = invokedExpression,
                        },

                        _ => null as Highlighting,
                    };

                    if (highlighting is { })
                    {
                        consumer.AddHighlighting(highlighting);
                    }
                    break;
                }

                case PropertyOfString propertyOfString when invocationExpression is { IsUsedAsStatement: false }
                    && (propertyOfString.MinimumFrameworkVersion == null
                        || invocationExpression.PsiModule.TargetFrameworkId.Version >= propertyOfString.MinimumFrameworkVersion)
                    && (!propertyOfString.EnsureExtensionInvokedAsExtension || !isExtension || invocationExpression.IsInvokedAsExtension())
                    && propertyOfString.Condition(arguments):
                {
                    Debug.Assert(propertyOfString.Name is { });

                    var propertyTypeElement = propertyOfString.EnsureExtensionInvokedAsExtension ? qualifier.Type().GetTypeElement() : containingType;

                    if (propertyTypeElement is { } && propertyTypeElement.HasProperty(new PropertySignature { Name = propertyOfString.Name }))
                    {
                        consumer.AddHighlighting(
                            new UsePropertySuggestion(inspection.Message(propertyOfString.Name))
                            {
                                InvocationExpression = invocationExpression,
                                InvokedExpression = invokedExpression,
                                PropertyName = propertyOfString.Name,
                                EnsureTargetType = propertyOfString.EnsureTargetType,
                            });
                    }
                    break;
                }

                case PropertyOfArray propertyOfArray when invocationExpression is { IsUsedAsStatement: false }
                    && (!propertyOfArray.EnsureExtensionInvokedAsExtension || !isExtension || invocationExpression.IsInvokedAsExtension())
                    && propertyOfArray.Condition(arguments):
                {
                    Debug.Assert(propertyOfArray.Name is { });

                    // intentionally skipping the check if the property exists

                    consumer.AddHighlighting(
                        new UsePropertySuggestion(inspection.Message(propertyOfArray.Name))
                        {
                            InvocationExpression = invocationExpression,
                            InvokedExpression = invokedExpression,
                            PropertyName = propertyOfArray.Name,
                            EnsureTargetType = propertyOfArray.EnsureTargetType,
                        });
                    break;
                }

                case PropertyOfCollection propertyOfCollection when invocationExpression is { IsUsedAsStatement: false }
                    && (!propertyOfCollection.EnsureExtensionInvokedAsExtension || !isExtension || invocationExpression.IsInvokedAsExtension())
                    && propertyOfCollection.Condition(arguments):
                {
                    Debug.Assert(propertyOfCollection.Name is { });

                    // intentionally skipping the check if the property exists

                    consumer.AddHighlighting(
                        new UsePropertySuggestion(inspection.Message(propertyOfCollection.Name))
                        {
                            InvocationExpression = invocationExpression,
                            InvokedExpression = invokedExpression,
                            PropertyName = propertyOfCollection.Name,
                            EnsureTargetType = propertyOfCollection.EnsureTargetType,
                        });
                    break;
                }

                case PropertyOfDateTime propertyOfDateTime when invokedExpression is
                    {
                        IsPropertyAssignment: false,
                        IsWithinNameofExpression: false,
                        QualifierExpression: IReferenceExpression { Reference: var reference, QualifierExpression: var qualifierExpression },
                    }
                    && propertyOfDateTime.Condition(reference):
                {
                    Debug.Assert(propertyOfDateTime.Name is { });

                    if (containingType.HasProperty(new PropertySignature { Name = propertyOfDateTime.Name, IsStatic = true }))
                    {
                        consumer.AddHighlighting(
                            new UseStaticPropertySuggestion(inspection.Message(propertyOfDateTime.Name), reference)
                            {
                                QualifierExpression = qualifierExpression,
                                ReferenceExpression = invokedExpression,
                                PropertyName = propertyOfDateTime.Name,
                            });
                    }

                    break;
                }

                case RangeIndexer rangeIndexer when invocationExpression is { IsUsedAsStatement: false }
                    && (rangeIndexer.MinimumLanguageVersion == null
                        || invocationExpression.GetLanguageVersion() >= rangeIndexer.MinimumLanguageVersion)
                    && (!rangeIndexer.EnsureExtensionInvokedAsExtension || !isExtension || invocationExpression.IsInvokedAsExtension())
                    && (!rangeIndexer.EnsureNoTypeArguments || invocationExpression.TypeArguments is [])
                    && rangeIndexer.TryGetReplacement(arguments) is { } replacement:
                {
                    consumer.AddHighlighting(
                        new UseRangeIndexerSuggestion(inspection.Message(""))
                        {
                            InvocationExpression = invocationExpression, InvokedExpression = invokedExpression, Replacement = replacement,
                        });
                    break;
                }

                case SuspiciousElementAccess suspiciousElementAccess when invocationExpression is { } && suspiciousElementAccess.Condition(arguments):
                {
                    consumer.AddHighlighting(new SuspiciousElementAccessWarning(inspection.Message(""), invocationExpression, invokedExpression));
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
                        resolvedMethod.GetExtensionMemberKind() is var kind
                        && (kind == ExtensionMemberKind.CLASSIC_METHOD || kind == ExtensionMemberKind.INSTANCE_METHOD),
                        containingType,
                        arguments);
                    break;
                }

                case IProperty { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC } resolvedProperty:
                {
                    if (resolvedProperty.ContainingType is { } containingType
                        && RuleDefinitions.TryGetProperty(containingType, resolvedProperty) is { } property)
                    {
                        Analyze(
                            consumer,
                            null,
                            qualifierExpression,
                            element,
                            property,
                            resolvedProperty.ShortName,
                            resolvedProperty.GetExtensionMemberKind() == ExtensionMemberKind.INSTANCE_PROPERTY,
                            containingType,
                            []);
                    }
                    break;
                }
            }
        }
    }
}