using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Assertions
{
    internal abstract class Assertion : IEquatable<Assertion>
    {
        [NotNull]
        [ItemNotNull]
        internal static HashSet<Assertion> CollectAssertions(
            [NotNull] AssertionMethodAnnotationProvider assertionMethodAnnotationProvider,
            [NotNull] AssertionConditionAnnotationProvider assertionConditionAnnotationProvider,
            [NotNull] ICSharpTreeNode rootNode)
        {
            var forTypeLevelInitializersOnly = rootNode is IClassLikeDeclaration;

            var assertions = new HashSet<Assertion>();

            foreach (var invocationExpression in rootNode.Descendants<IInvocationExpression>())
            {
                Debug.Assert(invocationExpression != null);

                var isInTypeLevelInitializer = invocationExpression.PathToRoot()
                    .Any(node => node is IExpressionInitializer && (node.Parent is IFieldDeclaration || node.Parent is IPropertyDeclaration));

                if (forTypeLevelInitializersOnly != isInTypeLevelInitializer)
                {
                    continue;
                }

                var assertionStatement = AssertionStatement.TryFromInvocationExpression(
                    invocationExpression,
                    assertionMethodAnnotationProvider,
                    assertionConditionAnnotationProvider);
                if (assertionStatement != null)
                {
                    assertions.Add(assertionStatement);
                }

                var inlineAssertion = InlineAssertion.TryFromInvocationExpression(invocationExpression);
                if (inlineAssertion != null)
                {
                    assertions.Add(inlineAssertion);
                }
            }

            return assertions;
        }

        public abstract AssertionConditionType AssertionConditionType { get; }

        public abstract override int GetHashCode();

        public sealed override bool Equals(object obj) => Equals(obj as Assertion);

        public abstract bool Equals(Assertion other);
    }
}