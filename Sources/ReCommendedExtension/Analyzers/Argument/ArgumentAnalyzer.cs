using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.Argument.Inspections;
using ReCommendedExtension.Analyzers.Argument.Rules;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;
using ConstructorSignature = ReCommendedExtension.Extensions.MethodFinding.ConstructorSignature;
using MethodSignature = ReCommendedExtension.Extensions.MethodFinding.MethodSignature;

namespace ReCommendedExtension.Analyzers.Argument;

[ElementProblemAnalyzer(
    typeof(ICSharpExpression),
    HighlightingTypes =
    [
        typeof(RedundantArgumentHint), typeof(RedundantArgumentRangeHint), typeof(RedundantElementHint), typeof(UseOtherArgumentSuggestion),
    ])]
public sealed class ArgumentAnalyzer : ElementProblemAnalyzer<ICSharpExpression>
{
    delegate bool HasMember(IReadOnlyList<Parameter> parameters, bool returnParameterNames, out string[] parameterNames);

    static void Analyze(
        IHighlightingConsumer consumer,
        Member member,
        int? parameterIndexOfParams,
        TreeNodeCollection<ICSharpArgument?> arguments,
        HasMember hasMember)
    {
        foreach (var inspection in member.Inspections)
        {
            switch (inspection)
            {
                case RedundantArgumentByPosition redundantArgumentByPosition:
                {
                    Debug.Assert(redundantArgumentByPosition.ParameterIndex >= 0);

                    if (arguments[redundantArgumentByPosition.ParameterIndex] is { } argument
                        && redundantArgumentByPosition.Condition(argument)
                        && (redundantArgumentByPosition.FurtherCondition == null || redundantArgumentByPosition.FurtherCondition(arguments))
                        && hasMember(
                            redundantArgumentByPosition.ReplacementSignatureParameters
                            ?? member.Signature.Parameters.WithoutElementAt(redundantArgumentByPosition.ParameterIndex),
                            false,
                            out _))
                    {
                        consumer.AddHighlighting(new RedundantArgumentHint(inspection.Message, argument));
                    }
                    break;
                }

                case DuplicateArgument duplicateArgument:
                {
                    foreach (var argument in duplicateArgument.Selector(arguments))
                    {
                        consumer.AddHighlighting(new RedundantArgumentHint(inspection.Message, argument));
                    }
                    break;
                }

                case RedundantArgumentRange redundantArgumentRange when arguments.AsPositionalArguments() is [_, _, ..] positionalArguments:
                {
                    var args = positionalArguments.GetSubrange(redundantArgumentRange.ParameterIndexRange);

                    if (redundantArgumentRange.Condition(args)
                        && hasMember(member.Signature.Parameters.WithoutElementsAt(redundantArgumentRange.ParameterIndexRange), false, out _))
                    {
                        consumer.AddHighlighting(new RedundantArgumentRangeHint(inspection.Message, args));
                    }
                    break;
                }

                case RedundantCollectionElement redundantCollectionElement:
                {
                    Debug.Assert(redundantCollectionElement.ParameterIndex >= 0);

                    if (arguments[redundantCollectionElement.ParameterIndex] is { } argument
                        && (parameterIndexOfParams is not { } paramsIndex
                            || redundantCollectionElement.ParameterIndex == paramsIndex && paramsIndex < arguments.Count)
                        && CollectionCreation.TryFrom(argument.Value) is { Count: > 1 } collectionCreation)
                    {
                        switch (redundantCollectionElement)
                        {
                            case DuplicateCollectionElement duplicateCollectionElement:
                                foreach (var initializerElement in duplicateCollectionElement.Selector(collectionCreation))
                                {
                                    consumer.AddHighlighting(new RedundantElementHint(inspection.Message, initializerElement));
                                }
                                break;

                            case DuplicateEquivalentCollectionElement duplicateEquivalentCollectionElement:
                                foreach (var (initializerElement, isEquivalent) in duplicateEquivalentCollectionElement.Selector(collectionCreation))
                                {
                                    consumer.AddHighlighting(
                                        new RedundantElementHint(
                                            isEquivalent ? duplicateEquivalentCollectionElement.MessageEquivalentElement : inspection.Message,
                                            initializerElement));
                                }
                                break;
                        }
                    }
                    break;
                }

                case OtherArgument otherArgument:
                {
                    Debug.Assert(
                        otherArgument is
                        {
                            ParameterIndex: >= 0,
                            FurtherArgumentCondition: not { ParameterIndex: < 0 },
                            ReplacementSignature: not { ParameterIndex: < 0 },
                        });

                    if (arguments[otherArgument.ParameterIndex] is { } argument
                        && otherArgument.TryGetReplacement(argument) is { } replacement
                        && (otherArgument.FurtherArgumentCondition == null
                            || arguments[otherArgument.FurtherArgumentCondition.ParameterIndex] is { } furtherArgument
                            && otherArgument.FurtherArgumentCondition.Condition(furtherArgument)))
                    {
                        if (otherArgument.ReplacementSignature is { })
                        {
                            if (hasMember(otherArgument.ReplacementSignature.Parameters, argument.NameIdentifier is { }, out var parameterNames))
                            {
                                consumer.AddHighlighting(
                                    new UseOtherArgumentSuggestion(
                                        inspection.Message,
                                        argument,
                                        otherArgument.ReplacementSignature.Parameters[otherArgument.ReplacementSignature.ParameterIndex].Kind,
                                        argument.NameIdentifier is { } ? parameterNames[otherArgument.ReplacementSignature.ParameterIndex] : null,
                                        replacement));
                            }
                        }
                        else
                        {
                            consumer.AddHighlighting(
                                new UseOtherArgumentSuggestion(
                                    inspection.Message,
                                    argument,
                                    member.Signature.Parameters[otherArgument.ParameterIndex].Kind,
                                    argument.NameIdentifier?.Name,
                                    replacement));
                        }
                    }

                    break;
                }
            }
        }
    }

