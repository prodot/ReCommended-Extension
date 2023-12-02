using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Daemon.CSharp.PropertiesExtender;
using JetBrains.ReSharper.Feature.Services.CSharp.PropertiesExtender;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.dataStructures;

namespace ReCommendedExtension.Analyzers.Await;

[ElementProblemAnalyzer(
    typeof(IAwaitExpression),
    HighlightingTypes = new[] { typeof(RedundantAwaitSuggestion), typeof(RedundantCapturedContextSuggestion) })]
public sealed class AwaitAnalyzer : ElementProblemAnalyzer<IAwaitExpression>
{
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
    static (bool boolean, bool configureAwaitOptions) GetConfigureParameterTypesAwaitForAwaitedExpression(IUnaryExpression? awaitedExpression)
    {
        var boolean = false;
        var configureAwaitOptions = false;

        if ((awaitedExpression?.Type() as IDeclaredType)?.GetTypeElement() is { } typeElement)
        {
            foreach (var method in typeElement.Methods)
            {
                if (method is { ShortName: nameof(Task.ConfigureAwait), Parameters: [{ } parameter] })
                {
                    if (parameter.Type.IsBool())
                    {
                        boolean = true;
                        continue;
                    }

                    if (parameter.Type.IsClrType(ClrTypeNames.ConfigureAwaitOptions))
                    {
                        configureAwaitOptions = true;
                    }
                }
            }
        }

        return (boolean, configureAwaitOptions);
    }

    [Pure]
    [ContractAnnotation("=> type: notnull, removeAsync: notnull", true)]
    static void TryGetContainerTypeAndAsyncKeyword(
        IParametersOwnerDeclaration container,
        out IType? type,
        out ITokenNode? asyncKeyword,
        out Action? removeAsync,
        out IAttributesOwnerDeclaration? attributesOwnerDeclaration)
    {
        switch (container)
        {
            case IMethodDeclaration methodDeclaration:
                type = methodDeclaration.Type;
                asyncKeyword = methodDeclaration.ModifiersList.Modifiers.FirstOrDefault(node => node.GetTokenType() == CSharpTokenType.ASYNC_KEYWORD);
                removeAsync = () => methodDeclaration.SetAsync(false);
                attributesOwnerDeclaration =
                    type.IsValueTask() || type.IsGenericValueTask() || methodDeclaration.IsNullableAnnotationsContextEnabled()
                        ? null
                        : methodDeclaration;
                return;

            case ILambdaExpression lambdaExpression:
                type = lambdaExpression.InferredReturnType;
                asyncKeyword = lambdaExpression.AsyncKeyword;
                removeAsync = () => lambdaExpression.SetAsync(false);
                attributesOwnerDeclaration = lambdaExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp100 ? lambdaExpression : null;
                return;

            case IAnonymousMethodExpression anonymousMethodExpression:
                type = anonymousMethodExpression.InferredReturnType;
                asyncKeyword = anonymousMethodExpression.AsyncKeyword;
                removeAsync = () => anonymousMethodExpression.SetAsync(false);
                attributesOwnerDeclaration = null;
                return;

            case ILocalFunctionDeclaration localFunctionDeclaration:
                type = localFunctionDeclaration.Type;
                asyncKeyword = localFunctionDeclaration.ModifiersList.Modifiers.FirstOrDefault(
                    node => node.GetTokenType() == CSharpTokenType.ASYNC_KEYWORD);
                removeAsync = () => localFunctionDeclaration.SetAsync(false);
                attributesOwnerDeclaration = localFunctionDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90
                    ? localFunctionDeclaration
                    : null;
                return;

            default:
                type = null;
                asyncKeyword = null;
                removeAsync = null;
                attributesOwnerDeclaration = null;
                return;
        }
    }

