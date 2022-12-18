using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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

namespace ReCommendedExtension.Analyzers.ThrowExceptionInUnexpectedLocation
{
    [ElementProblemAnalyzer(typeof(ICSharpTreeNode), HighlightingTypes = new[] { typeof(ThrowExceptionInUnexpectedLocationWarning) })]
    public sealed class ThrowExceptionInUnexpectedLocationAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
    {
        sealed class ControlFlowGraphInspectorWithValueAnalysis : CSharpControlFlowGraphInspector
        {
            /// <remarks>
            /// The originally used
            /// <see cref="CSharpControlFlowGraphInspector.Inspect(ICSharpControlFlowGraph,ValueAnalysisMode,Func{ITreeNode,ICSharpControlFlowGraph})"/>
            /// method turns off the value analysis mode (it effectively replaces the provided <paramref name="analysisMode"/> with
            /// <see cref="ValueAnalysisMode.OFF"/> when the nullable warning context is detected).
            /// </remarks>
            [NotNull]
            public static CSharpControlFlowGraphInspector Inspect([NotNull] ICSharpControlFlowGraph graph, ValueAnalysisMode analysisMode)
            {
                var element = (ICSharpTreeNode)graph.Declaration ?? graph.OwnerNode;
                var universalContext = new UniversalContext(element);
                var factory = new CSharpControlFlowContextFactory(graph, universalContext, analysisMode, ExecutionBehavior.InstantExecution);
                var inspector = new ControlFlowGraphInspectorWithValueAnalysis(graph, factory);

                inspector.Inspect();

                return inspector;
            }

            ControlFlowGraphInspectorWithValueAnalysis(
                [NotNull] ICSharpControlFlowGraph graph,
                [NotNull] CSharpControlFlowContextFactory factory) : base(graph, factory, null) { }
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

        [NotNull]
        static IMethod GetMethod([NotNull] ITypeElement type, [NotNull] string name) => type.Methods.First(m => m.ShortName == name);

        static Location? TryGetLocation([NotNull] ICSharpTreeNode element)
        {
            switch (element.GetContainingFunctionLikeDeclarationOrClosure())
            {
                case IAccessorDeclaration accessorDeclaration:
                    switch (accessorDeclaration.Kind)
                    {
                        case AccessorKind.GETTER:
                            switch (accessorDeclaration.Parent)
                            {
                                case IPropertyDeclaration _: return Location.PropertyGetter;

                                case IIndexerDeclaration _: return Location.IndexerGetter;
                            }
                            break;

                        case AccessorKind.ADDER:
                        case AccessorKind.REMOVER:
                            return Location.EventAccessor;
                    }
                    break;

                case IPropertyDeclaration propertyDeclaration:
                    var propertyInitializer = element.GetContainingNode<IExpressionInitializer>();
                    if (propertyInitializer != null && propertyInitializer.GetContainingFunctionLikeDeclarationOrClosure() == propertyDeclaration)
                    {
                        if (propertyDeclaration.IsStatic)
                        {
                            return Location.StaticPropertyInitializationExpression;
                        }

                        break; // non-static property initializer (not a getter)
                    }

                    return Location.PropertyGetter;

                case IIndexerDeclaration _: return Location.IndexerGetter;

                case IMethodDeclaration methodDeclaration when methodDeclaration.DeclaredElement != null:
                    var psiModule = element.GetPsiModule();

                    var objectClass = TypeElementUtil.GetTypeElementByClrName(PredefinedType.OBJECT_FQN, psiModule).AssertNotNull();
                    if (methodDeclaration.DeclaredElement.OverridesOrImplements(GetMethod(objectClass, nameof(object.Equals))))
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

                    if (methodDeclaration.DeclaredElement.OverridesOrImplements(
                        GetMethod(
                            TypeElementUtil.GetTypeElementByClrName(PredefinedType.GENERIC_IEQUATABLE_FQN, psiModule).AssertNotNull(),
                            nameof(IEquatable<int>.Equals))))
                    {
                        return Location.EqualsMethod;
                    }

                    var equalityComparerGenericInterface = TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.IEqualityComparerGeneric, psiModule)
                        .AssertNotNull();
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

                    var equalityComparerInterface =
                        TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.IEqualityComparer, psiModule).AssertNotNull();
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

