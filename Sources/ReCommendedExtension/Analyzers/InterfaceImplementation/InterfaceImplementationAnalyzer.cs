﻿using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[ElementProblemAnalyzer(
    typeof(IClassLikeDeclaration),
    HighlightingTypes =
    [
        typeof(ImplementEqualityOperatorsForClassesSuggestion),
        typeof(ImplementEqualityOperatorsForStructsSuggestion),
        typeof(ImplementEqualityOperatorsForRecordsSuggestion),
        typeof(ImplementComparisonOperatorsForClassesSuggestion),
        typeof(ImplementComparisonOperatorsForStructsSuggestion),
        typeof(ImplementComparisonOperatorsForRecordsSuggestion),
        typeof(ImplementEquatableWarning),
        typeof(OverrideEqualsWarning),
    ])]
public sealed class InterfaceImplementationAnalyzer : ElementProblemAnalyzer<IClassLikeDeclaration>
{
    [Pure]
    static IEnumerable<IOperator> GetOperators(ITypeElement declaredElement, IDeclaredType type)
        =>
            from op in declaredElement.Operators
            where op.ReturnType.IsBool()
                && op.Parameters is [{ } leftOperand, { } rightOperand]
                && TypeEqualityComparer.Default.Equals(leftOperand.Type, type)
                && TypeEqualityComparer.Default.Equals(rightOperand.Type, type)
            select op;

    [Pure]
    [ValueRange(0, 2)]
    static int CountEqualityOperators(ITypeElement declaredElement, IDeclaredType type)
    {
        var equalityOperator = false;
        var inequalityOperator = false;

        foreach (var op in GetOperators(declaredElement, type))
        {
            switch (op.ShortName)
            {
                case "op_Equality":
                    equalityOperator = true;
                    break;

                case "op_Inequality":
                    inequalityOperator = true;
                    break;
            }
        }

        return (equalityOperator ? 1 : 0) + (inequalityOperator ? 1 : 0);
    }

    [Pure]
    [ValueRange(0, 4)]
    static int CountComparisonOperators(ITypeElement declaredElement, IDeclaredType type)
    {
        var lessThanOperator = false;
        var lessThanOrEqualOperator = false;
        var greaterThanOperator = false;
        var greaterThanOrEqualOperator = false;

        foreach (var op in GetOperators(declaredElement, type))
        {
            switch (op.ShortName)
            {
                case "op_LessThan":
                    lessThanOperator = true;
                    break;

                case "op_LessThanOrEqual":
                    lessThanOrEqualOperator = true;
                    break;

                case "op_GreaterThan":
                    greaterThanOperator = true;
                    break;

                case "op_GreaterThanOrEqual":
                    greaterThanOrEqualOperator = true;
                    break;
            }
        }

        return (lessThanOperator ? 1 : 0) + (lessThanOrEqualOperator ? 1 : 0) + (greaterThanOperator ? 1 : 0) + (greaterThanOrEqualOperator ? 1 : 0);
    }

