using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[ElementProblemAnalyzer(
    typeof(IClassLikeDeclaration),
    HighlightingTypes = new[]
    {
        typeof(ImplementEqualityOperatorsForClassesSuggestion), typeof(ImplementEqualityOperatorsForStructsSuggestion),
        typeof(ImplementEqualityOperatorsForRecordsSuggestion),
    })]
public sealed class EquatableAnalyzer : ElementProblemAnalyzer<IClassLikeDeclaration>
{
    [Pure]
    [ValueRange(0, 2)]
    static int CountEqualityOperators(ITypeElement declaredElement, IDeclaredType type)
    {
        var equalityOperator = false;
        var inequalityOperator = false;

        foreach (var op in declaredElement.Operators)
        {
            if (op.ReturnType.IsBool()
                && op.Parameters is [{ } leftOperand, { } rightOperand]
                && leftOperand.Type.Equals(type)
                && rightOperand.Type.Equals(type))
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
        }

        return (equalityOperator ? 1 : 0) + (inequalityOperator ? 1 : 0);
    }

    [Pure]
    internal static ITypeElement? TryGetEqualityOperatorsInterface(IPsiModule psiModule)
        => TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.IEqualityOperators, psiModule);

    protected override void Run(IClassLikeDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.GetContainingClassLikeDeclaration() is { })
        {
            return; // ignore nested types
        }

        if (element.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && TryGetEqualityOperatorsInterface(element.GetPsiModule()) is { } equalityOperatorsInterface
            && element.DeclaredElement is { })
        {
            var type = TypeFactory.CreateType(element.DeclaredElement);

            switch (element)
            {
                case IClassDeclaration classDeclaration

                    // check if the class implements IEquatable<T> where T is the class
                    when classDeclaration.SuperTypes.Any(
                        baseType => baseType.IsIEquatable()
                            && baseType.TryGetGenericParameterTypes() is [{ } equatableType]
                            && equatableType.Equals(type))

                    // check if the class doesn't implement IEqualityOperators<T,T,bool> where T is the class
                    && !classDeclaration.SuperTypes.Any(
                        baseType => DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                            && baseType.TryGetGenericParameterTypes() is [{ } leftOperandType, { } rightOperandType, { } resultType]
                            && leftOperandType.Equals(type)
                            && rightOperandType.Equals(type)
                            && resultType.IsBool()):

                    consumer.AddHighlighting(
                        new ImplementEqualityOperatorsForClassesSuggestion(
                            CountEqualityOperators(element.DeclaredElement,type) switch
                            {
                                0 => $"Implement IEqualityOperators<{classDeclaration.DeclaredName}, {classDeclaration.DeclaredName}, bool> interface.",
                                1 => $"Declare IEqualityOperators<{classDeclaration.DeclaredName}, {classDeclaration.DeclaredName}, bool> interface (operators partially available).",
                                2 => $"Declare IEqualityOperators<{classDeclaration.DeclaredName}, {classDeclaration.DeclaredName}, bool> interface (operators available).",

                                _ => throw new NotSupportedException(),
                            },
                            classDeclaration));

                    break;

                case IStructDeclaration structDeclaration

                    // check if the struct implements IEquatable<T> where T is the struct
                    when structDeclaration.SuperTypes.Any(
                        baseType => baseType.IsIEquatable()
                            && baseType.TryGetGenericParameterTypes() is [{ } equatableType]
                            && equatableType.Equals(type))

                    // check if the struct doesn't implement IEqualityOperators<T,T,bool> where T is the struct
                    && !structDeclaration.SuperTypes.Any(
                        baseType => DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                            && baseType.TryGetGenericParameterTypes() is [{ } leftOperandType, { } rightOperandType, { } resultType]
                            && leftOperandType.Equals(type)
                            && rightOperandType.Equals(type)
                            && resultType.IsBool()):

                    consumer.AddHighlighting(
                        new ImplementEqualityOperatorsForStructsSuggestion(
                            CountEqualityOperators(element.DeclaredElement, type) switch
                            {
                                0 => $"Implement IEqualityOperators<{structDeclaration.DeclaredName}, {structDeclaration.DeclaredName}, bool> interface.",
                                1 => $"Declare IEqualityOperators<{structDeclaration.DeclaredName}, {structDeclaration.DeclaredName}, bool> interface (operators partially available).",
                                2 => $"Declare IEqualityOperators<{structDeclaration.DeclaredName}, {structDeclaration.DeclaredName}, bool> interface (operators available).",

                                _ => throw new NotSupportedException(),
                            },
                            structDeclaration));

                    break;

                case IRecordDeclaration recordDeclaration

                    // check if the record doesn't implement IEqualityOperators<T,T,bool> where T is the record
                    when !recordDeclaration.SuperTypes.Any(
                        baseType => DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                            && baseType.TryGetGenericParameterTypes() is [{ } leftOperandType, { } rightOperandType, { } resultType]
                            && leftOperandType.Equals(type)
                            && rightOperandType.Equals(type)
                            && resultType.IsBool()):

                    consumer.AddHighlighting(
                        new ImplementEqualityOperatorsForRecordsSuggestion(
                            $"Declare IEqualityOperators<{recordDeclaration.DeclaredName}, {recordDeclaration.DeclaredName}, bool> interface (operators available).",
                            recordDeclaration));

                    break;
            }
        }
    }
}