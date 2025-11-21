using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.Argument.Inspections;
using ReCommendedExtension.Analyzers.Argument.Rules;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.Collections;
using ReCommendedExtension.Extensions.MemberFinding;
using ConstructorSignature = ReCommendedExtension.Extensions.MemberFinding.ConstructorSignature;
using MethodSignature = ReCommendedExtension.Extensions.MemberFinding.MethodSignature;

namespace ReCommendedExtension.Analyzers.Argument;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(ICSharpInvocationInfo),
    HighlightingTypes =
    [
        typeof(RedundantArgumentHint),
        typeof(RedundantArgumentRangeHint),
        typeof(RedundantElementHint),
        typeof(UseOtherArgumentSuggestion),
        typeof(UseOtherArgumentRangeSuggestion),
    ])]
public sealed class ArgumentAnalyzer : ElementProblemAnalyzer<ICSharpInvocationInfo>
{
    delegate bool HasConstructor(IReadOnlyList<Parameter> parameters, bool returnParameterNames, out string[] parameterNames);

    delegate bool HasMethod(
        IReadOnlyList<Parameter> parameters,
        [NonNegativeValue] int genericParametersCount,
        bool returnParameterNames,
        out string[] parameterNames);

    static void Analyze(
        IHighlightingConsumer consumer,
        Member member,
        IList<IParameter> resolvedParameters,
        TreeNodeCollection<ICSharpArgument?> arguments,
        HasConstructor hasConstructor,
        HasMethod hasMethod)
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
                        && (redundantArgumentByPosition.FurtherCondition == null || redundantArgumentByPosition.FurtherCondition(arguments)))
                    {
                        var replacementSignatureParameters = redundantArgumentByPosition.ReplacementSignatureParameters
                            ?? member.Signature.Parameters.WithoutElementAt(redundantArgumentByPosition.ParameterIndex);

                        var memberExists = member.Signature switch
                        {
                            Rules.ConstructorSignature => hasConstructor(replacementSignatureParameters, false, out _),

                            Rules.MethodSignature methodSignature => hasMethod(
                                replacementSignatureParameters,
                                methodSignature.GenericParametersCount,
                                false,
                                out _),

                            _ => throw new NotSupportedException(),
                        };

                        if (memberExists)
                        {
                            consumer.AddHighlighting(new RedundantArgumentHint(inspection.Message, argument));
                        }
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

                case RedundantArgumentRange redundantArgumentRange when arguments.AsAllNonOptionalOrNull() is [_, _, ..] positionalArguments:
                {
                    var redundantArguments = positionalArguments.GetSubrange(redundantArgumentRange.ParameterIndexRange);

                    if (redundantArgumentRange.Condition(redundantArguments))
                    {
                        var replacementSignatureParameters =
                            member.Signature.Parameters.WithoutElementsAt(redundantArgumentRange.ParameterIndexRange);

                        var memberExists = member.Signature switch
                        {
                            Rules.ConstructorSignature => hasConstructor(replacementSignatureParameters, false, out _),

                            Rules.MethodSignature methodSignature => hasMethod(
                                replacementSignatureParameters,
                                methodSignature.GenericParametersCount,
                                false,
                                out _),

                            _ => throw new NotSupportedException(),
                        };

                        if (memberExists)
                        {
                            var highlighting = new RedundantArgumentRangeHint(inspection.Message, redundantArguments);

                            foreach (var argument in redundantArguments)
                            {
                                consumer.AddHighlighting(highlighting, argument.GetDocumentRange());
                            }
                        }
                    }
                    break;
                }

                case RedundantCollectionElement redundantCollectionElement:
                {
                    Debug.Assert(redundantCollectionElement.ParameterIndex >= 0);

                    if (arguments[redundantCollectionElement.ParameterIndex] is { } argument
                        && (!resolvedParameters[^1].IsParams || resolvedParameters.Count == arguments.Count)
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
                    Debug.Assert(otherArgument is { ParameterIndex: >= 0, FurtherArgumentCondition: not { ParameterIndex: < 0 } });

                    if (arguments[otherArgument.ParameterIndex] is { } argument
                        && otherArgument.TryGetReplacement(argument) is { } replacement
                        && (otherArgument.FurtherArgumentCondition == null
                            || arguments[otherArgument.FurtherArgumentCondition.ParameterIndex] is { } furtherArgument
                            && otherArgument.FurtherArgumentCondition.Condition(furtherArgument)))
                    {
                        if (otherArgument.ReplacementSignature is { })
                        {
                            Debug.Assert(
                                member.Signature is Rules.ConstructorSignature && otherArgument.ReplacementSignature.GenericParametersCount == null
                                || member.Signature is Rules.MethodSignature
                                && otherArgument.ReplacementSignature.GenericParametersCount is null or >= 0);

                            string[] parameterNames;

                            var memberExists = member.Signature switch
                            {
                                Rules.ConstructorSignature => hasConstructor(
                                    otherArgument.ReplacementSignature.Parameters,
                                    argument.NameIdentifier is { },
                                    out parameterNames),

                                Rules.MethodSignature methodSignature => hasMethod(
                                    otherArgument.ReplacementSignature.Parameters,
                                    otherArgument.ReplacementSignature.GenericParametersCount ?? methodSignature.GenericParametersCount,
                                    argument.NameIdentifier is { },
                                    out parameterNames),

                                _ => throw new NotSupportedException(),
                            };

                            if (memberExists)
                            {
                                consumer.AddHighlighting(
                                    new UseOtherArgumentSuggestion(
                                        inspection.Message,
                                        new ArgumentReplacement
                                        {
                                            Argument = argument,
                                            Replacement = new UpcomingArgument
                                            {
                                                ParameterKind =
                                                    otherArgument.ReplacementSignature
                                                        .Parameters[otherArgument.ReplacementSignature.ParameterIndex].Kind,
                                                ParameterName =
                                                    argument.NameIdentifier is { }
                                                        ? parameterNames[otherArgument.ReplacementSignature.ParameterIndex]
                                                        : null,
                                                Value = replacement,
                                            },
                                        },
                                        otherArgument.AdditionalArgument is { }
                                            ? new UpcomingArgument
                                            {
                                                ParameterKind =
                                                    otherArgument.ReplacementSignature
                                                        .Parameters[otherArgument.ReplacementSignature.ParameterIndex + 1].Kind,
                                                ParameterName =
                                                    argument.NameIdentifier is { }
                                                        ? parameterNames[otherArgument.ReplacementSignature.ParameterIndex + 1]
                                                        : null,
                                                Value = otherArgument.AdditionalArgument,
                                            }
                                            : null,
                                        otherArgument.RedundantArgumentIndex is { } redundantArgumentIndex
                                            ? arguments[redundantArgumentIndex]
                                            : null));
                            }
                        }
                        else
                        {
                            consumer.AddHighlighting(
                                new UseOtherArgumentSuggestion(
                                    inspection.Message,
                                    new ArgumentReplacement
                                    {
                                        Argument = argument,
                                        Replacement =
                                            new UpcomingArgument
                                            {
                                                ParameterKind = member.Signature.Parameters[otherArgument.ParameterIndex].Kind,
                                                ParameterName = argument.NameIdentifier?.Name,
                                                Value = replacement,
                                            },
                                    },
                                    otherArgument.AdditionalArgument is { }
                                        ? new UpcomingArgument
                                        {
                                            ParameterKind = member.Signature.Parameters[otherArgument.ParameterIndex + 1].Kind,
                                            ParameterName =
                                                argument.NameIdentifier is { }
                                                    ? resolvedParameters[otherArgument.ParameterIndex + 1].ShortName
                                                    : null,
                                            Value = otherArgument.AdditionalArgument,
                                        }
                                        : null,
                                    otherArgument.RedundantArgumentIndex is { } redundantArgumentIndex ? arguments[redundantArgumentIndex] : null));
                        }
                    }

                    break;
                }

                case OtherArgumentRange otherArgumentRange when arguments.AsAllNonOptionalOrNull() is [_, _, ..] positionalArguments:
                {
                    Debug.Assert(otherArgumentRange.FurtherArgumentCondition is not { ParameterIndex: < 0 });

                    var otherArguments = positionalArguments.GetSubrange(otherArgumentRange.ParameterIndexRange);

                    var highlighting = null as UseOtherArgumentRangeSuggestion;

                    if (otherArgumentRange.TryGetReplacements(otherArguments) is [_, _, ..] replacements
                        && (otherArgumentRange.FurtherArgumentCondition == null
                            || otherArgumentRange.FurtherArgumentCondition.Condition(
                                positionalArguments[otherArgumentRange.FurtherArgumentCondition.ParameterIndex])))
                    {
                        if (otherArgumentRange.ReplacementSignature is { })
                        {
                            string[] parameterNames;

                            var memberExists = member.Signature switch
                            {
                                Rules.ConstructorSignature => hasConstructor(
                                    otherArgumentRange.ReplacementSignature.Parameters,
                                    otherArguments.Any(arg => arg.NameIdentifier is { }),
                                    out parameterNames),

                                Rules.MethodSignature methodSignature => hasMethod(
                                    otherArgumentRange.ReplacementSignature.Parameters,
                                    methodSignature.GenericParametersCount,
                                    otherArguments.Any(arg => arg.NameIdentifier is { }),
                                    out parameterNames),

                                _ => throw new NotSupportedException(),
                            };

                            if (memberExists)
                            {
                                var (offset, _) = parameterNames is [_, ..]
                                    ? otherArgumentRange.ReplacementSignature.ParameterIndexRange.GetOffsetAndLength(parameterNames.Length)
                                    : (null as int?, null as int?);

                                Debug.Assert(otherArguments.All(arg => arg.NameIdentifier == null) || offset is { });

                                highlighting = new UseOtherArgumentRangeSuggestion(
                                    inspection.Message,
                                    [
                                        ..
                                        from i in Enumerable.Range(0, replacements.Count)
                                        select new ArgumentReplacement
                                        {
                                            Argument = otherArguments[i],
                                            Replacement = new UpcomingArgument
                                            {
                                                ParameterKind = otherArgumentRange.ReplacementSignature.Parameters[i].Kind,
                                                ParameterName = otherArguments[i].NameIdentifier is { } ? parameterNames[i + (int)offset!] : null,
                                                Value = replacements[i],
                                            },
                                        },
                                    ],
                                    otherArgumentRange.RedundantArgumentIndex is { } redundantArgumentIndex
                                        ? positionalArguments[redundantArgumentIndex]
                                        : null);
                            }
                        }
                        else
                        {
                            var (offset, _) = otherArgumentRange.ParameterIndexRange.GetOffsetAndLength(member.Signature.Parameters.Count);

                            highlighting = new UseOtherArgumentRangeSuggestion(
                                inspection.Message,
                                [
                                    ..
                                    from i in Enumerable.Range(0, replacements.Count)
                                    select new ArgumentReplacement
                                    {
                                        Argument = otherArguments[i],
                                        Replacement = new UpcomingArgument
                                        {
                                            ParameterKind = member.Signature.Parameters[i + offset].Kind,
                                            ParameterName = otherArguments[i].NameIdentifier?.Name,
                                            Value = replacements[i],
                                        },
                                    },
                                ],
                                otherArgumentRange.RedundantArgumentIndex is { } redundantArgumentIndex
                                    ? positionalArguments[redundantArgumentIndex]
                                    : null);
                        }
                    }

                    if (highlighting is { })
                    {
                        foreach (var argument in otherArguments)
                        {
                            consumer.AddHighlighting(highlighting, argument.GetDocumentRange());
                        }
                    }

                    break;
                }
            }
        }
    }

    protected override void Run(ICSharpInvocationInfo element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
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
                && RuleDefinitions.TryGetConstructor(containingType, resolvedConstructor) is { } constructor
                && objectCreationExpression.TryGetArgumentsInDeclarationOrder() is [_, ..] arguments:
            {
                Analyze(
                    consumer,
                    constructor,
                    resolvedConstructor.Parameters,
                    arguments,
                    (IReadOnlyList<Parameter> parameters, bool returnParameterNames, out string[] parameterNames) => containingType.HasConstructor(
                        new ConstructorSignature { Parameters = parameters },
                        returnParameterNames,
                        out parameterNames),
                    (IReadOnlyList<Parameter> _, int _, bool _, out string[] _) => throw new NotSupportedException());
                break;
            }

            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference },
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, ContainingType: { } containingType,
                } resolvedMethod
                && RuleDefinitions.TryGetMethod(containingType, resolvedMethod) is { } method
                && invocationExpression.TryGetArgumentsInDeclarationOrder() is [_, ..] arguments:
            {
                Debug.Assert(method.Signature is Rules.MethodSignature);

                var signature = (Rules.MethodSignature)method.Signature;

                Analyze(
                    consumer,
                    method,
                    resolvedMethod.Parameters,
                    arguments,
                    (IReadOnlyList<Parameter> _, bool _, out string[] _) => throw new NotSupportedException(),
                    (IReadOnlyList<Parameter> parameters, int genericParametersCount, bool returnParameterNames, out string[] parameterNames)
                        => containingType.HasMethod(
                            new MethodSignature
                            {
                                Name = resolvedMethod.ShortName,
                                Parameters = parameters,
                                IsStatic = signature.IsStatic,
                                GenericParametersCount = genericParametersCount,
                            },
                            returnParameterNames,
                            out parameterNames));
                break;
            }
        }
    }
}