    static void AnalyzeOperatorInterfaces(IClassLikeDeclaration element, IHighlightingConsumer consumer, IDeclaredType type)
    {
        Debug.Assert(element.DeclaredElement is { });

        if (element.DeclaredElement.GetContainingType() is { })
        {
            return; // ignore nested types
        }

        if (element.GetCSharpLanguageLevel() < CSharpLanguageLevel.CSharp110)
        {
            return;
        }

        var psiModule = element.GetPsiModule();

        if (ClrTypeNames.IEqualityOperators.TryGetTypeElement(psiModule) is { } equalityOperatorsInterface
            && ClrTypeNames.IComparisonOperators.TryGetTypeElement(psiModule) is { } comparisonOperatorsInterface)
        {
            var (declaresEquatable, declaresEqualityOperators, declaresComparable, declaresComparisonOperators) = GetInterfaces(
                element.DeclaredElement,
                type,
                equalityOperatorsInterface,
                comparisonOperatorsInterface,
                PredefinedType.GENERIC_ICOMPARABLE_FQN.TryGetTypeElement(psiModule));

            switch (element)
            {
                case IClassDeclaration classDeclaration:

                    if (declaresEquatable && !declaresEqualityOperators)
                    {
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var name = type.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new ImplementEqualityOperatorsForClassesSuggestion(
                                CountEqualityOperators(element.DeclaredElement, type) switch
                                {
                                    0 => $"Implement IEqualityOperators<{name}, {name}, bool> interface.",
                                    1 => $"Declare IEqualityOperators<{name}, {name}, bool> interface (operators partially available).",
                                    2 => $"Declare IEqualityOperators<{name}, {name}, bool> interface (operators available).",

                                    _ => throw new NotSupportedException(),
                                },
                                classDeclaration));
                    }

                    if (declaresComparable && !declaresComparisonOperators)
                    {
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var name = type.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new ImplementComparisonOperatorsForClassesSuggestion(
                                CountComparisonOperators(element.DeclaredElement, type) switch
                                {
                                    0 => $"Implement IComparisonOperators<{name}, {name}, bool> interface.",
                                    >= 1 and <= 3 => $"Declare IComparisonOperators<{name}, {name}, bool> interface (operators partially available).",
                                    4 => $"Declare IComparisonOperators<{name}, {name}, bool> interface (operators available).",

                                    _ => throw new NotSupportedException(),
                                },
                                classDeclaration));
                    }

                    break;

                case IStructDeclaration structDeclaration:

                    if (declaresEquatable && !declaresEqualityOperators)
                    {
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var name = type.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new ImplementEqualityOperatorsForStructsSuggestion(
                                CountEqualityOperators(element.DeclaredElement, type) switch
                                {
                                    0 => $"Implement IEqualityOperators<{name}, {name}, bool> interface.",
                                    1 => $"Declare IEqualityOperators<{name}, {name}, bool> interface (operators partially available).",
                                    2 => $"Declare IEqualityOperators<{name}, {name}, bool> interface (operators available).",

                                    _ => throw new NotSupportedException(),
                                },
                                structDeclaration));
                    }

