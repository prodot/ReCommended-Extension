using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

namespace ReCommendedExtension.Analyzers.Await
{
    [ElementProblemAnalyzer(typeof(IAwaitExpression), HighlightingTypes = new[] { typeof(RedundantCapturedContextSuggestion) })]
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
                    asyncKeyword = localFunctionDeclaration.ModifiersList?.Modifiers.FirstOrDefault(
                        node => node?.GetTokenType() == CSharpTokenType.ASYNC_KEYWORD);
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
                    n => n.LeftSiblings()
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

        static void AnalyzeRedundantCapturedContext(
            [NotNull] IParametersOwnerDeclaration container,
            [NotNull] IAwaitExpression awaitExpression,
            bool isLastExpression,
            [NotNull] IHighlightingConsumer consumer)
        {
            var hasRedundantCapturedContext = isLastExpression
                || awaitExpression.Parent is IReturnStatement returnStatement
                && !returnStatement.PathToRoot()
                    .Skip(1)
                    .TakeWhile(node => node != container)
                    .Any(node => node is IUsingStatement || node is ITryStatement)
                && !HasPreviousUsingDeclaration(returnStatement, container);

            if (hasRedundantCapturedContext && awaitExpression.Task.IsConfigureAwaitAvailable())
            {
                consumer.AddHighlighting(
                    new RedundantCapturedContextSuggestion(
                        $"Redundant captured context (add '.{nameof(Task.ConfigureAwait)}(false)')",
                        awaitExpression));
            }
        }

        [Pure]
        static ConfigureAwaitAnalysisMode GetConfigureAwaitAnalysisMode([NotNull] ElementProblemAnalyzerData data)
        {
            var key = new Key<Boxed<ConfigureAwaitAnalysisMode>>("ConfigureAwaitAnalysis");

            var keyData = data.GetData(key);
            if (keyData != null)
            {
                return keyData.Value;
            }

            var mode = ConfigureAwaitAnalysisProperty.Get(data.SettingsStore);

            data.PutData(key, new Boxed<ConfigureAwaitAnalysisMode>(mode));

            return mode;
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

            if (GetConfigureAwaitAnalysisMode(data) != ConfigureAwaitAnalysisMode.Library)
            {
                AnalyzeRedundantCapturedContext(container, element, isLastExpression, consumer);
            }
        }
    }
}