                    if (methodDeclaration.DeclaredElement.OverridesOrImplements(
                        GetMethod(
                            TypeElementUtil.GetTypeElementByClrName(PredefinedType.IDISPOSABLE_FQN, psiModule).AssertNotNull(),
                            nameof(IDisposable.Dispose))))
                    {
                        return Location.DisposeMethod;
                    }

                    var iAsyncDisposableTypeElement = TypeElementUtil.GetTypeElementByClrName(PredefinedType.IASYNCDISPOSABLE_FQN, psiModule);
                    if (iAsyncDisposableTypeElement != null
                        && methodDeclaration.DeclaredElement.OverridesOrImplements(GetMethod(iAsyncDisposableTypeElement, "DisposeAsync")))
                    {
                        // todo: use 'nameof(IAsyncDisposable.DisposeAsync)'
                        return Location.DisposeAsyncMethod;
                    }

                    if (methodDeclaration.DeclaredElement.ShortName == nameof(IDisposable.Dispose)
                        && methodDeclaration.DeclaredElement.Parameters.Count == 1)
                    {
                        var parameter = methodDeclaration.DeclaredElement.Parameters[0];
                        if (parameter != null && parameter.Type.IsBool())
                        {
                            var controlFlowGraph = (ICSharpControlFlowGraph)ControlFlowBuilder.GetGraph(methodDeclaration);

                            var controlFlowEdge = controlFlowGraph?.GetLeafElementsFor(element).FirstOrDefault()?.Entries.FirstOrDefault();
                            if (controlFlowEdge != null)
                            {
                                var inspector = ControlFlowGraphInspectorWithValueAnalysis.Inspect(controlFlowGraph, ValueAnalysisMode.OPTIMISTIC);

                                var variableInfo = inspector.FindVariableInfo(parameter);
                                if (variableInfo != null)
                                {
                                    var variableValue = inspector.GetContext(controlFlowEdge)?.GetVariableDefiniteState(variableInfo);
                                    if (variableValue == null || variableValue == CSharpControlFlowVariableValue.FALSE)
                                    {
                                        return Location.DisposeMethodWithParameterFalseCodePath;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case IConstructorDeclaration constructorDeclaration when constructorDeclaration.IsStatic: return Location.StaticConstructor;

                case IDestructorDeclaration _: return Location.Finalizer;

                case ISignOperatorDeclaration signOperator:
                    var tokenType = signOperator.OperatorSign?.GetTokenType();
                    if (tokenType == CSharpTokenType.EQEQ || tokenType == CSharpTokenType.NE)
                    {
                        return Location.EqualityOperator;
                    }
                    break;

                case IConversionOperatorDeclaration conversionOperatorDeclaration
                    when conversionOperatorDeclaration.Modifier?.GetTokenType() == CSharpTokenType.IMPLICIT_KEYWORD:
                    return Location.ImplicitCastOperator;
            }

            if (!element.IsInsideClosure())
            {
                var fieldDeclaration = element.GetContainingNode<IFieldDeclaration>();
                if (fieldDeclaration != null && fieldDeclaration.IsStatic)
                {
                    var initializer = element.GetContainingNode<IExpressionInitializer>();
                    if (initializer != null && initializer.GetContainingNode<IFieldDeclaration>() == fieldDeclaration)
                    {
                        return Location.StaticFieldInitializationExpression;
                    }
                }

                var eventDeclaration = element.GetContainingNode<IEventDeclaration>();
                if (eventDeclaration != null && eventDeclaration.IsStatic)
                {
                    var initializer = element.GetContainingNode<IExpressionInitializer>();
                    if (initializer != null && initializer.GetContainingNode<IEventDeclaration>() == eventDeclaration)
                    {
                        return Location.StaticEventInitializationExpression;
                    }
                }
            }

            return null;
        }

        static bool IsOrDerivesFrom([NotNull] IExpressionType exceptionType, [NotNull] ITypeElement baseExceptionType)
        {
            var exceptionTypeElement = exceptionType.ToIType().GetTypeElement();
            return exceptionTypeElement != null && exceptionTypeElement.IsDescendantOf(baseExceptionType);
        }

        [NotNull]
        [ItemNotNull]
        static IEnumerable<ITypeElement> GetAllowedExceptions(Location location, [NotNull] IPsiModule psiModule)
        {
            switch (location)
            {
                case Location.PropertyGetter:
                    yield return TypeElementUtil.GetTypeElementByClrName(PredefinedType.INVALIDOPERATIONEXCEPTION_FQN, psiModule).AssertNotNull();
                    yield return TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.NotSupportedException, psiModule).AssertNotNull();
                    break;

                case Location.IndexerGetter:
                    yield return TypeElementUtil.GetTypeElementByClrName(PredefinedType.ARGUMENTEXCEPTION_FQN, psiModule).AssertNotNull();
                    yield return TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.KeyNotFoundException, psiModule).AssertNotNull();
                    goto case Location.PropertyGetter;

                case Location.EventAccessor:
                    yield return TypeElementUtil.GetTypeElementByClrName(PredefinedType.INVALIDOPERATIONEXCEPTION_FQN, psiModule).AssertNotNull();
                    yield return TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.NotSupportedException, psiModule).AssertNotNull();
                    yield return TypeElementUtil.GetTypeElementByClrName(PredefinedType.ARGUMENTEXCEPTION_FQN, psiModule).AssertNotNull();
                    break;

                case Location.GetHashCodeMethodWithParameter:
                    yield return TypeElementUtil.GetTypeElementByClrName(PredefinedType.ARGUMENTEXCEPTION_FQN, psiModule).AssertNotNull();
                    break;
            }
        }

