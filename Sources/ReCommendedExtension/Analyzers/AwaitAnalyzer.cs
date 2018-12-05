using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(
        typeof(IAwaitExpression),
        HighlightingTypes = new[] { typeof(RedundantAwaitHighlighting), typeof(RedundantCapturedContextHighlighting) })]
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
        static void TryGetContainerTypeAndAsyncKeyword(
            [NotNull] IParametersOwnerDeclaration container,
            out IType type,
            out ITokenNode asyncKeyword,
            out Action removeAsync,
            out IAttributesOwnerDeclaration attributesOwnerDeclaration)
        {
            switch (container)
            {
                case IMethodDeclaration methodDeclaration:
                    type = methodDeclaration.Type;
                    asyncKeyword = methodDeclaration.ModifiersList?.ModifiersEnumerable.FirstOrDefault(
                        node => node?.GetTokenType() == CSharpTokenType.ASYNC_KEYWORD);
                    removeAsync = () => methodDeclaration.SetAsync(false);
                    attributesOwnerDeclaration = methodDeclaration;
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
                    asyncKeyword = localFunctionDeclaration.ModifiersList?.ModifiersEnumerable.FirstOrDefault(
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
                yield return childNode;

                foreach (var n in GetAllChildrenRecursive(childNode))
                {
                    yield return n;
                }
            }
        }

        [Pure]
        static bool IsLastExpression(
            [NotNull] IParametersOwnerDeclaration container,
            [NotNull] ICSharpExpression expression,
            out IExpressionStatement statementToBeReplacedWithReturnStatement)
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
                case IExpressionStatement expressionStatement when expressionStatement.Expression == expression:
                    statementToBeReplacedWithReturnStatement = expressionStatement;
                    return true;

                case IReturnStatement returnStatement when returnStatement.Value == expression:
                    statementToBeReplacedWithReturnStatement = null;
                    return true;

                default:
                    statementToBeReplacedWithReturnStatement = null;
                    return false;
            }
        }

        [Pure]
        static bool IsTestMethod(IMethodDeclaration methodDeclaration)
        {
            if (methodDeclaration == null || !methodDeclaration.IsDeclaredInMsTestProject())
            {
                return false;
            }

            var testMethodAttributeTypes = null as IDeclaredType[];

            return methodDeclaration.AttributesEnumerable.Any(
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
                            testMethodAttributeTypes[i] = TypeFactory.CreateTypeByCLRName(testMethodAttributes[i], methodDeclaration.GetPsiModule());
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
            IExpressionStatement statementToBeReplacedWithReturnStatement,
            [NotNull] ICSharpExpression expressionToReturn,
            IAttributesOwnerDeclaration attributesOwnerDeclaration,
            IInvocationExpression configureAwaitInvocationExpression = null)
        {
            var configureAwaitNode = configureAwaitInvocationExpression?.InvokedExpression?.LastChild;

            var highlightConfigureAwait = configureAwaitNode != null && configureAwaitInvocationExpression.ArgumentList != null;

            var highlighting = new RedundantAwaitHighlighting(
                $"Redundant 'await' (remove 'async'/'await'{(highlightConfigureAwait ? "/'" + ClrMethodsNames.ConfigureAwait + "(...)'" : "")})",
                removeAsync,
                awaitExpression,
                statementToBeReplacedWithReturnStatement,
                expressionToReturn,
                attributesOwnerDeclaration);

            consumer.AddHighlighting(highlighting, awaitExpression.AwaitKeyword.GetDocumentRange());

            consumer.AddHighlighting(highlighting, asyncKeyword.GetDocumentRange(), isSecondaryHighlighting: true);

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
                                .JoinRight(rightParenthesisToken.GetDocumentRange())),
                    isSecondaryHighlighting: true);
            }
        }

        static bool AnalyzeRedundantAwait(
            [NotNull] IAwaitExpression awaitExpression,
            bool isLastExpression,
            [NotNull] IParametersOwnerDeclaration container,
            IExpressionStatement statementToBeReplacedWithReturnStatement,
            [NotNull] IHighlightingConsumer consumer)
        {
            if (isLastExpression &&
                !IsTestMethod(container as IMethodDeclaration) &&
                !GetAllChildrenRecursive(container).OfType<IAwaitExpression>().HasMoreThan(1))
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

                    if (containerType.IsTask() && (awaitExpressionType.IsTask() || awaitExpressionType.IsGenericTask()))
                    {
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

                    if (containerType.IsGenericTask() &&
                        awaitExpressionType.IsGenericTask() &&
                        containerType.Equals(awaitExpressionType, TypeEqualityComparer.Default))
                    {
                        // container is Task<T>, awaitExpression is Task<T>, container type = awaitExpression type
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
                        if (method?.ShortName == ClrMethodsNames.ConfigureAwait)
                        {
                            var methodContainingTypeElement = method.GetContainingType();
                            if (methodContainingTypeElement != null)
                            {
                                var methodContainingType = TypeFactory.CreateType(methodContainingTypeElement);

                                var awaitExpressionWithoutConfigureAwait =
                                    (configureAwaitInvocationExpression.InvokedExpression as IReferenceExpression)?.QualifierExpression;
                                var awaitExpressionTypeWithoutConfigureAwait = awaitExpressionWithoutConfigureAwait?.Type();

                                if (containerType.IsTask() &&
                                    (methodContainingType.IsTask() && awaitExpressionTypeWithoutConfigureAwait.IsTask() ||
                                        methodContainingType.IsGenericTask() && awaitExpressionTypeWithoutConfigureAwait.IsGenericTask()))
                                {
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

                                if (containerType.IsGenericTask() &&
                                    methodContainingType.IsGenericTask() &&
                                    awaitExpressionTypeWithoutConfigureAwait.IsGenericTask() &&
                                    containerType.Equals(awaitExpressionTypeWithoutConfigureAwait, TypeEqualityComparer.Default))
                                {
                                    // container is Task<T>, awaitExpression (without "ConfigureAwait") is Task<T>, container type = awaitExpression type (without "ConfigureAwait")
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
            [NotNull] IAwaitExpression awaitExpression,
            bool isLastExpression,
            [NotNull] IHighlightingConsumer consumer)
        {
            if ((isLastExpression || awaitExpression.Parent is IReturnStatement) && awaitExpression.Task.IsConfigureAwaitAvailable())
            {
                consumer.AddHighlighting(
                    new RedundantCapturedContextHighlighting(
                        $"Redundant captured context (add '.{ClrMethodsNames.ConfigureAwait}(false)')",
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
                AnalyzeRedundantCapturedContext(element, isLastExpression, consumer);
            }
        }
    }
}