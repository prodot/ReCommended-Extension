using System.Collections;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.ControlFlow.Impl;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve.Managed;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;
using IBlock = JetBrains.ReSharper.Psi.CSharp.Tree.IBlock;
using IThrowStatement = JetBrains.ReSharper.Psi.CSharp.Tree.IThrowStatement;
using ITryStatement = JetBrains.ReSharper.Psi.CSharp.Tree.ITryStatement;

namespace ReCommendedExtension.Analyzers.ThrowExceptionInUnexpectedLocation;

[ElementProblemAnalyzer(typeof(ICSharpTreeNode), HighlightingTypes = [typeof(ThrowExceptionInUnexpectedLocationWarning)])]
public sealed class ThrowExceptionInUnexpectedLocationAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
{
    sealed class ControlFlowGraphInspectorWithValueAnalysis : CSharpControlFlowGraphInspector
    {
        /// <remarks>
        /// The originally used <see cref="CSharpControlFlowGraphInspector.Inspect()"/> method turns off the value analysis mode (it effectively
        /// replaces the provided <paramref name="analysisMode"/> with <see cref="ValueAnalysisMode.OFF"/> when the nullable warning context is
        /// detected).
        /// </remarks>
        [Pure]
        public static CSharpControlFlowGraphInspector Inspect(ICSharpControlFlowGraph graph, ValueAnalysisMode analysisMode)
        {
            var inspector = new ControlFlowGraphInspectorWithValueAnalysis(
                graph,
                new CSharpControlFlowContextFactory(
                    graph,
                    new UniversalContext((ICSharpTreeNode?)graph.Declaration ?? graph.OwnerNode),
                    analysisMode,
                    false,
                    ExecutionBehavior.InstantExecution));

            inspector.Inspect();

            return inspector;
        }

        ControlFlowGraphInspectorWithValueAnalysis(ICSharpControlFlowGraph graph, CSharpControlFlowContextFactory factory) : base(
            graph,
            factory,
            null) { }
    }

    enum Location
    {
        PropertyGetter,
        IndexerGetter,
        EventAccessor,
        EqualsMethod,
        EqualsMethodWithParameters,
        GetHashCodeMethod,
        GetHashCodeMethodWithParameter,
        ToStringMethod,
        StaticConstructor,
        StaticFieldInitializationExpression,
        StaticPropertyInitializationExpression,
        StaticEventInitializationExpression,
        Finalizer,
        DisposeMethod,
        DisposeAsyncMethod,
        DisposeMethodWithParameterFalseCodePath,
        EqualityOperator,
        ImplicitCastOperator,
        FinallyBlock,
        ExceptionFilterExpression,
    }

    [Pure]
    static IMethod? TryGetMethod(ITypeElement type, string name) => type.Methods.FirstOrDefault(m => m.ShortName == name);

