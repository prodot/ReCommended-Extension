using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.ControlFlow;

public abstract record Assertion
{
    /// <remarks>
    /// Indicates that the expression has already been processed.
    /// </remarks>
    static readonly Key<object> key = new(typeof(Assertion).FullName);

    [Pure]
    internal static HashSet<Assertion> CollectAssertions(
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

            if (expression is IInvocationExpression invocationExpression && !invocationExpression.UserData.HasKey(key))
            {
                if (AssertionStatement.TryFromInvocationExpression(
                        invocationExpression,
                        assertionMethodAnnotationProvider,
                        assertionConditionAnnotationProvider) is { } assertionStatement)
                {
                    assertions.Add(assertionStatement);
                }

                if (InlineAssertion.TryFromInvocationExpression(invocationExpression) is { } inlineAssertion)
                {
                    assertions.Add(inlineAssertion);
                }

                invocationExpression.UserData.PutKey(key);
            }
        }

        return assertions;
    }

    internal abstract AssertionConditionType AssertionConditionType { get; }
}