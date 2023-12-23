using JetBrains.ReSharper.Daemon.CSharp.PropertiesExtender;
using JetBrains.ReSharper.Feature.Services.CSharp.PropertiesExtender;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.dataStructures;

namespace ReCommendedExtension.Analyzers.Await;

[ElementProblemAnalyzer(typeof(IAwaitExpression), HighlightingTypes = [typeof(RedundantCapturedContextSuggestion)])]
public sealed class AwaitAnalyzer : ElementProblemAnalyzer<IAwaitExpression>
{
    [Pure]
    static (bool boolean, bool configureAwaitOptions) GetConfigureParameterTypesAwaitForAwaitedExpression(IUnaryExpression? awaitedExpression)
    {
        var boolean = false;
        var configureAwaitOptions = false;

        if ((awaitedExpression?.Type() as IDeclaredType)?.GetTypeElement() is { } typeElement)
        {
            foreach (var method in typeElement.Methods)
            {
                if (method is { ShortName: nameof(Task.ConfigureAwait), TypeParameters: [], Parameters: [{ } parameter] })
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
    static bool HasPreviousUsingDeclaration(ITreeNode node, IParametersOwnerDeclaration container)
        => node
            .SelfAndPathToRoot()
            .TakeWhile(n => n != container)
            .Any(n => n.LeftSiblings().Any(s => s is IDeclarationStatement { Declaration: IMultipleLocalVariableDeclaration { UsingKeyword: { } } }));

    [Pure]
    static bool IsLastExpression(IParametersOwnerDeclaration container, ICSharpExpression expression)
    {
        var block = null as IBlock;

        switch (container)
        {
            case IMethodDeclaration methodDeclaration:
                if (methodDeclaration.ArrowClause?.Expression == expression)
                {
                    return true;
                }

                block = methodDeclaration.Body;
                break;

            case ILambdaExpression lambdaExpression:
                if (lambdaExpression.BodyExpression == expression)
                {
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
                    return true;
                }

                block = localFunctionDeclaration.Body;
                break;
        }

        return block?.Children<ICSharpStatement>().LastOrDefault(s => s is not IDeclarationStatement { LocalFunctionDeclaration: { } }) switch
        {
            IExpressionStatement expressionStatement when expressionStatement.Expression == expression
                && !HasPreviousUsingDeclaration(expressionStatement, container) => true,

            IReturnStatement returnStatement when returnStatement.Value == expression && !HasPreviousUsingDeclaration(returnStatement, container) =>
                true,

            _ => false,
        };
    }

    static void AnalyzeRedundantCapturedContext(
        IParametersOwnerDeclaration container,
        IAwaitExpression awaitExpression,
        IHighlightingConsumer consumer)
    {
        var hasRedundantCapturedContext = IsLastExpression(container, awaitExpression)
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
        if (GetConfigureAwaitAnalysisMode(data) == ConfigureAwaitAnalysisMode.Library)
        {
            return;
        }

        if (element.Task is { } && element.GetContainingNode<IParametersOwnerDeclaration>() is { } container)
        {
            AnalyzeRedundantCapturedContext(container, element, consumer);
        }
    }
}