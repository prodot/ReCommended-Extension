using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ConditionalInvocation
{
    [ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = new[] { typeof(ConditionalInvocationHint) })]
    public sealed class ConditionalInvocationAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
    {
        [NotNull]
        static IList<string> GetConditionsIfConditionalMethodInvoked([NotNull] IInvocationExpression invocationExpression)
        {
            if (!(invocationExpression.Reference.Resolve().DeclaredElement is IMethod method))
            {
                return System.Array.Empty<string>(); // cannot analyze => do not highlight
            }

            var declaredConditions = (
                from attributeInstance in method.GetAttributeInstances(PredefinedType.CONDITIONAL_ATTRIBUTE_CLASS, false)
                where attributeInstance.PositionParameterCount == 1
                let constantValue = attributeInstance.PositionParameter(0).ConstantValue
                where constantValue.IsString() && !string.IsNullOrEmpty(constantValue.StringValue)
                select constantValue.StringValue).ToList();

            if (declaredConditions.Count == 0)
            {
                return System.Array.Empty<string>(); // no declared conditions => do not highlight
            }

            var sourceFile = invocationExpression.GetSourceFile();
            if (sourceFile == null)
            {
                return System.Array.Empty<string>(); // cannot analyze => do not highlight
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
                        case IDefineDirective defineDirective:
                            if (!string.IsNullOrEmpty(defineDirective.SymbolName))
                            {
                                currentConditions.Add(defineDirective.SymbolName);
                            }
                            continue;

                        case IUndefDirective undefDirective:
                            if (!string.IsNullOrEmpty(undefDirective.SymbolName))
                            {
                                currentConditions.Remove(undefDirective.SymbolName);
                            }
                            continue;
                    }

                    if (treeNode == invocationExpression
                        || treeNode is IUsingList
                        || treeNode is ICSharpNamespaceDeclaration
                        || treeNode is ICSharpTypeDeclaration)
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
            var conditions = GetConditionsIfConditionalMethodInvoked(element);
            switch (conditions.Count)
            {
                case 0: break;

                case 1:
                    consumer.AddHighlighting(
                        new ConditionalInvocationHint(
                            $"Method invocation will be skipped if the '{conditions[0]}' condition is not defined.",
                            element));
                    break;

                default:
                    var conditionList = string.Join(", ", from condition in conditions orderby condition select $"'{condition}'");

                    consumer.AddHighlighting(
                        new ConditionalInvocationHint(
                            $"Method invocation will be skipped if none of the following conditions is defined: {conditionList}.",
                            element));
                    break;
            }
        }
    }
}