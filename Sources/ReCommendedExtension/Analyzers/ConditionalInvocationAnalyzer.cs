using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = new[] { typeof(ConditionalInvocationHighlighting) })]
    public sealed class ConditionalInvocationAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
    {
        [NotNull]
        static readonly string[] emptyList = { };

        [NotNull]
        static IList<string> GetConditionsIfConditionalMethodInvoked([NotNull] IInvocationExpression invocationExpression)
        {
            var method = invocationExpression.Reference?.Resolve().DeclaredElement as IMethod;

            if (method == null)
            {
                return emptyList; // cannot analyze => do not highlight
            }

            var declaredConditions = (
                from attributeInstance in method.GetAttributeInstances(PredefinedType.CONDITIONAL_ATTRIBUTE_CLASS, false)
                where attributeInstance.AssertNotNull().PositionParameterCount == 1
                let constantValue = attributeInstance.PositionParameter(0).ConstantValue
                where constantValue != null
                where constantValue.IsString()
                let condition = (string)constantValue.Value
                where !string.IsNullOrEmpty(condition)
                select condition).ToList();

            if (declaredConditions.Count == 0)
            {
                return emptyList; // no declared conditions => do not highlight
            }

            var sourceFile = invocationExpression.GetSourceFile();
            if (sourceFile == null)
            {
                return emptyList; // cannot analyze => do not highlight
            }

            // initialize with assembly-level conditions
            var currentConditions = new HashSet<string>(
                from preProcessingDirective in sourceFile.Properties.GetDefines() select preProcessingDirective.Name);

            // process file-level conditions
            if (invocationExpression.GetContainingFile() is ICSharpFile file)
            {
                foreach (var treeNode in file.Descendants())
                {
                    if (treeNode is IDefineDirective defineDirective)
                    {
                        if (!string.IsNullOrEmpty(defineDirective.SymbolName))
                        {
                            currentConditions.Add(defineDirective.SymbolName);
                        }
                        continue;
                    }

                    if (treeNode is IUndefDirective undefDirective)
                    {
                        if (!string.IsNullOrEmpty(undefDirective.SymbolName))
                        {
                            currentConditions.Remove(undefDirective.SymbolName);
                        }
                        continue;
                    }

                    if (treeNode == invocationExpression ||
                        treeNode is IUsingList ||
                        treeNode is ICSharpNamespaceDeclaration ||
                        treeNode is ICSharpTypeDeclaration)
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
            if (conditions.Count > 0)
            {
                consumer.AddHighlighting(
                    new ConditionalInvocationHighlighting(
                        conditions.Count == 1
                            ? string.Format("Method invocation will be skipped if the '{0}' condition is not defined.", conditions[0])
                            : string.Format(
                                "Method invocation will be skipped if none of the following conditions is defined: {0}.",
                                string.Join(", ", from condition in conditions orderby condition select $"'{condition}'")),
                        element));
            }
        }
    }
}