    [Pure]
    static Location? TryGetLocation(ICSharpTreeNode element)
    {
        switch (element.GetContainingFunctionLikeDeclarationOrClosure())
        {
            case IAccessorDeclaration accessorDeclaration:
                switch (accessorDeclaration.Kind)
                {
                    case AccessorKind.GETTER:
                        switch (accessorDeclaration.Parent)
                        {
                            case IPropertyDeclaration: return Location.PropertyGetter;
                            case IIndexerDeclaration: return Location.IndexerGetter;
                        }
                        break;

                    case AccessorKind.ADDER or AccessorKind.REMOVER: return Location.EventAccessor;
                }
                break;

            case IPropertyDeclaration propertyDeclaration:
                if (element.GetContainingNode<IExpressionInitializer>() is { } propertyInitializer
                    && propertyInitializer.GetContainingFunctionLikeDeclarationOrClosure() == propertyDeclaration)
                {
                    if (propertyDeclaration.IsStatic)
                    {
                        return Location.StaticPropertyInitializationExpression;
                    }

                    break; // non-static property initializer (not a getter)
                }

                return Location.PropertyGetter;

            case IIndexerDeclaration: return Location.IndexerGetter;

            case IMethodDeclaration { DeclaredElement: { } } methodDeclaration:
                var psiModule = element.GetPsiModule();

                if (PredefinedType.OBJECT_FQN.TryGetTypeElement(psiModule) is { } objectClass)
                {
                    if (TryGetMethod(objectClass, nameof(Equals)) is { } equalsMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(equalsMethod))
                    {
                        return Location.EqualsMethod;
                    }

                    if (TryGetMethod(objectClass, nameof(GetHashCode)) is { } getHashCodeMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(getHashCodeMethod))
                    {
                        return Location.GetHashCodeMethod;
                    }

                    if (TryGetMethod(objectClass, nameof(ToString)) is { } toStringMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(toStringMethod))
                    {
                        return Location.ToStringMethod;
                    }
                }

                if (PredefinedType.GENERIC_IEQUATABLE_FQN.TryGetTypeElement(psiModule) is { } equatableGenericInterface)
                {
                    if (TryGetMethod(equatableGenericInterface, nameof(IEquatable<>.Equals)) is { } equalsMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(equalsMethod))
                    {
                        return Location.EqualsMethod;
                    }
                }

                if (ClrTypeNames.IEqualityComparerGeneric.TryGetTypeElement(psiModule) is { } equalityComparerGenericInterface)
                {
                    if (TryGetMethod(equalityComparerGenericInterface, nameof(IEqualityComparer<>.Equals)) is { } equalsMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(equalsMethod))
                    {
                        return Location.EqualsMethodWithParameters;
                    }

                    if (TryGetMethod(equalityComparerGenericInterface, nameof(IEqualityComparer<>.GetHashCode)) is { } getHashCodeMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(getHashCodeMethod))
                    {
                        return Location.GetHashCodeMethodWithParameter;
                    }
                }

                if (ClrTypeNames.IEqualityComparer.TryGetTypeElement(psiModule) is { } equalityComparerInterface)
                {
                    if (TryGetMethod(equalityComparerInterface, nameof(IEqualityComparer.Equals)) is { } equalsMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(equalsMethod))
                    {
                        return Location.EqualsMethodWithParameters;
                    }

                    if (TryGetMethod(equalityComparerInterface, nameof(IEqualityComparer.GetHashCode)) is { } getHashCodeMethod
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(getHashCodeMethod))
                    {
                        return Location.GetHashCodeMethodWithParameter;
                    }
                }

                if (methodDeclaration.DeclaredElement.IsDisposeMethod)
                {
                    return Location.DisposeMethod;
                }

                if (methodDeclaration.DeclaredElement.IsDisposeAsyncMethod)
                {
                    return Location.DisposeAsyncMethod;
                }

                if (methodDeclaration.DeclaredElement is { ShortName: nameof(IDisposable.Dispose), TypeParameters: [], Parameters: [{ } parameter] }
                    && parameter.Type.IsBool())
                {
                    var controlFlowGraph = (ICSharpControlFlowGraph?)ControlFlowBuilder.GetGraph(methodDeclaration);

                    if (controlFlowGraph?.GetLeafElementsFor(element).FirstOrDefault()?.Entries.FirstOrDefault() is { } controlFlowEdge)
                    {
                        var inspector = ControlFlowGraphInspectorWithValueAnalysis.Inspect(controlFlowGraph, ValueAnalysisMode.OPTIMISTIC);

                        if (inspector.FindVariableInfo(parameter) is { } variableInfo
                            && inspector.GetContext(controlFlowEdge)?.GetVariableDefiniteState(variableInfo) is null
                                or CSharpControlFlowVariableValue.FALSE)
                        {
                            return Location.DisposeMethodWithParameterFalseCodePath;
                        }
                    }
                }
                break;

            case IConstructorDeclaration { IsStatic: true }: return Location.StaticConstructor;

            case IDestructorDeclaration: return Location.Finalizer;

            case ISignOperatorDeclaration signOperator:
                var tokenType = signOperator.OperatorSign.GetTokenType();
                if (tokenType == CSharpTokenType.EQEQ || tokenType == CSharpTokenType.NE)
                {
                    return Location.EqualityOperator;
                }
                break;

            case IConversionOperatorDeclaration conversionOperatorDeclaration
                when conversionOperatorDeclaration.Modifier.GetTokenType() == CSharpTokenType.IMPLICIT_KEYWORD:
                return Location.ImplicitCastOperator;
        }

        if (!element.IsInsideClosure())
        {
            if (element.GetContainingNode<IFieldDeclaration>() is { IsStatic: true } fieldDeclaration
                && element.GetContainingNode<IExpressionInitializer>() is { } fieldInitializer
                && fieldInitializer.GetContainingNode<IFieldDeclaration>() == fieldDeclaration)
            {
                return Location.StaticFieldInitializationExpression;
            }

            if (element.GetContainingNode<IEventDeclaration>() is { IsStatic: true } eventDeclaration
                && element.GetContainingNode<IExpressionInitializer>() is { } eventInitializer
                && eventInitializer.GetContainingNode<IEventDeclaration>() == eventDeclaration)
            {
                return Location.StaticEventInitializationExpression;
            }
        }

        if (element
                .PathToRoot()
                .TakeWhile(node => node is not IAttributesOwnerDeclaration)
                .FirstOrDefault(node => node is ITryStatement or ICSharpClosure) is ITryStatement tryStatement
            && element.PathToRoot().TakeWhile(node => node != tryStatement).Any(node => node is IBlock block && block == tryStatement.FinallyBlock))
        {
            return Location.FinallyBlock;
        }

        if (element
                .PathToRoot()
                .TakeWhile(node => node is not IAttributesOwnerDeclaration)
                .FirstOrDefault(node => node is IExceptionFilterClause or ICSharpClosure) is IExceptionFilterClause)
        {
            return Location.ExceptionFilterExpression;
        }

        return null;
    }

