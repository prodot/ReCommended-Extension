using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ConditionalInvocation;

[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(ConditionalInvocationHint)])]
public sealed class ConditionalInvocationAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [Pure]
    static IReadOnlyList<string> GetConditionsIfConditionalMethodInvoked(IInvocationExpression invocationExpression)
    {
        if (invocationExpression.Reference.Resolve().DeclaredElement is not IMethod method)
        {
            return []; // cannot analyze => do not highlight
        }

        var sourceFile = invocationExpression.GetSourceFile();
        if (sourceFile is not { })
        {
            return []; // cannot analyze => do not highlight
        }

        var declaredConditions = null as List<string>;

        foreach (var attributeInstance in method.GetAttributeInstances(PredefinedType.CONDITIONAL_ATTRIBUTE_CLASS, true))
        {
            if (attributeInstance.PositionParameterCount == 1
                && attributeInstance.PositionParameter(0).ConstantValue is { Kind: ConstantValueKind.String, StringValue: [_, ..] condition })
            {
                declaredConditions ??= [];
                declaredConditions.Add(condition);
            }
        }

        if (declaredConditions is not { })
        {
            return []; // no declared conditions => do not highlight
        }

        // initialize with assembly-level conditions
        var currentConditions = new HashSet<string>(
            from preProcessingDirective in sourceFile.Properties.GetDefines() select preProcessingDirective.Name);

        // process file-level conditions
        if (invocationExpression.GetContainingFile() is ICSharpFile file)
        {
            foreach (var treeNode in file.Descendants())
            {
                switch (treeNode)
                {
                    case IDefineDirective { SymbolName: [_, ..] condition }:
                        currentConditions.Add(condition);
                        continue;

                    case IUndefDirective { SymbolName: [_, ..] condition }:
                        currentConditions.Remove(condition);
                        continue;
                }

                if (treeNode == invocationExpression || treeNode is IUsingList or ICSharpNamespaceDeclaration or ICSharpTypeDeclaration)
                {
                    break;
                }
            }
        }

        // leave only conditions that are not defined locally
        declaredConditions.RemoveAll(c => !currentConditions.Contains(c));

        return declaredConditions;
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (GetConditionsIfConditionalMethodInvoked(element))
        {
            case []: break;

            case [var condition]:
                consumer.AddHighlighting(
                    new ConditionalInvocationHint(
                        $"Method invocation will be skipped if the '{condition}' condition is not defined.",
                        element));
                break;

            case [_, _, ..] conditions:
                var conditionList = string.Join(", ", from condition in conditions orderby condition select $"'{condition}'");

                consumer.AddHighlighting(
                    new ConditionalInvocationHint(
                        $"Method invocation will be skipped if none of the following conditions is defined: {conditionList}.",
                        element));
                break;
        }
    }
}