        [NotNull]
        static string GetText(Location location)
        {
            switch (location)
            {
                case Location.PropertyGetter:
                case Location.IndexerGetter:
                    return "property getters";

                case Location.EventAccessor: return @"event accessors";

                case Location.EqualsMethod: return $"'{nameof(object.Equals)}' methods";

                case Location.EqualsMethodWithParameters: return $"'{nameof(IEquatable<int>.Equals)}' methods";

                case Location.GetHashCodeMethod: return $"'{nameof(GetHashCode)}' methods";

                case Location.GetHashCodeMethodWithParameter: return $"'{nameof(IEqualityComparer<int>.GetHashCode)}' methods";

                case Location.ToStringMethod: return $"'{nameof(ToString)}' methods";

                case Location.StaticConstructor: return "static constructors";

                case Location.StaticFieldInitializationExpression: return "static field initialization expressions";

                case Location.StaticPropertyInitializationExpression: return "static property initialization expressions";

                case Location.StaticEventInitializationExpression: return "static event initialization expressions";

                case Location.Finalizer: return "finalizers";

                case Location.DisposeMethod: return $"'{nameof(IDisposable.Dispose)}' methods";

                case Location.DisposeAsyncMethod: return "'DisposeAsync' methods"; // todo: use 'nameof(IAsyncDisposable.DisposeAsync)'

                case Location.DisposeMethodWithParameterFalseCodePath: return $"'{nameof(IDisposable.Dispose)}(false)' code paths";

                case Location.EqualityOperator: return "equality operators";

                case Location.ImplicitCastOperator: return "implicit cast operators";

                default: throw new NotSupportedException();
            }
        }

        protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            IExpressionType exceptionType;
            switch (element)
            {
                case IThrowStatement throwStatement:
                    exceptionType = throwStatement.Exception?.GetExpressionType();
                    break;

                case IThrowExpression throwExpression:
                    exceptionType = throwExpression.Exception?.GetExpressionType();
                    break;

                default: return;
            }

            if (exceptionType == null)
            {
                return;
            }

            var unreachableExceptionTypeElement = TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.UnreachableException, element.GetPsiModule());
            if (unreachableExceptionTypeElement != null && IsOrDerivesFrom(exceptionType, unreachableExceptionTypeElement))
            {
                return;
            }

            var location = TryGetLocation(element);
            if (location != null)
            {
                if (GetAllowedExceptions((Location)location, element.GetPsiModule()).Any(e => IsOrDerivesFrom(exceptionType, e)))
                {
                    return;
                }

                consumer.AddHighlighting(
                    new ThrowExceptionInUnexpectedLocationWarning($"Exceptions should never be thrown in {GetText((Location)location)}.", element));
            }
        }
    }
}