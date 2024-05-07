using System.Collections;
using JetBrains.Metadata.Reader.API;
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
using JetBrains.ReSharper.Psi.Util;

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
            var element = (ICSharpTreeNode?)graph.Declaration ?? graph.OwnerNode;
            var universalContext = new UniversalContext(element);
            var factory = new CSharpControlFlowContextFactory(graph, universalContext, analysisMode, false, ExecutionBehavior.InstantExecution);
            var inspector = new ControlFlowGraphInspectorWithValueAnalysis(graph, factory);

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
    }

    [Pure]
    static IMethod GetMethod(ITypeElement type, string name) => type.Methods.First(m => m.ShortName == name);

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

                var objectClass = PredefinedType.OBJECT_FQN.TryGetTypeElement(psiModule);
                Debug.Assert(objectClass is { });

                if (methodDeclaration.DeclaredElement.OverridesOrImplements(GetMethod(objectClass, nameof(Equals))))
                {
                    return Location.EqualsMethod;
                }
                if (methodDeclaration.DeclaredElement.OverridesOrImplements(GetMethod(objectClass, nameof(GetHashCode))))
                {
                    return Location.GetHashCodeMethod;
                }
                if (methodDeclaration.DeclaredElement.OverridesOrImplements(GetMethod(objectClass, nameof(ToString))))
                {
                    return Location.ToStringMethod;
                }

                var equatableGenericInterface = PredefinedType.GENERIC_IEQUATABLE_FQN.TryGetTypeElement(psiModule);
                Debug.Assert(equatableGenericInterface is { });

                if (methodDeclaration.DeclaredElement.OverridesOrImplements(GetMethod(equatableGenericInterface, nameof(IEquatable<int>.Equals))))
                {
                    return Location.EqualsMethod;
                }

                var equalityComparerGenericInterface = ClrTypeNames.IEqualityComparerGeneric.TryGetTypeElement(psiModule);
                Debug.Assert(equalityComparerGenericInterface is { });

                if (methodDeclaration.DeclaredElement.OverridesOrImplements(
                    GetMethod(equalityComparerGenericInterface, nameof(IEqualityComparer<int>.Equals))))
                {
                    return Location.EqualsMethodWithParameters;
                }
                if (methodDeclaration.DeclaredElement.OverridesOrImplements(
                    GetMethod(equalityComparerGenericInterface, nameof(IEqualityComparer<int>.GetHashCode))))
                {
                    return Location.GetHashCodeMethodWithParameter;
                }

                var equalityComparerInterface = ClrTypeNames.IEqualityComparer.TryGetTypeElement(psiModule);
                Debug.Assert(equalityComparerInterface is { });

                if (methodDeclaration.DeclaredElement.OverridesOrImplements(
                    GetMethod(equalityComparerInterface, nameof(IEqualityComparer.Equals))))
                {
                    return Location.EqualsMethodWithParameters;
                }
                if (methodDeclaration.DeclaredElement.OverridesOrImplements(
                    GetMethod(equalityComparerInterface, nameof(IEqualityComparer.GetHashCode))))
                {
                    return Location.GetHashCodeMethodWithParameter;
                }

                if (methodDeclaration.DeclaredElement.IsDisposeMethod())
                {
                    return Location.DisposeMethod;
                }

                if (methodDeclaration.DeclaredElement.IsDisposeAsyncMethod())
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

                        if (inspector.FindVariableInfo(parameter) is { } variableInfo)
                        {
                            var variableValue = inspector.GetContext(controlFlowEdge)?.GetVariableDefiniteState(variableInfo);
                            if (variableValue is not { } or CSharpControlFlowVariableValue.FALSE)
                            {
                                return Location.DisposeMethodWithParameterFalseCodePath;
                            }
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
            if (element.GetContainingNode<IFieldDeclaration>() is { IsStatic: true } fieldDeclaration)
            {
                var initializer = element.GetContainingNode<IExpressionInitializer>();
                if (initializer is { } && initializer.GetContainingNode<IFieldDeclaration>() == fieldDeclaration)
                {
                    return Location.StaticFieldInitializationExpression;
                }
            }

            if (element.GetContainingNode<IEventDeclaration>() is { IsStatic: true } eventDeclaration)
            {
                var initializer = element.GetContainingNode<IExpressionInitializer>();
                if (initializer is { } && initializer.GetContainingNode<IEventDeclaration>() == eventDeclaration)
                {
                    return Location.StaticEventInitializationExpression;
                }
            }
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
    static IEnumerable<ITypeElement> GetAllowedExceptions(Location location, IPsiModule psiModule)
    {
        ITypeElement GetTypeElementByClrName(IClrTypeName clrTypeName)
        {
            var typeElement = clrTypeName.TryGetTypeElement(psiModule);
            Debug.Assert(typeElement is { });

            return typeElement;
        }

        switch (location)
        {
            case Location.PropertyGetter:
                yield return GetTypeElementByClrName(PredefinedType.INVALIDOPERATIONEXCEPTION_FQN);
                yield return GetTypeElementByClrName(ClrTypeNames.NotSupportedException);
                break;

            case Location.IndexerGetter:
                yield return GetTypeElementByClrName(PredefinedType.ARGUMENTEXCEPTION_FQN);
                yield return GetTypeElementByClrName(ClrTypeNames.KeyNotFoundException);
                goto case Location.PropertyGetter;

            case Location.EventAccessor:
                yield return GetTypeElementByClrName(PredefinedType.INVALIDOPERATIONEXCEPTION_FQN);
                yield return GetTypeElementByClrName(ClrTypeNames.NotSupportedException);
                yield return GetTypeElementByClrName(PredefinedType.ARGUMENTEXCEPTION_FQN);
                break;

            case Location.GetHashCodeMethodWithParameter:
                yield return GetTypeElementByClrName(PredefinedType.ARGUMENTEXCEPTION_FQN);
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
            Location.EqualsMethodWithParameters => $"'{nameof(IEquatable<int>.Equals)}' methods",
            Location.GetHashCodeMethod => $"'{nameof(GetHashCode)}' methods",
            Location.GetHashCodeMethodWithParameter => $"'{nameof(IEqualityComparer<int>.GetHashCode)}' methods",
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

            _ => throw new NotSupportedException(),
        };

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        IExpressionType? exceptionType;
        switch (element)
        {
            case IThrowStatement throwStatement:
                exceptionType = throwStatement.Exception?.GetExpressionType();
                break;

            case IThrowExpression throwExpression:
                exceptionType = throwExpression.Exception.GetExpressionType();
                break;

            default: return;
        }

        if (exceptionType is not { })
        {
            return;
        }

        if (ClrTypeNames.UnreachableException.TryGetTypeElement(element.GetPsiModule()) is { } unreachableExceptionType
            && IsOrDerivesFrom(exceptionType, unreachableExceptionType))
        {
            return;
        }

        if (TryGetLocation(element) is { } location)
        {
            if (GetAllowedExceptions(location, element.GetPsiModule()).Any(e => IsOrDerivesFrom(exceptionType, e)))
            {
                return;
            }

            consumer.AddHighlighting(
                new ThrowExceptionInUnexpectedLocationWarning($"Exceptions should never be thrown in {GetText(location)}.", element));
        }
    }
}