                    if (declaresComparable && !declaresComparisonOperators)
                    {
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var name = type.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new ImplementComparisonOperatorsForStructsSuggestion(
                                CountComparisonOperators(element.DeclaredElement, type) switch
                                {
                                    0 => $"Implement IComparisonOperators<{name}, {name}, bool> interface.",
                                    >= 1 and <= 3 => $"Declare IComparisonOperators<{name}, {name}, bool> interface (operators partially available).",
                                    4 => $"Declare IComparisonOperators<{name}, {name}, bool> interface (operators available).",

                                    _ => throw new NotSupportedException(),
                                },
                                structDeclaration));
                    }

                    break;

                case IRecordDeclaration recordDeclaration:

                    if (!declaresEqualityOperators)
                    {
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var name = type.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new ImplementEqualityOperatorsForRecordsSuggestion(
                                $"Declare IEqualityOperators<{name}, {name}, bool> interface (operators available).",
                                recordDeclaration));
                    }

                    if (declaresComparable && !declaresComparisonOperators)
                    {
                        Debug.Assert(CSharpLanguage.Instance is { });

                        var name = type.GetPresentableName(CSharpLanguage.Instance);

                        consumer.AddHighlighting(
                            new ImplementComparisonOperatorsForRecordsSuggestion(
                                CountComparisonOperators(element.DeclaredElement, type) switch
                                {
                                    0 => $"Implement IComparisonOperators<{name}, {name}, bool> interface.",
                                    >= 1 and <= 3 => $"Declare IComparisonOperators<{name}, {name}, bool> interface (operators partially available).",
                                    4 => $"Declare IComparisonOperators<{name}, {name}, bool> interface (operators available).",

                                    _ => throw new NotSupportedException(),
                                },
                                recordDeclaration));
                    }

                    break;
            }
        }
    }

    static void AnalyzeEquatableInterface(IClassLikeDeclaration element, IHighlightingConsumer consumer, IDeclaredType type)
    {
        Debug.Assert(element.DeclaredElement is { });

        if (element is IClassDeclaration or IStructDeclaration)
        {
            var (declaresEquatable, _, _, _) = GetInterfaces(element.DeclaredElement, type, null, null, null);

            var overridesEquals = element.DeclaredElement.Methods.Any(
                method => method is { ShortName: nameof(IEquatable<int>.Equals), IsOverride: true, TypeParameters: [], Parameters: [{ } parameter] }
                    && method.ReturnType.IsBool()
                    && parameter.Type.IsObject());

            if (!declaresEquatable && overridesEquals && element is IStructDeclaration structDeclaration)
            {
                Debug.Assert(CSharpLanguage.Instance is { });

                var name = type.GetPresentableName(CSharpLanguage.Instance);

                consumer.AddHighlighting(
                    new ImplementEquatableWarning($"Implement IEquatable<{name}> when overriding Equals.", structDeclaration));
            }

            if (declaresEquatable && !overridesEquals)
            {
                Debug.Assert(CSharpLanguage.Instance is { });

                var name = type.GetPresentableName(CSharpLanguage.Instance);

                consumer.AddHighlighting(
                    new OverrideEqualsWarning($"Override Equals when implementing IEquatable<{name}>.", element));
            }
        }
    }

    [Pure]
    internal static (bool declaresEquatable, bool declaresEqualityOperators, bool declaresComparable, bool declaresComparisonOperators) GetInterfaces(
        ITypeElement declaredElement,
        IType type,
        ITypeElement? equalityOperatorsInterface,
        ITypeElement? comparisonOperatorsInterface,
        ITypeElement? comparableInterface)
    {
        var declaresEquatable = false;
        var declaresEqualityOperators = false;
        var declaresComparable = false;
        var declaresComparisonOperators = false;

        foreach (var baseType in declaredElement.GetAllSuperTypes())
        {
            if (baseType.IsObject())
            {
                continue;
            }

            if (!declaresEquatable
                && baseType.IsIEquatable()
                && baseType.TryGetGenericParameterTypes() is [{ } equatableType]
                && TypeEqualityComparer.Default.Equals(equatableType, type))
            {
                declaresEquatable = true;
                continue;
            }

            if (!declaresEqualityOperators
                && DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                && baseType.TryGetGenericParameterTypes() is [{ } leftEqualityOperandType, { } rightEqualityOperandType, { } equalityResultType]
                && TypeEqualityComparer.Default.Equals(leftEqualityOperandType, type)
                && TypeEqualityComparer.Default.Equals(rightEqualityOperandType, type)
                && equalityResultType.IsBool())
            {
                declaresEqualityOperators = true;
                continue;
            }

            if (!declaresComparable
                && DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), comparableInterface)
                && baseType.TryGetGenericParameterTypes() is [{ } comparableType]
                && TypeEqualityComparer.Default.Equals(comparableType, type))
            {
                declaresComparable = true;
                continue;
            }

            if (!declaresComparisonOperators
                && DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), comparisonOperatorsInterface)
                && baseType.TryGetGenericParameterTypes() is [{ } leftComparisonOperandType, { } rightComparisonOperandType, { } comparisonResultType]
                && TypeEqualityComparer.Default.Equals(leftComparisonOperandType, type)
                && TypeEqualityComparer.Default.Equals(rightComparisonOperandType, type)
                && comparisonResultType.IsBool())
            {
                declaresComparisonOperators = true;
            }
        }

        return (declaresEquatable, declaresEqualityOperators, declaresComparable, declaresComparisonOperators);
    }

    protected override void Run(IClassLikeDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.DeclaredElement is { })
        {
            var type = TypeFactory.CreateType(element.DeclaredElement);

            AnalyzeOperatorInterfaces(element, consumer, type);

            AnalyzeEquatableInterface(element, consumer, type);
        }
    }
}