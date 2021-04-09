using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Await
{
    [ElementProblemAnalyzer(
        typeof(IAwaitExpression),
        HighlightingTypes = new[] { typeof(RedundantAwaitSuggestion), typeof(RedundantCapturedContextSuggestion) })]
    public sealed class AwaitAnalyzer : ElementProblemAnalyzer<IAwaitExpression>
    {
        [NotNull]
        [ItemNotNull]
        static readonly IClrTypeName[] testMethodAttributes =
        {
            new ClrTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute"),
            new ClrTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute"),
            new ClrTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute"),
            new ClrTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.AssemblyInitializeAttribute"),
            new ClrTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute"),
            new ClrTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute"),
            new ClrTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.AssemblyCleanupAttribute"),
        };

        [Pure]
        [ContractAnnotation("=> type: notnull, removeAsync: notnull", true)]
        static void TryGetContainerTypeAndAsyncKeyword(
            [NotNull] IParametersOwnerDeclaration container,
            [CanBeNull] out IType type,
            [CanBeNull] out ITokenNode asyncKeyword,
            [CanBeNull] out Action removeAsync,
            [CanBeNull] out IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            switch (container)
            {
                case IMethodDeclaration methodDeclaration:
                    type = methodDeclaration.Type;
                    asyncKeyword = methodDeclaration.ModifiersList?.Modifiers.FirstOrDefault(
                        node => node?.GetTokenType() == CSharpTokenType.ASYNC_KEYWORD);
                    removeAsync = () => methodDeclaration.SetAsync(false);
                    attributesOwnerDeclaration =
                        type.IsValueTask() || type.IsGenericValueTask() || methodDeclaration.IsNullableAnnotationsContextEnabled()
                            ? null
                            : methodDeclaration;
                    return;

                case ILambdaExpression lambdaExpression:
                    type = lambdaExpression.ReturnType;
                    asyncKeyword = lambdaExpression.AsyncKeyword;
                    removeAsync = () => lambdaExpression.SetAsync(false);
                    attributesOwnerDeclaration = null;
                    return;

                case IAnonymousMethodExpression anonymousMethodExpression:
                    type = anonymousMethodExpression.ReturnType;
                    asyncKeyword = anonymousMethodExpression.AsyncKeyword;
                    removeAsync = () => anonymousMethodExpression.SetAsync(false);
                    attributesOwnerDeclaration = null;
                    return;

                case ILocalFunctionDeclaration localFunctionDeclaration:
                    type = localFunctionDeclaration.Type;
                    asyncKeyword = localFunctionDeclaration.ModifiersList?.Modifiers.FirstOrDefault(
                        node => node?.GetTokenType() == CSharpTokenType.ASYNC_KEYWORD);
                    removeAsync = () => localFunctionDeclaration.SetAsync(false);
                    attributesOwnerDeclaration = null;
                    return;

                default:
                    type = null;
                    asyncKeyword = null;
                    removeAsync = null;
                    attributesOwnerDeclaration = null;
                    return;
            }
        }

        [NotNull]
        [ItemNotNull]
        static IEnumerable<ICSharpTreeNode> GetAllChildrenRecursive([NotNull] ITreeNode node)
        {
            foreach (var childNode in node.Children<ICSharpTreeNode>())
            {
                if (childNode is ILocalFunctionDeclaration || childNode is IAnonymousFunctionExpression)
                {
                    continue; // skip the element (do not drill down)
                }

                yield return childNode;

                foreach (var n in GetAllChildrenRecursive(childNode))
                {
                    yield return n;
                }
            }
        }

        [Pure]
        static bool HasPreviousUsingDeclaration([NotNull] ITreeNode node, [NotNull] IParametersOwnerDeclaration container)
            => node.SelfAndPathToRoot()
                .TakeWhile(n => n != container)
                .Any(
                    n => n.AssertNotNull()
                        .LeftSiblings()
                        .Any(s => ((s as IDeclarationStatement)?.Declaration as IMultipleLocalVariableDeclaration)?.UsingKeyword != null));

        [Pure]
        static bool IsLastExpression(
            [NotNull] IParametersOwnerDeclaration container,
            [NotNull] ICSharpExpression expression,
            [CanBeNull] out IExpressionStatement statementToBeReplacedWithReturnStatement)
        {
            var block = null as IBlock;

            switch (container)
            {
                case IMethodDeclaration methodDeclaration:
                    if (methodDeclaration.ArrowClause?.Expression == expression)
                    {
                        statementToBeReplacedWithReturnStatement = null;
                        return true;
                    }

                    block = methodDeclaration.Body;
                    break;

                case ILambdaExpression lambdaExpression:
                    if (lambdaExpression.BodyExpression == expression)
                    {
                        statementToBeReplacedWithReturnStatement = null;
                        return true;
                    }

                    block = lambdaExpression.BodyBlock;
                    break;

                case IAnonymousMethodExpression anonymousMethodExpression:
                    block = anonymousMethodExpression.Body;
                    break;

                case ILocalFunctionDeclaration localFunctionDeclaration:
                    if (localFunctionDeclaration.ArrowClause?.Expression == expression)
                    {
                        statementToBeReplacedWithReturnStatement = null;
                        return true;
                    }

                    block = localFunctionDeclaration.Body;
                    break;
            }

            var lastStatement = block?.Children<ICSharpStatement>()
                .LastOrDefault(s => !(s is IDeclarationStatement declarationStatement) || declarationStatement.LocalFunctionDeclaration == null);

            switch (lastStatement)
            {
                case IExpressionStatement expressionStatement when expressionStatement.Expression == expression &&
                    !HasPreviousUsingDeclaration(expressionStatement, container):
                    statementToBeReplacedWithReturnStatement = expressionStatement;
                    return true;

                case IReturnStatement returnStatement when returnStatement.Value == expression &&
                    !HasPreviousUsingDeclaration(returnStatement, container):
                    statementToBeReplacedWithReturnStatement = null;
                    return true;

                default:
                    statementToBeReplacedWithReturnStatement = null;
                    return false;
            }
        }

        [Pure]
        static bool IsTestMethodOfOldMsTest([CanBeNull] IMethodDeclaration methodDeclaration)
        {
            if (methodDeclaration == null || !methodDeclaration.IsDeclaredInOldMsTestProject())
            {
                return false;
            }

            var testMethodAttributeTypes = null as IDeclaredType[];

            return methodDeclaration.Attributes.Any(
                attribute =>
                {
                    if (attribute == null)
                    {
                        return false;
                    }

                    if (testMethodAttributeTypes == null)
                    {
                        testMethodAttributeTypes = new IDeclaredType[testMethodAttributes.Length];
                        for (var i = 0; i < testMethodAttributes.Length; i++)
                        {
                            testMethodAttributeTypes[i] = TypeFactory.CreateTypeByCLRName(
                                testMethodAttributes[i],
                                NullableAnnotation.Unknown,
                                methodDeclaration.GetPsiModule());
                        }
                    }

                    var attributeType = attribute.GetAttributeInstance().GetAttributeType();

                    return testMethodAttributeTypes.Any(
                        type => attributeType.Equals(type, TypeEqualityComparer.Default) || attributeType.IsSubtypeOf(type));
                });
        }

        static void AddRedundantAwaitHighlightings(
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] ITokenNode asyncKeyword,
            [NotNull] Action removeAsync,
            [NotNull] IAwaitExpression awaitExpression,
            [CanBeNull] IExpressionStatement statementToBeReplacedWithReturnStatement,
            [NotNull] ICSharpExpression expressionToReturn,
            [CanBeNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [CanBeNull] IInvocationExpression configureAwaitInvocationExpression = null)
        {
            var configureAwaitNode = configureAwaitInvocationExpression?.InvokedExpression?.LastChild;

            var highlightConfigureAwait = configureAwaitNode != null && configureAwaitInvocationExpression.ArgumentList != null;

            var highlighting = new RedundantAwaitSuggestion(
                $"Redundant 'await' (remove 'async'/'await'{(highlightConfigureAwait ? "/'" + nameof(Task.ConfigureAwait) + "(...)'" : "")})",
                removeAsync,
                awaitExpression,
                statementToBeReplacedWithReturnStatement,
                expressionToReturn,
                attributesOwnerDeclaration);

            consumer.AddHighlighting(highlighting, awaitExpression.AwaitKeyword.GetDocumentRange());

            consumer.AddHighlighting(highlighting, asyncKeyword.GetDocumentRange());

            if (highlightConfigureAwait)
            {
                var dotToken = configureAwaitNode.GetPreviousMeaningfulToken();
                var leftParenthesisToken = configureAwaitInvocationExpression.ArgumentList.GetPreviousMeaningfulToken();
                var rightParenthesisToken = configureAwaitInvocationExpression.ArgumentList.GetNextMeaningfulToken();

                consumer.AddHighlighting(
                    highlighting,
                    configureAwaitNode.GetDocumentRange()
                        .JoinLeft(dotToken.GetDocumentRange())
                        .JoinRight(
                            configureAwaitInvocationExpression.ArgumentList.GetDocumentRange()
                                .JoinLeft(leftParenthesisToken.GetDocumentRange())
                                .JoinRight(rightParenthesisToken.GetDocumentRange())));
            }
        }

        static bool AnalyzeRedundantAwait(
            [NotNull] IAwaitExpression awaitExpression,
            bool isLastExpression,
            [NotNull] IParametersOwnerDeclaration container,
            [CanBeNull] IExpressionStatement statementToBeReplacedWithReturnStatement,
            [NotNull] IHighlightingConsumer consumer)
        {
            if (isLastExpression &&
                !IsTestMethodOfOldMsTest(container as IMethodDeclaration) &&
                !GetAllChildrenRecursive(container).OfType<IAwaitExpression>().HasMoreThan(1) &&
                GetAllChildrenRecursive(container).OfType<IForeachStatement>().All(s => s.AwaitKeyword == null) &&
                !GetAllChildrenRecursive(container).OfType<IReturnStatement>().HasMoreThan(awaitExpression.Parent is IReturnStatement ? 1 : 0))
            {
                TryGetContainerTypeAndAsyncKeyword(
                    container,
                    out var containerType,
                    out var asyncKeyword,
                    out var removeAsync,
                    out var attributesOwnerDeclaration);

                if (asyncKeyword != null)
                {
                    var awaitExpressionType = awaitExpression.Task?.Type();

                    if (containerType.IsValueTask() && awaitExpressionType.IsValueTask() ||
                        containerType.IsTask() && (awaitExpressionType.IsTask() || awaitExpressionType.IsGenericTask()))
                    {
                        // container is ValueTask, awaitExpression is ValueTask
                        // or
                        // container is Task, awaitExpression is Task or Task<T>

                        AddRedundantAwaitHighlightings(
                            consumer,
                            asyncKeyword,
                            removeAsync,
                            awaitExpression,
                            statementToBeReplacedWithReturnStatement,
                            awaitExpression.Task,
                            attributesOwnerDeclaration);
                        return true;
                    }

                    if ((containerType.IsGenericValueTask() && awaitExpressionType.IsGenericValueTask() ||
                            containerType.IsGenericTask() && awaitExpressionType.IsGenericTask()) &&
                        containerType.Equals(awaitExpressionType, TypeEqualityComparer.Default))
                    {
                        // container is ValueTask<T>, awaitExpression is ValueTask<T>, container type == awaitExpression type
                        // or
                        // container is Task<T>, awaitExpression is Task<T>, container type == awaitExpression type

                        AddRedundantAwaitHighlightings(
                            consumer,
                            asyncKeyword,
                            removeAsync,
                            awaitExpression,
                            statementToBeReplacedWithReturnStatement,
                            awaitExpression.Task,
                            attributesOwnerDeclaration);
                        return true;
                    }

                    if (awaitExpression.Task is IInvocationExpression configureAwaitInvocationExpression)
                    {
                        var method = configureAwaitInvocationExpression.InvocationExpressionReference.Resolve().DeclaredElement as IMethod;
                        if (method?.ShortName == nameof(Task.ConfigureAwait))
                        {
                            var methodContainingTypeElement = method.GetContainingType();
                            if (methodContainingTypeElement != null)
                            {
                                var methodContainingType = TypeFactory.CreateType(methodContainingTypeElement);

                                var awaitExpressionWithoutConfigureAwait =
                                    (configureAwaitInvocationExpression.InvokedExpression as IReferenceExpression)?.QualifierExpression;
                                var awaitExpressionTypeWithoutConfigureAwait = awaitExpressionWithoutConfigureAwait?.Type();

                                if (containerType.IsValueTask() &&
                                    methodContainingType.IsValueTask() &&
                                    awaitExpressionTypeWithoutConfigureAwait.IsValueTask() ||
                                    containerType.IsTask() &&
                                    (methodContainingType.IsTask() && awaitExpressionTypeWithoutConfigureAwait.IsTask() ||
                                        methodContainingType.IsGenericTask() && awaitExpressionTypeWithoutConfigureAwait.IsGenericTask()))
                                {
                                    // container is ValueTask, awaitExpression (without "ConfigureAwait") is ValueTask
                                    // or
                                    // container is Task, awaitExpression (without "ConfigureAwait") is Task or Task<T>

                                    AddRedundantAwaitHighlightings(
                                        consumer,
                                        asyncKeyword,
                                        removeAsync,
                                        awaitExpression,
                                        statementToBeReplacedWithReturnStatement,
                                        awaitExpressionWithoutConfigureAwait,
                                        attributesOwnerDeclaration,
                                        configureAwaitInvocationExpression);
                                    return true;
                                }

                                if ((containerType.IsGenericValueTask() &&
                                        methodContainingType.IsGenericValueTask() &&
                                        awaitExpressionTypeWithoutConfigureAwait.IsGenericValueTask() ||
                                        containerType.IsGenericTask() &&
                                        methodContainingType.IsGenericTask() &&
                                        awaitExpressionTypeWithoutConfigureAwait.IsGenericTask()) &&
                                    containerType.Equals(awaitExpressionTypeWithoutConfigureAwait, TypeEqualityComparer.Default))
                                {
                                    // container is ValueTask<T>, awaitExpression (without "ConfigureAwait") is ValueTask<T>, container type == awaitExpression type (without "ConfigureAwait")
                                    // or
                                    // container is Task<T>, awaitExpression (without "ConfigureAwait") is Task<T>, container type == awaitExpression type (without "ConfigureAwait")

                                    AddRedundantAwaitHighlightings(
                                        consumer,
                                        asyncKeyword,
                                        removeAsync,
                                        awaitExpression,
                                        statementToBeReplacedWithReturnStatement,
                                        awaitExpressionWithoutConfigureAwait,
                                        attributesOwnerDeclaration,
                                        configureAwaitInvocationExpression);
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        static void AnalyzeRedundantCapturedContext(
            [NotNull] IParametersOwnerDeclaration container,
            [NotNull] IAwaitExpression awaitExpression,
            bool isLastExpression,
            [NotNull] IHighlightingConsumer consumer)
        {
            var hasRedundantCapturedContext = isLastExpression ||
                awaitExpression.Parent is IReturnStatement returnStatement &&
                !returnStatement.PathToRoot()
                    .Skip(1)
                    .TakeWhile(node => node != container)
                    .Any(node => node is IUsingStatement || node is ITryStatement) &&
                !HasPreviousUsingDeclaration(returnStatement, container);

            if (hasRedundantCapturedContext && awaitExpression.Task.IsConfigureAwaitAvailable())
            {
                consumer.AddHighlighting(
                    new RedundantCapturedContextSuggestion(
                        $"Redundant captured context (add '.{nameof(Task.ConfigureAwait)}(false)')",
                        awaitExpression));
            }
        }

        protected override void Run(IAwaitExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.Task == null)
            {
                return;
            }

            var container = element.GetContainingNode<IParametersOwnerDeclaration>();

            if (container == null)
            {
                return;
            }

            var isLastExpression = IsLastExpression(container, element, out var statementToBeReplacedWithReturnStatement);

            var highlightingsAdded = AnalyzeRedundantAwait(element, isLastExpression, container, statementToBeReplacedWithReturnStatement, consumer);

            if (!highlightingsAdded)
            {
                AnalyzeRedundantCapturedContext(container, element, isLastExpression, consumer);
            }
        }
    }
}