    protected override void Run(ICSharpExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IObjectCreationExpression { ConstructorReference: var reference } objectCreationExpression
                when reference.Resolve().DeclaredElement is IConstructor
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    IsStatic: false,
                    ContainingType: { } containingType,
                } resolvedConstructor
                && RuleDefinitions.TryGetConstructor(containingType, resolvedConstructor, out var parameterIndexOfParams) is { } constructor
                && objectCreationExpression.TryGetArgumentsInDeclarationOrder() is [_, ..] arguments:
            {
                Analyze(
                    consumer,
                    constructor,
                    parameterIndexOfParams,
                    arguments,
                    (IReadOnlyList<Parameter> parameters, bool returnParameterNames, out string[] parameterNames) => containingType.HasConstructor(
                        new ConstructorSignature { Parameters = parameters },
                        returnParameterNames,
                        out parameterNames));
                break;
            }

            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression
                    {
                        QualifierExpression: { } qualifierExpression, Reference: var reference,
                    } invokedExpression,
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, ContainingType: { } containingType,
                } resolvedMethod
                && RuleDefinitions.TryGetMethod(containingType, resolvedMethod, out var parameterIndexOfParams) is { } method
                && invocationExpression.TryGetArgumentsInDeclarationOrder() is [_, ..] arguments:
            {
                Debug.Assert(method.Signature is Rules.MethodSignature);

                var signature = (Rules.MethodSignature)method.Signature;

                Analyze(
                    consumer,
                    method,
                    parameterIndexOfParams,
                    arguments,
                    (IReadOnlyList<Parameter> parameters, bool returnParameterNames, out string[] parameterNames) => containingType.HasMethod(
                        new MethodSignature
                        {
                            Name = resolvedMethod.ShortName,
                            Parameters = parameters,
                            IsStatic = signature.IsStatic,
                            GenericParametersCount = signature.GenericParametersCount,
                        },
                        returnParameterNames,
                        out parameterNames));
                break;
            }
        }
    }
}