    [Pure]
    static bool IsOrDerivesFrom(IExpressionType exceptionType, ITypeElement baseExceptionType)
    {
        var exceptionTypeElement = exceptionType.ToIType().GetTypeElement();
        return exceptionTypeElement is { } && exceptionTypeElement.IsDescendantOf(baseExceptionType);
    }

    [Pure]
    static IEnumerable<ITypeElement?> GetAllowedExceptions(Location location, IPsiModule psiModule)
    {
        switch (location)
        {
            case Location.PropertyGetter:
                yield return PredefinedType.INVALIDOPERATIONEXCEPTION_FQN.TryGetTypeElement(psiModule);
                yield return ClrTypeNames.NotSupportedException.TryGetTypeElement(psiModule);
                break;

            case Location.IndexerGetter:
                yield return PredefinedType.ARGUMENTEXCEPTION_FQN.TryGetTypeElement(psiModule);
                yield return ClrTypeNames.KeyNotFoundException.TryGetTypeElement(psiModule);
                goto case Location.PropertyGetter;

            case Location.EventAccessor:
                yield return PredefinedType.INVALIDOPERATIONEXCEPTION_FQN.TryGetTypeElement(psiModule);
                yield return ClrTypeNames.NotSupportedException.TryGetTypeElement(psiModule);
                yield return PredefinedType.ARGUMENTEXCEPTION_FQN.TryGetTypeElement(psiModule);
                break;

            case Location.GetHashCodeMethodWithParameter:
                yield return PredefinedType.ARGUMENTEXCEPTION_FQN.TryGetTypeElement(psiModule);
                break;
        }
    }

    [Pure]
    static string GetText(Location location)
        => location switch
        {
            Location.PropertyGetter or Location.IndexerGetter => "property getters",
            Location.EventAccessor => "event accessors",
            Location.EqualsMethod => $"'{nameof(Equals)}' methods",
            Location.EqualsMethodWithParameters => $"'{nameof(IEquatable<>.Equals)}' methods",
            Location.GetHashCodeMethod => $"'{nameof(GetHashCode)}' methods",
            Location.GetHashCodeMethodWithParameter => $"'{nameof(IEqualityComparer<>.GetHashCode)}' methods",
            Location.ToStringMethod => $"'{nameof(ToString)}' methods",
            Location.StaticConstructor => "static constructors",
            Location.StaticFieldInitializationExpression => "static field initialization expressions",
            Location.StaticPropertyInitializationExpression => "static property initialization expressions",
            Location.StaticEventInitializationExpression => "static event initialization expressions",
            Location.Finalizer => "finalizers",
            Location.DisposeMethod => $"'{nameof(IDisposable.Dispose)}' methods",
            Location.DisposeAsyncMethod => "'DisposeAsync' methods", // todo: use 'nameof(IAsyncDisposable.DisposeAsync)'
            Location.DisposeMethodWithParameterFalseCodePath => $"'{nameof(IDisposable.Dispose)}(false)' code paths",
            Location.EqualityOperator => "equality operators",
            Location.ImplicitCastOperator => "implicit cast operators",
            Location.FinallyBlock => "finally blocks",
            Location.ExceptionFilterExpression => "exception filter expressions",

            _ => throw new NotSupportedException(),
        };

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        var exceptionType = element switch
        {
            IThrowStatement throwStatement => throwStatement.Exception?.GetExpressionType(),
            IThrowExpression throwExpression => throwExpression.Exception.GetExpressionType(),

            _ => null,
        };

        if (exceptionType is { })
        {
            if (ClrTypeNames.UnreachableException.TryGetTypeElement(element.GetPsiModule()) is { } unreachableExceptionType
                && IsOrDerivesFrom(exceptionType, unreachableExceptionType))
            {
                return;
            }

            if (TryGetLocation(element) is { } location)
            {
                if (GetAllowedExceptions(location, element.GetPsiModule()).Any(e => e is { } && IsOrDerivesFrom(exceptionType, e)))
                {
                    return;
                }

                consumer.AddHighlighting(
                    new ThrowExceptionInUnexpectedLocationWarning($"Exceptions should never be thrown in {GetText(location)}.", element));
            }
        }
    }
}