    static IEnumerable<ICSharpTreeNode> GetAllChildrenRecursive(ITreeNode node)
    {
        foreach (var childNode in node.Children<ICSharpTreeNode>())
        {
            if (childNode is ILocalFunctionDeclaration or IAnonymousFunctionExpression)
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
    static bool HasPreviousUsingDeclaration(ITreeNode node, IParametersOwnerDeclaration container)
        => node
            .SelfAndPathToRoot()
            .TakeWhile(n => n != container)
            .Any(n => n.LeftSiblings().Any(s => s is IDeclarationStatement { Declaration: IMultipleLocalVariableDeclaration { UsingKeyword: { } } }));

    [Pure]
    static bool IsLastExpression(
        IParametersOwnerDeclaration container,
        ICSharpExpression expression,
        out IExpressionStatement? statementToBeReplacedWithReturnStatement)
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

        var lastStatement = block?.Children<ICSharpStatement>().LastOrDefault(s => s is not IDeclarationStatement { LocalFunctionDeclaration: { } });

        switch (lastStatement)
        {
            case IExpressionStatement expressionStatement when expressionStatement.Expression == expression
                && !HasPreviousUsingDeclaration(expressionStatement, container):
                statementToBeReplacedWithReturnStatement = expressionStatement;
                return true;

            case IReturnStatement returnStatement
                when returnStatement.Value == expression && !HasPreviousUsingDeclaration(returnStatement, container):
                statementToBeReplacedWithReturnStatement = null;
                return true;

            default:
                statementToBeReplacedWithReturnStatement = null;
                return false;
        }
    }

    [Pure]
    static bool IsTestMethodOfOldMsTest(IMethodDeclaration? methodDeclaration)
    {
        if (methodDeclaration is not { } || !methodDeclaration.IsDeclaredInOldMsTestProject())
        {
            return false;
        }

        var testMethodAttributeTypes = null as IDeclaredType[];

        return methodDeclaration.Attributes.Any(
            attribute =>
            {
                if (attribute is { })
                {
                    if (testMethodAttributeTypes is not { })
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
                }

                return false;
            });
    }

    static void AddRedundantAwaitHighlightings(
        IHighlightingConsumer consumer,
        ITokenNode asyncKeyword,
        Action removeAsync,
        IAwaitExpression awaitExpression,
        IExpressionStatement? statementToBeReplacedWithReturnStatement,
        ICSharpExpression expressionToReturn,
        IAttributesOwnerDeclaration? attributesOwnerDeclaration,
        IInvocationExpression? configureAwaitInvocationExpression = null)
    {
        var configureAwaitNode = configureAwaitInvocationExpression?.InvokedExpression?.LastChild;

        var configureAwaitArgument = configureAwaitNode is { } && configureAwaitInvocationExpression is { Arguments: [{ } argument] }
            ? argument.GetText()
            : null;

        var highlighting = new RedundantAwaitSuggestion(
            $"Redundant 'await' (remove 'async'/'await'{
                (configureAwaitArgument is { } ? $"/'{nameof(Task.ConfigureAwait)}({configureAwaitArgument})'" : "")
            })",
            removeAsync,
            awaitExpression,
            statementToBeReplacedWithReturnStatement,
            expressionToReturn,
            attributesOwnerDeclaration,
            configureAwaitArgument);

        consumer.AddHighlighting(highlighting, awaitExpression.AwaitKeyword.GetDocumentRange());

        consumer.AddHighlighting(highlighting, asyncKeyword.GetDocumentRange());

