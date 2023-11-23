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
                            $"Declare implementation of the IEqualityOperators<{classDeclaration.DeclaredName}, {classDeclaration.DeclaredName}, bool> interface.",
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
                            $"Declare implementation of the IEqualityOperators<{structDeclaration.DeclaredName}, {structDeclaration.DeclaredName}, bool> interface.",
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
                            $"Declare implementation of the IEqualityOperators<{recordDeclaration.DeclaredName}, {recordDeclaration.DeclaredName}, bool> interface.",
                            recordDeclaration));

                    break;
            }
        }
    }
}