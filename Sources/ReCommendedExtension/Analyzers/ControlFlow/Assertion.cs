using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ControlFlow;

internal abstract record Assertion
{
    public static HashSet<Assertion> CollectAssertions(
        AssertionMethodAnnotationProvider assertionMethodAnnotationProvider,
        AssertionConditionAnnotationProvider assertionConditionAnnotationProvider,
        ICSharpTreeNode rootNode)
    {
        var forTypeLevelInitializersOnly = rootNode is IClassLikeDeclaration;

        var assertions = new HashSet<Assertion>();

        foreach (var expression in rootNode.Descendants<ICSharpExpression>())
        {
            var isInTypeLevelInitializer =
                expression.PathToRoot().Any(node => node is IExpressionInitializer { Parent: IFieldDeclaration or IPropertyDeclaration });

            if (forTypeLevelInitializersOnly != isInTypeLevelInitializer)
            {
                continue;
            }

            if (expression is IInvocationExpression invocationExpression)
            {
                var assertionStatement = AssertionStatement.TryFromInvocationExpression(
                    invocationExpression,
                    assertionMethodAnnotationProvider,
                    assertionConditionAnnotationProvider);
                if (assertionStatement is { })
                {
                    assertions.Add(assertionStatement);
                }

                if (InlineAssertion.TryFromInvocationExpression(invocationExpression) is { } inlineAssertion)
                {
                    assertions.Add(inlineAssertion);
                }
            }
        }

        return assertions;
    }

    public abstract AssertionConditionType AssertionConditionType { get; }
}