        if (configureAwaitNode is { })
        {
            Debug.Assert(configureAwaitInvocationExpression is { ArgumentList: { } });

            var dotToken = configureAwaitNode.GetPreviousMeaningfulToken();
            var leftParenthesisToken = configureAwaitInvocationExpression.ArgumentList.GetPreviousMeaningfulToken();
            var rightParenthesisToken = configureAwaitInvocationExpression.ArgumentList.GetNextMeaningfulToken();

            consumer.AddHighlighting(
                highlighting,
                configureAwaitNode
                    .GetDocumentRange()
                    .JoinLeft(dotToken.GetDocumentRange())
                    .JoinRight(
                        configureAwaitInvocationExpression
                            .ArgumentList.GetDocumentRange()
                            .JoinLeft(leftParenthesisToken.GetDocumentRange())
                            .JoinRight(rightParenthesisToken.GetDocumentRange())));
        }
    }

    static bool AnalyzeRedundantAwait(
        IAwaitExpression awaitExpression,
        bool isLastExpression,
        IParametersOwnerDeclaration container,
        IExpressionStatement? statementToBeReplacedWithReturnStatement,
        IHighlightingConsumer consumer)
    {
        if (isLastExpression
            && !IsTestMethodOfOldMsTest(container as IMethodDeclaration)
            && !GetAllChildrenRecursive(container).OfType<IAwaitExpression>().HasMoreThan(1)
            && GetAllChildrenRecursive(container).OfType<IForeachStatement>().All(s => s.AwaitKeyword is not { })
            && !GetAllChildrenRecursive(container).OfType<IReturnStatement>().HasMoreThan(awaitExpression.Parent is IReturnStatement ? 1 : 0))
        {
            TryGetContainerTypeAndAsyncKeyword(
                container,
                out var containerType,
                out var asyncKeyword,
                out var removeAsync,
                out var attributesOwnerDeclaration);

            if (asyncKeyword is { })
            {
                var awaitExpressionType = awaitExpression.Task.Type();

                if (containerType.IsValueTask() && awaitExpressionType.IsValueTask()
                    || containerType.IsTask() && (awaitExpressionType.IsTask() || awaitExpressionType.IsGenericTask()))
                {
                    // container is ValueTask, awaitExpression is ValueTask
                    // or
                    // container is Task, awaitExpression is Task or Task<T>

                    Debug.Assert(removeAsync is { });
                    Debug.Assert(awaitExpression.Task is { });

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

                if (containerType is { }
                    && (containerType.IsGenericValueTask() && awaitExpressionType.IsGenericValueTask()
                        || containerType.IsGenericTask() && awaitExpressionType.IsGenericTask())
                    && containerType.Equals(awaitExpressionType, TypeEqualityComparer.Default))
                {
                    // container is ValueTask<T>, awaitExpression is ValueTask<T>, container type == awaitExpression type
                    // or
                    // container is Task<T>, awaitExpression is Task<T>, container type == awaitExpression type

                    Debug.Assert(removeAsync is { });
                    Debug.Assert(awaitExpression.Task is { });

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

                if (awaitExpression.Task is IInvocationExpression { Arguments: [{ Value: { } argument }] } configureAwaitInvocationExpression
                    && configureAwaitInvocationExpression.InvocationExpressionReference.Resolve().DeclaredElement is IMethod
                    {
                        ShortName: nameof(Task.ConfigureAwait), ContainingType: { } methodContainingTypeElement,
                    })
                {
                    var argumentType = argument.Type();

                    if (argumentType.IsBool()
                        || argumentType.IsClrType(ClrTypeNames.ConfigureAwaitOptions)
                        && argument is IReferenceExpression { ConstantValue: { Kind: ConstantValueKind.Enum, IntValue: 0 or 1 } })
                    {
                        var methodContainingType = TypeFactory.CreateType(methodContainingTypeElement);

                        var awaitExpressionWithoutConfigureAwait =
                            (configureAwaitInvocationExpression.InvokedExpression as IReferenceExpression)?.QualifierExpression;
                        var awaitExpressionTypeWithoutConfigureAwait = awaitExpressionWithoutConfigureAwait?.Type();

                        if (containerType.IsValueTask()
                            && methodContainingType.IsValueTask()
                            && awaitExpressionTypeWithoutConfigureAwait.IsValueTask()
                            || containerType.IsTask()
                            && (methodContainingType.IsTask() && awaitExpressionTypeWithoutConfigureAwait.IsTask()
                                || methodContainingType.IsGenericTask() && awaitExpressionTypeWithoutConfigureAwait.IsGenericTask()))
                        {
                            // container is ValueTask, awaitExpression (without "ConfigureAwait") is ValueTask
                            // or
                            // container is Task, awaitExpression (without "ConfigureAwait") is Task or Task<T>

                            Debug.Assert(removeAsync is { });
                            Debug.Assert(awaitExpressionWithoutConfigureAwait is { });

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

                        if (containerType is { }
                            && (containerType.IsGenericValueTask()
                                && methodContainingType.IsGenericValueTask()
                                && awaitExpressionTypeWithoutConfigureAwait.IsGenericValueTask()
                                || containerType.IsGenericTask()
                                && methodContainingType.IsGenericTask()
                                && awaitExpressionTypeWithoutConfigureAwait.IsGenericTask())
                            && containerType.Equals(awaitExpressionTypeWithoutConfigureAwait, TypeEqualityComparer.Default))
                        {
                            // container is ValueTask<T>, awaitExpression (without "ConfigureAwait") is ValueTask<T>, container type == awaitExpression type (without "ConfigureAwait")
                            // or
                            // container is Task<T>, awaitExpression (without "ConfigureAwait") is Task<T>, container type == awaitExpression type (without "ConfigureAwait")

                            Debug.Assert(removeAsync is { });
                            Debug.Assert(awaitExpressionWithoutConfigureAwait is { });

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
                    }
                }
            }
        }

        return false;
    }

    static void AnalyzeRedundantCapturedContext(
        IParametersOwnerDeclaration container,
        IAwaitExpression awaitExpression,
        bool isLastExpression,
        IHighlightingConsumer consumer)
    {
        var hasRedundantCapturedContext = isLastExpression
            || awaitExpression.Parent is IReturnStatement returnStatement
            && !returnStatement
                .PathToRoot()
                .Skip(1)
                .TakeWhile(node => node != container)
                .Any(node => node is IUsingStatement or ITryStatement)
            && !HasPreviousUsingDeclaration(returnStatement, container);

        if (hasRedundantCapturedContext)
        {
            var (boolean, configureAwaitOptions) = GetConfigureParameterTypesAwaitForAwaitedExpression(awaitExpression.Task);

            var actionHint = (boolean, configureAwaitOptions) switch
            {
                (true, false) => $"add '.{nameof(Task.ConfigureAwait)}(false)'",
                (true, true) => $"add '.{nameof(Task.ConfigureAwait)}(false)' or '.{nameof(Task.ConfigureAwait)}(ConfigureAwaitOptions.None)'",
                (false, true) => $"add '.{nameof(Task.ConfigureAwait)}(ConfigureAwaitOptions.None)'",

                (false, false) => null,
            };

            if (actionHint is { })
            {
                consumer.AddHighlighting(new RedundantCapturedContextSuggestion($"Redundant captured context ({actionHint})", awaitExpression));
            }
        }
    }

    [Pure]
    static ConfigureAwaitAnalysisMode GetConfigureAwaitAnalysisMode(ElementProblemAnalyzerData data)
    {
        var key = new Key<Boxed<ConfigureAwaitAnalysisMode>>("ConfigureAwaitAnalysis");

        if (data.GetData(key) is { } keyData)
        {
            return keyData.Value;
        }

        var mode = ConfigureAwaitAnalysisProperty.Get(data.SettingsStore);

        data.PutData(key, new Boxed<ConfigureAwaitAnalysisMode>(mode));

        return mode;
    }

    protected override void Run(IAwaitExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.Task is { } && element.GetContainingNode<IParametersOwnerDeclaration>() is { } container)
        {
            var isLastExpression = IsLastExpression(container, element, out var statementToBeReplacedWithReturnStatement);

            var highlightingsAdded = AnalyzeRedundantAwait(element, isLastExpression, container, statementToBeReplacedWithReturnStatement, consumer);

            if (!highlightingsAdded && GetConfigureAwaitAnalysisMode(data) != ConfigureAwaitAnalysisMode.Library)
            {
                AnalyzeRedundantCapturedContext(container, element, isLastExpression, consumer);
            }
        }
    }
}