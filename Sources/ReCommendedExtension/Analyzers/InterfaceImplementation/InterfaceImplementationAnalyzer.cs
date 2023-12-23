using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[ElementProblemAnalyzer(
    typeof(IClassLikeDeclaration),
    HighlightingTypes =
    [
        typeof(ImplementEqualityOperatorsForClassesSuggestion), typeof(ImplementEqualityOperatorsForStructsSuggestion),
        typeof(ImplementEqualityOperatorsForRecordsSuggestion), typeof(ImplementComparisonOperatorsForClassesSuggestion),
        typeof(ImplementComparisonOperatorsForStructsSuggestion), typeof(ImplementComparisonOperatorsForRecordsSuggestion),
        typeof(ImplementEquatableWarning), typeof(OverrideEqualsWarning),
    ])]
public sealed class InterfaceImplementationAnalyzer : ElementProblemAnalyzer<IClassLikeDeclaration>
{
    [Pure]
    static IEnumerable<IOperator> GetOperators(ITypeElement declaredElement, IDeclaredType type)
        =>
            from op in declaredElement.Operators
            where op.ReturnType.IsBool()
                && op.Parameters is [{ } leftOperand, { } rightOperand]
                && leftOperand.Type.Equals(type)
                && rightOperand.Type.Equals(type)
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

        if (element.GetContainingClassLikeDeclaration() is { })
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
                element,
                type,
                equalityOperatorsInterface,
                comparisonOperatorsInterface,
                PredefinedType.GENERIC_ICOMPARABLE_FQN.TryGetTypeElement(psiModule));

            switch (element)
            {
                case IClassDeclaration classDeclaration:

                    if (declaresEquatable && !declaresEqualityOperators)
                    {
                        var name = classDeclaration.DeclaredName;

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
                        var name = classDeclaration.DeclaredName;

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
                        var name = structDeclaration.DeclaredName;

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
                        var name = structDeclaration.DeclaredName;

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
                        var name = recordDeclaration.DeclaredName;

                        consumer.AddHighlighting(
                            new ImplementEqualityOperatorsForRecordsSuggestion(
                                $"Declare IEqualityOperators<{name}, {name}, bool> interface (operators available).",
                                recordDeclaration));
                    }

                    if (declaresComparable && !declaresComparisonOperators)
                    {
                        var name = recordDeclaration.DeclaredName;

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
            var (declaresEquatable, _, _, _) = GetInterfaces(element, type, null, null, null);

            var overridesEquals = element.DeclaredElement.Methods.Any(
                method => method is { ShortName: nameof(IEquatable<int>.Equals), IsOverride: true, TypeParameters: [], Parameters: [{ } parameter] }
                    && method.ReturnType.IsBool()
                    && parameter.Type.IsObject());

            if (!declaresEquatable && overridesEquals && element is IStructDeclaration structDeclaration)
            {
                consumer.AddHighlighting(
                    new ImplementEquatableWarning($"Implement IEquatable<{element.DeclaredName}> when overriding Equals.", structDeclaration));
            }

            if (declaresEquatable && !overridesEquals)
            {
                consumer.AddHighlighting(
                    new OverrideEqualsWarning($"Override Equals when implementing IEquatable<{element.DeclaredName}>.", element));
            }
        }
    }

    [Pure]
    internal static (bool declaresEquatable, bool declaresEqualityOperators, bool declaresComparable, bool declaresComparisonOperators) GetInterfaces(
        IClassLikeDeclaration declaration,
        IDeclaredType type,
        ITypeElement? equalityOperatorsInterface,
        ITypeElement? comparisonOperatorsInterface,
        ITypeElement? comparableInterface)
    {
        var declaresEquatable = false;
        var declaresEqualityOperators = false;
        var declaresComparable = false;
        var declaresComparisonOperators = false;

        foreach (var baseType in declaration.SuperTypes)
        {
            if (baseType.IsIEquatable() && baseType.TryGetGenericParameterTypes() is [{ } equatableType] && equatableType.Equals(type))
            {
                declaresEquatable = true;
                continue;
            }

            if (DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                && baseType.TryGetGenericParameterTypes() is [{ } leftEqualityOperandType, { } rightEqualityOperandType, { } equalityResultType]
                && leftEqualityOperandType.Equals(type)
                && rightEqualityOperandType.Equals(type)
                && equalityResultType.IsBool())
            {
                declaresEqualityOperators = true;
                continue;
            }

            if (DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), comparableInterface)
                && baseType.TryGetGenericParameterTypes() is [{ } comparableType]
                && comparableType.Equals(type))
            {
                declaresComparable = true;
                continue;
            }

            if (DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), comparisonOperatorsInterface)
                && baseType.TryGetGenericParameterTypes() is [{ } leftComparisonOperandType, { } rightComparisonOperandType, { } comparisonResultType]
                && leftComparisonOperandType.Equals(type)
                && rightComparisonOperandType.Equals(type)
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